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
            var rule = (await _unitOfWork.RequesterLimitRules.FindAsync(r => r.RequesterLimitRuleCategoryId == categoryId && r.IsActive)).FirstOrDefault();

            if (rule == null)
                return true;

            var (start, end) = GetPeriod(DateTime.UtcNow, rule.PeriodType);

            var usage = (await _unitOfWork.RequesterLimitUsages.FindAsync(u =>
                u.RequesterId == userId &&
                u.RuleId == rule.RequesterLimitRuleId &&
                u.PeriodStart == start)).FirstOrDefault();

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
            var rule = (await _unitOfWork.RequesterLimitRules.FindAsync(r => r.RequesterLimitRuleCategoryId == categoryId && r.IsActive)).FirstOrDefault();

            if (rule == null)
                return true;

            var (start, end) = GetPeriod(DateTime.UtcNow, rule.PeriodType);

            var usage = (await _unitOfWork.RequesterLimitUsages.FindAsync(u =>
                u.RequesterId == userId &&
                u.RuleId == rule.RequesterLimitRuleId &&
                u.PeriodStart == start)).FirstOrDefault();

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

                "quarterly" => (
                    new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1),
                    new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1).AddMonths(3)
                ),
                "semiannual" => (
                    now.Month <= 6
                        ? new DateTime(now.Year, 1, 1)
                        : new DateTime(now.Year, 7, 1),
                    now.Month <= 6
                        ? new DateTime(now.Year, 7, 1)
                        : new DateTime(now.Year + 1, 1, 1)
                ),
                _ => (
                    new DateTime(now.Year, now.Month, 1),
                    new DateTime(now.Year, now.Month, 1).AddMonths(1)
                )
            };
        }

        public async Task<bool> DecreaseLimitUsage(int userId, int categoryId)
        {
            var rule = (await _unitOfWork.RequesterLimitRules.FindAsync(r => r.RequesterLimitRuleCategoryId == categoryId && r.IsActive)).FirstOrDefault();

            if (rule == null) return false;

            var now = DateTime.UtcNow;

            var usage = (await _unitOfWork.RequesterLimitUsages.FindAsync(x =>
                    x.RequesterId == userId &&
                    x.RuleId == rule.RequesterLimitRuleId &&
                    x.PeriodStart <= now &&
                    x.PeriodEnd > now)).FirstOrDefault();

            if (usage == null) return false;

            if (usage.UsedQuantity > 0)
                usage.UsedQuantity--;

            _unitOfWork.RequesterLimitUsages.Update(usage);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
