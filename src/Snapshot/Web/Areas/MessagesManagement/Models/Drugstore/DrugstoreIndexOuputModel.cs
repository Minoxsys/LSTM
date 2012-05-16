using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.MessagesManagement.Models.Drugstore
{
    public class DrugstoreIndexOuputModel
    {
        public DrugstoreModel[] Messages { get; set; }
        public int TotalItems { get; set; }
    }
}