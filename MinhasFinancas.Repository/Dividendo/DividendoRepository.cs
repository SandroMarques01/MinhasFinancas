using Dapper;
using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MinhasFinancas.Repository.Dividendo
{
    public class DividendoRepository : Repository<Infra.Models.Dividendo>, IDividendoRepository
    {
        public DividendoRepository(AppDbContext db) : base(db) { }

        public async Task<IEnumerable<Infra.Models.Dividendo>> RetornaTotalDividendosPorMes(int mesRetroativo, int tipoPapel = 0, Guid papelId = default)
        {
            using (SqlConnection oSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MinhasFinancasDB"].ConnectionString))
            {
                oSqlConnection.Open();

                var parameters = new { @MesRetroativo = mesRetroativo, TipoPapel = tipoPapel, PapelId = papelId };

                string sComando = $@"   select FORMAT(d.Data , 'MM-yyyy') as Data, FORMAT(d.Data , 'yyyy-MM') as Ordenacao,
                                        sum(d.ValorRecebido) as ValorRecebido
                                        from TbDividendo d
                                        join TbPapel p on d.PapelId = p.Id
                                        where cast(concat(YEAR(d.Data),'-',MONTH(d.Data),'-01') as date) >= DATEADD (MONTH,-@MesRetroativo, cast(concat(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01') as date))
                                    ";
                if(tipoPapel > 0)
                {
                    sComando += $@"     and tipoPapel = @TipoPapel ";
                }
                if (papelId != default)
                {
                    sComando += $@"     and d.PapelId = @PapelId ";
                }

                sComando += $@"         group by  FORMAT(d.Data , 'MM-yyyy'), FORMAT(d.Data , 'yyyy-MM')
                                        order by FORMAT(d.Data , 'yyyy-MM')
                                    ";


                List<Infra.Models.Dividendo> finishedProductReview = oSqlConnection.Query<Infra.Models.Dividendo>(sComando, parameters).ToList();
                oSqlConnection.Close();

                return finishedProductReview;
            }
        }

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