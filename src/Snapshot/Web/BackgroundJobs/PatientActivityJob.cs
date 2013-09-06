using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Persistence;
using Domain;
using Web.Bootstrap;
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
                    var cutoffDate = DateTime.UtcNow.AddHours(-48);
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

                    foreach (var messageFromDrugShop in referrals)
                    {
                        if (_smsService()
                            .SendMessage(string.Format(Resources.Resources.DidNotAttendSmsText, AppSettings.SmsGatewayShortcode),
                                         messageFromDrugShop.PatientPhoneNumber))
                        {
                            messageFromDrugShop.PatientReferralReminderSentDate = DateTime.UtcNow;
                            _saveDrugShopMsgCmd().Execute(messageFromDrugShop);
                        }
                    }

                });
        }

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMinutes(10); }
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