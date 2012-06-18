using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.SmsRequest
{
    public class ResponseModel
    {
        public string Id { get; set; }
        public string RefId { get; set; }
        public string PhoneNumber { get; set; }
        public string ServiceNo { get; set; }
        public string Operator { get; set; }
        public string DeferDate { get; set; }
        public string Valability { get; set; }
        public string Priority { get; set; }
        public string Content { get; set; }
    }
}