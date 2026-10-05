using DevHabit.API.Entities;
using FluentValidation;

namespace DevHabit.API.DTOs
{
    public class CreateHabitDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public HabitTypeDto Type { get; set; }
        public FrequencyDto Frequency { get; set; } = new();
        public TargetDto Target { get; set; } = new();
        public DateOnly? EndDate { get; set; }
        public MilestoneDto? Milestone { get; set; }
    }


    public class CreateHabitDtoValidator: AbstractValidator<CreateHabitDto>
    {
        private static readonly string[] AllowedUnits =
        [
            "muntes", "hours", "steps", "km", "cal",
            "pages", "books", "tasks", "sessions"
        ];

        private static readonly string[] AllowedUnitsForBinaryHabits = ["sessions", "tasks"];

        public CreateHabitDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Habit name must be between 3 and 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(200)
                .When(x => x.Description is not null)
                .WithMessage("Message cannot exceed 500 characters");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid habit type");

            RuleFor(x => x.Frequency.Type)
                .IsInEnum()
                .WithMessage("Invalid frequency period");

            RuleFor(x => x.Frequency.TimesPerPeriod)
                .GreaterThan(0)
                .WithMessage("Frequency must be greater than 0");

            RuleFor(x => x.Target.Value)
                .GreaterThan(0)
                .WithMessage("Target value must be greater than 0");

            RuleFor(x => x.Target.Unit)
                .NotEmpty()
                .Must(unit => AllowedUnits.Contains(unit.ToLowerInvariant()))
                .WithMessage($"Unit must be one of {string.Join(", ", AllowedUnits)}");

            // End Date Validation
            RuleFor(x => x.EndDate)
                .Must(date => date is null || date.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("End date must be the future");

            // Milestone Validation
            When(x => x.Milestone is not null, () =>
            {
                RuleFor(x => x.Milestone!.Target)
                .GreaterThan(0)
                .WithMessage("Milestone target must be greater than 0");
            });

            // Complex rules
            RuleFor(x => x.Target.Unit)
                .Must((dto, unit) => IsTargetUnitCompatibleWithType(dto.Type, unit))
                .WithMessage("Target unit is not compatible with the habit type");
        }


        private static bool IsTargetUnitCompatibleWithType(HabitTypeDto type, string unit)
        {
            string normalizedUnit = unit.ToLowerInvariant();

            return type switch
            {
                // Binary habits should only use count-based units
                HabitTypeDto.Binary => AllowedUnitsForBinaryHabits.Contains(normalizedUnit),
                // Measurale habits can be use any of the allowed units
                HabitTypeDto.Measurable => AllowedUnits.Contains(normalizedUnit),
                _ => false // None is valid
            };
        }
    }


    public sealed class UpdateHabitDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public HabitTypeDto Type { get; set; }
        public FrequencyDto Frequency { get; set; } = new();
        public TargetDto Target { get; set; } = new();
        public DateOnly? EndDate { get; set; }
        public UpdateMilestoneDto? UpdateMilestone { get; set; }
    }

    public sealed class UpdateMilestoneDto
    {
        public int Target { get; set; }
    }
}
