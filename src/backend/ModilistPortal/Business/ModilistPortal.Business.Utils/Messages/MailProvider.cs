using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using ModilistPortal.Business.Utils.DTOs;
using ModilistPortal.Business.Utils.Exceptions;
using ModilistPortal.Infrastructure.Shared.Configurations;

using Newtonsoft.Json;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace ModilistPortal.Business.Utils.Messages
{
    public interface IMailProvider
    {
        public Task SendMailAsync(SendMail sendMail, CancellationToken cancellationToken);

        public Task SendMailWithAttachmentsAsync(SendMailWithAttachments sendMailWithAttachments, CancellationToken cancellationToken);
    }

    internal class MailProvider : IMailProvider
    {
        private readonly SendGridClient _sendGridClient;

        public MailProvider(HttpClient httpClient, IOptions<SendGridOptions> options)
        {
            if (options == null || string.IsNullOrEmpty(options.Value?.APIKey))
            {
                throw new MailProviderConfigurationException("SendGridOptions object or the APIKey is possibly null. Please add the SendGridOptions with an APIKey in the appsettings.");
            }

            _sendGridClient = new SendGridClient(httpClient, new SendGridClientOptions
            {
                ApiKey = options.Value.APIKey,
            });
        }

        public async Task SendMailAsync(SendMail sendMail, CancellationToken cancellationToken)
        {
            var message = new SendGridMessage
            {
                TemplateId = sendMail.TemplateId,
                From = new EmailAddress(sendMail.From, sendMail.SenderName),
                Personalizations = new List<Personalization>
                    {
                        new Personalization
                        {
                            TemplateData = sendMail.TemplateData,
                            Tos = new List<EmailAddress>
                            {
                                new EmailAddress(sendMail.To)
                            }
                        }
                    },
            };

            var sendMailResponse = await _sendGridClient.SendEmailAsync(message, cancellationToken);

            if (!sendMailResponse.IsSuccessStatusCode)
            {
                string? reason = await sendMailResponse?.Body?.ReadAsStringAsync(cancellationToken);

                throw new SendMailFailureException((int)sendMailResponse.StatusCode, reason);
            }
        }

        public async Task SendMailWithAttachmentsAsync(SendMailWithAttachments sendMail, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(sendMail.TemplateData))
            {
                throw new ArgumentNullException(nameof(sendMail.TemplateData));
            }

            Dictionary<string, string> templateData = JsonConvert.DeserializeObject<Dictionary<string, string>>(sendMail.TemplateData);

            List<Attachment> attachments = new List<Attachment>();
            foreach (var attachment in sendMail.Attachments)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await attachment.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();
                    attachments.Add(new Attachment
                    {
                        Content = Convert.ToBase64String(bytes),
                        Disposition = "attachment",
                        Filename = attachment.FileName,
                        Type = attachment.ContentType
                    });
                }
            }

            var message = new SendGridMessage
            {
                TemplateId = sendMail.TemplateId,
                From = new EmailAddress(sendMail.From, sendMail.SenderName),
                Personalizations = new List<Personalization>
                    {
                        new Personalization
                        {
                            TemplateData = templateData,
                            Tos = new List<EmailAddress>
                            {
                                new EmailAddress(sendMail.To)
                            }
                        }
                    },
                Attachments = attachments
            };

            var sendMailResponse = await _sendGridClient.SendEmailAsync(message, cancellationToken);

            if (!sendMailResponse.IsSuccessStatusCode)
            {
                string? reason = await sendMailResponse?.Body?.ReadAsStringAsync(cancellationToken);

                throw new SendMailFailureException((int)sendMailResponse.StatusCode, reason);
            }
        }
    }
}
