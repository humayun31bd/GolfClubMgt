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
	public partial class S3BlobAdapter : S3BlobAdapterBase
    {
        
        public S3BlobAdapter(string controller, BlobAdapterArguments arguments) : 
                base(controller, arguments)
        {
        }
    }
    
    public class S3BlobAdapterBase : BlobAdapter
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _accessKeyID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _secretAccessKey;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bucket;
        
        public S3BlobAdapterBase(string controller, BlobAdapterArguments arguments) : 
                base(controller, arguments)
        {
        }
        
        public virtual string AccessKeyID
        {
            get
            {
                return this._accessKeyID;
            }
            set
            {
                this._accessKeyID = value;
            }
        }
        
        public virtual string SecretAccessKey
        {
            get
            {
                return this._secretAccessKey;
            }
            set
            {
                this._secretAccessKey = value;
            }
        }
        
        public virtual string Bucket
        {
            get
            {
                return this._bucket;
            }
            set
            {
                this._bucket = value;
            }
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            if (Arguments.ContainsKey("access-key-id"))
            	AccessKeyID = Arguments["access-key-id"];
            if (Arguments.ContainsKey("secret-access-key"))
            	SecretAccessKey = Arguments["secret-access-key"];
            if (Arguments.ContainsKey("bucket"))
            	Bucket = Arguments["bucket"];
        }
        
        public override Stream ReadBlob(string keyValue)
        {
            string extendedPath = KeyValueToPath(keyValue);
            string httpVerb = "GET";
            System.DateTime date = DateTime.UtcNow;
            string canonicalizedAmzHeaders = ("x-amz-date:" + date.ToString("R", CultureInfo.InvariantCulture));
            string canonicalizedResource = String.Format("/{0}/{1}", this.Bucket, extendedPath);
            string stringToSign = String.Format("{0}\n\n\n\n{1}\n{2}", httpVerb, canonicalizedAmzHeaders, canonicalizedResource);
            string authorization = CreateAuthorizationHeaderForS3(stringToSign);
            Uri uri = new Uri((("http://" + this.Bucket) 
                            + (".s3.amazonaws.com/" + extendedPath)));
            HttpWebRequest request = ((HttpWebRequest)(WebRequest.Create(uri)));
            request.Method = httpVerb;
            request.Headers.Add("x-amz-date", date.ToString("R", CultureInfo.InvariantCulture));
            request.Headers.Add("Authorization", authorization);
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
            string extendedPath = KeyValueToPath(keyValue);
            Stream stream = file.InputStream;
            int blobLength = ((int)(stream.Length));
            byte[] blobContent = new byte[blobLength];
            stream.Read(blobContent, 0, blobLength);
            string httpVerb = "PUT";
            System.DateTime date = DateTime.UtcNow;
            string canonicalizedAmzHeaders = ("x-amz-date:" + date.ToString("R", CultureInfo.InvariantCulture));
            string canonicalizedResource = String.Format("/{0}/{1}", this.Bucket, extendedPath);
            string stringToSign = String.Format("{0}\n\n\n\n{1}\n{2}", httpVerb, canonicalizedAmzHeaders, canonicalizedResource);
            string authorization = CreateAuthorizationHeaderForS3(stringToSign);
            Uri uri = new Uri((("http://" + this.Bucket) 
                            + (".s3.amazonaws.com/" + extendedPath)));
            HttpWebRequest request = ((HttpWebRequest)(WebRequest.Create(uri)));
            request.Method = httpVerb;
            request.ContentLength = blobLength;
            request.Headers.Add("x-amz-date", date.ToString("R", CultureInfo.InvariantCulture));
            request.Headers.Add("Authorization", authorization);
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    int bufferSize = (1024 * 64);
                    int offset = 0;
                    while (offset < blobLength)
                    {
                        int bytesToWrite = (blobLength - offset);
                        if ((offset + bufferSize) < blobLength)
                        	bytesToWrite = bufferSize;
                        requestStream.Write(blobContent, offset, bytesToWrite);
                        offset = (offset + bytesToWrite);
                    }
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
        
        protected virtual string CreateAuthorizationHeaderForS3(string canonicalizedString)
        {
            Encoding ae = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1();
            signature.Key = ae.GetBytes(this.SecretAccessKey);
            byte[] bytes = ae.GetBytes(canonicalizedString);
            byte[] moreBytes = signature.ComputeHash(bytes);
            string encodedCanonical = Convert.ToBase64String(moreBytes);
            return String.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", "AWS", this.AccessKeyID, encodedCanonical);
        }
    }
}
