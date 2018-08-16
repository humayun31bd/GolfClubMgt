using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml.XPath;
using System.Drawing.Drawing2D;
using Newtonsoft.Json.Linq;
using MyCompany.Data;
using MyCompany.Services;

namespace MyCompany.Handlers
{
	public enum BlobMode
    {
        
        Thumbnail,
        
        Original,
        
        Upload,
    }
    
    public class BlobHandlerInfo
    {
        
        private string _key;
        
        private string _tableName;
        
        private string _fieldName;
        
        private string[] _keyFieldNames;
        
        private string _text;
        
        private string _contentType;
        
        private string _dataController;
        
        private string _controllerFieldName;
        
        public BlobHandlerInfo()
        {
        }
        
        public BlobHandlerInfo(string key, string tableName, string fieldName, string[] keyFieldNames, string text, string contentType) : 
                this(key, tableName, fieldName, keyFieldNames, text, contentType, String.Empty, String.Empty)
        {
        }
        
        public BlobHandlerInfo(string key, string tableName, string fieldName, string[] keyFieldNames, string text, string contentType, string dataController, string controllerFieldName)
        {
            this.Key = key;
            this.TableName = tableName;
            this.FieldName = fieldName;
            this.KeyFieldNames = keyFieldNames;
            this.Text = text;
            this._contentType = contentType;
            this.DataController = dataController;
            this.ControllerFieldName = controllerFieldName;
        }
        
        public virtual string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        
        protected string this[string name]
        {
            get
            {
                return ((string)(HttpContext.Current.Items[("BlobHandlerInfo_" + name)]));
            }
            set
            {
                HttpContext.Current.Items[("BlobHandlerInfo_" + name)] = value;
            }
        }
        
        public virtual string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }
        
        public virtual string FieldName
        {
            get
            {
                return _fieldName;
            }
            set
            {
                _fieldName = value;
            }
        }
        
        public virtual string[] KeyFieldNames
        {
            get
            {
                return _keyFieldNames;
            }
            set
            {
                _keyFieldNames = value;
            }
        }
        
        public virtual string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
        
        public virtual string Error
        {
            get
            {
                return this["Error"];
            }
            set
            {
                this["Error"] = value;
            }
        }
        
        public virtual string FileName
        {
            get
            {
                return this["FileName"];
            }
            set
            {
                this["FileName"] = value;
            }
        }
        
        public virtual string ContentType
        {
            get
            {
                string s = this["ContentType"];
                if (String.IsNullOrEmpty(s))
                	s = this._contentType;
                return s;
            }
            set
            {
                this["ContentType"] = value;
            }
        }
        
        public virtual string DataController
        {
            get
            {
                return _dataController;
            }
            set
            {
                _dataController = value;
            }
        }
        
        public virtual string UploadDownloadViewName
        {
            get
            {
                return Controller.GetUpdateView(DataController);
            }
        }
        
        public virtual string ControllerFieldName
        {
            get
            {
                return _controllerFieldName;
            }
            set
            {
                _controllerFieldName = value;
            }
        }
        
        public static BlobHandlerInfo Current
        {
            get
            {
                BlobHandlerInfo d = ((BlobHandlerInfo)(HttpContext.Current.Items["BlobHandlerInfo_Current"]));
                if (d == null)
                	foreach (string key in HttpContext.Current.Request.QueryString.Keys)
                    	if (!(String.IsNullOrEmpty(key)) && BlobFactory.Handlers.ContainsKey(key))
                        {
                            d = BlobFactory.Handlers[key];
                            HttpContext.Current.Items["BlobHandlerInfo_Current"] = d;
                            break;
                        }
                return d;
            }
        }
        
        public BlobMode Mode
        {
            get
            {
                if (Value.StartsWith("u|"))
                	return BlobMode.Upload;
                if (Value.StartsWith("t|"))
                	return BlobMode.Thumbnail;
                else
                	return BlobMode.Original;
            }
        }
        
        public bool AllowCaching
        {
            get
            {
                return ((Mode == BlobMode.Thumbnail) || ((Mode == BlobMode.Original) && MaxWidth.HasValue));
            }
        }
        
        public int? MaxWidth
        {
            get
            {
                Match m = Regex.Match(Value, "^(\\w{2,3})\\|");
                string size = m.Groups[1].Value;
                if (size == "tn")
                	return 280;
                if (size == "xxs")
                	return 320;
                if (size == "xs")
                	return 480;
                if (size == "sm")
                	return 576;
                if (size == "md")
                	return 768;
                if (size == "lg")
                	return 992;
                if (size == "xl")
                	return 200;
                if (size == "xxl")
                	return 1366;
                return null;
            }
        }
        
        public string Value
        {
            get
            {
                string v = this["Value"];
                if (String.IsNullOrEmpty(v))
                	v = HttpContext.Current.Request.QueryString[Key];
                return v;
            }
        }
        
        public string Reference
        {
            get
            {
                string s = Value.Replace("|", "_");
                return s.Substring(1);
            }
        }
        
        public virtual string ContentTypeField
        {
            get
            {
                string fieldName = this[(ControllerFieldName + "_ContentTypeField")];
                if (!(String.IsNullOrEmpty(fieldName)))
                	return fieldName;
                return (ControllerFieldName + "ContentType");
            }
            set
            {
                this[(ControllerFieldName + "_ContentTypeField")] = value;
            }
        }
        
        public virtual string FileNameField
        {
            get
            {
                string fieldName = this[(ControllerFieldName + "_FileNameField")];
                if (!(String.IsNullOrEmpty(fieldName)))
                	return fieldName;
                return (_controllerFieldName + "FileName");
            }
            set
            {
                this[(ControllerFieldName + "_FileNameField")] = value;
            }
        }
        
        public virtual string LengthField
        {
            get
            {
                string fieldName = this[(ControllerFieldName + "_LengthField")];
                if (!(String.IsNullOrEmpty(fieldName)))
                	return fieldName;
                return (_controllerFieldName + "Length");
            }
            set
            {
                this[(ControllerFieldName + "_LengthField")] = value;
            }
        }
        
        public virtual bool SaveFile(HttpContext context)
        {
            return this.SaveFile(context, null, null);
        }
        
        public virtual bool SaveFile(HttpContext context, BlobAdapter ba, string keyValue)
        {
            if (context.Request.Files.Count != 1)
            	return false;
            try
            {
                if ((BlobHandlerInfo.Current != null) && BlobHandlerInfo.Current.ProcessUploadViaBusinessRule(ba))
                	return true;
                if (ba == null)
                	using (SqlStatement updateBlob = BlobFactory.CreateBlobUpdateStatement())
                    	return (updateBlob.ExecuteNonQuery() == 1);
                else
                {
                    HttpPostedFile file = context.Request.Files[0];
                    if (file.ContentLength.Equals(0))
                    	return true;
                    return ba.WriteBlob(file, keyValue);
                }
            }
            catch (Exception err)
            {
                Error = err.Message;
                return false;
            }
        }
        
