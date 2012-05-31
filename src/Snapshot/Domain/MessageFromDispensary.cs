using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class MessageFromDispensary : DomainEntity
    {
        public virtual MessageFromDrugShop MessageFromDrugShop { get; set; }
        public virtual DateTime SentDate { get; set; }
        public virtual Guid OutpostId { get; set; }
        public virtual IList<Diagnosis> Diagnosises { get; set; }
        public virtual IList<Treatment> Treatments { get; set; }
        public virtual IList<Advice> Advices { get; set; }

        public MessageFromDispensary()
        {
            Diagnosises = new List<Diagnosis>();
            Treatments = new List<Treatment>();
            Advices = new List<Advice>();
        }

        public virtual void AddDiagnosis(Diagnosis diagnosis)
        {
            Diagnosises.Add(diagnosis);
        }

        public virtual void AddTreatment(Treatment treatment)
        {
            Treatments.Add(treatment);
        }

        public virtual void AddAdvice(Advice advice)
        {
            Advices.Add(advice);
        }

        public virtual void RemoveDiagnosis(Diagnosis diagnosis)
        {
            diagnosis.Messages.Remove(this);
            this.Diagnosises.Remove(diagnosis);
        }

        public virtual void RemoveTreatment(Treatment treatment)
        {
            treatment.Messages.Remove(this);
            this.Treatments.Remove(treatment);
        }

        public virtual void RemoveAdvice(Advice advice)
        {
            advice.Messages.Remove(this);
            this.Advices.Remove(advice);
        }
    }
}
