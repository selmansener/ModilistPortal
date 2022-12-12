using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ModilistPortal.Business.Utils.DTOs
{
    public class SendMailWithAttachments
    {
        public string TemplateId { get; set; }

        public string SenderName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string TemplateData { get; set; }

        public IEnumerable<IFormFile> Attachments { get; set; }
    }
}
