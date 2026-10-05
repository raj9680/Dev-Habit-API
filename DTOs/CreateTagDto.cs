using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace DevHabit.API.DTOs
{
    public class CreateTagDto
    {
        [Required]
        [MinLength(3)]
        public required string Name { get; set; }

        [MaxLength(120)]
        public string? Description { get; set; }
    }

    public sealed class CreateTagDtoValidator: AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Description).MaximumLength(50);
        }
    }
}
