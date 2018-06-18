using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using System.Globalization;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MyCompany.Data;
using MyCompany.Handlers;

namespace MyCompany.Data
{
	public partial class FileSystemBlobAdapter : FileSystemBlobAdapterBase
    {
        
        public FileSystemBlobAdapter(string controller, BlobAdapterArguments arguments) : 
                base(controller, arguments)
        {
        }
    }
    
    public class FileSystemBlobAdapterBase : BlobAdapter
    {
        
        public FileSystemBlobAdapterBase(string controller, BlobAdapterArguments arguments) : 
                base(controller, arguments)
        {
        }
        
        public override Stream ReadBlob(string keyValue)
        {
            string fileName = ExtendPathTemplate(keyValue);
            return File.OpenRead(fileName);
        }
        
        public override bool WriteBlob(HttpPostedFile file, string keyValue)
        {
            string fileName = ExtendPathTemplate(keyValue);
            string directoryName = Path.GetDirectoryName(fileName);
            if (!(Directory.Exists(directoryName)))
            	Directory.CreateDirectory(directoryName);
            Stream stream = file.InputStream;
            file.SaveAs(fileName);
            return true;
        }
        
        public override void ValidateFieldValue(FieldValue fv)
        {
        }
    }
    
    public partial class AzureBlobAdapter : AzureBlobAdapterBase
    {
        
        public AzureBlobAdapter(string controller, BlobAdapterArguments arguments) : 
                base(controller, arguments)
        {
        }
    }
    
    public class AzureBlobAdapterBase : BlobAdapter
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _account;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _key;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _container;
        
        public AzureBlobAdapterBase(string controller, BlobAdapterArguments arguments) : 
                base(controller, arguments)
        {
        }
        
        public virtual string Account
        {
            get
            {
                return this._account;
            }
            set
            {
                this._account = value;
            }
        }
        
        public virtual string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }
        
        public virtual string Container
        {
            get
            {
                return this._container;
            }
            set
            {
                this._container = value;
            }
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            if (Arguments.ContainsKey("account"))
            	Account = Arguments["account"];
            if (Arguments.ContainsKey("key"))
            	Key = Arguments["key"];
            if (Arguments.ContainsKey("container"))
            	Container = Arguments["container"];
        }
        
        public override Stream ReadBlob(string keyValue)
        {
            string urlPath = String.Format("{0}/{1}", this.Container, KeyValueToPath(keyValue));
            string requestMethod = "GET";
            string storageServiceVersion = "2015-12-11";
            string blobType = "BlockBlob";
            string dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            string canonicalizedHeaders = String.Format("x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}", blobType, dateInRfc1123Format, storageServiceVersion);
            string canonicalizedResource = String.Format("/{0}/{1}", this.Account, urlPath);
            string blobLength = "";
            string stringToSign = String.Format("{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}", requestMethod, blobLength, canonicalizedHeaders, canonicalizedResource);
            string authorizationHeader = CreateAuthorizationHeaderForAzure(stringToSign);
            Uri uri = new Uri((("http://" + this.Account) 
                            + (".blob.core.windows.net/" + urlPath)));
            HttpWebRequest request = ((HttpWebRequest)(WebRequest.Create(uri)));
            request.Method = requestMethod;
            request.Headers.Add("x-ms-blob-type", blobType);
            request.Headers.Add("x-ms-date", dateInRfc1123Format);
            request.Headers.Add("x-ms-version", storageServiceVersion);
            request.Headers.Add("Authorization", authorizationHeader);
            try
            {
                string tempFileName = Path.GetTempFileName();
                Stream stream = File.Create(tempFileName);
                using (HttpWebResponse response = ((HttpWebResponse)(request.GetResponse())))
                	using (Stream dataStream = response.GetResponseStream())
                    	CopyData(dataStream, stream);
                return stream;
            }
            catch (Exception e)
            {
                string message = e.Message;
                return null;
            }
        }
        
        public override bool WriteBlob(HttpPostedFile file, string keyValue)
        {
            string requestMethod = "PUT";
            string urlPath = String.Format("{0}/{1}", this.Container, KeyValueToPath(keyValue));
            string storageServiceVersion = "2015-12-11";
            string dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            Stream stream = file.InputStream;
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            int blobLength = ((int)(stream.Length));
            byte[] blobContent = new byte[blobLength];
            stream.Read(blobContent, 0, blobLength);
            string blobType = "BlockBlob";
            string canonicalizedHeaders = String.Format("x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}", blobType, dateInRfc1123Format, storageServiceVersion);
            string canonicalizedResource = String.Format("/{0}/{1}", this.Account, urlPath);
            string stringToSign = String.Format("{0}\n\n\n{1}\n\n{4}\n\n\n\n\n\n\n{2}\n{3}", requestMethod, blobLength, canonicalizedHeaders, canonicalizedResource, file.ContentType);
            string authorizationHeader = CreateAuthorizationHeaderForAzure(stringToSign);
            Uri uri = new Uri((("http://" + this.Account) 
                            + (".blob.core.windows.net/" + urlPath)));
            HttpWebRequest request = ((HttpWebRequest)(WebRequest.Create(uri)));
            request.Method = requestMethod;
            request.Headers.Add("x-ms-blob-type", blobType);
            request.Headers.Add("x-ms-date", dateInRfc1123Format);
            request.Headers.Add("x-ms-version", storageServiceVersion);
            request.Headers.Add("Authorization", authorizationHeader);
            request.ContentLength = blobLength;
            request.ContentType = file.ContentType;
            try
            {
                int bufferSize = (1024 * 64);
                int offset = 0;
                using (Stream requestStream = request.GetRequestStream())
                	while (offset < blobLength)
                    {
                        int bytesToWrite = (blobLength - offset);
                        if ((offset + bufferSize) < blobLength)
                        	bytesToWrite = bufferSize;
                        requestStream.Write(blobContent, offset, bytesToWrite);
                        offset = (offset + bytesToWrite);
                    }
                using (HttpWebResponse response = ((HttpWebResponse)(request.GetResponse())))
                {
                    string ETag = response.Headers["ETag"];
                    if (((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Accepted)) || (response.StatusCode == HttpStatusCode.Created))
                    	return true;
                }
            }
            catch (WebException webEx)
            {
                if (webEx != null)
                {
                    WebResponse resp = webEx.Response;
                    if (resp != null)
                    	using (StreamReader sr = new StreamReader(resp.GetResponseStream(), true))
                        	throw new Exception(sr.ReadToEnd());
                }
            }
            return false;
        }
        
        protected string CreateAuthorizationHeaderForAzure(string canonicalizedString)
        {
            string signature = String.Empty;
            byte[] storageKey = Convert.FromBase64String(this.Key);
            using (HMACSHA256 hmacSha256 = new HMACSHA256(storageKey))
            {
                byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }
            string authorizationHeader = String.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", "SharedKey", this.Account, signature);
            return authorizationHeader;
        }
    }
}
