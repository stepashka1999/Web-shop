using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Web_shop.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly MailJetSettings _settings;

        public EmailSender(IConfiguration configuration)
        {
            _settings = configuration.GetSection("MailJet").Get<MailJetSettings>();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new MailjetClient(_settings.ApiKey, _settings.SecretKey)
            {
                Version = ApiVersion.V3_1
            };
            var request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.Messages, new JArray 
            {
                new JObject 
                {
                    {
                        "From",
                        new JObject {
                            {"Email", "199stepashka199@gmail.com"},
                            {"Name", "Stepan"}
                        }
                    }, 
                    {
                        "To",
                        new JArray
                        {
                            new JObject 
                            {
                                {
                                    "Email",
                                    email
                                }, 
                                {
                                    "Name",
                                    "WebShop"
                                }
                            }
                        }
                    },
                    {
                        "Subject",
                        subject
                    },
                    {
                        "HTMLPart",
                        htmlMessage
                    }
                }
             });

            var _ = await client.PostAsync(request);
        }
    }
}
