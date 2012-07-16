using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ConditionManagement.Models.Appointment
{
    public class AppointmentIndexOuputModel
    {
        public AppointmentModel[] Appointment { get; set; }
        public int TotalItems { get; set; }
    }
}