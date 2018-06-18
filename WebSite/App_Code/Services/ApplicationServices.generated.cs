
using MyCompany.Handlers;

namespace MyCompany.Services
{
	public class AppFrameworkConfig
    {
        
        public virtual void Initialize()
        {
            ApplicationServices.Version = "8.7.2.0";
            ApplicationServices.JqmVersion = "1.4.6";
            ApplicationServices.HostVersion = "1.0.0.0";
            BlobFactoryConfig.Initialize();
        }
    }
}
