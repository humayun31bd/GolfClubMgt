
using MyCompany.Handlers;

namespace MyCompany.Services
{
	public class AppFrameworkConfig
    {
        
        public virtual void Initialize()
        {
            ApplicationServices.FrameworkAppName = "Golf Club";
            ApplicationServices.Version = "8.7.4.0";
            ApplicationServices.JqmVersion = "1.4.6";
            ApplicationServices.HostVersion = "1.2.4.0";
            BlobFactoryConfig.Initialize();
        }
    }
}