        public List<string> CreateKeyValues()
        {
            List<string> keyValues = new List<string>();
            keyValues.Add(Value.Split('|')[1]);
            return keyValues;
        }
        
        private List<FieldValue> CreateActionValues(Stream stream, string contentType, string fileName, int contentLength)
        {
            bool deleting = (((contentType == "application/octet-stream") && (contentLength == 0)) && (String.IsNullOrEmpty(fileName) || (fileName == "_delete_")));
            List<string> keyValues = CreateKeyValues();
            int keyValueIndex = 0;
            List<FieldValue> actionValues = new List<FieldValue>();
            ControllerConfiguration config = Controller.CreateConfigurationInstance(typeof(Controller), DataController);
            XPathNodeIterator keyFieldIterator = config.Select("/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']");
            while (keyFieldIterator.MoveNext())
            {
                FieldValue v = new FieldValue(keyFieldIterator.Current.GetAttribute("name", String.Empty));
                if (keyValueIndex < keyValues.Count)
                {
                    v.OldValue = keyValues[keyValueIndex];
                    v.Modified = false;
                    keyValueIndex++;
                }
                actionValues.Add(v);
            }
            if (stream != null)
            {
                XPathNavigator lengthFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'{0}\']", this.LengthField);
                if (lengthFieldNav == null)
                	lengthFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'{0}Length\' or @name=\'{0}LENGTH\']", ControllerFieldName);
                if (lengthFieldNav == null)
                	lengthFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'Length\' or @name=\'LENGTH\']", ControllerFieldName);
                if (lengthFieldNav != null)
                {
                    string fieldName = lengthFieldNav.GetAttribute("name", String.Empty);
                    if (fieldName != this.LengthField)
                    	this.LengthField = fieldName;
                    actionValues.Add(new FieldValue(fieldName, contentLength));
                    if (deleting)
                    	ClearLastFieldValue(actionValues);
                }
                XPathNavigator contentTypeFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'{0}\']", this.ContentTypeField);
                if (contentTypeFieldNav == null)
                	contentTypeFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'{0}ContentType\' or @name=\'{0}CONTENTTYP" +
                            "E\']", ControllerFieldName);
                if (contentTypeFieldNav == null)
                	contentTypeFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'ContentType\' or @name=\'CONTENTTYPE\']", ControllerFieldName);
                if (contentTypeFieldNav != null)
                {
                    string fieldName = contentTypeFieldNav.GetAttribute("name", String.Empty);
                    if (fieldName != this.ContentTypeField)
                    	this.ContentTypeField = fieldName;
                    actionValues.Add(new FieldValue(fieldName, contentType));
                    if (deleting)
                    	ClearLastFieldValue(actionValues);
                }
                XPathNavigator fileNameFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'{0}\']", this.FileNameField);
                if (fileNameFieldNav == null)
                	fileNameFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'{0}FileName\' or @name=\'{0}FILENAME\']", ControllerFieldName);
                if (fileNameFieldNav == null)
                	fileNameFieldNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'FileName\' or @name=\'FILENAME\']", ControllerFieldName);
                if (fileNameFieldNav != null)
                {
                    string fieldName = fileNameFieldNav.GetAttribute("name", String.Empty);
                    if (fieldName != this.FileNameField)
                    	this.FileNameField = fieldName;
                    actionValues.Add(new FieldValue(fieldName, Path.GetFileName(fileName)));
                    if (deleting)
                    	ClearLastFieldValue(actionValues);
                }
                actionValues.Add(new FieldValue(ControllerFieldName, stream));
            }
            return actionValues;
        }
        
        private void ClearLastFieldValue(List<FieldValue> values)
        {
            FieldValue v = values[(values.Count - 1)];
            v.NewValue = null;
            v.Modified = true;
        }
        
        private bool ProcessUploadViaBusinessRule(BlobAdapter ba)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            List<FieldValue> actionValues = CreateActionValues(file.InputStream, file.ContentType, file.FileName, file.ContentLength);
            if (ba != null)
            	foreach (FieldValue fv in actionValues)
                	ba.ValidateFieldValue(fv);
            // try process uploading via a business rule
            ActionArgs args = new ActionArgs();
            args.Controller = DataController;
            args.CommandName = "UploadFile";
            args.CommandArgument = ControllerFieldName;
            args.Values = actionValues.ToArray();
            ActionResult r = Blob.CreateDataController().Execute(DataController, UploadDownloadViewName, args);
            bool supportsContentType = false;
            bool supportsFileName = false;
            DetectSupportForSpecialFields(actionValues, out supportsContentType, out supportsFileName);
            bool canceled = r.Canceled;
            if (canceled && !((supportsContentType || supportsFileName)))
            	return true;
            // update Content Type and Length
            args.LastCommandName = "Edit";
            args.CommandName = "Update";
            args.CommandArgument = UploadDownloadViewName;
            actionValues.RemoveAt((actionValues.Count - 1));
            if (HttpContext.Current.Request.Url.ToString().EndsWith("&_v=2"))
            	foreach (FieldValue v in actionValues)
                	if (v.Name == FileNameField)
                    {
                        actionValues.Remove(v);
                        break;
                    }
            args.Values = actionValues.ToArray();
            args.IgnoreBusinessRules = true;
            r = Blob.CreateDataController().Execute(DataController, UploadDownloadViewName, args);
            return canceled;
        }
        
        public virtual void LoadFile(Stream stream)
        {
            if ((BlobHandlerInfo.Current != null) && BlobHandlerInfo.Current.ProcessDownloadViaBusinessRule(stream))
            	return;
            using (SqlStatement getBlob = BlobFactory.CreateBlobSelectStatement())
            	if (getBlob.Read())
                {
                    object v = getBlob[0];
                    if (!(DBNull.Value.Equals(v)))
                    	if (typeof(string).Equals(getBlob.Reader.GetFieldType(0)))
                        {
                            byte[] stringData = Encoding.Default.GetBytes(((string)(v)));
                            stream.Write(stringData, 0, stringData.Length);
                        }
                        else
                        {
                            byte[] data = ((byte[])(v));
                            stream.Write(data, 0, data.Length);
                        }
                }
        }
        
        private void DetectSupportForSpecialFields(List<FieldValue> values, out bool supportsContentType, out bool supportsFileName)
        {
            supportsContentType = false;
            supportsFileName = false;
            foreach (FieldValue v in values)
            	if (v.Name.Equals(ContentTypeField, StringComparison.OrdinalIgnoreCase))
                	supportsContentType = true;
                else
                	if (v.Name.Equals(FileNameField, StringComparison.OrdinalIgnoreCase))
                    	supportsFileName = true;
        }
        
        public bool ProcessDownloadViaBusinessRule(Stream stream)
        {
            bool supportsContentType = false;
            bool supportsFileName = false;
            List<FieldValue> actionValues = CreateActionValues(stream, null, null, 0);
            DetectSupportForSpecialFields(actionValues, out supportsContentType, out supportsFileName);
            // try processing download via a business rule
            ActionArgs args = new ActionArgs();
            args.Controller = DataController;
            args.CommandName = "DownloadFile";
            args.CommandArgument = ControllerFieldName;
            args.Values = actionValues.ToArray();
            ActionResult r = Blob.CreateDataController().Execute(DataController, UploadDownloadViewName, args);
            foreach (FieldValue v in r.Values)
            	if (v.Name.Equals(ContentTypeField, StringComparison.OrdinalIgnoreCase))
                	Current.ContentType = Convert.ToString(v.Value);
                else
                	if (v.Name.Equals(FileNameField, StringComparison.OrdinalIgnoreCase))
                    	Current.FileName = Convert.ToString(v.Value);
            // see if we still need to retrieve the content type or the file name from the database
            bool needsContentType = String.IsNullOrEmpty(Current.ContentType);
            bool needsFileName = String.IsNullOrEmpty(Current.FileName);
            if ((needsContentType && supportsContentType) || (needsFileName && supportsFileName))
            {
                actionValues = CreateActionValues(null, null, null, 0);
                List<string> filter = new List<string>();
                foreach (FieldValue v in actionValues)
                	filter.Add(String.Format("{0}:={1}", v.Name, v.Value));
                PageRequest request = new PageRequest();
                request.Controller = DataController;
                request.View = UploadDownloadViewName;
                request.PageSize = 1;
                request.RequiresMetaData = true;
                request.Filter = filter.ToArray();
                request.MetadataFilter = new string[] {
                        "fields"};
                ViewPage page = Blob.CreateDataController().GetPage(request.Controller, request.View, request);
                if (page.Rows.Count == 1)
                {
                    object[] row = page.Rows[0];
                    if (supportsContentType)
                    	Current.ContentType = Convert.ToString(page.SelectFieldValue(ContentTypeField, row));
                    if (supportsFileName)
                    	Current.FileName = Convert.ToString(page.SelectFieldValue(FileNameField, row));
                }
            }
            return r.Canceled;
        }
    }
    
    public partial class BlobFactory
    {
        
        public static SortedDictionary<string, BlobHandlerInfo> Handlers = new SortedDictionary<string, BlobHandlerInfo>();
        
        public static void RegisterHandler(string key, string tableName, string fieldName, string[] keyFieldNames, string text, string contentType)
        {
            Handlers.Add(key, new BlobHandlerInfo(key, tableName, fieldName, keyFieldNames, text, contentType));
        }
        
        public static void RegisterHandler(string key, string tableName, string fieldName, string[] keyFieldNames, string text, string dataController, string controllerFieldName)
        {
            Handlers.Add(key, new BlobHandlerInfo(key, tableName, fieldName, keyFieldNames, text, String.Empty, dataController, controllerFieldName));
        }
        
        public static SqlStatement CreateBlobSelectStatement()
        {
            BlobHandlerInfo handler = BlobHandlerInfo.Current;
            if (handler != null)
            {
                string parameterMarker = SqlStatement.GetParameterMarker(String.Empty);
                List<string> keyValues = handler.CreateKeyValues();
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("select {0} from {1} where ", handler.FieldName, handler.TableName);
                for (int i = 0; (i < handler.KeyFieldNames.Length); i++)
                {
                    if (i > 0)
                    	sb.Append(" and ");
                    sb.AppendFormat("{0}={1}p{2}", handler.KeyFieldNames[i], parameterMarker, i);
                }
                SqlText getBlob = new SqlText(sb.ToString());
                for (int j = 0; (j < handler.KeyFieldNames.Length); j++)
                	getBlob.AddParameter(String.Format("{0}p{1}", parameterMarker, j), getBlob.StringToValue(keyValues[j]));
                return getBlob;
            }
            return null;
        }
        
        public static SqlStatement CreateBlobUpdateStatement()
        {
            BlobHandlerInfo handler = BlobHandlerInfo.Current;
            if (handler != null)
            {
                string parameterMarker = SqlStatement.GetParameterMarker(String.Empty);
                List<string> keyValues = handler.CreateKeyValues();
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("update {0} set {1} = ", handler.TableName, handler.FieldName);
                if (file.ContentLength == 0)
                	sb.Append("null");
                else
                	sb.AppendFormat("{0}blob", parameterMarker);
                sb.Append(" where ");
                for (int i = 0; (i < handler.KeyFieldNames.Length); i++)
                {
                    if (i > 0)
                    	sb.Append(" and ");
                    sb.AppendFormat("{0}={1}p{2}", handler.KeyFieldNames[i], parameterMarker, i);
                }
                SqlText updateBlob = new SqlText(sb.ToString());
                if (file.ContentLength > 0)
                {
                    byte[] data = new byte[file.ContentLength];
                    file.InputStream.Read(data, 0, data.Length);
                    updateBlob.AddParameter((parameterMarker + "blob"), data);
                }
                for (int j = 0; (j < handler.KeyFieldNames.Length); j++)
                	updateBlob.AddParameter(String.Format("{0}p{1}", parameterMarker, j), updateBlob.StringToValue(keyValues[j]));
                return updateBlob;
            }
            return null;
        }
    }
    
    public class Blob : GenericHandlerBase, IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        
        public const int ThumbnailCacheTimeout = 5;
        
        public static SortedDictionary<Guid, string> ImageFormats;
        
        public static SortedDictionary<int, RotateFlipType> JpegOrientationRotateFlips;
        
        static Blob()
        {
            ImageFormats = new SortedDictionary<Guid, string>();
            ImageFormats.Add(ImageFormat.Bmp.Guid, "image/bmp");
            ImageFormats.Add(ImageFormat.Emf.Guid, "image/emf");
            ImageFormats.Add(ImageFormat.Exif.Guid, "image/bmp");
            ImageFormats.Add(ImageFormat.Gif.Guid, "image/gif");
            ImageFormats.Add(ImageFormat.Jpeg.Guid, "image/jpeg");
            ImageFormats.Add(ImageFormat.Png.Guid, "image/png");
            ImageFormats.Add(ImageFormat.Tiff.Guid, "image/tiff");
            ImageFormats.Add(ImageFormat.Wmf.Guid, "image/Wmf");
            JpegOrientationRotateFlips = new SortedDictionary<int, RotateFlipType>();
            JpegOrientationRotateFlips.Add(1, RotateFlipType.RotateNoneFlipNone);
            JpegOrientationRotateFlips.Add(2, RotateFlipType.RotateNoneFlipX);
            JpegOrientationRotateFlips.Add(3, RotateFlipType.Rotate180FlipNone);
            JpegOrientationRotateFlips.Add(4, RotateFlipType.Rotate180FlipX);
            JpegOrientationRotateFlips.Add(5, RotateFlipType.Rotate90FlipX);
            JpegOrientationRotateFlips.Add(6, RotateFlipType.Rotate90FlipNone);
            JpegOrientationRotateFlips.Add(7, RotateFlipType.Rotate270FlipX);
            JpegOrientationRotateFlips.Add(8, RotateFlipType.Rotate270FlipNone);
        }
        
        bool IHttpHandler.IsReusable
        {
            get
            {
                return false;
            }
        }
        
        public static bool DirectAccessMode
        {
            get
            {
                return (BinaryData != null);
            }
        }
        
        public static byte[] BinaryData
        {
            get
            {
                object o = HttpContext.Current.Items["BlobHandlerInfo_Data"];
                if (o == null)
                	return null;
                return ((byte[])(o));
            }
            set
            {
                HttpContext.Current.Items["BlobHandlerInfo_Data"] = value;
            }
        }
        
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            BlobHandlerInfo handler = BlobHandlerInfo.Current;
            if (handler == null)
            	throw new HttpException(404, String.Empty);
            BlobAdapter ba = null;
            if (handler.DataController != null)
            	ba = BlobAdapterFactory.Create(handler.DataController, handler.FieldName.Replace("\"", String.Empty));
            if (ba != null)
            {
                handler.ContentTypeField = ba.ContentTypeField;
                handler.FileNameField = ba.FileNameField;
                handler.LengthField = ba.LengthField;
            }
            string val = handler.Value.Split('|')[1];
            if (!(ValidateBlobAccess(context, handler, ba, val)))
            {
                context.Response.StatusCode = 403;
                return;
            }
            if ((handler.Mode == BlobMode.Original) || (context.Request.HttpMethod == "POST"))
            	AppendDownloadTokenCookie();
            if (handler.Mode == BlobMode.Upload)
            {
                bool success = handler.SaveFile(context, ba, val);
                if (!(ApplicationServices.IsTouchClient))
                	RenderUploader(context, handler, success);
                else
                	if (!(success))
                    	throw new HttpException(500, handler.Error);
            }
            else
            	if (Blob.DirectAccessMode)
                {
                    Stream stream = null;
                    if (ba == null)
                    {
                        stream = new MemoryStream();
                        handler.LoadFile(stream);
                    }
                    else
                    	stream = ba.ReadBlob(val);
                    stream.Position = 0;
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    stream.Close();
                    Blob.BinaryData = data;
                    return;
                }
                else
                	if (ba == null)
                    {
                        string tempFileName = Path.GetTempFileName();
                        Stream stream = File.Create(tempFileName);
                        handler.LoadFile(stream);
                        CopyToOutput(context, stream, handler);
                        stream.Close();
                        File.Delete(tempFileName);
                    }
                    else
                    {
                        Stream stream = null;
                        if (handler.Mode.Equals(BlobMode.Thumbnail))
                        {
                            string contentType = ba.ReadContentType(val);
                            if (String.IsNullOrEmpty(contentType) || !(contentType.StartsWith("image/")))
                            	stream = new MemoryStream();
                        }
                        if (stream == null)
                        	stream = ba.ReadBlob(val);
                        handler.ProcessDownloadViaBusinessRule(stream);
                        CopyToOutput(context, stream, handler);
                        if (stream != null)
                        	stream.Close();
                    }
            HttpRequest request = context.Request;
            bool requireCaching = (request.IsSecureConnection && ((request.Browser.Browser == "IE") && (request.Browser.MajorVersion < 9)));
            if (!(requireCaching) && !(handler.AllowCaching))
            	context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
        
        private bool ValidateBlobAccess(HttpContext context, BlobHandlerInfo handler, BlobAdapter ba, string val)
        {
            if (Blob.DirectAccessMode)
            	return true;
            string key = context.Request.Params["_validationKey"];
            if (((ba == null) || !(ba.IsPublic)) && (!(context.User.Identity.IsAuthenticated) && key != BlobAdapter.ValidationKey))
            	return false;
            PageRequest pr = new PageRequest(0, 1, String.Empty, null);
            ControllerConfiguration config = Controller.CreateConfigurationInstance(GetType(), handler.DataController);
            XPathNodeIterator iterator = config.Select("/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']");
            List<string> filter = new List<string>();
            string[] vals = val.Split('|');
            int count = 0;
            while (iterator.MoveNext())
            {
                filter.Add(String.Format("{0}:={1}", iterator.Current.GetAttribute("name", String.Empty), vals[count]));
                count++;
            }
            pr.Filter = filter.ToArray();
            string view = String.Empty;
            iterator = config.Select("/c:dataController/c:views/c:view[@type=\'Form\']", String.Empty);
            if (iterator.MoveNext())
            	view = iterator.Current.GetAttribute("id", String.Empty);
            else
            	view = Controller.GetSelectView(handler.DataController);
            pr.FieldFilter = new string[] {
                    handler.ControllerFieldName};
            ViewPage page = ControllerFactory.CreateDataController().GetPage(handler.DataController, view, pr);
            if ((page.Rows.Count == 0) || !((page.Rows[0].Length == (count + 1))))
            	return false;
            return true;
        }
        
        public static IDataController CreateDataController()
        {
            IDataController controller = ControllerFactory.CreateDataController();
            if (DirectAccessMode)
            	((DataControllerBase)(controller)).AllowPublicAccess = true;
            return controller;
        }
        
        public static byte[] Read(string key)
        {
            string[] keyInfo = key.Split(new char[] {
                        '='});
            return Blob.Read(keyInfo[0], keyInfo[1]);
        }
        
        public static byte[] Read(string blobHandler, object keyValue)
        {
            string v = keyValue.ToString();
            if (!(v.StartsWith("o|")))
            	v = ("o|" + v);
            HttpContext context = HttpContext.Current;
            context.Items["BlobHandlerInfo_Current"] = BlobFactory.Handlers[blobHandler];
            context.Items["BlobHandlerInfo_Value"] = v;
            Blob.BinaryData = new byte[0];
            ((IHttpHandler)(new Blob())).ProcessRequest(context);
            byte[] result = Blob.BinaryData;
            context.Items.Remove("BlobHandlerInfo_Current");
            context.Items.Remove("BlobHandlerInfo_Value");
            context.Items.Remove("BlobHandlerInfo_Data");
            return result;
        }
        
        public static ImageCodecInfo ImageFormatToEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            	if (codec.FormatID == format.Guid)
                	return codec;
            return null;
        }
        
        private void CopyToOutput(HttpContext context, Stream stream, BlobHandlerInfo handler)
        {
            int offset = 0;
            stream.Position = offset;
            byte[] buffer = null;
            Image img = null;
            long streamLength = stream.Length;
            // attempt to auto-detect content type as an image
            string contentType = handler.ContentType;
            if ((String.IsNullOrEmpty(contentType) || contentType.StartsWith("image/")) && (stream.Length > 0))
            	try
                {
                    img = Image.FromStream(stream);
                    if (img.RawFormat.Equals(ImageFormat.Jpeg))
                    	foreach (PropertyItem p in img.PropertyItems)
                        	if ((p.Id == 274) && (p.Type == 3))
                            {
                                ushort orientation = BitConverter.ToUInt16(p.Value, 0);
                                RotateFlipType flipType;
                                JpegOrientationRotateFlips.TryGetValue(orientation, out flipType);
                                if (flipType != RotateFlipType.RotateNoneFlipNone)
                                {
                                    img.RotateFlip(flipType);
                                    img.RemovePropertyItem(p.Id);
                                    stream = new MemoryStream();
                                    EncoderParameters saveParams = new EncoderParameters();
                                    saveParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ((uint)(93)));
                                    img.Save(stream, ImageFormatToEncoder(ImageFormat.Jpeg), saveParams);
                                    streamLength = stream.Length;
                                    contentType = "image/jpg";
                                    break;
                                }
                            }
                }
                catch (Exception )
                {
                    try
                    {
                        // Correction for Northwind database image format
                        offset = 78;
                        stream.Position = offset;
                        buffer = new byte[(streamLength - offset)];
                        stream.Read(buffer, 0, buffer.Length);
                        img = Image.FromStream(new MemoryStream(buffer, 0, buffer.Length));
                        streamLength = (streamLength - offset);
                    }
                    catch (Exception ex)
                    {
                        offset = 0;
                        context.Trace.Write(ex.ToString());
                    }
                }
            // send an original or a thumbnail to the output
            if (handler.AllowCaching)
            {
                // draw a thumbnail
                int thumbWidth = 92;
                int thumbHeight = 64;
                bool crop = !(context.Request.RawUrl.Contains("_nocrop"));
                if (ApplicationServices.IsTouchClient)
                {
                    thumbWidth = 80;
                    thumbHeight = 80;
                    JObject settings = ((JObject)(ApplicationServices.Create().DefaultSettings["ui"]["thumbnail"]));
                    if (settings != null)
                    {
                        if (settings["width"] != null)
                        	thumbWidth = ((int)(settings["width"]));
                        if (settings["height"] != null)
                        	thumbHeight = ((int)(settings["height"]));
                        if (settings["crop"] != null)
                        	crop = ((bool)(settings["crop"]));
                    }
                }
                if ((img != null) && (handler.Mode == BlobMode.Original))
                {
                    thumbWidth = handler.MaxWidth.Value;
                    thumbHeight = Convert.ToInt32((img.Height 
                                    * (thumbWidth / Convert.ToDouble(img.Width))));
                    crop = !(context.Request.RawUrl.Contains("_nocrop"));
                }
                Bitmap thumbnail = new Bitmap(thumbWidth, thumbHeight);
                Graphics g = Graphics.FromImage(thumbnail);
                Rectangle r = new Rectangle(0, 0, thumbWidth, thumbHeight);
                g.FillRectangle(Brushes.Transparent, r);
                if (img != null)
                {
                    if (!(handler.MaxWidth.HasValue))
                    {
                        double thumbnailAspect = (Convert.ToDouble(r.Height) / Convert.ToDouble(r.Width));
                        if ((img.Width < r.Width) && (img.Height < r.Height))
                        {
                            r.Width = img.Width;
                            r.Height = img.Height;
                        }
                        else
                        	if (img.Width > img.Height)
                            {
                                r.Height = Convert.ToInt32((Convert.ToDouble(r.Width) * thumbnailAspect));
                                r.Width = Convert.ToInt32((Convert.ToDouble(r.Height) 
                                                * (Convert.ToDouble(img.Width) / Convert.ToDouble(img.Height))));
                            }
                            else
                            	if (img.Height > img.Width)
                                {
                                    thumbnailAspect = (Convert.ToDouble(r.Width) / Convert.ToDouble(r.Height));
                                    r.Width = Convert.ToInt32((Convert.ToDouble(r.Height) * thumbnailAspect));
                                    r.Height = Convert.ToInt32((Convert.ToDouble(r.Width) 
                                                    * (Convert.ToDouble(img.Height) / Convert.ToDouble(img.Width))));
                                }
                                else
                                {
                                    r.Width = Convert.ToInt32((Convert.ToDouble(img.Height) * thumbnailAspect));
                                    r.Height = r.Width;
                                }
                    }
                    double aspect = (Convert.ToDouble(thumbnail.Width) / r.Width);
                    if (r.Width <= r.Height)
                    	aspect = (Convert.ToDouble(thumbnail.Height) / r.Height);
                    if (!(handler.MaxWidth.HasValue))
                    {
                        if (aspect > 1)
                        	aspect = 1;
                        r.Width = Convert.ToInt32((Convert.ToDouble(r.Width) * aspect));
                        r.Height = Convert.ToInt32((Convert.ToDouble(r.Height) * aspect));
                    }
                    if (crop)
                    	if (r.Width > r.Height)
                        	r.Inflate((r.Width - r.Height), Convert.ToInt32((Convert.ToDouble((r.Width - r.Height)) * aspect)));
                        else
                        	r.Inflate(Convert.ToInt32((Convert.ToDouble((r.Height - r.Width)) * aspect)), (r.Height - r.Width));
                    r.Location = new Point(((thumbnail.Width - r.Width) 
                                    / 2), ((thumbnail.Height - r.Height) 
                                    / 2));
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, r);
                }
                else
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    Font f = new Font("Arial", ((float)(7.5D)));
                    string text = handler.FileName;
                    if (String.IsNullOrEmpty(text))
                    	text = handler.Text;
                    else
                    {
                        text = Path.GetExtension(text);
                        if (text.StartsWith(".") && (text.Length > 1))
                        {
                            text = text.Substring(1).ToLower();
                            f = new Font("Arial", ((float)(12)), FontStyle.Bold);
                        }
                    }
                    g.FillRectangle(Brushes.White, r);
                    g.DrawString(text, f, Brushes.Black, r);
                }
                // produce thumbnail data
                MemoryStream ts = new MemoryStream();
                if (handler.MaxWidth.HasValue)
                {
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt64(90));
                    thumbnail.Save(ts, ImageFormatToEncoder(ImageFormat.Jpeg), encoderParams);
                }
                else
                	thumbnail.Save(ts, ImageFormat.Png);
                ts.Flush();
                ts.Position = 0;
                byte[] td = new byte[ts.Length];
                ts.Read(td, 0, td.Length);
                ts.Close();
                // Send thumbnail to the output
                context.Response.AddHeader("Content-Length", td.Length.ToString());
                context.Response.ContentType = "image/png";
                context.Response.OutputStream.Write(td, 0, td.Length);
                if ((img == null) && !(handler.AllowCaching))
                	context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                else
                {
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(Blob.ThumbnailCacheTimeout));
                }
            }
            else
            {
                if ((img != null) && String.IsNullOrEmpty(contentType))
                	contentType = ImageFormats[img.RawFormat.Guid];
                if (String.IsNullOrEmpty(contentType))
                	contentType = "application/octet-stream";
                string fileName = handler.FileName;
                if (String.IsNullOrEmpty(fileName))
                	fileName = String.Format("{0}{1}.{2}", handler.Key, handler.Reference, contentType.Substring((contentType.IndexOf("/") + 1)));
                context.Response.ContentType = contentType;
                context.Response.AddHeader("Content-Disposition", ("filename=" + HttpUtility.UrlEncode(fileName)));
                context.Response.AddHeader("Content-Length", streamLength.ToString());
                if (stream.Length == 0)
                {
                    context.Response.StatusCode = 404;
                    return;
                }
                stream.Position = offset;
                buffer = new byte[(1024 * 32)];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                while (bytesRead > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, bytesRead);
                    offset = (offset + bytesRead);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                }
            }
        }
        
        private void RenderUploader(HttpContext context, BlobHandlerInfo handler, bool uploadSuccess)
        {
            HtmlTextWriter writer = new HtmlTextWriter(context.Response.Output);
            writer.WriteLine("<!DOCTYPE html PUBLIC \\\"-//W3C//DTD XHTML 1.0 Transitional//EN\\\" \\\"http://www.w3." +
                    "org/TR/xhtml1/DTD/xhtml1-transitional.dtd\\\">");
            writer.AddAttribute("xmlns", "http://www.w3.org/1999/xhtml");
            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            // head
            writer.RenderBeginTag(HtmlTextWriterTag.Head);
            writer.RenderBeginTag(HtmlTextWriterTag.Title);
            writer.Write("Uploader");
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            string script = @"
                       
function ShowUploadControls() { 
    document.getElementById('UploadControlsPanel').style.display ='block'; 
    document.getElementById('StartUploadPanel').style.display = 'none';   
    document.getElementById('FileUpload').focus();      
} 
function Owner() {
    var m = window.location.href.match(/owner=(.+?)&/);
    return m ? parent.$find(m[1]) : null;
}
function StartUpload(msg) {
    if (msg && !window.confirm(msg)) return;
    if (parent && parent.window.Web) {
        var m = window.location.href.match(/&index=(\d+)$/);
        if (m) Owner()._showUploadProgress(m[1], document.forms[0]);
    }
}
function UploadSuccess(key, message) { 
    if (!Owner().get_isInserting())
        if (parent && parent.window.Web) { 
            parent.Web.DataView.showMessage(message); 
            Owner().refresh(false,null,'FIELD_NAME');
        }     
        else 
            alert('Success');
}";
            writer.WriteLine(script.Replace("FIELD_NAME", String.Format("^({0}|{1}|{2})?$", handler.ContentTypeField, handler.FileNameField, handler.LengthField)));
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
            writer.RenderBeginTag(HtmlTextWriterTag.Style);
            writer.WriteLine("body{font-family:tahoma;font-size:8.5pt;margin:4px;background-color:white;}");
            writer.WriteLine("input{font-family:tahoma;font-size:8.5pt;}");
            writer.WriteLine("input.FileUpload{padding:3px}");
            writer.RenderEndTag();
            writer.RenderEndTag();
            // body
            string message = null;
            if (uploadSuccess)
            	if (HttpContext.Current.Request.Files[0].ContentLength > 0)
                	message = String.Format(Localizer.Replace("BlobUploded", "<b>Confirmation:</b> {0} has been uploaded successfully. <b>It may take up to {1}" +
                                " minutes for the thumbnail to reflect the uploaded content.</b>"), handler.Text.ToLower(), Blob.ThumbnailCacheTimeout);
                else
                	message = String.Format(Localizer.Replace("BlobCleared", "<b>Confirmation:</b> {0} has been cleared."), handler.Text.ToLower());
            else
            	if (!(String.IsNullOrEmpty(handler.Error)))
                	message = String.Format(Localizer.Replace("BlobUploadError", "<b>Error:</b> failed to upload {0}. {1}"), handler.Text.ToLower(), BusinessRules.JavaScriptString(handler.Error));
            if (!(String.IsNullOrEmpty(message)))
            	writer.AddAttribute("onload", String.Format("UploadSuccess(\'{0}={1}\', \'{2}\')", handler.Key, handler.Value.Replace("u|", "t|"), BusinessRules.JavaScriptString(message)));
            writer.RenderBeginTag(HtmlTextWriterTag.Body);
            // form
            writer.AddAttribute(HtmlTextWriterAttribute.Name, "form1");
            writer.AddAttribute("method", "post");
            writer.AddAttribute("action", context.Request.RawUrl);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "form1");
            writer.AddAttribute("enctype", "multipart/form-data");
            writer.RenderBeginTag(HtmlTextWriterTag.Form);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            // begin "start upload" controls
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "StartUploadPanel");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(Localizer.Replace("BlobUploadLinkPart1", "Click"));
            writer.Write(" ");
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "ShowUploadControls();return false");
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(Localizer.Replace("BlobUploadLinkPart2", "here"));
            writer.RenderEndTag();
            writer.Write(" ");
            writer.Write(Localizer.Replace("BlobUploadLinkPart3", "to upload or clear {0} file."), handler.Text.ToLower());
            // end of "start upload" controls
            writer.RenderEndTag();
            // begin "upload controls"
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "UploadControlsPanel");
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            // "FileUpload" input
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "File");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, "FileUpload");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "FileUpload");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "FileUpload");
            writer.AddAttribute(HtmlTextWriterAttribute.Onchange, "StartUpload()");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            // "FileClear" input
            if (!((context.Request.QueryString[handler.Key] == "u|")))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
                writer.AddAttribute(HtmlTextWriterAttribute.Id, "FileClear");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "FileClear");
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, String.Format("StartUpload(\'{0}\')", BusinessRules.JavaScriptString(Localizer.Replace("BlobClearConfirm", "Clear?"))));
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Localizer.Replace("BlobClearText", "Clear"));
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
            }
            // end of "upload controls"
            writer.RenderEndTag();
            // close "div"
            writer.RenderEndTag();
            // close "form"
            writer.RenderEndTag();
            // close "body"
            writer.RenderEndTag();
            // close "html"
            writer.RenderEndTag();
            writer.Close();
        }
        
        public static Image ResizeImage(Image image, int width, int height)
        {
            try
            {
                Rectangle destRect = new Rectangle(0, 0, width, height);
                Bitmap destImage = new Bitmap(width, height);
                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (Graphics graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    using (ImageAttributes wrap = new ImageAttributes())
                    {
                        wrap.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrap);
                    }
                }
                return destImage;
            }
            catch (Exception )
            {
                return image;
            }
        }
    }
    
    public class BlobAdapterArguments : SortedDictionary<string, string>
    {
    }
    
    public class BlobAdapterFactoryBase
    {
        
        public static Regex ArgumentParserRegex = new Regex("^\\s*(?\'ArgumentName\'[\\w\\-]+)\\s*:\\s*(?\'ArgumentValue\'[\\s\\S]+?)\\s*$", (RegexOptions.Multiline | RegexOptions.IgnoreCase));
        
        protected virtual BlobAdapterArguments ParseAdapterConfig(string fieldName, string config)
        {
            bool capture = false;
            BlobAdapterArguments args = new BlobAdapterArguments();
            Match m = ArgumentParserRegex.Match(config);
            while (m.Success)
            {
                string name = m.Groups["ArgumentName"].Value.ToLower();
                string value = m.Groups["ArgumentValue"].Value;
                if (name.Equals("field"))
                	capture = (fieldName == value);
                if (capture)
                	args[name] = value;
                m = m.NextMatch();
            }
            return args;
        }
        
        protected virtual BlobAdapter CreateFromConfig(string controller, string fieldName, string adapterConfig)
        {
            if (!(adapterConfig.Contains(fieldName)))
            	return null;
            BlobAdapterArguments arguments = ParseAdapterConfig(fieldName, adapterConfig);
            if (arguments.Count.Equals(0))
            	return null;
            ProcessArguments(controller, fieldName, arguments);
            try
            {
                string storageSystem = arguments["storage-system"].ToLower();
                if (storageSystem == "file")
                	return new FileSystemBlobAdapter(controller, arguments);
                if (storageSystem == "azure")
                	return new AzureBlobAdapter(controller, arguments);
            }
            catch (Exception )
            {
            }
            return null;
        }
        
        void ProcessArguments(string controller, string fieldName, BlobAdapterArguments args)
        {
            string config = ConfigurationManager.AppSettings[String.Format("{0}{1}BlobAdapter", controller, fieldName)];
            string storageSystem = args["storage-system"].ToLower();
            if (!(String.IsNullOrEmpty(config)))
            {
                string[] configArgs = config.Split(new char[] {
                            ';'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string arg in configArgs)
                {
                    string[] parts = arg.Split(new char[] {
                                ':',
                                '='}, 2);
                    if (parts.Length == 2)
                    	args[parts[0].Trim().ToLower()] = parts[1].Trim();
                }
            }
            SortedDictionary<string, string> replacements = new SortedDictionary<string, string>();
            foreach (string key in args.Keys)
            {
                string value = args[key];
                if (value.StartsWith("$"))
                	replacements[key] = ConfigurationManager.AppSettings[value.Substring(1)];
                else
                	if (key == "storage-system")
                    	storageSystem = value.ToLower();
            }
            if (storageSystem != "file")
            {
                string keyName = "key";
                string settingName = "AzureBlobStorageKey";
                if (storageSystem == "s3")
                {
                    keyName = "access-key";
                    settingName = "AmazonS3StorageKey";
                }
                if (!(replacements.ContainsKey(keyName)))
                {
                    replacements[keyName] = ConfigurationManager.AppSettings["BlobStorageKey"];
                    if (String.IsNullOrEmpty(replacements[keyName]))
                    	replacements[keyName] = ConfigurationManager.AppSettings[settingName];
                }
            }
            foreach (KeyValuePair<string, string> replacement in replacements)
            	if (!(String.IsNullOrEmpty(replacement.Value)))
                	args[replacement.Key] = replacement.Value;
        }
        
        protected static string ReadConfig(string controller)
        {
            ControllerConfiguration config = DataControllerBase.CreateConfigurationInstance(typeof(BlobAdapter), controller);
            return ((string)(config.Evaluate("string(/c:dataController/c:blobAdapterConfig)"))).Trim();
        }
        
        public static BlobAdapter Create(string controller, string fieldName)
        {
            string adapterConfig = ReadConfig(controller);
            if (String.IsNullOrEmpty(adapterConfig))
            	return null;
            BlobAdapterFactory factory = new BlobAdapterFactory();
            return factory.CreateFromConfig(controller, fieldName, adapterConfig);
        }
        
        public static void InitializeRow(ViewPage page, object[] row)
        {
            string adapterConfig = ReadConfig(page.Controller);
            if (String.IsNullOrEmpty(adapterConfig))
            	return;
            BlobAdapterFactory factory = new BlobAdapterFactory();
            int blobFieldIndex = 0;
            foreach (DataField field in page.Fields)
            {
                BlobAdapter ba = factory.CreateFromConfig(page.Controller, field.Name, adapterConfig);
                if (ba != null)
                {
                    object pk = null;
                    int primaryKeyFieldIndex = 0;
                    foreach (DataField keyField in page.Fields)
                    {
                        if (keyField.IsPrimaryKey)
                        {
                            pk = row[primaryKeyFieldIndex];
                            if ((pk != null) && (pk.GetType() == typeof(byte[])))
                            	pk = new Guid(((byte[])(pk)));
                            break;
                        }
                        primaryKeyFieldIndex++;
                    }
                    int utilityFieldIndex = 0;
                    string fileName = String.Empty;
                    string contentType = String.Empty;
                    int length = -1;
                    foreach (DataField utilityField in page.Fields)
                    {
                        if (utilityField.Name == ba.FileNameField)
                        	fileName = Convert.ToString(row[utilityFieldIndex]);
                        else
                        	if (utilityField.Name == ba.ContentTypeField)
                            	contentType = Convert.ToString(row[utilityFieldIndex]);
                            else
                            	if (utilityField.Name == ba.LengthField)
                                	length = Convert.ToInt32(row[utilityFieldIndex]);
                        utilityFieldIndex++;
                    }
                    if (length != 0 && (!(String.IsNullOrEmpty(fileName)) || !(String.IsNullOrEmpty(contentType))))
                    	row[blobFieldIndex] = pk.ToString();
                }
                blobFieldIndex++;
            }
        }
    }
    
    public partial class BlobAdapterFactory : BlobAdapterFactoryBase
    {
    }
    
    public class BlobAdapter
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _controller;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _fieldName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private BlobAdapterArguments _arguments;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _pathTemplate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _contentTypeField;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _lengthField;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _fileNameField;
        
        protected ViewPage _page;
        
        protected string _keyValue;
        
        public BlobAdapter(string controller, BlobAdapterArguments arguments)
        {
            this.Controller = controller;
            this.Arguments = arguments;
            Initialize();
        }
        
        public string Controller
        {
            get
            {
                return this._controller;
            }
            set
            {
                this._controller = value;
            }
        }
        
        public string FieldName
        {
            get
            {
                return this._fieldName;
            }
            set
            {
                this._fieldName = value;
            }
        }
        
        public BlobAdapterArguments Arguments
        {
            get
            {
                return this._arguments;
            }
            set
            {
                this._arguments = value;
            }
        }
        
        public string PathTemplate
        {
            get
            {
                return this._pathTemplate;
            }
            set
            {
                this._pathTemplate = value;
            }
        }
        
        public string ContentTypeField
        {
            get
            {
                return this._contentTypeField;
            }
            set
            {
                this._contentTypeField = value;
            }
        }
        
        public string LengthField
        {
            get
            {
                return this._lengthField;
            }
            set
            {
                this._lengthField = value;
            }
        }
        
        public string FileNameField
        {
            get
            {
                return this._fileNameField;
            }
            set
            {
                this._fileNameField = value;
            }
        }
        
        public static string ValidationKey
        {
            get
            {
                return "B060A558855FB057ECE5DDE64E9CCD4F0ADB138D3A6F370FE5A31008E4FA91786DEE109330B56EDF1" +
                    "58FE8FE8443DF90FE552DA507D6A40F85BE2FDD06D57BA3";
            }
        }
        
        public virtual bool IsPublic
        {
            get
            {
                return false;
            }
        }
        
        protected virtual void Initialize()
        {
            this.FieldName = Arguments["field"];
            string s = null;
            if (Arguments.TryGetValue("path-template", out s))
            	this.PathTemplate = s;
            if (Arguments.TryGetValue("content-type-field", out s))
            	this.ContentTypeField = s;
            else
            	this.ContentTypeField = (FieldName + "ContentType");
            if (Arguments.TryGetValue("length-field", out s))
            	this.LengthField = s;
            else
            	this.LengthField = (FieldName + "Length");
            if (Arguments.TryGetValue("file-name-field", out s))
            	this.FileNameField = s;
            else
            	this.FileNameField = (FieldName + "FileName");
        }
        
        public virtual Stream ReadBlob(string keyValue)
        {
            return null;
        }
        
        public virtual bool WriteBlob(HttpPostedFile file, string keyValue)
        {
            return false;
        }
        
        public virtual ViewPage SelectViewPageByKey(string keyValue)
        {
            ControllerConfiguration config = DataControllerBase.CreateConfigurationInstance(typeof(BlobAdapter), this.Controller);
            string keyField = ((string)(config.Evaluate("string(/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']/@name)")));
            PageRequest request = new PageRequest();
            request.Controller = Controller;
            request.View = DataControllerBase.GetSelectView(Controller);
            request.Filter = new string[] {
                    String.Format("{0}:={1}", keyField, keyValue)};
            request.RequiresMetaData = true;
            request.PageSize = 1;
            ViewPage page = Blob.CreateDataController().GetPage(request.Controller, request.View, request);
            return page;
        }
        
        public virtual void CopyData(Stream input, Stream output)
        {
            byte[] buffer = new byte[(16 * 1024)];
            int bytesRead;
            bool readNext = true;
            while (readNext)
            {
                bytesRead = input.Read(buffer, 0, buffer.Length);
                output.Write(buffer, 0, bytesRead);
                if (bytesRead == 0)
                	readNext = false;
            }
        }
        
        public string KeyValueToPath(string keyValue)
        {
            string extendedPath = ExtendPathTemplate(keyValue);
            if (extendedPath.StartsWith("/"))
            	extendedPath = extendedPath.Substring(1);
            return extendedPath;
        }
        
        public virtual string ExtendPathTemplate(string keyValue)
        {
            return ExtendPathTemplate(PathTemplate, keyValue);
        }
        
        public virtual string ExtendPathTemplate(string template, string keyValue)
        {
            if (String.IsNullOrEmpty(template) || !(template.Contains("{")))
            	return keyValue;
            _keyValue = keyValue;
            string extendedPath = Regex.Replace(template, "\\{(\\$?\\w+)\\}", DoReplaceFieldNameInTemplate);
            if (extendedPath.StartsWith("~"))
            {
                extendedPath = extendedPath.Substring(1);
                if (extendedPath.StartsWith("\\"))
                	extendedPath = extendedPath.Substring(1);
                extendedPath = Path.Combine(HttpRuntime.AppDomainAppPath, extendedPath);
            }
            return extendedPath;
        }
        
        protected virtual string DoReplaceFieldNameInTemplate(Match m)
        {
            if (this._page == null)
            	this._page = SelectViewPageByKey(this._keyValue);
            int fieldIndex = 0;
            string targetFieldName = m.Groups[1].Value;
            string fieldName = targetFieldName;
            bool requiresProcessing = fieldName.StartsWith("$");
            if (requiresProcessing)
            	fieldName = this.FileNameField;
            foreach (DataField df in this._page.Fields)
            {
                if (df.Name == fieldName)
                {
                    string v = Convert.ToString(this._page.Rows[0][fieldIndex]);
                    if (requiresProcessing)
                    {
                        if (targetFieldName.Equals("$Extension", StringComparison.OrdinalIgnoreCase))
                        {
                            string extension = Path.GetExtension(v);
                            if (extension.StartsWith("."))
                            	extension = extension.Substring(1);
                            return extension;
                        }
                        if (targetFieldName.Equals("$FileNameWithoutExtension", StringComparison.OrdinalIgnoreCase))
                        	return Path.GetFileNameWithoutExtension(v);
                    }
                    return v;
                }
                fieldIndex++;
            }
            return String.Empty;
        }
        
        public virtual void ValidateFieldValue(FieldValue fv)
        {
            if ((fv.Name == FileNameField) && fv.Modified)
            {
                string newValue = Convert.ToString(fv.NewValue);
                if (!(String.IsNullOrEmpty(newValue)))
                	fv.NewValue = Regex.Replace(newValue, "[^\\w\\.]", "-");
            }
        }
        
        public virtual string ReadContentType(string keyValue)
        {
            return ExtendPathTemplate(String.Format("{{{0}}}", ContentTypeField), keyValue);
        }
    }
}
