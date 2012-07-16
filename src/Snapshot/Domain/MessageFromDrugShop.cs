using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class MessageFromDrugShop : DomainEntity
    {
        public virtual string Initials { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual string Gender { get; set; }
        public virtual Guid OutpostId { get; set; }
        public virtual DateTime SentDate { get; set; }
        public virtual string IDCode { get; set; }
        public virtual IList<Condition> ServicesNeeded { get; set; }
        public virtual IList<Appointment> Appointments { get; set; }

        public MessageFromDrugShop()
        {
            ServicesNeeded = new List<Condition>();
            Appointments = new List<Appointment>();
        }

        public virtual void AddCondition(Condition service)
        {
            ServicesNeeded.Add(service);
        }

        public virtual void RemoveCondition(Condition service)
        {
            service.Messages.Remove(this);
            this.ServicesNeeded.Remove(service);
        }

        public virtual void AddAppointment(Appointment service)
        {
            Appointments.Add(service);
        }

        public virtual void RemoveAppointment(Appointment service)
        {
            service.Messages.Remove(this);
            this.Appointments.Remove(service);
        }

    }
}
