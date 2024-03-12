namespace How.Core.Services.TestService.Validators;

using FluentValidation;
using Shared.DTO.Test;

public class TestPostRequestDTOValidator : AbstractValidator<TestPostRequestDTO>
{
    public TestPostRequestDTOValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(5)
            .WithMessage("Bobr!");
        
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Bobr where is Name!");
    }
}