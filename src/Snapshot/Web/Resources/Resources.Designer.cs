﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Web.Resources.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bado haujakwenda kwenye kituo cha afya baada ya kupewa rufaa. Tafadhali jibu kwa kuandika namba inayowakilisha hali yako:
        ///1.     Nitakwenda kesho (jibu kwa kutumia 1)
        ///2.     Nimekwishakwenda tayari (jibu kwa kutumia 2)
        ///3.     Kituo cha afya kiko mbali (jibu kwa kutumia 3)
        ///4.     Nilinunua dawa kutoka sehemu nyingine (jibu kwa kutumia 4)
        ///5.     Siumwi tena kwa sasa (jibu kwa kutumia 5)
        ///Tuma jibu lako kwa: {0}
        ///HUDUMA HII NI YA BURE.
        /// </summary>
        internal static string DidNotAttendSmsText {
            get {
                return ResourceManager.GetString("DidNotAttendSmsText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Siumwi tena kwa sasa.
        /// </summary>
        internal static string I_am_no_longer_sick {
            get {
                return ResourceManager.GetString("I_am_no_longer_sick", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nilinunua dawa kutoka sehemu nyingine.
        /// </summary>
        internal static string I_bought_drugs_from_somewhere_else {
            get {
                return ResourceManager.GetString("I_bought_drugs_from_somewhere_else", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nimekwishakwenda tayari.
        /// </summary>
        internal static string I_have_already_gone {
            get {
                return ResourceManager.GetString("I_have_already_gone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nitakwenda kesho.
        /// </summary>
        internal static string I_will_go_tomorrow {
            get {
                return ResourceManager.GetString("I_will_go_tomorrow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Simu ya mgonjwa simu si halali..
        /// </summary>
        internal static string InvalidPatientPhoneNumber {
            get {
                return ResourceManager.GetString("InvalidPatientPhoneNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Namba ya mgonjwa uliyotuma siyo sahihi, tafadhali angalia namba hiyo na utume tena..
        /// </summary>
        internal static string InvalidPatientTelphoneDescriptiveMessage {
            get {
                return ResourceManager.GetString("InvalidPatientTelphoneDescriptiveMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Umepewa rufaa. Kama utakwenda kituo cha afya utapokea muda wa maongezi kwa simu yako. Neno la siri: {0}. Zahanati: {1}.
        /// </summary>
        internal static string PatientConfirmationSmsText {
            get {
                return ResourceManager.GetString("PatientConfirmationSmsText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kituo cha afya kiko mbali.
        /// </summary>
        internal static string The_health_facility_is_too_far {
            get {
                return ResourceManager.GetString("The_health_facility_is_too_far", resourceCulture);
            }
        }
    }
}
