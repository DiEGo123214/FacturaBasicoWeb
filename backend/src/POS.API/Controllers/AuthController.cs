using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Auth.Commands;

namespace POS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);

            if (response == null)
            {
                return Unauthorized(new { error = "Usuario o contraseña incorrectos." });
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);

            if (response == null)
            {
                return Unauthorized(new { error = "Token de refresco inválido o expirado." });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // [HttpPost("forgot-password")]
    // public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    // {
    //     await _mediator.Send(command);
    //     return Ok(new { message = "Si el correo existe, recibirás un enlace para restablecer tu contraseña." });
    // }

    // [HttpPost("reset-password")]
    // public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    // {
    //     var result = await _mediator.Send(command);
    //     if (!result)
    //         return BadRequest(new { error = "Token inválido o expirado." });
    //     return Ok(new { message = "Contraseña restablecida correctamente." });
    // }
}
