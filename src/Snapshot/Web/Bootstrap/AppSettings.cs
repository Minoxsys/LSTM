using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Web.Bootstrap
{
	public class AppSettings
	{
		public static TimeSpan DefaultCacheAbsoluteTimeExpiration = TimeSpan.Parse(ConfigurationManager.AppSettings["DefaultCacheAbsoluteTimeExpiration"]);
		public static TimeSpan StaticFileHttpMaxAge = TimeSpan.Parse(ConfigurationManager.AppSettings["StaticFileHttpMaxAge"]);

		public static string ScriptsVersion = ConfigurationManager.AppSettings["ScriptsVersion"];

        public static string EmailResponseUrl = ConfigurationManager.AppSettings["EmailResponse.Url"];

        /// <summary>
        /// The URL of the SMS API used to post the SMS data.
        /// </summary>
        public static string SmsGatewayUrl = ConfigurationManager.AppSettings["SmsGateway.Url"];

        /// <summary>
        /// The SMS API account username to be used.
        /// </summary>
        public static string SmsGatewayUserName = ConfigurationManager.AppSettings["SmsGateway.UserName"];

        /// <summary>
        /// The SMS API account password to be used.
        /// </summary>
        public static string SmsGatewayPassword = ConfigurationManager.AppSettings["SmsGateway.Password"];

        public static string SmsGatewayShortcode = ConfigurationManager.AppSettings["SmsGateway.Shortcode"];

        public static string SmsGatewayKeyword = ConfigurationManager.AppSettings["SmsGateway.Keyword"];

        public static string SendEmailHost = ConfigurationManager.AppSettings["SendEmail.Host"];
        public static string SendEmailPort = ConfigurationManager.AppSettings["SendEmail.Port"];
        public static string SendEmailFrom = ConfigurationManager.AppSettings["SendEmail.From"];
        public static string SendEmailPassword = ConfigurationManager.AppSettings["SendEmail.Password"];
        public static string SendEmailTo = ConfigurationManager.AppSettings["SendEmail.To"];
        public static string SendEmailCC = ConfigurationManager.AppSettings["SendEmail.CC"];
	}
}