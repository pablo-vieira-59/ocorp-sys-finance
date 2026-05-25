using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysFinance.Domain.Entities
{
    public class VariableIncomeInvestment
    {
        public Guid? Id { get; set; } = Guid.NewGuid();
        public Guid? InvestmentId { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentQuotePrice { get; set; }
        public decimal MonthlyDividendYield { get; set; }

        public virtual Investment? Investment { get; set; }
    }
}
