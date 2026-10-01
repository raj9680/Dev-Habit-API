using Azure;
using DevHabit.API.Database;
using DevHabit.API.DTOs;
using DevHabit.API.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.API.Controllers
{
    [ApiController]
    [Route("habits")]
    public sealed  class HabitsController: ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public HabitsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
        {
            List<HabitsDto> habits = await _dbContext.Habits.Select(HabitQueries.ProjectToDto()).ToListAsync();

            var habitsCollectionDto = new HabitsCollectionDto
            {
                Data = habits
            };

            return Ok(habitsCollectionDto);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<HabitsDto>> GetHabit(int id)
        {
            var habit = await _dbContext
                .Habits
                .Where(h => h.Id == id)
                .Select(HabitQueries.ProjectToDto()).FirstOrDefaultAsync();

            if (habit == null)
                return NotFound();

            return Ok(habit);
        }


        [HttpPost]
        public async Task<ActionResult<HabitsDto>> CreateHabit(CreateHabitDto? request)
        {
            Habit habit = request.ToEntity();
            _dbContext.Habits.Add(habit);
            await _dbContext.SaveChangesAsync();

            HabitsDto habitDto = habit.ToDto();

            return CreatedAtAction(nameof(GetHabit), new {id = habitDto.Id}, habitDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateHabit(int id, [FromBody] UpdateHabitDto updateHabitDto)
        {
            Habit? habit = await _dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

            if (habit == null) return NotFound();

            habit.UpdateFromDto(updateHabitDto);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchHabit(int id, JsonPatchDocument<HabitsDto> patchDocument)
        {
            Habit? habit = await _dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

            if(habit == null) return NotFound();

            HabitsDto habitDto = habit.ToDto();

            patchDocument.ApplyTo(habitDto, ModelState);

            if(!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Update
            habit.Name = habitDto.Name;
            habit.Description = habitDto.Description;
            habit.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHabit(int id)
        {
            var habit = await _dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

            if (habit == null) return NotFound();

            _dbContext.Habits.Remove(habit);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
