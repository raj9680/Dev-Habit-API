using DevHabit.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.API.DTOs
{
    public class HabitsQueryParameters
    {
        [FromQuery(Name = "q")]
        public string? Search { get; set; }
        public HabitStatus? Status { get; set; }
        public HabitType? Type { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        public sealed class PaginationResult<T>
        {
            public required List<T> Items { get; init; }
            public required int Page { get; init; }
            public required int PageSize { get; init; }
            public required int TotalCount { get; init; }

            public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
            public bool HasPreviousPage => Page > 1;
            public bool HasNextPage => Page < TotalPages;

            public static async Task<PaginationResult<T>> CreateAsync(
                IQueryable<T> query, int page, int pageSize)
            {
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 100);

                int totalCount = await query.CountAsync();

                List<T> items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginationResult<T>
                {
                    Items = items,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
        }
    }
}
