using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Collections;

namespace MyCompany.Data
{
	public class ImportMapDictionary : SortedDictionary<int, DataField>
    {
    }
    
    public class ImportLookupDictionary : SortedDictionary<string, DataField>
    {
    }
    
    public partial class ImportProcessor : ImportProcessorBase
    {
    }
    
    public partial class ImportProcessorFactory : ImportProcessorFactoryBase
    {
    }
    
    public class ImportProcessorFactoryBase
    {
        
        public virtual ImportProcessorBase CreateProcessor(string fileName)
        {
            throw new Exception(String.Format("The format of file <b>{0}</b> is not supported.", Path.GetFileName(fileName)));
        }
        
        public static ImportProcessorBase Create(string fileName)
        {
            ImportProcessorFactoryBase factory = new ImportProcessorFactory();
            return factory.CreateProcessor(fileName);
        }
    }
    
    public class ImportProcessorBase
    {
        
        public ImportProcessorBase()
        {
        }
        
        public static string SharedTempPath
        {
            get
            {
                string p = WebConfigurationManager.AppSettings["SharedTempPath"];
                if (String.IsNullOrEmpty(p))
                	p = Path.GetTempPath();
                if (!(Path.IsPathRooted(p)))
                	p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, p);
                return p;
            }
        }
        
        public static void Execute(ActionArgs args)
        {
        }
    }
}
