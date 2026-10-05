using DevHabit.API.Entities;
using System.Linq.Expressions;

namespace DevHabit.API.DTOs
{
    public static class TagQueries
    {
        public static Expression<Func<Tag, TagDto>> ProjectToDto()
        {
            return t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CreatedAtUtc = t.CreatedAtUtc,
                UpdatedAtUtc = t.UpdatedAtUtc
            };
        }
    }
}
