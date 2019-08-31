using System.Net;
using System.Net.Mail;

namespace Lanina.Public.Web.Api.ThirdPartyClients.Mail
{
    public class MailClient
    {
        private readonly MailSettings _mailSettings;
        public MailClient(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public void SendMail(string to, string body, string subject, string cc=null, string replyTo=null, string bcc = null, string filePath = null)
        {
            using (SmtpClient sc = new SmtpClient(_mailSettings.Host, _mailSettings.Port))
            {
                sc.EnableSsl = false;

                sc.Credentials = new NetworkCredential(_mailSettings.SenderAddress, _mailSettings.SenderPassword);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_mailSettings.SenderAddress, "La Nina Yazılım");
                    mail.To.Add(to);

                    if (!string.IsNullOrWhiteSpace(replyTo))
                    {
                        mail.ReplyToList.Add(replyTo);
                    }

                    if (!string.IsNullOrWhiteSpace(cc))
                    {
                        mail.CC.Add(cc);
                    }

                    if (!string.IsNullOrWhiteSpace(bcc))
                    {
                        mail.Bcc.Add(bcc);
                    }

                    mail.Subject = subject;
                    mail.IsBodyHtml = false;
                    mail.Body = body;

                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        mail.Attachments.Add(new Attachment(filePath));
                    }
                    
                    sc.Send(mail);
                }
            }
        }
    }
}
