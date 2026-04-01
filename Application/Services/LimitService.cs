using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
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
                // Ez teljesen új rekord, használj AddAsync-t
                usage = new RequesterLimitUsage
                {
                    RequesterId = userId,
                    RuleId = rule.RequesterLimitRuleId,
                    PeriodStart = start,
                    PeriodEnd = end,
                    UsedQuantity = quantity, // azonnal hozzáadjuk a quantity-t
                    LastResetAt = DateTime.UtcNow
                };

                await _unitOfWork.RequesterLimitUsages.AddAsync(usage);
            }
            else
            {
                // ellenőrizzük, hogy lejárt-e a periódus
                if (start > usage.PeriodEnd)
                {
                    usage.PeriodStart = start;
                    usage.PeriodEnd = end;
                    usage.UsedQuantity = quantity; // reset + új mennyiség
                    usage.LastResetAt = start;
                }
                else
                {
                    // Perióduson belül vagyunk
                    if (usage.UsedQuantity + quantity > rule.MaxQuantity)
                        return false; // limit elérve

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
    }
}
