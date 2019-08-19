using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Services.EmailService {
    public class EmailService : IEmailService {
        private readonly IOptions<EmailSettings> _options;
        private readonly ILogger<EmailService> _logger;

        public EmailService (IOptions<EmailSettings> options, ILogger<EmailService> logger) {
            this._options = options ??
                throw new ArgumentNullException (nameof (options));
            this._logger = logger ??
                throw new ArgumentNullException (nameof (logger));
        }

        public async Task SendEmailAsync (string email, string subject, string messageHtml, CancellationToken ct) {
            using (_logger.BeginScope ("Sending email scope")) {
                _logger.LogInformation (LoggingEvents.SendItem, "Send email {email} start.", email);

                var settings = _options.Value;
                var message = new MimeMessage ();

                message.From.Add (new MailboxAddress (settings.Name, settings.Email));
                message.To.Add (new MailboxAddress ("", email));
                message.Subject = subject;
                message.Body = new TextPart (MimeKit.Text.TextFormat.Html) {
                    Text = messageHtml
                };

                using (var client = new SmtpClient ()) {
                    client.ServerCertificateValidationCallback = (s,c,h,e) => true;

                    await client.ConnectAsync (settings.Server, settings.Port, SecureSocketOptions.Auto, ct);
                    await client.AuthenticateAsync (settings.Email, settings.Password, ct);
                    await client.SendAsync (message, ct);
                    await client.DisconnectAsync (true, ct);
                }

                _logger.LogInformation (LoggingEvents.SendItem, "Send email {email} success.", email);
            }
        }
    }
}