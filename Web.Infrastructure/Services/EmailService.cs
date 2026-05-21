using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Web.Application.Common.Interfaces;
using Web.Domain.Primitives;
using Web.Infrastructure.TemplateMail;

namespace Web.Infrastructure.Services
{
    public class EmailSettings
    {
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPass { get; set; } = string.Empty;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<Result> SendOtpAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Tạo nội dung Email (MimeMessage)
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Mã xác thực OTP của bạn";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = EmailTemplates.GetOtpEmailTemplate(otp)
                };
                email.Body = bodyBuilder.ToMessageBody();

                // 2. Kết nối và gửi qua SMTP
                using var smtp = new SmtpClient();

                // Kiểm tra port để dùng loại bảo mật phù hợp (StartTls hoặc Ssl)
                var secureOption = _emailSettings.SmtpPort == 1025 ? SecureSocketOptions.None : (_emailSettings.SmtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

                if (string.IsNullOrWhiteSpace(_emailSettings.SmtpHost))
                {
                    throw new InvalidOperationException("SMTP host is not configured.");
                }

                await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, secureOption, cancellationToken);

                // Xác thực tài khoản gửi
                if (!string.IsNullOrWhiteSpace(_emailSettings.SmtpUser))
                {
                    await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass, cancellationToken);
                }

                // Gửi
                await smtp.SendAsync(email, cancellationToken);
                await smtp.DisconnectAsync(true, cancellationToken);

                return Result.Success("OTP email sent successfully");
            }
            catch (Exception ex)
            {
                // Trả về lỗi để Handler biết việc gửi mail thất bại
                return Error.Fail("Email.SendFailed", "Failed to send OTP");
            }
        }
    }
}
