using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using MyCompany.Data;
using MyCompany.Security;

namespace MyCompany.Services
{
	public partial class EnterpriseApplicationServices : EnterpriseApplicationServicesBase
    {
    }
    
    public class EnterpriseApplicationServicesBase : ApplicationServicesBase
    {
        
        public static Regex AppServicesRegex = new Regex("/appservices/(?\'Controller\'\\w+?)(/|$)", RegexOptions.IgnoreCase);
        
        public static Regex DynamicResourceRegex = new Regex("(\\.js$|^_(invoke|authenticate)$)", RegexOptions.IgnoreCase);
        
        public static Regex DynamicWebResourceRegex = new Regex("\\.(js|css)$", RegexOptions.IgnoreCase);
        
        public override void RegisterServices()
        {
            RegisterREST();
            base.RegisterServices();
            OAuthHandlerFactory.Handlers.Add("facebook", typeof(FacebookOAuthHandler));
            OAuthHandlerFactory.Handlers.Add("google", typeof(GoogleOAuthHandler));
            OAuthHandlerFactory.Handlers.Add("msgraph", typeof(MSGraphOAuthHandler));
            OAuthHandlerFactory.Handlers.Add("windowslive", typeof(WindowsLiveOAuthHandler));
            OAuthHandlerFactory.Handlers.Add("sharepoint", typeof(SharePointOAuthHandler));
            OAuthHandlerFactory.Handlers.Add("identityserver", typeof(IdentityServerOAuthHandler));
        }
        
        public virtual void RegisterREST()
        {
            RouteCollection routes = RouteTable.Routes;
            routes.RouteExistingFiles = true;
            GenericRoute.Map(routes, new RepresentationalStateTransfer(), "appservices/{Controller}/{Segment1}/{Segment2}/{Segment3}/{Segment4}");
            GenericRoute.Map(routes, new RepresentationalStateTransfer(), "appservices/{Controller}/{Segment1}/{Segment2}/{Segment3}");
            GenericRoute.Map(routes, new RepresentationalStateTransfer(), "appservices/{Controller}/{Segment1}/{Segment2}");
            GenericRoute.Map(routes, new RepresentationalStateTransfer(), "appservices/{Controller}/{Segment1}");
            GenericRoute.Map(routes, new RepresentationalStateTransfer(), "appservices/{Controller}");
        }
        
        public override bool RequiresAuthentication(HttpRequest request)
        {
            bool result = base.RequiresAuthentication(request);
            if (result)
            	return true;
            Match m = AppServicesRegex.Match(request.Path);
            if (m.Success)
            {
                ControllerConfiguration config = null;
                try
                {
                    string controllerName = m.Groups["Controller"].Value;
                    if ((controllerName == "_authenticate") || (controllerName == "saas"))
                    	return false;
                    if (!(DynamicResourceRegex.IsMatch(controllerName)))
                    	config = DataControllerBase.CreateConfigurationInstance(GetType(), controllerName);
                }
                catch (Exception )
                {
                }
                if (config == null)
                	return !(DynamicWebResourceRegex.IsMatch(request.Path));
                return RequiresRESTAuthentication(request, config);
            }
            return false;
        }
        
        public virtual bool RequiresRESTAuthentication(HttpRequest request, ControllerConfiguration config)
        {
            return UriRestConfig.RequiresAuthentication(request, config);
        }
    }
    
    public class ScheduleStatus
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _schedule;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _exceptions;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _success;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime _nextTestDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _expired;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _precision;
        
        /// The definition of the schedule.
        public virtual string Schedule
        {
            get
            {
                return this._schedule;
            }
            set
            {
                this._schedule = value;
            }
        }
        
        /// The defintion of excepetions to the schedule. Exceptions are expressed as another schedule.
        public virtual string Exceptions
        {
            get
            {
                return this._exceptions;
            }
            set
            {
                this._exceptions = value;
            }
        }
        
        /// True if the schedule is valid at this time.
        public virtual bool Success
        {
            get
            {
                return this._success;
            }
            set
            {
                this._success = value;
            }
        }
        
        /// The next date and time when the schedule is invalid.
        public virtual DateTime NextTestDate
        {
            get
            {
                return this._nextTestDate;
            }
            set
            {
                this._nextTestDate = value;
            }
        }
        
        /// True if the schedule has expired. For internal use only.
        public virtual bool Expired
        {
            get
            {
                return this._expired;
            }
            set
            {
                this._expired = value;
            }
        }
        
