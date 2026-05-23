using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysFinance.Domain.Entities
{
    public class AssetHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime Date {  get; set; }
        public decimal Amount { get; set; }
    }
}
