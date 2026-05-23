using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysFinance.Domain.Entities
{
    public class Income
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Discounts { get; set; }
        public string Type { get; set; } = string.Empty;

        public virtual User? User { get; set; }
    }
}
