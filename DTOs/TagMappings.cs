using DevHabit.API.Entities;
using System.Runtime.CompilerServices;

namespace DevHabit.API.DTOs
{
    public static class TagMappings
    {
        public static TagDto ToDto(this Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                CreatedAtUtc = tag.CreatedAtUtc,
                UpdatedAtUtc = tag.UpdatedAtUtc
            };
        }

        public static Tag ToEntity(this CreateTagDto dto)
        {
            Tag habit = new()
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAtUtc = DateTime.UtcNow
            };

            return habit;
        }

        public static void UpdateFromDto(this Tag tag, UpdateTagDto dto)
        {
            tag.Name = dto.Name;
            tag.Description = dto.Description;
            tag.UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
