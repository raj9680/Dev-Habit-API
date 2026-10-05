namespace DevHabit.API.DTOs
{
    public class UpdateTagDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
