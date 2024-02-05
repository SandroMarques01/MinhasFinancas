using ClosedXML.Excel;
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

                        var papel = lstPapel.Where(x => x.Codigo == codigoPapel).FirstOrDefault();

                        if (papel == null)
                        {
                            papel = new Infra.Models.Papel();
                            papel.Nome = nomeCompletoPapel.Replace(codigoPapel + " - ", "").Trim();
                            papel.Codigo = codigoPapel;
                            papel.TipoPapel = codigoPapel.Contains("34") ? TipoPapel.BDR : (codigoPapel.Contains("11") || codigoPapel.Contains("13") ? TipoPapel.Fii : TipoPapel.Acao);
                            papel.CotacaoAtual = 0;
                            papel.Descricao = "";
                            papel.Ativo = true;

                            await _papelService.Add(papel);
                            lstPapel.Add(papel);
                        }

                        switch (planilha.Cell($"C{l}").Value.ToString())
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

                                //if (lstTransacao.Where(x => x.PapelId == transacao.PapelId
                                //                        && x.Data == transacao.Data
                                //                        && x.Quantidade == transacao.Quantidade).Count() == 0)
                                //{
                                    await _transacaoService.Add(transacao);
                                    lstTransacao.Add(transacao);
                                //}
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
                                dividendo.Descricao = planilha.Cell($"C{l}").Value.ToString();
                                dividendo.Ativo = true;

                                //Verificar se já existe dividendo

                                //if (lstDividendo.Where(x => x.PapelId == dividendo.PapelId
                                //                        && x.Data == dividendo.Data
                                //                        && x.Quantidade == dividendo.Quantidade).Count() == 0)
                                //{
                                    await _dividendoService.Add(dividendo);
                                    lstDividendo.Add(dividendo);
                                //}
                                break;
                            case "Desdobro":
                                var lstTransacaoDesdobro = lstTransacao.Where(x => x.PapelId == papel.Id).ToList();
                                var lstDividendosDesdobro = lstDividendo.Where(x => x.PapelId == papel.Id).ToList();

                                var qntComprada = lstTransacaoDesdobro.Sum(x => x.Quantidade);
                                var qntDesdobrada = Convert.ToInt32(planilha.Cell($"F{l}").Value.ToString());
                                int fatorMultiplicador = (qntComprada + qntDesdobrada) / qntComprada;

                                lstTransacaoDesdobro.ForEach(x =>
                                {
                                    x.Quantidade = x.Quantidade * fatorMultiplicador;
                                    x.ValorUnt = x.ValorUnt / fatorMultiplicador;
                                    _transacaoService.Update(x);
                                });

                                lstDividendosDesdobro.ForEach(x =>
                                {
                                    x.Quantidade = x.Quantidade * fatorMultiplicador;
                                    _dividendoService.Update(x);
                                });
                                break;
                        }

                        //await _papelService.Add(lstPapel.Where(a => a.Id == papel.Id).FirstOrDefault());
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