using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvestmentAppProd.Models;

namespace InvestmentAppProd.Data
{
    public class InvestmentDBContext : DbContext
    {
        public DbSet<Investment> Investments { get; set; }

        public InvestmentDBContext(DbContextOptions<InvestmentDBContext> options) : base(options)
        {
        }

    }
}
