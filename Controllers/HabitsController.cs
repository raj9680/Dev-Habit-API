using DevHabit.API.Database;
using DevHabit.API.DTOs;
using DevHabit.API.Entities;
using FluentValidation;
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
        public async Task<ActionResult<HabitWithTagsDto>> GetHabits([FromQuery] HabitsQueryParameters habitsQueryParameter)
        {
            habitsQueryParameter.Search ??= habitsQueryParameter.Search?.Trim().ToLower();

            IQueryable<Habit> query = _dbContext.Habits;

            if (!string.IsNullOrWhiteSpace(habitsQueryParameter.Search))
            {
                query = query.Where(h => h.Name.ToLower().Contains(habitsQueryParameter.Search) ||
                                    h.Description != null && h.Description.ToLower().Contains(habitsQueryParameter.Search));
            }

            if (habitsQueryParameter.Type != null)
            {
                query = query.Where(t => t.Type == habitsQueryParameter.Type);
            }

            if(habitsQueryParameter.Status != null)
            {
                query = query.Where(q => q.Status == habitsQueryParameter.Status);
            }

            int totalCount = await query.CountAsync();
            query = query.Skip((habitsQueryParameter.Page - 1) * habitsQueryParameter.PageSize).Take(habitsQueryParameter.PageSize);

            List<HabitWithTagsDto> habits = await query.Select(HabitQueries.ProjectToDtoWithTags()).ToListAsync();

            // List<HabitsDto> habits = await query.Select(HabitQueries.ProjectToDto()).ToListAsync();


            //var paginationResult = new PaginationResult<HabitsDto>
            //{
            //    Items = habits
            //};

            if (habits is null)
            {
                return NoContent();
            }

            return Ok(habits);
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
        public async Task<ActionResult<HabitsDto>> CreateHabit(CreateHabitDto? request,
            IValidator<CreateHabitDto> validator)
        {
            await validator.ValidateAndThrowAsync(request);  // MW 2

            // var validationResult = await validator.ValidateAsync(request);

            //if (!validationResult.IsValid)
            //{
            //    var errors = validationResult.Errors
            //        .GroupBy(e => e.PropertyName)
            //        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            //    return BadRequest(errors);
            //}

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

            if (habit == null) return StatusCode(StatusCodes.Status410Gone);

            _dbContext.Habits.Remove(habit);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
