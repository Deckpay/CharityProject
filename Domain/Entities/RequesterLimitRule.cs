using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class RequesterLimitRule
    {
        [Key]
        public int RequesterLimitRuleId { get; set; }
        public int RequesterLimitRuleCategoryId { get; set; }
        public string? PeriodType { get; set; }
        public int MaxQuantity { get; set; }
        public string? RequesterLimitRuleDescription { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
