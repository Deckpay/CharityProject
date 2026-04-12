namespace Application.DTOs
{
    public class RequesterLimitRuleDto
    {
        public int RequesterLimitRuleId { get; set; }
        public int RequesterLimitRuleCategoryId { get; set; }
        public string? PeriodType { get; set; }
        public int MaxQuantity { get; set; }
        public string? RequesterLimitRuleDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
