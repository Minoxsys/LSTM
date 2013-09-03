using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Persistence;
using Domain;
using Web.Services;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class PatientActivityJob : IJob
    {
        private const string ActivityJobName = "PatientActivityJob";
        private readonly Func<IQueryService<MessageFromDrugShop>> _drugShopMsgQuery;
        private readonly Func<ISmsRequestService> _smsService;
        private readonly Func<ISaveOrUpdateCommand<MessageFromDrugShop>> _saveDrugShopMsgCmd;

        public PatientActivityJob(Func<IQueryService<MessageFromDrugShop>> drugShopMsgQuery, Func<ISaveOrUpdateCommand<MessageFromDrugShop>> saveDrugShopMsgCmd,
                                  Func<ISmsRequestService> smsService)
        {
            _saveDrugShopMsgCmd = saveDrugShopMsgCmd;
            _smsService = smsService;
            _drugShopMsgQuery = drugShopMsgQuery;
        }

        public Task Execute()
        {
            return new Task(() =>
                {

                    _smsService().SendMessage("begin of task", "_");

                    var cutoffDate = DateTime.UtcNow.AddMinutes(-12);
                    List<MessageFromDrugShop> referrals =
                        _drugShopMsgQuery()
                            .Query()
                            .Where(
                                m => m.PatientReferralConsumed == false && //did not went to the health facility
                                     m.PatientPhoneNumber != null && //is a recent entry (after this change request upgrade)
                                     m.ReminderAnswer == 0 && //it is not answered already
                                     m.PatientReferralReminderSentDate == null &&
                                     m.SentDate < cutoffDate // the message is older than 48 hours
                            ).ToList();

                    _smsService().SendMessage("ref cnt=" + referrals.Count.ToString(), "___");

                    foreach (var messageFromDrugShop in referrals)
                    {
                        if (_smsService().SendMessage(Resources.Resources.DidNotAttendSmsText, messageFromDrugShop.PatientPhoneNumber))
                        {
                            messageFromDrugShop.PatientReferralReminderSentDate = DateTime.UtcNow;
                            _saveDrugShopMsgCmd().Execute(messageFromDrugShop);
                        }
                    }

                });
        }

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMinutes(2); }
        }

        public string Name
        {
            get { return ActivityJobName; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(10); }
        }
    }
}