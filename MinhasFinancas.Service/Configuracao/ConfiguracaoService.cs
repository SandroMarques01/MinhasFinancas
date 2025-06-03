using ClosedXML.Excel;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Infra.Models.Partial;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Dividendo;
using MinhasFinancas.Service.Papel;
using MinhasFinancas.Service.Transacao;
using System;
using System.Collections.Generic;
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

        public async Task ImportarExcelB3(List<ExcelB3> lstB3, string userId)
        {
            var totalLinhas = lstB3.Count();

            #region gambiarra
            var lstP = await _papelService.Get(x => x.LoginId.ToString() == userId, includeProperties: "Transacao,Dividendo");
            var lstT = await _transacaoService.Get(x => x.Papel.LoginId.ToString() == userId, includeProperties: "Papel");
            var lstD = await _dividendoService.Get(x => x.Papel.LoginId.ToString() == userId, includeProperties: "Papel");

            List<Infra.Models.Papel> lstPapel = lstP.ToList();
            List<Infra.Models.Transacao> lstTransacao = lstT.ToList();
            List<Infra.Models.Dividendo> lstDividendo = lstD.ToList();

            lstP = null;
            lstT = null;
            lstD = null;
            #endregion

            foreach (var b3 in lstB3)
            {
                string nomeCompletoPapel = b3.Produto;
                string codigoPapel = nomeCompletoPapel.Split('-')[0].Trim();
                codigoPapel = codigoPapel.Replace("12", "11").Replace("13", "11");

                var papel = lstPapel.Where(x => x.Codigo == codigoPapel).FirstOrDefault();

                var tipoMov = b3.Movimentacao.Trim().ToUpper();

                if (papel == null && (tipoMov == "BONIFICAÇÃO EM ATIVOS" || tipoMov == "SOLICITAÇÃO DE SUBSCRIÇÃO" ||
                                      tipoMov == "TRANSFERÊNCIA - LIQUIDAÇÃO" || tipoMov == "JUROS SOBRE CAPITAL PRÓPRIO" ||
                                      tipoMov == "RENDIMENTO" || tipoMov == "DIVIDENDO" || tipoMov == "LEILÃO DE FRAÇÃO" ||
                                      tipoMov == "DESDOBRO"))
                {
                    papel = new Infra.Models.Papel();
                    papel.Nome = nomeCompletoPapel.Replace(codigoPapel + " - ", "").Trim();
                    papel.Codigo = codigoPapel;
                    papel.TipoPapel = codigoPapel.EndsWith("32") || codigoPapel.EndsWith("33") || codigoPapel.EndsWith("34") ? TipoPapel.BDR
                                        : (codigoPapel.EndsWith("11") || codigoPapel.EndsWith("12") || codigoPapel.EndsWith("13") ? TipoPapel.FII
                                        : TipoPapel.Acao);
                    papel.CotacaoAtual = 0;
                    papel.Descricao = "";
                    papel.Ativo = true;
                    papel.LoginId = new Guid(userId);

                    await _papelService.Add(papel);
                    lstPapel.Add(papel);
                }

                switch (tipoMov)
                {
                    case "BONIFICAÇÃO EM ATIVOS":
                    case "SOLICITAÇÃO DE SUBSCRIÇÃO":
                    case "TRANSFERÊNCIA - LIQUIDAÇÃO":
                        try
                        {
                            var transacao = new Infra.Models.Transacao();
                            transacao.PapelId = papel.Id;
                            string valorUnt = b3.PrecoUnitario.Trim().Replace("R$", "").Trim();
                            transacao.ValorUnt = Convert.ToDouble(valorUnt.Trim().Replace("-", "") == "" ? "0" : valorUnt);
                            string qtdT = b3.Quantidade == "" ? "0" : b3.Quantidade;
                            transacao.Quantidade = Convert.ToDouble(qtdT);
                            transacao.Data = Convert.ToDateTime(b3.Data);
                            transacao.TipoTransacao = b3.Movimentacao == "Solicitação de Subscrição" ? TipoTransacao.Compra : b3.EntradaSaida == "Credito" ? TipoTransacao.Compra : TipoTransacao.Venda;
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

                                //if (lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Transacao == null)
                                //    lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Transacao = new List<Infra.Models.Transacao>();
                                //lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Transacao.Add(transacao);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        break;
                    case "JUROS SOBRE CAPITAL PRÓPRIO":
                    case "RENDIMENTO":
                    case "DIVIDENDO":
                    case "LEILÃO DE FRAÇÃO":
                        try
                        {
                            var dividendo = new Infra.Models.Dividendo();
                            dividendo.PapelId = papel.Id;
                            dividendo.ValorRecebido = Convert.ToDouble(b3.ValorOperacao);
                            string qtdD = b3.Quantidade == "" ? "0" : b3.Quantidade;
                            dividendo.Quantidade = Convert.ToDouble(qtdD);
                            dividendo.Data = Convert.ToDateTime(b3.Data);
                            dividendo.Ativo = true;
                            dividendo.TipoDividendo = tipoMov == "DIVIDENDO" ? TipoDividendo.Dividendo
                                : tipoMov == "JUROS SOBRE CAPITAL PRÓPRIO" ? TipoDividendo.JSCP
                                : tipoMov == "RENDIMENTO" ? TipoDividendo.Rendimento
                                : TipoDividendo.Outro;


                            //Verificar se já existe dividendo
                            if (!lstDividendo.Exists(x => x.PapelId == dividendo.PapelId
                                                    && x.Data == dividendo.Data
                                                    && ((x.Quantidade == dividendo.Quantidade && x.ValorRecebido == dividendo.ValorRecebido)
                                                        || (!string.IsNullOrWhiteSpace(x.Descricao) && x.Descricao.StartsWith("D|")))))
                            {
                                await _dividendoService.Add(dividendo);
                                lstDividendo.Add(dividendo);

                                //if (lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Dividendo == null)
                                //    lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Dividendo = new List<Infra.Models.Dividendo>();
                                //lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Dividendo.Add(dividendo);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        break;
                    case "DESDOBRO":
                        try
                        {
                            DateTime dataDesdobro = Convert.ToDateTime(b3.Data);

                            var lstTransacaoDesdobro = lstTransacao.Where(x => x.PapelId == papel.Id && x.Data < dataDesdobro && (string.IsNullOrWhiteSpace(x.Descricao) || !x.Descricao.StartsWith("D|") || x.Descricao.Substring(2, 10) != b3.Data)).ToList();
                            var lstDividendosDesdobro = lstDividendo.Where(x => x.PapelId == papel.Id && x.Data < dataDesdobro && (string.IsNullOrWhiteSpace(x.Descricao) || !x.Descricao.StartsWith("D|") || x.Descricao.Substring(2, 10) != b3.Data)).ToList();

                            var qntComprada = lstTransacaoDesdobro.Sum(x => x.Quantidade);
                            var qntDesdobrada = Convert.ToInt32(b3.Quantidade);

                            if (lstTransacaoDesdobro.Any())
                            //if (lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Transacao.Where(x => x.PapelId == papel.Id && x.Data < dataDesdobro && (string.IsNullOrWhiteSpace(x.Descricao) || !x.Descricao.StartsWith("D|") || x.Descricao.Substring(2, 10) != b3.Data)).ToList().Any())
                            {
                                double fatorMultiplicador = (qntComprada + qntDesdobrada) / qntComprada;

                                foreach (var item in lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Transacao.Where(x => x.PapelId == papel.Id && x.Data < dataDesdobro && (string.IsNullOrWhiteSpace(x.Descricao) || !x.Descricao.StartsWith("D|") || x.Descricao.Substring(2, 10) != b3.Data)).ToList())
                                {
                                    //item.Papel = null;
                                    //item.Quantidade = item.Quantidade * fatorMultiplicador;
                                    //item.ValorUnt = item.ValorUnt / fatorMultiplicador;
                                    //item.Descricao = "D|" + b3.Data + "|";
                                    //await _transacaoService.Update(item);

                                    await _transacaoService.DeleteById(item.Id);

                                    var tr = new Infra.Models.Transacao();
                                    tr.PapelId = item.PapelId;
                                    tr.Quantidade = item.Quantidade * fatorMultiplicador;
                                    tr.ValorUnt = item.ValorUnt / fatorMultiplicador;
                                    tr.Descricao = "D|" + b3.Data + "|";
                                    tr.Data = item.Data;
                                    tr.TipoTransacao = item.TipoTransacao;
                                    tr.Ativo = item.Ativo;

                                    await _transacaoService.Add(tr);

                                    lstTransacao.Add(tr);
                                    lstTransacao.Remove(item);
                                }

                                foreach (var item in lstPapel.Where(x => x.Id == papel.Id).FirstOrDefault().Dividendo.Where(x => x.PapelId == papel.Id && x.Data < dataDesdobro && (string.IsNullOrWhiteSpace(x.Descricao) || !x.Descricao.StartsWith("D|") || x.Descricao.Substring(2, 10) != b3.Data)).ToList())
                                {
                                    //item.Papel = null;
                                    //item.Quantidade = item.Quantidade * fatorMultiplicador;
                                    //item.Descricao = "D|" + b3.Data + "|";
                                    //await _dividendoService.Update(item);

                                    await _dividendoService.DeleteById(item.Id);

                                    var dv = new Infra.Models.Dividendo();
                                    dv.PapelId = item.PapelId;
                                    dv.Quantidade = item.Quantidade * fatorMultiplicador;
                                    dv.ValorRecebido = item.ValorRecebido;
                                    dv.Data = item.Data;
                                    dv.TipoDividendo = item.TipoDividendo;
                                    dv.Descricao = "D|" + b3.Data + "|";
                                    dv.Ativo = item.Ativo;

                                    await _dividendoService.Add(dv);

                                    lstDividendo.Add(dv);
                                    lstDividendo.Remove(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task ImportarExcelCotacaoAtual(HttpPostedFileBase fileB3, string userId)
        {
            using (var stream = fileB3.InputStream)
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var planilha = workbook.Worksheets.First();
                    var totalLinhas = planilha.Rows().Count();

                    for (int l = 1; l <= totalLinhas; l++)
                    {
                        try
                        {
                            string codPapel = planilha.Cell($"A{l}").Value.ToString();

                            IEnumerable<Infra.Models.Papel> Ipapel = await _papelService.Get(f => f.Codigo == codPapel && f.LoginId.ToString() == userId);
                            Infra.Models.Papel papel = Ipapel.FirstOrDefault();

                            if (papel != null)
                            {
                                papel.CotacaoAtual = Convert.ToDouble(planilha.Cell($"B{l}").Value.ToString().Replace("R$", "").Trim());

                                await _papelService.Update(papel);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        public async Task DeletaTodoBanco(string userId)
        {
            await _dividendoService.DeleteAllByUser(userId);
            await _transacaoService.DeleteAllByUser(userId);
            await _papelService.DeleteAllByUser(userId);

        }

        public void Dispose()
        {
            //_baseRepository?.Dispose();
            //_baseRepository?.Dispose();
        }
    }
}