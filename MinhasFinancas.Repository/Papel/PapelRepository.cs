using Dapper;
using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasFinancas.Repository.Papel
{
    public class PapelRepository : Repository<Infra.Models.Papel>, IPapelRepository
    {
        public PapelRepository(AppDbContext db) : base(db) { }

        public async Task DeleteAllByUser(string userId)
        {
            using (SqlConnection oSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MinhasFinancasDB"].ConnectionString))
            {
                string sComando = $@" delete from TbPapel where LoginId = '{userId}' ";

                oSqlConnection.Query(sComando);
                oSqlConnection.Close();
            }
        }
    }
}