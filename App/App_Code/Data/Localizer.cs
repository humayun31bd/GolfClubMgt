using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;

namespace MyCompany.Data
{
	public class LocalizationDictionary : SortedDictionary<string, string>
    {
    }
    
    public class Localizer
    {
        
        public static Regex TokenRegex = new Regex("\\^(\\w+)\\^([\\s\\S]+?)\\^(\\w+)\\^", RegexOptions.Compiled);
        
        public static Regex ScriptRegex = new Regex("<script.+?>([\\s\\S]*?)</script>", (RegexOptions.Compiled | RegexOptions.IgnoreCase));
        
        public static Regex StateRegex = new Regex("(<input.+?name=.__VIEWSTATE.+?/>)", (RegexOptions.Compiled | RegexOptions.IgnoreCase));
        
        public const string StateRegexReplace = "$1\r\n<input type=\"text\" name=\"__COTSTATE\" id=\"__COTSTATE\" style=\"display:none\" />";
        
        public const string StateRegexReplaceIE = "$1\r\n<input type=\"hidden\" name=\"__COTSTATE\" id=\"__COTSTATE\" />";
        
        private string _baseName;
        
        private string _objectName;
        
        private string _text;
        
        private LocalizationDictionary _dictionary;
        
        private LocalizationDictionary _sharedDictionary;
        
        private bool _scriptMode;
        
        public Localizer(string baseName, string objectName, string text)
        {
            _baseName = baseName;
            if (!(String.IsNullOrEmpty(baseName)))
            	_baseName = (_baseName + ".");
            _objectName = objectName;
            _text = text;
        }
        
        public static string Replace(string token, string text)
        {
            return Replace(String.Empty, "Resources", token, text);
        }
        
        public static string Replace(string baseName, string objectName, string token, string text)
        {
            return Replace(baseName, objectName, String.Format("^{0}^{1}^{0}^", token, text));
        }
        
        public static string Replace(string baseName, string objectName, string text)
        {
            if (!(TokenRegex.IsMatch(text)))
            	return text;
            Localizer l = new Localizer(baseName, objectName, text);
            return l.Replace();
        }
        
        public virtual string Replace()
        {
            _sharedDictionary = CreateDictionary(String.Empty, "CombinedSharedResources");
            _dictionary = CreateDictionary(_baseName, _objectName);
            if (_baseName == "Pages.")
            {
                _scriptMode = true;
                string stateInput = StateRegexReplace;
                if (HttpContext.Current.Request.Browser.Browser == "IE")
                	stateInput = StateRegexReplaceIE;
                string output = StateRegex.Replace(ScriptRegex.Replace(_text.Trim(), DoReplaceScript), stateInput);
                _scriptMode = false;
                return TokenRegex.Replace(output, DoReplaceToken);
            }
            else
            	return TokenRegex.Replace(_text, DoReplaceToken);
        }
        
        private string DoReplaceScript(Match m)
        {
            return TokenRegex.Replace(m.Value, DoReplaceToken);
        }
        
        private string DoReplaceToken(Match m)
        {
            string token = m.Groups[1].Value;
            if (token == m.Groups[3].Value)
            {
                string result = null;
                if (!(_dictionary.TryGetValue(token, out result)))
                {
                    if (!(_sharedDictionary.TryGetValue(token, out result)))
                    	result = m.Groups[2].Value;
                }
                if (_scriptMode)
                	result = BusinessRules.JavaScriptString(result);
                return result;
            }
            else
            	return m.Value;
        }
        
        public static Stream CreateDictionaryStream(string culture, string baseName, string objectName, out string[] files)
        {
            files = null;
            string fileName = String.Format("{0}.txt", objectName);
            Type t = typeof(Controller);
            Stream result = t.Assembly.GetManifestResourceStream(CultureManager.ResolveEmbeddedResourceName(String.Format("MyCompany.{0}{1}", baseName, fileName), culture));
            if (result == null)
            	result = t.Assembly.GetManifestResourceStream(CultureManager.ResolveEmbeddedResourceName(String.Format("MyCompany.{0}", fileName), culture));
            if (result == null)
            {
                fileName = String.Format("{0}.{1}.txt", objectName, culture);
                string objectPath = Path.Combine(Path.Combine(HttpRuntime.AppDomainAppPath, baseName), fileName);
                if (File.Exists(objectPath))
                {
                    files = new string[] {
                            objectPath};
                    result = new FileStream(objectPath, FileMode.Open, FileAccess.Read);
                }
                else
                	if (String.IsNullOrEmpty(baseName) && (objectName == "CombinedSharedResources"))
                    {
                        List<string> dependencies = new List<string>();
                        result = new MemoryStream();
                        string root = HttpContext.Current.Server.MapPath("~/");
                        string[] list = null;
                        // try loading "Resources.CULTURE-NAME.txt" files
                        Stream rs = CreateDictionaryStream(culture, String.Empty, "Resources", out list);
                        MergeStreams(result, rs, dependencies, list);
                        //  try loading "Web.Sitemap.CULTURE_NAME" files
                        rs = CreateDictionaryStream(culture, String.Empty, "web.sitemap", out list);
                        MergeStreams(result, rs, dependencies, list);
                        // try loading "Controls\ControlName.ascx.CULTURE_NAME" files
                        string controlsPath = Path.Combine(root, "Controls");
                        if (Directory.Exists(controlsPath))
                        	foreach (string f in Directory.GetFiles(controlsPath, "*.ascx"))
                            {
                                rs = CreateDictionaryStream(culture, "Controls", Path.GetFileName(f), out list);
                                MergeStreams(result, rs, dependencies, list);
                            }
                        // complete processing of combined shared resources
                        result.Position = 0;
                        files = dependencies.ToArray();
                    }
            }
            return result;
        }
        
        private static void MergeStreams(Stream result, Stream source, List<string> dependencies, string[] list)
        {
            if (source != null)
            {
                byte[] buffer = new byte[32768];
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                while (bytesRead > 0)
                {
                    result.Write(buffer, 0, buffer.Length);
                    bytesRead = source.Read(buffer, 0, buffer.Length);
                }
                source.Close();
                if (list != null)
                	dependencies.AddRange(list);
            }
        }
        
        public static LocalizationDictionary CreateDictionary(string baseName, string objectName)
        {
            string culture = Thread.CurrentThread.CurrentUICulture.Name;
            string fileName = String.Format("MyCompany.{0}.{1}.txt", objectName, culture);
            LocalizationDictionary dictionary = ((LocalizationDictionary)(HttpRuntime.Cache[fileName]));
            if (dictionary == null)
            {
                dictionary = new LocalizationDictionary();
                string[] files = null;
                Stream s = CreateDictionaryStream(culture, baseName, objectName, out files);
                if ((s == null) && culture.Contains("-"))
                {
                    culture = culture.Substring(0, culture.IndexOf("-"));
                    s = CreateDictionaryStream(culture, baseName, objectName, out files);
                }
                if (s != null)
                {
                    PopulateDictionary(dictionary, new StreamReader(s).ReadToEnd());
                    s.Close();
                }
                CacheDependency dependency = null;
                if (files != null)
                	dependency = new CacheDependency(files);
                HttpRuntime.Cache.Insert(fileName, dictionary, dependency);
            }
            return dictionary;
        }
        
        private static void PopulateDictionary(LocalizationDictionary dictionary, string text)
        {
            Match m = TokenRegex.Match(text);
            while (m.Success)
            {
                string token = m.Groups[1].Value;
                if (token == m.Groups[3].Value)
                	dictionary[token] = m.Groups[2].Value;
                m = m.NextMatch();
            }
        }
    }
}
