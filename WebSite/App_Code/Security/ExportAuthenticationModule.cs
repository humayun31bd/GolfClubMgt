using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using MyCompany.Services;

namespace MyCompany.Security
{
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public partial class ExportAuthenticationModule : ExportAuthenticationModuleBase
    {
    }
    
    public class ExportAuthenticationModuleBase : IHttpModule
    {
        
        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.contextBeginRequest);
            context.AuthenticateRequest += new EventHandler(this.contextAuthenticateRequest);
            context.EndRequest += new EventHandler(this.contextEndRequest);
        }
        
        void IHttpModule.Dispose()
        {
        }
        
        private void contextBeginRequest(object sender, EventArgs e)
        {
            HttpRequest request = HttpContext.Current.Request;
            string origin = request.Headers["Origin"];
            if (!(String.IsNullOrEmpty(origin)))
            {
                string myOrigin = (request.Url.Scheme 
                            + (Uri.SchemeDelimiter + request.Url.Host));
                if (!(request.Url.IsDefaultPort))
                	myOrigin = (myOrigin 
                                + (":" + Convert.ToString(request.Url.Port)));
                if (origin != myOrigin)
                {
                    bool allowed = false;
                    SortedDictionary<string, string> config = ApplicationServices.Create().CorsConfiguration(request);
                    if (config != null)
                    	foreach (KeyValuePair<string, string> kvp in config)
                        {
                            HttpContext.Current.Response.Headers[kvp.Key] = kvp.Value;
                            if ((kvp.Key == "Access-Control-Allow-Origin") && ((kvp.Value == "*") || kvp.Value.Split(',').Contains(origin)))
                            	allowed = true;
                        }
                    if (!(allowed) || (request.HttpMethod == "OPTIONS"))
                    	((HttpApplication)(sender)).CompleteRequest();
                }
            }
        }
        
        private void contextAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = ((HttpApplication)(sender));
            ApplicationServices appServices = new ApplicationServices();
            string authorization = app.Request.Headers["Authorization"];
            if (!(String.IsNullOrEmpty(authorization)))
            {
                // validate auth header
                if (authorization.StartsWith("Basic", StringComparison.CurrentCultureIgnoreCase))
                	ValidateUserIdentity(app, authorization);
                else
                	if (authorization.StartsWith("Bearer ", StringComparison.CurrentCultureIgnoreCase))
                    	ValidateUserToken(app, authorization.Substring(7));
            }
            else
            {
                if (!(appServices.RequiresAuthentication(app.Context.Request)))
                	return;
                if (appServices.AuthenticateRequest(app.Context))
                	return;
                RequestAuthentication(app);
            }
        }
        
        private void contextEndRequest(object sender, EventArgs e)
        {
            HttpApplication app = ((HttpApplication)(sender));
            if ((app.Response.StatusCode == 401) && (app.Context.Items["IgnoreBasicAuthenticationRequest"] == null))
            	RequestAuthentication(app);
        }
        
        private void RequestAuthentication(HttpApplication app)
        {
            ApplicationServices appServices = new ApplicationServices();
            app.Response.AppendHeader("WWW-Authenticate", String.Format("Basic realm=\"{0}\"", appServices.Realm));
            app.Response.StatusCode = 401;
            app.CompleteRequest();
        }
        
        private void ValidateUserIdentity(HttpApplication app, string authorization)
        {
            string[] login = Encoding.Default.GetString(Convert.FromBase64String(authorization.Substring(6))).Split(new char[] {
                        ':'}, 2);
            if (Membership.ValidateUser(login[0], login[1]))
            	app.Context.User = new RolePrincipal(new FormsIdentity(new FormsAuthenticationTicket(login[0], false, 10)));
            else
            {
                app.Response.StatusCode = 401;
                app.Response.StatusDescription = "Access Denied";
                app.Response.Write("Access denied. Please enter a valid user name and password.");
                app.CompleteRequest();
            }
        }
        
        private void ValidateUserToken(HttpApplication app, string authorization)
        {
            if (!(ApplicationServices.Current.ValidateToken(authorization)))
            {
                app.Context.Items["IgnoreBasicAuthenticationRequest"] = true;
                app.Response.StatusCode = 401;
                app.Response.StatusDescription = "Access Denied";
                app.Response.Write("Access denied. Please authenticate.");
                app.CompleteRequest();
            }
        }
    }
}
