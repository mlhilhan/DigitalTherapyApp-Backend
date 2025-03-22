using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using DigitalTherapyBackendApp.Domain.Constants;
using DigitalTherapyBackendApp.Api.Features.Auth.Responses;
using DigitalTherapyBackendApp.Api.Features.Auth.Payloads;
using System.Web;
using Azure.Core;

namespace DigitalTherapyBackendApp.Api.Features.Auth.Commands
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
    {
        public ForgotPasswordPayload Payload { get; }

        public ForgotPasswordCommand(ForgotPasswordPayload payload)
        {
            Payload = payload;
        }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly string _frontendBaseUrl;

        public ForgotPasswordCommandHandler(
            UserManager<User> userManager,
            IEmailService emailService,
            ILogger<ForgotPasswordCommandHandler> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
            _frontendBaseUrl = configuration["AppSettings:FrontendBaseUrl"];
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Development
                if (WebApplication.CreateBuilder().Build().Environment.IsDevelopment())
                {
                    Console.WriteLine($"To: {request.Payload.Email}");
                    Console.WriteLine($"Subject: Digital Therapy Assistant Password Reset");
                    Console.WriteLine($"Body: <html>\r\n                    <body>\r\n                        <h2>Password Reset Request</h2>\r\n                        <p>Hello {request.Payload.Email},</p>\r\n                        <p>To reset your password, please click the link below:</p>\r\n                        <p><a href=''>Reset My Password</a></p>\r\n                        <p>This link is valid for 24 hours.</p>\r\n                        <p>If you did not make this request, please ignore this email.</p>\r\n                        <p>Best regards,<br>Digital Therapy Assistant Team</p>\r\n                    </body>\r\n                    </html>");

                    return new ForgotPasswordResponse
                    {
                        Success = true,
                        Message = "If your email is in our system, you will receive password reset instructions."
                    };
                }

                var user = await _userManager.FindByEmailAsync(request.Payload.Email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Payload.Email);
                    return new ForgotPasswordResponse
                    {
                        Success = true,
                        Message = "If your email is in our system, you will receive password reset instructions."
                    };
                }

                // Reset Password Token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);
                var resetLink = $"{_frontendBaseUrl}/reset-password?email={user.Email}&token={encodedToken}";

                string subject = "Digital Therapy Assistant Password Reset";
                string htmlBody = $@"
                    <html>
                    <body>
                        <h2>Password Reset Request</h2>
                        <p>Hello {user.UserName},</p>
                        <p>To reset your password, please click the link below:</p>
                        <p><a href='{resetLink}'>Reset My Password</a></p>
                        <p>This link is valid for 24 hours.</p>
                        <p>If you did not make this request, please ignore this email.</p>
                        <p>Best regards,<br>Digital Therapy Assistant Team</p>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(user.Email, subject, htmlBody);

                _logger.LogInformation("Password reset email sent to: {Email}", user.Email);
                return new ForgotPasswordResponse
                {
                    Success = true,
                    Message = "Password reset instructions sent to your email."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing password reset request for {Email}", request.Payload.Email);
                return new ForgotPasswordResponse
                {
                    Success = false,
                    Message = "An error occurred while processing your request.",
                    ErrorCode = ErrorCodes.PasswordResetError
                };
            }
        }
    }
}