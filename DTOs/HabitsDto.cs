namespace DevHabit.API.DTOs
{

    public interface PaginationResult<T>
    {
        List<T> Items { get; set; }
    }

    public sealed record HabitsCollectionDto : PaginationResult<HabitsDto>
    {
        public List<HabitsDto> Items { get; set; }
    }

    public class HabitWithTagsDto: HabitsDto
    {
        public required string[] Tags { get; set; }
    }

    public class HabitsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public HabitTypeDto Type { get; set; }
        public FrequencyDto Frequency { get; set; } = new();
        public TargetDto Target { get; set; } = new();
        public HabitStatusDto Status { get; set; }
        public bool IsArchived { get; set; }
        public DateOnly? EndDate { get; set; }
        public MilestoneDto? Milestone { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? LastCompletedAtUtc { get; set; }
    }

    public enum HabitTypeDto
    {
        None = 0,
        Binary = 1,
        Measurable = 2,
    }

    public sealed class FrequencyDto
    {
        public FrequencyType Type { get; set; }
        public int TimesPerPeriod { get; set; }
    }

    public enum FrequencyType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public sealed class TargetDto
    {
        public int Value { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public enum HabitStatusDto
    {
        None = 0,
        Ongoing = 1,
        Completed = 2
    }

    public sealed class MilestoneDto
    {
        public int Target { get; set; }
        public int Current { get; set; }
    }
}