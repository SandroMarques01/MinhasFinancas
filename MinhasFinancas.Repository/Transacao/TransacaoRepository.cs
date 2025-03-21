using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using Dapper;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasFinancas.Repository.Transacao
{
    public class TransacaoRepository : Repository<Infra.Models.Transacao>, ITransacaoRepository
    {
        public TransacaoRepository(AppDbContext db) : base(db) { }

        public async Task DeleteAllByUser(string userId)
        {
            using (SqlConnection oSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MinhasFinancasDB"].ConnectionString))
            {
                string sComando = $@" delete from TbTransacao where PapelId in (select Id from TbPapel where LoginId = '{userId}') ";

                oSqlConnection.Query(sComando);
                oSqlConnection.Close();
            }
        }

        //public List<Infra.Models.Transacao> RetornaTotalTransacaoPorMes(int mesRetroativo, Guid acaoId = default)
        //{
        //    using (SqlConnection oSqlConnection = new SqlConnection(GetConnectionString()))
        //    {
        //        oSqlConnection.Open();

        //        var parameters = new { mesRetroativo = mesRetroativo, acaoId = acaoId };

        //        string sComando = $@"   select FORMAT(t.Data , 'MM-yyyy') as Data, FORMAT(t.Data , 'yyyy-MM') as Ordenacao,
        //                                sum(t.ValorUnt * t.Quantidade) as ValorUnt
        //                                from TbTransacao t
        //                                where t.TipoTransacao = 1
        //                                and cast(concat(YEAR(t.Data),'-',MONTH(t.Data),'-01') as date) >= DATEADD (MONTH,-5, cast(concat(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01') as date))
        //                                group by  FORMAT(t.Data , 'MM-yyyy'), FORMAT(t.Data , 'yyyy-MM')
        //                                order by FORMAT(t.Data , 'yyyy-MM');
        //                            ";


        //        List<Infra.Models.Transacao> finishedProductReview = oSqlConnection.Query<Infra.Models.Transacao>(sComando, parameters).ToList();
        //        oSqlConnection.Close();

        //        return finishedProductReview;
        //    }
        //}

        //public List<Infra.Models.Transacao> RetornaTransacaoPorMeseTipo(int mesRetroativo)
        //{
        //    using (SqlConnection oSqlConnection = new SqlConnection(GetConnectionString()))
        //    {
        //        oSqlConnection.Open();

        //        var parameters = new { mesRetroativo = mesRetroativo };

        //        string sComando = $@"   select sum(t.ValorUnt * t.Quantidade) as ValorUnt, p.TipoPapel, FORMAT(t.Data , 'MM-yyyy') as Data, FORMAT(t.Data , 'yyyy-MM') as Ordenacao
        //                                from TbTransacao t
        //                                join TbPapel p on p.Id = t.PapelId
        //                                where cast(concat(YEAR(t.Data),'-',MONTH(t.Data),'-01') as date) >= DATEADD (MONTH,-5, cast(concat(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01') as date))
        //                                group by p.TipoPapel, FORMAT(t.Data , 'MM-yyyy'), FORMAT(t.Data , 'yyyy-MM')
        //                                order by FORMAT(t.Data , 'yyyy-MM')
        //                            ";


        //        List<Infra.Models.Transacao> finishedProductReview = oSqlConnection.Query<Infra.Models.Transacao>(sComando, parameters).ToList();
        //        oSqlConnection.Close();

        //        return finishedProductReview;
        //    }
        //}
    }
}