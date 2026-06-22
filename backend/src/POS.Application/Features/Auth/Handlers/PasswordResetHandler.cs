// using MediatR;
// using Microsoft.Extensions.Configuration;
// using MailKit.Net.Smtp;
// using MimeKit;
// using POS.Application.Features.Auth.Commands;
// using POS.Domain.Interfaces;
// using BC = BCrypt.Net.BCrypt;

// namespace POS.Application.Features.Auth.Handlers;

// public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, bool>
// {
//     private readonly IUnitOfWork _uow;
//     private readonly IConfiguration _config;

//     public ForgotPasswordHandler(IUnitOfWork uow, IConfiguration config)
//     {
//         _uow = uow;
//         _config = config;
//     }

//     public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken ct)
//     {
//         var usuario = await _uow.Usuarios.GetByEmailAsync(request.Email);
//         if (usuario == null || !usuario.Activo) return true; // No revelar si existe

//         // Generar token
//         var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
//             .Replace("+", "-").Replace("/", "_").Replace("=", "");

//         usuario.PasswordResetToken = token;
//         usuario.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
//         _uow.Usuarios.Update(usuario);
//         await _uow.CommitAsync(ct);

//         // Mandar correo
//         var frontendUrl = _config["Frontend:Url"] ?? "http://localhost:5173";
//         var link = $"{frontendUrl}/reset-password?token={token}";

//         var message = new MimeMessage();
//         message.From.Add(MailboxAddress.Parse(_config["Email:From"]));
//         message.To.Add(MailboxAddress.Parse(request.Email));
//         message.Subject = "Recuperación de contraseña - POS Villarreal";
//         message.Body = new TextPart("html")
//         {
//             Text = $"""
//                 <h2>Recuperación de contraseña</h2>
//                 <p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p>
//                 <a href="{link}" style="background:#16a34a;color:white;padding:12px 24px;border-radius:8px;text-decoration:none;font-weight:bold;">
//                     RESTABLECER CONTRASEÑA
//                 </a>
//                 <p>Este enlace expira en <strong>15 minutos</strong>.</p>
//                 <p>Si no solicitaste esto, ignora este correo.</p>
//             """
//         };

//         using var smtp = new SmtpClient();
//         await smtp.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]!), MailKit.Security.SecureSocketOptions.StartTls, ct);
//         await smtp.AuthenticateAsync(_config["Email:From"], _config["Email:Password"], ct);
//         await smtp.SendAsync(message, ct);
//         await smtp.DisconnectAsync(true, ct);

//         return true;
//     }
// }

// public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
// {
//     private readonly IUnitOfWork _uow;

//     public ResetPasswordHandler(IUnitOfWork uow)
//     {
//         _uow = uow;
//     }

//     public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct)
//     {
//         var usuario = await _uow.Usuarios.GetByResetTokenAsync(request.Token);

//         if (usuario == null ||
//             usuario.PasswordResetTokenExpiry == null ||
//             usuario.PasswordResetTokenExpiry <= DateTime.UtcNow)
//         {
//             return false;
//         }

//         usuario.PasswordHash = BC.HashPassword(request.NuevaPassword);
//         usuario.PasswordResetToken = null;
//         usuario.PasswordResetTokenExpiry = null;
//         _uow.Usuarios.Update(usuario);
//         await _uow.CommitAsync(ct);

//         return true;
//     }
// }