using FluentValidation;

namespace Application.UseCases.Users;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Nickname).NotEmpty().MinimumLength(3).MaximumLength(20);

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(30);
    }
}
