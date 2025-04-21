using FluentValidation;
using Domain.Entities;

namespace Application.UseCases.Chats;

public class ChatValidator : AbstractValidator<Chat>
{
    public ChatValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Chat name cannot be empty");
    }
}
