using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysFinance.Domain.Entities
{
    public class FixedIncomeInvestment
    {
        public Guid? Id { get; set; } = Guid.NewGuid();
        public Guid? InvestmentId { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal InterestRate { get; set; }

        public virtual Investment? Investment { get; set; }
    }
}