        /// The precision of the schedule. For internal use only.
        public virtual string Precision
        {
            get
            {
                return this._precision;
            }
            set
            {
                this._precision = value;
            }
        }
    }
    
    public partial class Scheduler : SchedulerBase
    {
    }
    
    public class SchedulerBase
    {
        
        public static Regex NodeMatchRegex = new Regex("(?\'Depth\'\\++)\\s*(?\'NodeType\'\\S+)\\s*(?\'Properties\'[^\\+]*)");
        
        public static Regex PropertyMatchRegex = new Regex("\\s*(?\'Name\'[a-zA-Z]*)\\s*[:=]?\\s*(?\'Value\'.+?)(\\n|;|$)");
        
        private static string[] _nodeTypes = new string[] {
                "yearly",
                "monthly",
                "weekly",
                "daily",
                "once"};
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime _testDate;
        
        public virtual DateTime TestDate
        {
            get
            {
                return this._testDate;
            }
            set
            {
                this._testDate = value;
            }
        }
        
        public virtual bool UsePreciseProbe
        {
            get
            {
                return false;
            }
        }
        
        /// Check if a free form text schedule is valid now.
        public static ScheduleStatus Test(string schedule)
        {
            return Test(schedule, null, DateTime.Now);
        }
        
        /// Check if a free form text schedule is valid on the testDate.
        public static ScheduleStatus Test(string schedule, DateTime testDate)
        {
            return Test(schedule, null, testDate);
        }
        
        /// Check if a free form text schedule with exceptions is valid now.
        public static ScheduleStatus Test(string schedule, string exceptions)
        {
            return Test(schedule, exceptions, DateTime.Now);
        }
        
        /// Check if a free form text schedule with exceptions is valid on the testDate.
        public static ScheduleStatus Test(string schedule, string exceptions, DateTime testDate)
        {
            Scheduler s = new Scheduler();
            s.TestDate = testDate;
            ScheduleStatus status = s.CheckSchedule(schedule, exceptions);
            status.Schedule = schedule;
            status.Exceptions = exceptions;
            return status;
        }
        
        public virtual ScheduleStatus CheckSchedule(string schedule)
        {
            return CheckSchedule(StringToXml(schedule), null);
        }
        
        public virtual ScheduleStatus CheckSchedule(string schedule, string exceptions)
        {
            return CheckSchedule(StringToXml(schedule), StringToXml(exceptions));
        }
        
        /// Check an XML schedule.
        public virtual ScheduleStatus CheckSchedule(Stream schedule)
        {
            return CheckSchedule(schedule, null);
        }
        
        /// Check an XML schedule with exceptions.
        public virtual ScheduleStatus CheckSchedule(Stream schedule, Stream exceptions)
        {
            ScheduleStatus sched = new ScheduleStatus();
            sched.Precision = String.Empty;
            ScheduleStatus xSched = new ScheduleStatus();
            xSched.Precision = String.Empty;
            XPathNavigator nav = null;
            XPathNavigator xNav = null;
            if ((schedule == null) || schedule.Equals(Stream.Null))
            	sched.Success = true;
            else
            {
                XPathDocument doc = new XPathDocument(schedule);
                nav = doc.CreateNavigator();
                if (!(nav.MoveToChild(XPathNodeType.Element)) || nav.Name != "schedule")
                	sched.Success = true;
                else
                	CheckNode(nav, DateTime.Now, ref sched);
            }
            if ((exceptions != null) && !(exceptions.Equals(Stream.Null)))
            {
                XPathDocument xDoc = new XPathDocument(exceptions);
                xNav = xDoc.CreateNavigator();
                if (!(xNav.MoveToChild(XPathNodeType.Element)) || xNav.Name != "schedule")
                	xSched.Success = true;
                else
                	CheckNode(xNav, DateTime.Now, ref xSched);
            }
            if (xSched.Success)
            	sched.Success = false;
            if (UsePreciseProbe)
            	sched = ProbeScheduleExact(nav, xNav, sched, xSched);
            else
            	sched = ProbeSchedule(nav, xNav, sched, xSched);
            return sched;
        }
        
        /// Converts plain text schedule format into XML stream.
        private Stream StringToXml(string text)
        {
            if (String.IsNullOrEmpty(text))
            	return null;
            // check for shorthand "start"
            DateTime testDate = DateTime.Now;
            if (DateTime.TryParse(text, out testDate))
            	String.Format("+once start: {0}", text);
            // compose XML document
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(dec);
            XmlNode schedule = doc.CreateNode(XmlNodeType.Element, "schedule", null);
            doc.AppendChild(schedule);
            // configure nodes
            MatchCollection nodes = NodeMatchRegex.Matches(text);
            XmlNode lastNode = schedule;
            int lastDepth = 0;
            foreach (Match node in nodes)
            {
                string nodeType = node.Groups["NodeType"].Value;
                int depth = node.Groups["Depth"].Value.Length;
                string properties = node.Groups["Properties"].Value;
                if (_nodeTypes.Contains(nodeType))
                {
                    XmlNode newNode = doc.CreateNode(XmlNodeType.Element, nodeType, null);
                    MatchCollection propertyMatches = PropertyMatchRegex.Matches(node.Groups["Properties"].Value);
                    // populate attributes
                    foreach (Match property in propertyMatches)
                    {
                        string name = property.Groups["Name"].Value.Trim();
                        string val = property.Groups["Value"].Value.Trim();
                        // group value
                        if (String.IsNullOrEmpty(name))
                        	name = "value";
                        XmlAttribute attr = doc.CreateAttribute(name);
                        attr.Value = val;
                        newNode.Attributes.Append(attr);
                    }
                    // insert node
                    if (depth > lastDepth)
                    	lastNode.AppendChild(newNode);
                    else
                    	if (depth < lastDepth)
                        {
                            while (lastNode.Name != "schedule" && lastNode.Name != nodeType)
                            	lastNode = lastNode.ParentNode;
                            if (lastNode.Name == nodeType)
                            	lastNode = lastNode.ParentNode;
                            lastNode.AppendChild(newNode);
                        }
                        else
                        	lastNode.ParentNode.AppendChild(newNode);
                    lastNode = newNode;
                    lastDepth = depth;
                }
            }
            // save and return
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;
            return stream;
        }
        
        /// Checks the current navigator if the current nodes define an active schedule. An empty schedule will set Match to true.
        private bool CheckNode(XPathNavigator nav, DateTime checkDate, ref ScheduleStatus sched)
        {
            if (nav == null)
            	return false;
            sched.Precision = nav.Name;
            if (!(nav.MoveToFirstChild()))
            {
                // no schedule limitation
                sched.Success = true;
                return true;
            }
            while (true)
            {
                // ignore comments
                if (!(nav.NodeType.Equals(XPathNodeType.Comment)))
                {
                    string name = nav.Name;
                    if (name == "once")
                    {
                        if (CheckInterval(nav, checkDate))
                        	sched.Success = true;
                    }
                    else
                    	if (CheckInterval(nav, checkDate))
                        {
                            string value = nav.GetAttribute("value", String.Empty);
                            string every = nav.GetAttribute("every", String.Empty);
                            int check = 0;
                            if (name == "yearly")
                            	check = checkDate.Year;
                            else
                            	if (name == "monthly")
                                	check = checkDate.Month;
                                else
                                	if (name == "weekly")
                                    	check = GetWeekOfMonth(checkDate);
                                    else
                                    	if (name == "daily")
                                        	check = ((int)(checkDate.DayOfWeek));
                            if (CheckNumberInterval(value, check, every))
                            	CheckNode(nav, checkDate, ref sched);
                        }
                    // found a match
                    if (sched.Expired || sched.Success)
                    	break;
                }
                // no more nodes
                if (!(nav.MoveToNext()))
                	break;
            }
            return sched.Success;
        }
        
        /// Checks to see if a series of comma-separated numbers and/or dash-separated intervals contain a specific number
        private bool CheckNumberInterval(string interval, int number, string every)
        {
            if (String.IsNullOrEmpty(interval))
            	return true;
            // process numbers and number ranges
            string[] strings = interval.Split(',');
            List<int> numbers = new List<int>();
            foreach (string str in strings)
            	if (str.Contains('-'))
                {
                    string[] intervalString = str.Split('-');
                    int interval1 = Convert.ToInt32(intervalString[0]);
                    int interval2 = Convert.ToInt32(intervalString[1]);
                    for (int i = interval1; (i <= interval2); i++)
                    	numbers.Add(i);
                }
                else
                	if (!(String.IsNullOrEmpty(str)))
                    	numbers.Add(Convert.ToInt32(str));
            numbers.Sort();
            // check if "every" used
            int everyNum = 1;
            if (!(String.IsNullOrEmpty(every)))
            	everyNum = Convert.ToInt32(every);
            if (everyNum > 1)
            {
                // if "every" is greater than available numbers
                if (everyNum >= numbers.Count)
                	return numbers.First().Equals(number);
                List<int> allNumbers = new List<int>(numbers);
                numbers.Clear();
                for (int i = 0; (i <= (allNumbers.Count / everyNum)); i++)
                	numbers.Add(allNumbers.ElementAt((i * everyNum)));
            }
            return numbers.Contains(number);
        }
        
        /// Checks to see if the current node's start and end attributes are valid.
        private bool CheckInterval(XPathNavigator nav, DateTime checkDate)
        {
            DateTime start = checkDate;
            DateTime end = checkDate;
            if (!(DateTime.TryParse(nav.GetAttribute("start", String.Empty), out start)))
            	start = StartOfDay(TestDate);
            if (!(DateTime.TryParse(nav.GetAttribute("end", String.Empty), out end)))
            	end = DateTime.MaxValue;
            if (!(((start <= checkDate) && (checkDate <= end))))
            	return false;
            return true;
        }
        
        private ScheduleStatus ProbeSchedule(XPathNavigator document, XPathNavigator exceptionsDocument, ScheduleStatus schedule, ScheduleStatus exceptionsSchedule)
        {
            ScheduleStatus testSched = new ScheduleStatus();
            ScheduleStatus testExceptionSched = new ScheduleStatus();
            DateTime nextDate = DateTime.Now;
            bool initialState = schedule.Success;
            for (int probeCount = 0; (probeCount <= 30); probeCount++)
            {
                nextDate = nextDate.AddSeconds(1);
                // reset variables
                testSched.Success = false;
                testSched.Expired = false;
                document.MoveToRoot();
                document.MoveToFirstChild();
                if (exceptionsDocument != null)
                {
                    exceptionsDocument.MoveToRoot();
                    exceptionsDocument.MoveToFirstChild();
                    testExceptionSched.Success = false;
                    testExceptionSched.Expired = false;
                }
                bool valid = (CheckNode(document, nextDate, ref testSched) && ((exceptionsDocument == null) || !(CheckNode(exceptionsDocument, nextDate, ref testExceptionSched))));
                if (valid != initialState)
                	return schedule;
                schedule.NextTestDate = nextDate;
            }
            return schedule;
        }
        
        private ScheduleStatus ProbeScheduleExact(XPathNavigator document, XPathNavigator exceptionsDocument, ScheduleStatus schedule, ScheduleStatus exceptionsSchedule)
        {
            ScheduleStatus testSched = new ScheduleStatus();
            ScheduleStatus testExceptionSched = new ScheduleStatus();
            int sign = 1;
            DateTime nextDate = DateTime.Now;
            bool initialState = schedule.Success;
            int jump = 0;
            if (schedule.Precision.Equals("daily") || exceptionsSchedule.Precision.Equals("daily"))
            	jump = (6 * 60);
            else
            	if (schedule.Precision.Equals("weekly") || exceptionsSchedule.Precision.Equals("weekly"))
                	jump = (72 * 60);
                else
                	if (schedule.Precision.Equals("monthly") || exceptionsSchedule.Precision.Equals("monthly"))
                    	jump = (360 * 60);
                    else
                    	if (schedule.Precision.Equals("yearly") || exceptionsSchedule.Precision.Equals("yearly"))
                        	jump = ((720 * 6) 
                                        * 60);
                        else
                        	jump = (6 * 60);
            for (int probeCount = 1; (probeCount <= 20); probeCount++)
            {
                // reset variables
                testSched.Success = false;
                testSched.Expired = false;
                document.MoveToRoot();
                document.MoveToFirstChild();
                if (exceptionsDocument != null)
                {
                    exceptionsDocument.MoveToRoot();
                    exceptionsDocument.MoveToFirstChild();
                    testExceptionSched.Success = false;
                    testExceptionSched.Expired = false;
                }
                // set next date to check
                nextDate = nextDate.AddMinutes((jump * sign));
                bool valid = (CheckNode(document, nextDate, ref testSched) && ((exceptionsDocument == null) || !(CheckNode(exceptionsDocument, nextDate, ref testExceptionSched))));
                if (valid == initialState)
                	sign = 1;
                else
                	sign = -1;
                // keep moving forward and expand jump if no border found, otherwise narrow jump
                if (sign == -1)
                	jump = (jump / 2);
                else
                {
                    jump = (jump * 2);
                    probeCount--;
                }
                if (jump < 5)
                	jump++;
                // no border found
                if (nextDate > DateTime.Now.AddYears(5))
                	break;
            }
            schedule.NextTestDate = nextDate.AddMinutes((jump * -1));
            return schedule;
        }
        
        private int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);
            while (!((date.Date.AddDays(1).DayOfWeek == CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)))
            	date = date.AddDays(1);
            return (((int)((((double)(date.Subtract(beginningOfMonth).TotalDays)) / 7))) + 1);
        }
        
        private DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
        
        private DateTime EndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }
    }
}
