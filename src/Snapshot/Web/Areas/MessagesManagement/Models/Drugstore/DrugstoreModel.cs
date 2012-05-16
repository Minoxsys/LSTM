using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.MessagesManagement.Models.Drugstore
{
    public class DrugstoreModel
    {
        public string Sender { get; set; }
        public string OutpostName { get; set; }
        public string Date { get; set; }
        public string Content { get; set; }
        public bool ParseSucceeded { get; set; }
        public string ParseErrorMessage { get; set; }
    }
}