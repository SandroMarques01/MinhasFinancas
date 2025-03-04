using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;
using MinhasFinancas.Repository.Dividendo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MinhasFinancas.Repository.Login
{
	public class LoginRepository : Repository<Infra.Models.Login>, ILoginRepository
    {
        public LoginRepository(AppDbContext db) : base(db) { }
    }
}