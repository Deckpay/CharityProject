using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RequesterLimitRuleDto
    {
        public int RequesterLimitRuleId { get; set; }
        public int RequesterLimitRuleCategoryId { get; set; }
        public string? PeriodType { get; set; } // 'Daily', 'Monthly', 'Weekly'
        public int MaxQuantity { get; set; }
        public string? RequesterLimitRuleDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
