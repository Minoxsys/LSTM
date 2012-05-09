﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.OutpostManagement.Models.Country
{
    public class CountryDictionary
    {
        public List<SelectListItem> items;

        public CountryDictionary()
        {
            //Dictionary<CountryList> items = new Dictionary<CountryList>();
        }

        public void AddCountries(List<SelectListItem>  items)
        {
         // items.Add( new SelectListItem { Value="_", Text ="-please select a country-" });
          //items.Add(new SelectListItem { Text = "Afghanistan", Value = "AF", Selected = true });
          //items.Add(new SelectListItem { Value = "AL", Text = "Albania" });
          //items.Add(new SelectListItem { Value = "DZ", Text = "Algeria" });
          //items.Add(new SelectListItem { Value = "AS", Text = "American Samoa" });
          //items.Add(new SelectListItem { Value = "AD", Text = "Andorra" });
          //items.Add(new SelectListItem { Value = "AO", Text = "Angola" });
          //items.Add( new SelectListItem { Value="AI", Text ="Anguilla"  });
          //items.Add( new SelectListItem { Value="AQ", Text ="Antarctica"  });
          //items.Add( new SelectListItem { Value="AG", Text ="Antigua and Barbuda"  });
          //items.Add( new SelectListItem { Value="AR", Text ="Argentina"  });
          //items.Add( new SelectListItem { Value="AM", Text ="Armenia"  });
          //items.Add( new SelectListItem { Value="AW", Text ="Aruba"  });
          //items.Add( new SelectListItem { Value="AU", Text ="Australia"  });
          //items.Add( new SelectListItem { Value="AT", Text ="Austria"  });
          //items.Add( new SelectListItem { Value="AZ", Text ="Azerbaijan"  });
          //items.Add( new SelectListItem { Value="BS", Text ="Bahamas"  });
          //items.Add( new SelectListItem { Value="BH", Text ="Bahrain" });
          //items.Add( new SelectListItem { Value="BD", Text ="Bangladesh" });
          //items.Add( new SelectListItem { Value="BB", Text ="Barbados" });
          //items.Add( new SelectListItem { Value="BY", Text ="Belarus" });
          //items.Add( new SelectListItem { Value="BE", Text ="Belgium" });
          //items.Add( new SelectListItem { Value="BZ", Text ="Belize" });
          //items.Add( new SelectListItem { Value="BJ", Text ="Benin" });
          //items.Add( new SelectListItem { Value="BM", Text ="Bermuda" });
          //items.Add( new SelectListItem { Value="BT", Text ="Bhutan" });
          //items.Add( new SelectListItem { Value="BO", Text ="Bolivia" });
          //items.Add( new SelectListItem { Value="BA", Text ="Bosnia and Herzegowina" });
          //items.Add( new SelectListItem { Value="BW", Text ="Botswana" });
          //items.Add( new SelectListItem { Value="BV", Text ="Bouvet Island" });
          //items.Add( new SelectListItem { Value="BR", Text ="Brazil" });
          //items.Add( new SelectListItem { Value="IO", Text ="British Indian Ocean Territory" });
          //items.Add( new SelectListItem { Value="BN", Text ="Brunei Darussalam" });
          //items.Add( new SelectListItem { Value="BG", Text ="Bulgaria" });
          //items.Add( new SelectListItem { Value="BF", Text ="Burkina Faso" });
          //items.Add( new SelectListItem { Value="BI", Text ="Burundi" });
          //items.Add( new SelectListItem { Value="KH", Text ="Cambodia" });
          //items.Add( new SelectListItem { Value="CM", Text ="Cameroon" });
          //items.Add( new SelectListItem { Value="CA", Text ="Canada" });
          //items.Add( new SelectListItem { Value="CV", Text ="Cape Verde" });
          //items.Add( new SelectListItem { Value="KY", Text ="Cayman Islands" });
          //items.Add( new SelectListItem { Value="CF", Text ="Central African Republic" });
          //items.Add( new SelectListItem { Value="TD", Text ="Chad" });
          //items.Add( new SelectListItem { Value="CL", Text ="Chile" });
          //items.Add( new SelectListItem { Value="CN", Text ="China" });
          //items.Add( new SelectListItem { Value="CX", Text ="Christmas Island" });
          //items.Add( new SelectListItem { Value="CC", Text ="Cocos (Keeling) Islands" });
          //items.Add( new SelectListItem { Value="CO", Text ="Colombia" });
          //items.Add( new SelectListItem { Value="KM", Text ="Comoros" });
          //items.Add( new SelectListItem { Value="CG", Text ="Congo" });
          //items.Add( new SelectListItem { Value="CD", Text ="Congo, the Democratic Republic of the" });
          //items.Add( new SelectListItem { Value="CK", Text ="Cook Islands" });
          //items.Add( new SelectListItem { Value="CR", Text ="Costa Rica" });
          //items.Add( new SelectListItem { Value="CI", Text ="Cote d'Ivoire" });
          //items.Add( new SelectListItem { Value="HR", Text ="Croatia (Hrvatska)" });
          //items.Add( new SelectListItem { Value="CU", Text ="Cuba" });
          //items.Add( new SelectListItem { Value="CY", Text ="Cyprus" });
          //items.Add( new SelectListItem { Value="CZ", Text ="Czech Republic" });
          //items.Add( new SelectListItem { Value="DK", Text ="Denmark" });
          //items.Add( new SelectListItem { Value="DJ", Text ="Djibouti" });
          //items.Add( new SelectListItem { Value="DM", Text ="Dominica" });
          //items.Add(new SelectListItem { Value = "DO", Text = "Dominican Republic" });
          //items.Add( new SelectListItem { Value="TP", Text ="East Timor" });
          //items.Add( new SelectListItem { Value="EC", Text ="Ecuador" });
          //items.Add( new SelectListItem { Value="EG", Text ="Egypt" });
          //items.Add( new SelectListItem { Value="SV", Text ="El Salvador" });
          //items.Add( new SelectListItem { Value="GQ", Text ="Equatorial Guinea" });
          //items.Add( new SelectListItem { Value="ER", Text ="Eritrea" });
          //items.Add( new SelectListItem { Value="EE", Text ="Estonia" });
          //items.Add( new SelectListItem { Value="ET", Text ="Ethiopia" });
          //items.Add( new SelectListItem { Value="FK", Text ="Falkland Islands (Malvinas)" });
          //items.Add( new SelectListItem { Value="FO", Text ="Faroe Islands" });
          //items.Add( new SelectListItem { Value="FJ", Text ="Fiji" });
          //items.Add( new SelectListItem { Value="FI", Text ="Finland" });
          //items.Add( new SelectListItem { Value="FR", Text ="France" });
          //items.Add( new SelectListItem { Value="FX", Text ="France, Metropolitan" });
          //items.Add( new SelectListItem { Value="GF", Text ="French Guiana" });
          //items.Add( new SelectListItem { Value="PF", Text ="French Polynesia" });
          //items.Add( new SelectListItem { Value="TF", Text ="French Southern Territories" });
          //items.Add( new SelectListItem { Value="GA", Text ="Gabon" });
          //items.Add( new SelectListItem { Value="GM", Text ="Gambia" });
          //items.Add( new SelectListItem { Value="GE", Text ="Georgia" });
          //items.Add( new SelectListItem { Value="DE", Text ="Germany" });
          //items.Add( new SelectListItem { Value="GH", Text ="Ghana" });
          //items.Add( new SelectListItem { Value="GI", Text ="Gibraltar" });
          //items.Add( new SelectListItem { Value="GR", Text ="Greece" });
          //items.Add( new SelectListItem { Value="GL", Text ="Greenland" });
          //items.Add( new SelectListItem { Value="GD", Text ="Grenada" });
          //items.Add( new SelectListItem { Value="GP", Text ="Guadeloupe" });
          //items.Add( new SelectListItem { Value="GU", Text ="Guam" });
          //items.Add( new SelectListItem { Value="GT", Text ="Guatemala" });
          //items.Add( new SelectListItem { Value="GN", Text ="Guinea" });
          //items.Add( new SelectListItem { Value="GW", Text ="Guinea-Bissau" });
          //items.Add( new SelectListItem { Value="GY", Text ="Guyana" });
          //items.Add( new SelectListItem { Value="HT", Text ="Haiti" });
          //items.Add( new SelectListItem { Value="HM", Text ="Heard and Mc Donald Islands" });
          //items.Add( new SelectListItem { Value="VA", Text ="Holy See (Vatican City State)" });
          //items.Add( new SelectListItem { Value="HN", Text ="Honduras" });
          //items.Add( new SelectListItem { Value="HK", Text ="Hong Kong" });
          //items.Add( new SelectListItem { Value="HU", Text ="Hungary" });
          //items.Add( new SelectListItem { Value="IS", Text ="Iceland" });
          //items.Add( new SelectListItem { Value="IN", Text ="India" });
          //items.Add( new SelectListItem { Value="ID", Text ="Indonesia" });
          //items.Add( new SelectListItem { Value="IR", Text ="Iran (Islamic Republic of)" });
          //items.Add( new SelectListItem { Value="IQ", Text ="Iraq" });
          //items.Add( new SelectListItem { Value="IE", Text ="Ireland" });
          //items.Add( new SelectListItem { Value="IL", Text ="Israel" });
          //items.Add( new SelectListItem { Value="IT", Text ="Italy" });
          //items.Add( new SelectListItem { Value="JM", Text ="Jamaica" });
          //items.Add( new SelectListItem { Value="JP", Text ="Japan" });
          //items.Add( new SelectListItem { Value="JO", Text ="Jordan" });
          //items.Add( new SelectListItem { Value="KZ", Text ="Kazakhstan" });
          //items.Add( new SelectListItem { Value="KE", Text ="Kenya" });
          //items.Add( new SelectListItem { Value="KI", Text ="Kiribati" });
          //items.Add( new SelectListItem { Value="KP", Text ="Korea, Democratic People's Republic of" });
          //items.Add( new SelectListItem { Value="KR", Text ="Korea, Republic of" });
          //items.Add( new SelectListItem { Value="KW", Text ="Kuwait" });
          //items.Add( new SelectListItem { Value="KG", Text ="Kyrgyzstan" });
          //items.Add( new SelectListItem { Value="LA", Text ="Lao People's Democratic Republic" });
          //items.Add( new SelectListItem { Value="LV", Text ="Latvia" });
          //items.Add( new SelectListItem { Value="LB", Text ="Lebanon" });
          //items.Add( new SelectListItem { Value="LS", Text ="Lesotho" });
          //items.Add( new SelectListItem { Value="LR", Text ="Liberia" });
          //items.Add( new SelectListItem { Value="LY", Text ="Libyan Arab Jamahiriya" });
          //items.Add( new SelectListItem { Value="LI", Text ="Liechtenstein" });
          //items.Add( new SelectListItem { Value="LT", Text ="Lithuania" });
          //items.Add( new SelectListItem { Value="LU", Text ="Luxembourg" });
          //items.Add( new SelectListItem { Value="MO", Text ="Macau" });
          //items.Add( new SelectListItem { Value="MK", Text ="Macedonia, The Former Yugoslav Republic of" });
          //items.Add( new SelectListItem { Value="MG", Text ="Madagascar" });
          //items.Add( new SelectListItem { Value="MW", Text ="Malawi" });
          //items.Add( new SelectListItem { Value="MY", Text ="Malaysia" });
          //items.Add( new SelectListItem { Value="MV", Text ="Maldives" });
          //items.Add( new SelectListItem { Value="ML", Text ="Mali" });
          //items.Add( new SelectListItem { Value="MT", Text ="Malta" });
          //items.Add( new SelectListItem { Value="MH", Text ="Marshall Islands" });
          //items.Add( new SelectListItem { Value="MQ", Text ="Martinique" });
          //items.Add( new SelectListItem { Value="MR", Text ="Mauritania" });
          //items.Add( new SelectListItem { Value="MU", Text ="Mauritius" });
          //items.Add( new SelectListItem { Value="YT", Text ="Mayotte" });
          //items.Add( new SelectListItem { Value="MX", Text ="Mexico" });
          //items.Add( new SelectListItem { Value="FM", Text ="Micronesia, Federated States of" });
          //items.Add( new SelectListItem { Value="MD", Text ="Moldova, Republic of" });
          //items.Add( new SelectListItem { Value="MC", Text ="Monaco" });
          //items.Add( new SelectListItem { Value="MN", Text ="Mongolia" });
          //items.Add( new SelectListItem { Value="MS", Text ="Montserrat" });
          //items.Add( new SelectListItem { Value="MA", Text ="Morocco" });
          //items.Add( new SelectListItem { Value="MZ", Text ="Mozambique" });
          //items.Add( new SelectListItem { Value="MM", Text ="Myanmar" });
          //items.Add( new SelectListItem { Value="NA", Text ="Namibia" });
          //items.Add( new SelectListItem { Value="NR", Text ="Nauru" });
          //items.Add( new SelectListItem { Value="NP", Text ="Nepal" });
          //items.Add( new SelectListItem { Value="NL", Text ="Netherlands" });
          //items.Add( new SelectListItem { Value="AN", Text ="Netherlands Antilles" });
          //items.Add( new SelectListItem { Value="NC", Text ="New Caledonia" });
          //items.Add( new SelectListItem { Value="NZ", Text ="New Zealand" });
          //items.Add( new SelectListItem { Value="NI", Text ="Nicaragua" });
          //items.Add( new SelectListItem { Value="NE", Text ="Niger" });
          //items.Add( new SelectListItem { Value="NG", Text ="Nigeria" });
          //items.Add( new SelectListItem { Value="NU", Text ="Niue" });
          //items.Add( new SelectListItem { Value="NF", Text ="Norfolk Island" });
          //items.Add( new SelectListItem { Value="MP", Text ="Northern Mariana Islands" });
          //items.Add( new SelectListItem { Value="NO", Text ="Norway" });
          //items.Add( new SelectListItem { Value="OM", Text ="Oman" });
          //items.Add( new SelectListItem { Value="PK", Text ="Pakistan" });
          //items.Add( new SelectListItem { Value="PW", Text ="Palau" });
          //items.Add( new SelectListItem { Value="PA", Text ="Panama" });
          //items.Add( new SelectListItem { Value="PG", Text ="Papua New Guinea" });
          //items.Add( new SelectListItem { Value="PY", Text ="Paraguay" });
          //items.Add( new SelectListItem { Value="PE", Text ="Peru" });
          //items.Add( new SelectListItem { Value="PH", Text ="Philippines" });
          //items.Add( new SelectListItem { Value="PN", Text ="Pitcairn" });
          //items.Add( new SelectListItem { Value="PL", Text ="Poland" });
          //items.Add( new SelectListItem { Value="PT", Text ="Portugal" });
          //items.Add( new SelectListItem { Value="PR", Text ="Puerto Rico" });
          //items.Add( new SelectListItem { Value="QA", Text ="Qatar" });
          //items.Add( new SelectListItem { Value="RE", Text ="Reunion" });
          //items.Add( new SelectListItem { Value="RO", Text ="Romania" });
          //items.Add( new SelectListItem { Value="RU", Text ="Russian Federation" });
          //items.Add( new SelectListItem { Value="RW", Text ="Rwanda" });
          //items.Add( new SelectListItem { Value="KN", Text ="Saint Kitts and Nevis " });
          //items.Add( new SelectListItem { Value="LC", Text ="Saint LUCIA" });
          //items.Add( new SelectListItem { Value="VC", Text ="Saint Vincent and the Grenadines" });
          //items.Add( new SelectListItem { Value="WS", Text ="Samoa" });
          //items.Add( new SelectListItem { Value="SM", Text ="San Marino" });
          //items.Add( new SelectListItem { Value="ST", Text ="Sao Tome and Principe " });
          //items.Add( new SelectListItem { Value="SA", Text ="Saudi Arabia" });
          //items.Add( new SelectListItem { Value="SN", Text ="Senegal" });
          //items.Add( new SelectListItem { Value="SC", Text ="Seychelles" });
          //items.Add( new SelectListItem { Value="SL", Text ="Sierra Leone" });
          //items.Add( new SelectListItem { Value="SG", Text ="Singapore" });
          //items.Add( new SelectListItem { Value="SK", Text ="Slovakia (Slovak Republic)" });
          //items.Add( new SelectListItem { Value="SI", Text ="Slovenia" });
          //items.Add( new SelectListItem { Value="SB", Text ="Solomon Islands" });
          //items.Add( new SelectListItem { Value="SO", Text ="Somalia" });
          //items.Add( new SelectListItem { Value="ZA", Text ="South Africa" });
          //items.Add( new SelectListItem { Value="GS", Text ="South Georgia and the South Sandwich Islands" });
          //items.Add( new SelectListItem { Value="ES", Text ="Spain" });
          //items.Add( new SelectListItem { Value="LK", Text ="Sri Lanka" });
          //items.Add( new SelectListItem { Value="SH", Text ="St. Helena" });
          //items.Add( new SelectListItem { Value="PM", Text ="St. Pierre and Miquelon" });
          //items.Add( new SelectListItem { Value="SD", Text ="Sudan" });
          //items.Add( new SelectListItem { Value="SR", Text ="Suriname" });
          //items.Add( new SelectListItem { Value="SJ", Text ="Svalbard and Jan Mayen Islands" });
          //items.Add( new SelectListItem { Value="SZ", Text ="Swaziland" });
          //items.Add( new SelectListItem { Value="SE", Text ="Sweden" });
          //items.Add( new SelectListItem { Value="CH", Text ="Switzerland" });
          //items.Add( new SelectListItem { Value="SY", Text ="Syrian Arab Republic" });
          //items.Add( new SelectListItem { Value="TW", Text ="Taiwan, Province of China" });
          //items.Add( new SelectListItem { Value="TJ", Text ="Tajikistan" });
          //items.Add( new SelectListItem { Value="TZ", Text ="Tanzania, United Republic of" });
          //items.Add( new SelectListItem { Value="TH", Text ="Thailand" });
          //items.Add( new SelectListItem { Value="TG", Text ="Togo" });
          //items.Add( new SelectListItem { Value="TK", Text ="Tokelau" });
          //items.Add( new SelectListItem { Value="TO", Text ="Tonga" });
          //items.Add( new SelectListItem { Value="TT", Text ="Trinidad and Tobago" });
          //items.Add( new SelectListItem { Value="TN", Text ="Tunisia" });
          //items.Add( new SelectListItem { Value="TR", Text ="Turkey" });
          //items.Add( new SelectListItem { Value="TM", Text ="Turkmenistan" });
          //items.Add( new SelectListItem { Value="TC", Text ="Turks and Caicos Islands" });
          //items.Add( new SelectListItem { Value="TV", Text ="Tuvalu" });
          //items.Add( new SelectListItem { Value="UG", Text ="Uganda" });
          //items.Add( new SelectListItem { Value="UA", Text ="Ukraine" });
          //items.Add( new SelectListItem { Value="AE", Text ="United Arab Emirates" });
          //items.Add( new SelectListItem { Value="GB", Text ="United Kingdom" });
          //items.Add( new SelectListItem { Value="US", Text ="United States" });
          //items.Add( new SelectListItem { Value="UM", Text ="United States Minor Outlying Islands" });
          //items.Add( new SelectListItem { Value="UY", Text ="Uruguay" });
          //items.Add( new SelectListItem { Value="UZ", Text ="Uzbekistan" });
          //items.Add( new SelectListItem { Value="VU", Text ="Vanuatu" });
          //items.Add( new SelectListItem { Value="VE", Text ="Venezuela" });
          //items.Add( new SelectListItem { Value="VN", Text ="Viet Nam" });
          //items.Add( new SelectListItem { Value="VG", Text ="Virgin Islands (British)" });
          //items.Add( new SelectListItem { Value="VI", Text ="Virgin Islands (U.S.)" });
          //items.Add( new SelectListItem { Value="WF", Text ="Wallis and Futuna Islands" });
          //items.Add( new SelectListItem { Value="EH", Text ="Western Sahara" });
          //items.Add( new SelectListItem { Value="YE", Text ="Yemen" });
          //items.Add( new SelectListItem { Value="YU", Text ="Yugoslavia" });
          //items.Add( new SelectListItem { Value="ZM", Text ="Zambia" });
          //items.Add( new SelectListItem { Value="ZW", Text ="Zimbabwe" });
    }
    }
}