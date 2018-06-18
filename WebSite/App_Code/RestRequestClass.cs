using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCompany.Web
{
    public class RestRequestClass
    {
        public string from { get; set; }
        public string to { get; set; }
        public string text { get; set; }
    }

    public class RestRequestRegistration
    {
        public string memberid { get; set; }
        public string to { get; set; }
        public string text { get; set; }
    }

    public class RestRequestClassForReports
    {
        public string messageId { get; set; }
        
    }
    public class RestResponseClass
    {
        public string to { get; set; }
       
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int statusid { get; set; }
        public string StatusName { get; set; }
        public string description { get; set; }
        public int smsCount { get; set; }
        public string messageId { get; set; }
    }
}