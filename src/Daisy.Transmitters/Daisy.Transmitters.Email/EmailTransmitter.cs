using Daisy.Resources.Interfaces;
using Daisy.Resources.Signals;
using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace Daisy.Transmitters.Email
{
    /// <summary>
    /// Transmitter for sending impulse output via email.
    /// Falls back to mock email if SMTP settings are not configured.
    /// </summary>
    public class EmailTransmitter : IExternalTransmitter
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _toEmail;

        public EmailTransmitter()
        {
            // Read SMTP settings from environment variables or use defaults
            _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? string.Empty;
            _smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587;
            _smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? string.Empty;
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? string.Empty;
            _fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM") ?? "noreply@daisy.com";
            _toEmail = Environment.GetEnvironmentVariable("EMAIL_TO") ?? "user@example.com";
        }

        public bool CanTransmit(Impulse impulse)
        {
            // Only transmit for football workflow outputs
            return impulse.Output?.Contains("footballScores", StringComparison.OrdinalIgnoreCase) == true;
        }

        public void Transmit(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            // Check if SMTP is configured
            if (string.IsNullOrWhiteSpace(_smtpHost))
            {
                MockSendEmail(impulse);
                return;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Daisy Football Workflow", _fromEmail));
                message.To.Add(new MailboxAddress("Recipient", _toEmail));
                message.Subject = "Football Scores Report";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = impulse.Output
                };

                if (!string.IsNullOrWhiteSpace(impulse.Error))
                {
                    bodyBuilder.TextBody += $"\n\nErrors:\n{impulse.Error}";
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.Connect(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(_smtpUsername, _smtpPassword);
                client.Send(message);
                client.Disconnect(true);

                System.Console.WriteLine($"Email sent to {_toEmail}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Failed to send email: {ex.Message}. Using mock email instead.");
                MockSendEmail(impulse);
            }
        }

        private void MockSendEmail(Impulse impulse)
        {
            System.Console.WriteLine("=== [Mock Email] ===");
            System.Console.WriteLine($"From: {_fromEmail}");
            System.Console.WriteLine($"To: {_toEmail}");
            System.Console.WriteLine($"Subject: Football Scores Report");
            System.Console.WriteLine($"Body:\n{impulse.Output}");
            
            if (!string.IsNullOrWhiteSpace(impulse.Error))
            {
                System.Console.WriteLine($"\nErrors:\n{impulse.Error}");
            }
            
            System.Console.WriteLine("=== [End Mock Email] ===");
        }
    }
}
