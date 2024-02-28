﻿using ClosedXML.Excel;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Dividendo;
using MinhasFinancas.Service.Papel;
using MinhasFinancas.Service.Transacao;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MinhasFinancas.Service.Configuracao
{
    public class ConfiguracaoService : BaseService, IConfiguracaoService
    {
        IPapelService _papelService;
        ITransacaoService _transacaoService;
        IDividendoService _dividendoService;

        public ConfiguracaoService(IPapelService papelService,
                                    ITransacaoService transacaoService,
                                    IDividendoService dividendoService, INotificador notificador) : base(notificador)
        {
            _papelService = papelService;
            _transacaoService = transacaoService;
            _dividendoService = dividendoService;
        }

        public async Task ImportarExcelB3(HttpPostedFileBase fileB3)
        {
            // Abre o arquivo Excel
            using (var stream = fileB3.InputStream)
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var planilha = workbook.Worksheets.First();
                    var totalLinhas = planilha.Rows().Count();

                    #region gambiarra
                    var lstP = await _papelService.Get(includeProperties: "Transacao,Dividendo");
                    var lstT = await _transacaoService.Get();
                    var lstD = await _dividendoService.Get();

                    List<Infra.Models.Papel> lstPapel = lstP.ToList();
                    List<Infra.Models.Transacao> lstTransacao = lstT.ToList();
                    List<Infra.Models.Dividendo> lstDividendo = lstD.ToList();

                    lstP = null;
                    lstT = null;
                    lstD = null;
                    #endregion

                    for (int l = 2; l <= totalLinhas; l++)
                    {
                        string nomeCompletoPapel = planilha.Cell($"D{l}").Value.ToString();
                        string codigoPapel = nomeCompletoPapel.Split('-')[0].Trim();
                        codigoPapel = codigoPapel.Replace("12", "11").Replace("13", "11");

                        var papel = lstPapel.Where(x => x.Codigo == codigoPapel).FirstOrDefault();

                        var tipoMov = planilha.Cell($"C{l}").Value.ToString();

                        if (papel == null && (tipoMov == "Bonificação em Ativos" || tipoMov == "Solicitação de Subscrição" ||
                                              tipoMov == "Transferência - Liquidação" || tipoMov == "Juros Sobre Capital Próprio" ||
                                              tipoMov == "Rendimento" || tipoMov == "Dividendo" || tipoMov == "Leilão de Fração" ||
                                              tipoMov == "Desdobro"))
                        {
                            papel = new Infra.Models.Papel();
                            papel.Nome = nomeCompletoPapel.Replace(codigoPapel + " - ", "").Trim();
                            papel.Codigo = codigoPapel;
                            papel.TipoPapel = codigoPapel.Contains("34") ? TipoPapel.BDR : (codigoPapel.Contains("11") || codigoPapel.Contains("12") || codigoPapel.Contains("13") ? TipoPapel.Fii : TipoPapel.Acao);
                            papel.CotacaoAtual = 0;
                            papel.Descricao = "";
                            papel.Ativo = true;

                            await _papelService.Add(papel);
                            lstPapel.Add(papel);
                        }

                        switch (tipoMov)
                        {
                            case "Bonificação em Ativos":
                            case "Solicitação de Subscrição":
                            case "Transferência - Liquidação":
                                var transacao = new Infra.Models.Transacao();
                                transacao.PapelId = papel.Id;
                                string valorUnt = planilha.Cell($"G{l}").Value.ToString();
                                transacao.ValorUnt = Convert.ToDouble(valorUnt.Trim().Replace("-", "") == "" ? "0" : valorUnt);
                                string qtdT = planilha.Cell($"F{l}").Value.ToString() == "" ? "0" : planilha.Cell($"F{l}").Value.ToString();
                                transacao.Quantidade = Convert.ToInt32(Math.Round(Convert.ToDouble(qtdT), 0));
                                transacao.Data = Convert.ToDateTime(planilha.Cell($"B{l}").Value.ToString());
                                transacao.TipoTransacao = planilha.Cell($"C{l}").Value.ToString() == "Solicitação de Subscrição" ? TipoTransacao.Compra : planilha.Cell($"A{l}").Value.ToString() == "Credito" ? TipoTransacao.Compra : TipoTransacao.Venda;
                                transacao.Descricao = "";
                                transacao.Ativo = true;

                                //Verificar se já existe Transação
                                if (!lstTransacao.Any(x => x.PapelId == transacao.PapelId
                                                        && x.Data == transacao.Data
                                                        && ((x.Quantidade == transacao.Quantidade && x.ValorUnt == transacao.ValorUnt)
                                                            || (!string.IsNullOrWhiteSpace(x.Descricao) && x.Descricao.StartsWith("D|")))))
                                {
                                    await _transacaoService.Add(transacao);
                                    lstTransacao.Add(transacao);
                                }
                                break;
                            case "Juros Sobre Capital Próprio":
                            case "Rendimento":
                            case "Dividendo":
                            case "Leilão de Fração":
                                var dividendo = new Infra.Models.Dividendo();
                                dividendo.PapelId = papel.Id;
                                dividendo.ValorRecebido = Convert.ToDouble(planilha.Cell($"H{l}").Value.ToString());
                                string qtdD = planilha.Cell($"F{l}").Value.ToString() == "" ? "0" : planilha.Cell($"F{l}").Value.ToString();
                                dividendo.Quantidade = Convert.ToInt32(Math.Round(Convert.ToDouble(qtdD), 0));
                                dividendo.Data = Convert.ToDateTime(planilha.Cell($"B{l}").Value.ToString());
                                dividendo.Ativo = true;

                                //Verificar se já existe dividendo
                                if (!lstDividendo.Exists(x => x.PapelId == dividendo.PapelId
                                                        && x.Data == dividendo.Data
                                                        && ((x.Quantidade == dividendo.Quantidade && x.ValorRecebido == dividendo.ValorRecebido)
                                                            || (!string.IsNullOrWhiteSpace(x.Descricao) && x.Descricao.StartsWith("D|")))))
                                {
                                    await _dividendoService.Add(dividendo);
                                    lstDividendo.Add(dividendo);
                                }
                                break;
                            case "Desdobro":
                                DateTime dataDesdobro = Convert.ToDateTime(planilha.Cell($"B{l}").Value.ToString());

                                var lstTransacaoDesdobro = lstTransacao.Where(x => x.PapelId == papel.Id && x.Data < dataDesdobro && (string.IsNullOrWhiteSpace(x.Descricao) || !x.Descricao.StartsWith("D|") || x.Descricao.Substring(2, 10) != planilha.Cell($"B{l}").Value.ToString())).ToList();
                                var lstDividendosDesdobro = lstDividendo.Where(x => x.PapelId == papel.Id && x.Data < dataDesdobro && (string.IsNullOrWhiteSpace(x.Descricao) || !x.Descricao.StartsWith("D|") || x.Descricao.Substring(2, 10) != planilha.Cell($"B{l}").Value.ToString())).ToList();

                                var qntComprada = lstTransacaoDesdobro.Sum(x => x.Quantidade);
                                var qntDesdobrada = Convert.ToInt32(planilha.Cell($"F{l}").Value.ToString());

                                if (lstTransacaoDesdobro.Any())
                                {
                                    int fatorMultiplicador = (qntComprada + qntDesdobrada) / qntComprada;

                                    foreach (var item in lstTransacaoDesdobro)
                                    {
                                        item.Quantidade = item.Quantidade * fatorMultiplicador;
                                        item.ValorUnt = item.ValorUnt / fatorMultiplicador;
                                        item.Descricao = "D|" + planilha.Cell($"B{l}").Value.ToString() + "|";
                                        await _transacaoService.Update(item);
                                    }

                                    foreach (var item in lstDividendosDesdobro)
                                    {
                                        item.Quantidade = item.Quantidade * fatorMultiplicador;
                                        item.Descricao = "D|" + planilha.Cell($"B{l}").Value.ToString() + "|";
                                        await _dividendoService.Update(item);
                                    }
                                }
                                break;
                        }

                        //await _papelService.Add(lstPapel.Where(a => a.Id == papel.Id).FirstOrDefault());
                    }
                }
            }
        }

        public async Task ImportarExcelCotacaoAtual(HttpPostedFileBase fileB3)
        {
            using (var stream = fileB3.InputStream)
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var planilha = workbook.Worksheets.First();
                    var totalLinhas = planilha.Rows().Count();

                    for (int l = 1; l <= totalLinhas; l++)
                    {
                        string codPapel = planilha.Cell($"A{l}").Value.ToString();

                        IEnumerable<Infra.Models.Papel> Ipapel = await _papelService.Get(f => f.Codigo == codPapel);
                        Infra.Models.Papel papel = Ipapel.FirstOrDefault();

                        if (papel != null)
                        {
                            papel.CotacaoAtual = Convert.ToDouble(planilha.Cell($"B{l}").Value.ToString().Replace("R$", "").Trim());

                            await _papelService.Update(papel);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            //_baseRepository?.Dispose();
            //_baseRepository?.Dispose();
        }
    }
}