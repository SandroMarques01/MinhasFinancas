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

        public async Task<IEnumerable<Infra.Models.Dividendo>> RetornaTotalDividendosPorMes(string userId, int mesRetroativo, int tipoPapel = 0, Guid papelId = default)
        {
            using (SqlConnection oSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MinhasFinancasDB"].ConnectionString))
            {
                oSqlConnection.Open();

                var parameters = new { UserId = userId, MesRetroativo = mesRetroativo, TipoPapel = tipoPapel, PapelId = papelId };

                string sComando = $@"   WITH UltimosSeisMeses AS (
                                            SELECT 
                                                FORMAT(DATEADD(MONTH, -n, GETDATE()), 'MM-yyyy') AS Data,
                                                FORMAT(DATEADD(MONTH, -n, GETDATE()), 'yyyy-MM') AS Ordenacao
                                            FROM (VALUES (0), (1), (2), (3), (4), (5)) AS T(n)
                                        )
                                        SELECT 
                                            u.Data, 
                                            u.Ordenacao, 
                                            ISNULL(SUM(sub.ValorRecebido), 0) AS ValorRecebido
                                        FROM UltimosSeisMeses u
                                        LEFT JOIN (
                                            SELECT 
                                                FORMAT(d.Data, 'yyyy-MM') AS Ordenacao, 
                                                SUM(d.ValorRecebido) AS ValorRecebido
                                            FROM TbDividendo d
                                            JOIN TbPapel p ON d.PapelId = p.Id
                                            WHERE p.LoginId = @UserId
                                            AND p.TipoPapel = @TipoPapel
                                            AND CAST(CONCAT(YEAR(d.Data), '-', MONTH(d.Data), '-01') AS DATE) >= 
                                                DATEADD(MONTH, -5, CAST(CONCAT(YEAR(GETDATE()), '-', MONTH(GETDATE()), '-01') AS DATE))
                                            GROUP BY FORMAT(d.Data, 'yyyy-MM')
                                        ) sub ON u.Ordenacao = sub.Ordenacao
                                        GROUP BY u.Data, u.Ordenacao
                                        ORDER BY u.Ordenacao;
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