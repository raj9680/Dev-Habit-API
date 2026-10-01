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
