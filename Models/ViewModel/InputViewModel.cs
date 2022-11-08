using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebHome.Models.Locale;

namespace WebHome.Models.ViewModel
{
    public class InputViewModel
    {
        public String Name { get; set; }
        public String Id { get; set; }
        public String Label { get; set; }
        public String Value { get; set; }
        public String PlaceHolder { get; set; }
        public String ErrorMessage { get; set; }
        public bool? IsValid { get; set; }
        public String InputType { get; set; }
        public Object DefaultValue { get; set; }
        public String ButtonStyle { get; set; }
        public String IconStyle { get; set; }
        public String Href { get; set; }
    }

    public class UserProfileQueryViewModel : QueryViewModel
    {
        public int? UID { get; set; }
        public String UserName { get; set; }
        public String ThemeName { get; set; }
        public String RealName { get; set; }
        public String PID { get; set; }
        public String Phone { get; set; }
        public String Call
        {
            get
            {
                return RealName;
            }
            set
            {
                RealName = value;
            }
        }
        public String Caller { get; set; }
        public String SipUrl
        {
            get
            {
                return RealName;
            }
            set
            {
                RealName = value;
            }
        }

        public Naming.SensorType? Sensor { get; set; }
        public int? Io { get; set; }
    }
}