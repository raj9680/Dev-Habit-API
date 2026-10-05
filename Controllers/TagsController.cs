using DevHabit.API.Database;
using DevHabit.API.DTOs;
using DevHabit.API.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.API.Controllers
{
    [ApiController]
    [Route("tags")]
    public sealed class TagsController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<TagsCollectionDto>> GetTags()
        {
            List<TagDto> tags = await _context.Tags.Select(TagQueries.ProjectToDto()).ToListAsync();

            var habitsCollectionDto = new TagsCollectionDto
            {
                Data = tags
            };

            return Ok(habitsCollectionDto);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTag(int id)
        {
            TagDto? tag = await _context.Tags.Where(t=> t.Id == id)
                .Select(TagQueries.ProjectToDto())
                .FirstOrDefaultAsync();

            if(tag is null)
            {
                return NotFound();
            }

            return Ok(tag);
        }


        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto createTagDto, IValidator<CreateTagDto> validator)
        {
            ValidationResult validationResult = await validator.ValidateAsync(createTagDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return ValidationProblem(new ValidationProblemDetails(errors));
            }

            Tag tag = createTagDto.ToEntity();

            if (await _context.Tags.AnyAsync(tag => tag.Name == createTagDto.Name))
            {
                return Problem(detail: $"The tag '{tag.Name}' already exists", 
                    statusCode: StatusCodes.Status409Conflict);
            }

            _context.Tags.Add(tag);

            await _context.SaveChangesAsync();

            TagDto tagDto = tag.ToDto();

            return CreatedAtAction(nameof(GetTag), new { id = tagDto.Id }, tagDto);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTag(int id, UpdateTagDto updateTagDto)
        {
            Tag? tag = await _context.Tags.FirstOrDefaultAsync( h => h.Id == id);

            if (tag is null)
            {
                return NotFound();
            }

            tag.UpdateFromDto(updateTagDto);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(int id)
        {
            Tag? tag = await _context.Tags.FirstOrDefaultAsync(h => h.Id == id);

            if(tag is null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
