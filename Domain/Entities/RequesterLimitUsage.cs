using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RequesterLimitUsage
    {
        [Key]
        public int RequesterLimitUsageId { get; set; }
        public int RequesterId { get; set; }
        public int RuleId { get; set; }
        public RequesterLimitRule? Rule { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int UsedQuantity { get; set; }
        public DateTime LastResetAt { get; set; }
    }
}
