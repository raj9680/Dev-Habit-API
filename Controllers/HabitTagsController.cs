using DevHabit.API.Database;
using DevHabit.API.DTOs;
using DevHabit.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.API.Controllers
{
    [ApiController]
    [Route("habits/{habitId}/tags")]
    public sealed class HabitTagsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HabitTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // tags
        // habits/:id/tags/:tagId
        // [HttpPut("{id}/tags/{tagId}")] // http://localhost:5001/habits/2/tags
        [HttpPut]              // Update & Insert
        public async Task<ActionResult> UpsertHabitTags(int habitId, UpsertHabitTagsDto upsertHabitsTagsDto)
        {
            // Load habit with TagIds
            Habit? habit = await _context.Habits
                .Include(h => h.HabitTags)
                .FirstOrDefaultAsync(h => h.Id == habitId);

            // Check if habits exists
            if(habit is null)
            {
                return NotFound();
            }

            // check if current and upserthabits tags are same
            var currentTagIds = habit.HabitTags.Select(ht => ht.TagId).ToHashSet();
            if (currentTagIds.SetEquals(upsertHabitsTagsDto.TagIds))
            {
                return NoContent();
            }

            // check if the UpsertTagIds exists
            List<int> existingTagIds = await _context.Tags
                .Where(t => upsertHabitsTagsDto.TagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();

            if(existingTagIds.Count != upsertHabitsTagsDto.TagIds.Count)
            {
                return BadRequest("One or more tag IDs is invalid");
            }

            // remove duplicate ids that already exists
            habit.HabitTags.RemoveAll(ht => !upsertHabitsTagsDto.TagIds.Contains(ht.TagId));

            // add only the one that does not already exists
            int[] tagIdsToAdd = upsertHabitsTagsDto.TagIds.Except(currentTagIds).ToArray();

            habit.HabitTags.AddRange(tagIdsToAdd.Select(tagId => new HabitTag
            {
                HabitId = habitId,
                TagId = tagId,
                CreatedAtUtc = DateTime.UtcNow
            }));

            await _context.SaveChangesAsync();
            return Ok();
        }



        [HttpDelete("{tagId}")]
        public async Task<ActionResult> DeleteHabitTag(int habitId, int tagId)
        {
            HabitTag? habitTag = await _context.HabitTags.SingleOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);

            if(habitTag is null)
            {
                return NotFound();
            }

            _context.HabitTags.Remove(habitTag);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
