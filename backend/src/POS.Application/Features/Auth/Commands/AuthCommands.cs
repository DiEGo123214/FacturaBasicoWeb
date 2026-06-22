using MediatR;

namespace POS.Application.Features.Auth.Commands;

public record LoginCommand(string Username, string Password) : IRequest<LoginResponse?>;

public record LoginResponse(string Token, string RefreshToken, int UserId, string Username, string Nombre, string Apellido, string Role);

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<LoginResponse?>;

// public record ForgotPasswordCommand(string Email) : IRequest<bool>;

// public record ResetPasswordCommand(string Token, string NuevaPassword) : IRequest<bool>;