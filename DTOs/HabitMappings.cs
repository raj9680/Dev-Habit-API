using DevHabit.API.Entities;
using System.Linq.Expressions;

namespace DevHabit.API.DTOs
{
    public static class HabitMappings
    {
        public static HabitsDto ToDto(this Habit h)
        {
            return new HabitsDto
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


        public static Habit ToEntity(this CreateHabitDto dto)
        {
            Habit habit = new()
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = (HabitType)dto.Type,
                Frequency = new Frequency
                {
                    Type = (Entities.FrequencyType)dto.Frequency.Type,
                    TimesPerPeriod = dto.Frequency.TimesPerPeriod
                },
                Target = new Target
                {
                    Value = dto.Target.Value,
                    Unit = dto.Target.Unit
                },
                Status = HabitStatus.Ongoing,
                IsArchived = false,
                EndDate = dto.EndDate,
                Milestone = dto.Milestone is not null
                    ? new Milestone
                    {
                        Target = dto.Milestone.Target,
                        Current = 0
                    }
                    : null,
                CreatedAtUtc = DateTime.UtcNow
            };

            return habit;
        }

        public static void UpdateFromDto(this Habit habit, UpdateHabitDto dto)
        {
            // Update basic properties
            habit.Name = dto.Name;
            habit.Description = dto.Description;
            habit.Type = (HabitType)dto.Type;
            habit.EndDate = dto.EndDate;

            // Update frequency (assuming it's immutable, create new instance)
            habit.Frequency = new Frequency
            {
                Type = (Entities.FrequencyType)dto.Frequency.Type,
                TimesPerPeriod = dto.Frequency.TimesPerPeriod
            };

            // Update Target 
            habit.Target = new Target
            {
                Value = dto.Target.Value,
                Unit = dto.Target.Unit
            };

            // Update Milestone if Provided
            if(dto.UpdateMilestone != null)
            {
                habit.Milestone = habit.Milestone ?? new Milestone();
                habit.Milestone.Target = dto.UpdateMilestone.Target;
            }

            habit.UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
