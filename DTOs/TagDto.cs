namespace DevHabit.API.DTOs
{

    public record TagsCollectionDto
    {
        public List<TagDto> Data { get; set; }
    }


    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
