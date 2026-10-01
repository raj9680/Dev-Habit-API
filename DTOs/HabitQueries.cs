using DevHabit.API.Entities;
using System.Linq.Expressions;

namespace DevHabit.API.DTOs
{

    internal static class HabitQueries
    {
        public static Expression<Func<Habit, HabitsDto>> ProjectToDto()
        {
            return h => new HabitsDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = (HabitTypeDto)h.Type,
                Frequency = new FrequencyDto
                {
                    Type = (DTOs.FrequencyType)h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetDto
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = (HabitStatusDto)h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = h.Milestone == null ? null : new MilestoneDto
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                },
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            };
        }
    }

}
