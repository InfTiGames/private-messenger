using MediatR;
using Application.Common;
using Application.DTOs;

namespace Application.UseCases.Users;

public class RegisterUserCommand : IRequest<Result<UserDto>>
{
    public string Email { get; set; } = default!;
    public string Nickname { get; set; } = default!;
    public string Password { get; set; } = default!;
}
