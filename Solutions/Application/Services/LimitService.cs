using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    /// <summary>
    /// A kérési limitek ellenőrzéséért és felhasználásának kezeléséért felelős szolgáltatás.
    /// </summary>
    public class LimitService : ILimitService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LimitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CanUserRequestProduct(int userId, int categoryId)
        {
            var allRules = await _unitOfWork.RequesterLimitRules.GetAllAsync();
            var rule = allRules.FirstOrDefault(r => r.RequesterLimitRuleCategoryId == categoryId && r.IsActive);

            if (rule == null)
                return true;

            var (start, end) = GetPeriod(DateTime.UtcNow, rule.PeriodType);

            var allUsages = await _unitOfWork.RequesterLimitUsages.GetAllAsync();
            var usage = allUsages.FirstOrDefault(u =>
                u.RequesterId == userId &&
                u.RuleId == rule.RequesterLimitRuleId &&
                u.PeriodStart == start);

            if (usage == null)
                return true;

            if (start > usage.PeriodEnd)
                return true;

            return usage.UsedQuantity < rule.MaxQuantity;
        }

        public async Task<bool> UpdateLimitUsage(int userId, int categoryId)
        {
            return await TryConsumeLimit(userId, categoryId, 1);
        }

        public async Task<bool> TryConsumeLimit(int userId, int categoryId, int quantity = 1)
        {
            var allRules = await _unitOfWork.RequesterLimitRules.GetAllAsync();
            var rule = allRules.FirstOrDefault(r => r.RequesterLimitRuleCategoryId == categoryId && r.IsActive);

            if (rule == null)
                return true;

            var (start, end) = GetPeriod(DateTime.UtcNow, rule.PeriodType);

            var allUsages = await _unitOfWork.RequesterLimitUsages.GetAllAsync();
            var usage = allUsages.FirstOrDefault(u =>
                u.RequesterId == userId &&
                u.RuleId == rule.RequesterLimitRuleId &&
                u.PeriodStart == start);

            if (usage == null)
            {
                usage = new RequesterLimitUsage
                {
                    RequesterId = userId,
                    RuleId = rule.RequesterLimitRuleId,
                    PeriodStart = start,
                    PeriodEnd = end,
                    UsedQuantity = quantity,
                    LastResetAt = DateTime.UtcNow
                };

                await _unitOfWork.RequesterLimitUsages.AddAsync(usage);
            }
            else
            {
                if (start > usage.PeriodEnd)
                {
                    usage.PeriodStart = start;
                    usage.PeriodEnd = end;
                    usage.UsedQuantity = quantity;
                    usage.LastResetAt = start;
                }
                else
                {
                    if (usage.UsedQuantity + quantity > rule.MaxQuantity)
                        return false;

                    usage.UsedQuantity += quantity;
                }
                _unitOfWork.RequesterLimitUsages.Update(usage);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        private (DateTime start, DateTime end) GetPeriod(DateTime now, string? type)
        {
            return type?.ToLower() switch
            {
                "weekly" => (
                    now.Date.AddDays(-(int)now.DayOfWeek),
                    now.Date.AddDays(7 - (int)now.DayOfWeek)
                ),

                "monthly" => (
                    new DateTime(now.Year, now.Month, 1),
                    new DateTime(now.Year, now.Month, 1).AddMonths(1)
                ),

                "daily" => (
                    now.Date,
                    now.Date.AddDays(1)
                ),

                _ => (
                    new DateTime(now.Year, now.Month, 1),
                    new DateTime(now.Year, now.Month, 1).AddMonths(1)
                )
            };
        }

        public async Task<bool> DecreaseLimitUsage(int userId, int categoryId)
        {
            var allRules = await _unitOfWork.RequesterLimitRules.GetAllAsync();
            var rule = allRules.FirstOrDefault(r => r.RequesterLimitRuleCategoryId == categoryId && r.IsActive);

            if (rule == null) return false;

            var now = DateTime.UtcNow;

            var allUsage = await _unitOfWork.RequesterLimitUsages.GetAllAsync();

            var usage = allUsage
                .FirstOrDefault(x =>
                    x.RequesterId == userId &&
                    x.RuleId == rule.RequesterLimitRuleId &&
                    x.PeriodStart <= now &&
                    x.PeriodEnd > now);

            if (usage == null) return false;

            if (usage.UsedQuantity > 0)
                usage.UsedQuantity--;

            _unitOfWork.RequesterLimitUsages.Update(usage);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
