using MediatR;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases.Users;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public RegisterUserHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<Result<UserDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            return Result<UserDto>.Fail("Пользователь с таким Email уже существует.");

        var passwordHash = _authService.HashPassword(request.Password);

        var newUser = new User
        {
            Email = request.Email,
            Nickname = request.Nickname,
            PasswordHash = passwordHash
        };

        await _userRepository.AddAsync(newUser, cancellationToken);

        return Result<UserDto>.Ok(
            new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Nickname = newUser.Nickname
            }
        );
    }
}
