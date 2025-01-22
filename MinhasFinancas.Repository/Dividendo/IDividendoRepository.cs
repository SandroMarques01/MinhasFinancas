﻿using MinhasFinancas.Repository.Core;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace MinhasFinancas.Repository.Dividendo
{
    public interface IDividendoRepository : IRepository<Infra.Models.Dividendo>
    {
        Task<IEnumerable<Infra.Models.Dividendo>> RetornaTotalDividendosPorMes(int mesRetroativo, int tipoPapel = 0, Guid papelId = default);
    }
}
