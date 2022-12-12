using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Business.Utils.DTOs
{
    public class SendMail
    {
        public string TemplateId { get; set; }

        public string SenderName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public Dictionary<string, string> TemplateData { get; set; }
    }
}
