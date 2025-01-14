using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MinhasFinancas.Repository.Dividendo
{
    public class DividendoRepository : Repository<Infra.Models.Dividendo>, IDividendoRepository
    {
        public DividendoRepository(AppDbContext db) : base(db) { }

        //public List<Infra.Models.Dividendo> RetornaTotalDividendosPorMes(int mesRetroativo, Guid acaoId = default)
        //{
        //    using (SqlConnection oSqlConnection = new SqlConnection(GetConnectionString()))
        //    {
        //        oSqlConnection.Open();

        //        var parameters = new { mesRetroativo = mesRetroativo, acaoId = acaoId };

        //        string sComando = $@"   select FORMAT(d.Data , 'MM-yyyy') as Data, FORMAT(d.Data , 'yyyy-MM') as Ordenacao,
        //                                sum(d.ValorRecebido) as ValorRecebido
        //                                from TbDividendo d
        //                                where cast(concat(YEAR(d.Data),'-',MONTH(d.Data),'-01') as date) >= DATEADD (MONTH,-5, cast(concat(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01') as date))
        //                                group by  FORMAT(d.Data , 'MM-yyyy'), FORMAT(d.Data , 'yyyy-MM')
        //                                order by FORMAT(d.Data , 'yyyy-MM')
        //                            ";


        //        List<Infra.Models.Dividendo> finishedProductReview = oSqlConnection.Query<Infra.Models.Dividendo>(sComando, parameters).ToList();
        //        oSqlConnection.Close();

        //        return finishedProductReview;
        //    }
        //}

        //public List<Infra.Models.Dividendo> RetornaDividendosPorMeseTipo(int mesRetroativo)
        //{
        //    using (SqlConnection oSqlConnection = new SqlConnection(GetConnectionString()))
        //    {
        //        oSqlConnection.Open();

        //        var parameters = new { mesRetroativo = mesRetroativo };

        //        string sComando = $@"   select sum(d.ValorRecebido) as ValorRecebido, p.TipoPapel, FORMAT(d.Data , 'MM-yyyy') as Data, FORMAT(d.Data , 'yyyy-MM') as Ordenacao
        //                                from TbDividendo d
        //                                join TbPapel p on p.Id = d.PapelId
        //                                where cast(concat(YEAR(d.Data),'-',MONTH(d.Data),'-01') as date) >= DATEADD (MONTH,-5, cast(concat(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01') as date))
        //                                group by p.TipoPapel, FORMAT(d.Data , 'MM-yyyy'), FORMAT(d.Data , 'yyyy-MM')
        //                                order by FORMAT(d.Data , 'yyyy-MM')
        //                            ";


        //        List<Infra.Models.Dividendo> finishedProductReview = oSqlConnection.Query<Infra.Models.Dividendo>(sComando, parameters).ToList();
        //        oSqlConnection.Close();

        //        return finishedProductReview;
        //    }
        //}
    }
}