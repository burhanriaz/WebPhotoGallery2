        using Microsoft.VisualBasic;
        using System.Collections.Generic;
        using System.Net.Mail;
namespace WebPhotoGallery.Service
{
        public class Email
        {
            public Email(IEnumerable<string> to, string subject, string content, bool isContentHtml, string from = null, string displayName = null)
            {
                To = to;
                From = from;
                DisplayName =  displayName;
                Subject = subject;
                Content = content;
                IsContentHtml = isContentHtml;
            }

            public IEnumerable<string> To { get; private set; }

            public string From { get; private set; }
            public string DisplayName { get; set; }
            public string Subject { get; private set; }
            public string Content { get; private set; }
            public bool IsContentHtml { get; private set; }
            public List<Attachment> Attachments { get; set; }
        }
    }

