﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using MyCompany.Handlers;

namespace MyCompany.Data
{
	public class AnnotationPlugIn : IPlugIn
    {
        
        private ControllerConfiguration _config;
        
        private List<FieldValue> _annotations;
        
        private bool _retrieveAnnotations;
        
        private bool _requireProcessing;
        
        private XPathNavigator _fields;
        
        static AnnotationPlugIn()
        {
            BlobFactory.Handlers.Add("AnnotationPlugIn", new AnnotationBlobHandler());
        }
        
        public static string AnnotationsPath
        {
            get
            {
                string p = WebConfigurationManager.AppSettings["AnnotationsPath"];
                if (String.IsNullOrEmpty(p))
                	p = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data");
                return p;
            }
        }
        
        ControllerConfiguration IPlugIn.Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }
        
        protected XPathNavigator Fields
        {
            get
            {
                if (_fields == null)
                	_fields = _config.SelectSingleNode("/c:dataController/c:fields");
                return _fields;
            }
        }
        
        string KeyFields
        {
            get
            {
                string kf = String.Empty;
                XPathNodeIterator iterator = Fields.Select("c:field[@isPrimaryKey=\'true\']/@name", _config.Resolver);
                while (iterator.MoveNext())
                {
                    if (kf.Length > 0)
                    	kf = (kf + ",");
                    kf = (kf + iterator.Current.Value);
                }
                return kf;
            }
        }
        
        public static string UserEmail
        {
            get
            {
                if (HttpContext.Current.User.Identity.GetType() == typeof(System.Security.Principal.WindowsIdentity))
                	return String.Empty;
                else
                {
                    MembershipUser user = Membership.GetUser();
                    if (user == null)
                    	return String.Empty;
                    return user.Email;
                }
            }
        }
        
        public static string GenerateDataRecordPath()
        {
            return GenerateDataRecordPath(null, null, null, 0);
        }
        
        public static string GenerateDataRecordPath(string controller, ViewPage page, FieldValue[] values, int rowIndex)
        {
            // Sample path:
            // [Documents]\Code OnTime\Projects\Web Site Factory\Annotations\App_Data\OrderDetails\10248,11
            // Sample URL parameter:
            // u|OrderDetails,_Annotation_AttachmentNew|10248|11
            string p = AnnotationPlugIn.AnnotationsPath;
            if (String.IsNullOrEmpty(controller))
            {
                string handlerInfo = HttpContext.Current.Request["AnnotationPlugIn"];
                Match m = Regex.Match(handlerInfo, "^((t|o|u)\\|){0,1}\\w+\\|(\\w+).+?\\|(.+)?$");
                if (m.Success)
                {
                    p = Path.Combine(p, m.Groups[3].Value);
                    p = Path.Combine(p, m.Groups[4].Value.Replace("|", ","));
                }
            }
            else
            {
                p = Path.Combine(p, controller);
                string keys = String.Empty;
                foreach (DataField field in page.Fields)
                	if (field.IsPrimaryKey)
                    {
                        string keyValue = null;
                        if (values == null)
                        	keyValue = Convert.ToString(page.Rows[rowIndex][page.Fields.IndexOf(field)]);
                        else
                        	foreach (FieldValue v in values)
                            	if (v.Name == field.Name)
                                {
                                    keyValue = Convert.ToString(v.Value);
                                    break;
                                }
                        if (keys.Length > 0)
                        	keys = (keys + ",");
                        keys = (keys + keyValue.Trim());
                    }
                p = Path.Combine(p, keys);
            }
            return p;
        }
        
        ControllerConfiguration IPlugIn.Create(ControllerConfiguration config)
        {
            if (config.Navigator.CanEdit)
            	return config;
            XmlDocument document = new XmlDocument();
            document.LoadXml(config.Navigator.OuterXml);
            return new ControllerConfiguration(document.CreateNavigator());
        }
        
        void IPlugIn.PreProcessPageRequest(PageRequest request, ViewPage page)
        {
            XPathNavigator view = _config.SelectSingleNode("//c:view[@id=\'{0}\' and @type=\'Form\']/c:categories", request.View);
            if ((view != null) && ((request.PageSize > 0) && (!(request.Inserting) && (_config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'_Annotation_NoteNew\']") == null))))
            {
                _requireProcessing = true;
                string ns = ControllerConfiguration.Namespace;
                List<DynamicExpression> expressions = new List<DynamicExpression>(_config.Expressions);
                // create NewXXX fields under "fields" node
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                XmlWriter writer = XmlWriter.Create(sb, settings);
                // NoteNew field
                writer.WriteStartElement("field", ns);
                writer.WriteAttributeString("name", "_Annotation_NoteNew");
                writer.WriteAttributeString("type", "String");
                writer.WriteAttributeString("allowSorting", "false");
                writer.WriteAttributeString("allowQBE", "false");
                writer.WriteAttributeString("label", Localizer.Replace("AnnotationNoteNewFieldLabel", "Notes"));
                writer.WriteAttributeString("computed", "true");
                writer.WriteElementString("formula", ns, "null");
                writer.WriteEndElement();
                DynamicExpression de = new DynamicExpression();
                de.Target = "_Annotation_NoteNew";
                de.Scope = DynamicExpressionScope.DataFieldVisibility;
                de.Type = DynamicExpressionType.ClientScript;
                de.Test = "this.get_isEditing()";
                de.ViewId = request.View;
                expressions.Add(de);
                // AttachmentNew field
                writer.WriteStartElement("field", ns);
                writer.WriteAttributeString("name", "_Annotation_AttachmentNew");
                writer.WriteAttributeString("type", "Byte[]");
                writer.WriteAttributeString("onDemand", "true");
                writer.WriteAttributeString("sourceFields", this.KeyFields);
                writer.WriteAttributeString("onDemandHandler", "AnnotationPlugIn");
                writer.WriteAttributeString("allowQBE", "false");
                writer.WriteAttributeString("allowSorting", "false");
                writer.WriteAttributeString("label", Localizer.Replace("AnnotationAttachmentNewFieldLabel", "Attachment"));
                writer.WriteAttributeString("computed", "true");
                writer.WriteElementString("formula", ns, "null");
                writer.WriteEndElement();
                writer.Close();
                this.Fields.AppendChild(sb.ToString());
                DynamicExpression ade = new DynamicExpression();
                ade.Target = "_Annotation_AttachmentNew";
                ade.Scope = DynamicExpressionScope.DataFieldVisibility;
                ade.Type = DynamicExpressionType.ClientScript;
                ade.Test = "this.get_isEditing()";
                ade.ViewId = request.View;
                expressions.Add(ade);
                // create NewXXX data fields under "view/dataFields" node
                sb = new StringBuilder();
                writer = XmlWriter.Create(sb);
                writer.WriteStartElement("category", ns);
                writer.WriteAttributeString("id", "Annotations");
                writer.WriteAttributeString("headerText", Localizer.Replace("AnnotationCategoryHeaderText", "Notes and Attachments"));
                writer.WriteElementString("description", ns, Localizer.Replace("AnnotationCategoryDescription", "Enter optional notes and attach files."));
                writer.WriteStartElement("dataFields", ns);
                // _Annotation_NoteNew dataField
                writer.WriteStartElement("dataField", ns);
                writer.WriteAttributeString("fieldName", "_Annotation_NoteNew");
                writer.WriteAttributeString("columns", "50");
                writer.WriteAttributeString("rows", "7");
                writer.WriteEndElement();
                // _Annotation_AttachmentNew
                writer.WriteStartElement("dataField", ns);
                writer.WriteAttributeString("fieldName", "_Annotation_AttachmentNew");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();
                view.AppendChild(sb.ToString());
                _retrieveAnnotations = !(request.Inserting);
                _config.Expressions = expressions.ToArray();
            }
        }
        
        void IPlugIn.ProcessPageRequest(PageRequest request, ViewPage page)
        {
            if (page.Rows.Count == 0)
            {
                page.Icons = new string[0];
                return;
            }
            if (!(_requireProcessing))
            {
                List<string> icons = new List<string>();
                for (int i = 0; (i < page.Rows.Count); i++)
                {
                    string rowDir = AnnotationPlugIn.GenerateDataRecordPath(request.Controller, page, null, i);
                    if (Directory.Exists(rowDir))
                    	icons.Add("Attachment");
                    else
                    	icons.Add(null);
                }
                page.Icons = icons.ToArray();
                return;
            }
            List<DynamicExpression> expressions = new List<DynamicExpression>(page.Expressions);
            DynamicExpression de = new DynamicExpression();
            de.Target = "Annotations";
            de.Scope = DynamicExpressionScope.CategoryVisibility;
            de.Type = DynamicExpressionType.ClientScript;
            de.Test = "!this.get_isInserting()";
            de.ViewId = page.View;
            expressions.Add(de);
            page.Expressions = expressions.ToArray();
            if (!(_retrieveAnnotations))
            	return;
            DataField field = page.FindField("_Annotation_AttachmentNew");
            if (field != null)
            {
                int fieldIndex = page.Fields.IndexOf(field);
                string newValue = String.Format("{0},{1}|{2}", request.Controller, field.Name, Regex.Replace(((string)(page.Rows[0][fieldIndex])), "^\\w+\\|(.+)$", "$1"));
                if (field.Name == "_Annotation_AttachmentNew")
                	newValue = ("null|" + newValue);
                page.Rows[0][fieldIndex] = newValue;
            }
            string p = AnnotationPlugIn.GenerateDataRecordPath(request.Controller, page, null, 0);
            if (Directory.Exists(p))
            {
                string[] files = Directory.GetFiles(p, "*.xml");
                List<object> values = new List<object>(page.Rows[0]);
                int i = (files.Length - 1);
                while (i >= 0)
                {
                    string filename = files[i];
                    XPathDocument doc = new XPathDocument(filename);
                    XPathNavigator nav = doc.CreateNavigator().SelectSingleNode("/*");
                    DataField f = null;
                    if (nav.Name == "note")
                    {
                        f = new DataField();
                        f.Name = "_Annotation_Note";
                        f.Type = "String";
                        f.HeaderText = String.Format(Localizer.Replace("AnnotationNoteDynamicFieldHeaderText", "{0} written at {1}"), ReadNameAndEmail(nav), Convert.ToDateTime(nav.GetAttribute("timestamp", String.Empty)));
                        f.Columns = 50;
                        f.Rows = 7;
                        f.TextMode = TextInputMode.Note;
                        values.Add(nav.Value);
                    }
                    else
                    	if (nav.Name == "attachment")
                        {
                            f = new DataField();
                            f.Name = "_Annotation_Attachment";
                            f.Type = "Byte[]";
                            f.HeaderText = String.Format(Localizer.Replace("AnnotationAttachmentDynamicFieldHeaderText", "{0} attached <b>{1}</b> at {2}"), ReadNameAndEmail(nav), nav.GetAttribute("fileName", String.Empty), Convert.ToDateTime(nav.GetAttribute("timestamp", String.Empty)));
                            f.OnDemand = true;
                            f.OnDemandHandler = "AnnotationPlugIn";
                            f.OnDemandStyle = OnDemandDisplayStyle.Link;
                            if (nav.GetAttribute("contentType", String.Empty).StartsWith("image/"))
                            	f.OnDemandStyle = OnDemandDisplayStyle.Thumbnail;
                            f.CategoryIndex = (page.Categories.Count - 1);
                            values.Add(nav.GetAttribute("value", String.Empty));
                        }
                    if (f != null)
                    {
                        f.Name = (f.Name + Path.GetFileNameWithoutExtension(filename));
                        f.AllowNulls = true;
                        f.CategoryIndex = (page.Categories.Count - 1);
                        if (!(Controller.UserIsInRole("Administrators")))
                        	f.ReadOnly = true;
                        page.Fields.Add(f);
                    }
                    i = (i - 1);
                }
                page.Rows[0] = values.ToArray();
                if (files.Length > 0)
                {
                    page.Categories[(page.Categories.Count - 1)].Tab = Localizer.Replace("AnnotationTab", "Notes & Attachments");
                    expressions.RemoveAt((expressions.Count - 1));
                    page.Expressions = expressions.ToArray();
                }
            }
            else
            {
                de.Test = "this.get_isEditing() && this.get_view()._displayAnnotations";
                ActionGroup g = new ActionGroup();
                page.ActionGroups.Add(g);
                g.Scope = "ActionBar";
                g.Flat = true;
                Action a = new Action();
                g.Actions.Add(a);
                a.WhenLastCommandName = "Edit";
                a.WhenView = page.View;
                a.CommandName = "ClientScript";
                a.CommandArgument = "this.get_view()._displayAnnotations=true;this._focusedFieldName = \'_Annotation_No" +
                    "teNew\';this._raiseSelectedDelayed=false;";
                a.HeaderText = Localizer.Replace("AnnotationActionHeaderText", "Annotate");
                a.CssClass = "AttachIcon";
                a.WhenClientScript = "this.get_view()._displayAnnotations!=true;";
            }
        }
        
        private string ReadNameAndEmail(XPathNavigator nav)
        {
            string userName = nav.GetAttribute("username", String.Empty);
            string email = nav.GetAttribute("email", String.Empty);
            if (String.IsNullOrEmpty(email))
            	return userName;
            return String.Format("<a href=\"mailto:{0}\" title=\"{0}\" target=\"_blank\">{1}</a>", email, userName);
        }
        
        void IPlugIn.PreProcessArguments(ActionArgs args, ActionResult result, ViewPage page)
        {
            _annotations = new List<FieldValue>();
            if (args.Values != null)
            	foreach (FieldValue v in args.Values)
                	if (v.Name.StartsWith("_Annotation_") && v.Modified)
                    {
                        _annotations.Add(v);
                        v.Modified = false;
                    }
        }
        
        void IPlugIn.ProcessArguments(ActionArgs args, ActionResult result, ViewPage page)
        {
            if (_annotations.Count == 0)
            	return;
            string p = AnnotationPlugIn.GenerateDataRecordPath(args.Controller, page, args.Values, 0);
            if (!(Directory.Exists(p)))
            	Directory.CreateDirectory(p);
            foreach (FieldValue v in _annotations)
            {
                Match m = Regex.Match(v.Name, "^_Annotation_(Note)(New|\\w+)$");
                if (m.Success)
                {
                    if (m.Groups[1].Value == "Note")
                    {
                        string fileName = m.Groups[2].Value;
                        if (fileName == "New")
                        {
                            fileName = DateTime.Now.ToString("u");
                            fileName = Regex.Replace(fileName, "[\\W]", String.Empty);
                        }
                        fileName = Path.Combine(p, (fileName + ".xml"));
                        if (!(String.IsNullOrEmpty(Convert.ToString(v.NewValue))))
                        {
                            XmlWriterSettings settings = new XmlWriterSettings();
                            settings.CloseOutput = true;
                            XmlWriter writer = XmlWriter.Create(new FileStream(fileName, FileMode.Create), settings);
                            try
                            {
                                writer.WriteStartElement("note");
                                writer.WriteAttributeString("timestamp", DateTime.Now.ToString("o"));
                                writer.WriteAttributeString("username", HttpContext.Current.User.Identity.Name);
                                writer.WriteAttributeString("email", AnnotationPlugIn.UserEmail);
                                writer.WriteString(Convert.ToString(v.NewValue));
                                writer.WriteEndElement();
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }
                        else
                        {
                            File.Delete(fileName);
                            if (Directory.GetFiles(p).Length == 0)
                            	Directory.Delete(p);
                        }
                    }
                }
            }
        }
    }
    
    public class AnnotationBlobHandler : BlobHandlerInfo
    {
        
        public AnnotationBlobHandler()
        {
            this.Key = "AnnotationPlugIn";
        }
        
        public override string Text
        {
            get
            {
                return "Attachment";
            }
            set
            {
                base.Text = value;
            }
        }
        
        public override bool SaveFile(HttpContext context, BlobAdapter ba, string keyValue)
        {
            if (context.Request.Files.Count != 1)
            	return false;
            HttpPostedFile file = context.Request.Files[0];
            string p = AnnotationPlugIn.GenerateDataRecordPath();
            if (!(Directory.Exists(p)))
            	Directory.CreateDirectory(p);
            // u|OrderDetails,_Annotation_AttachmentNew|10248|11
            Match m = Regex.Match(this.Value, "_Annotation_Attachment(\\w+)\\|");
            if (m.Success)
            {
                string fileName = m.Groups[1].Value;
                if (fileName == "New")
                {
                    fileName = DateTime.Now.ToString("u");
                    fileName = Regex.Replace(fileName, "[\\W]", String.Empty);
                    if (System.IO.File.Exists(Path.Combine(p, (fileName + ".xml"))))
                    	fileName = (fileName + "_");
                }
                fileName = Path.Combine(p, (fileName + ".xml"));
                if (file.ContentLength == 0)
                	foreach (string f in Directory.GetFiles(p, (Path.GetFileNameWithoutExtension(fileName) + "*.*")))
                    	System.IO.File.Delete(f);
                else
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.CloseOutput = true;
                    XmlWriter writer = XmlWriter.Create(new FileStream(fileName, FileMode.Create), settings);
                    try
                    {
                        writer.WriteStartElement("attachment");
                        writer.WriteAttributeString("timestamp", DateTime.Now.ToString("o"));
                        writer.WriteAttributeString("username", HttpContext.Current.User.Identity.Name);
                        writer.WriteAttributeString("email", AnnotationPlugIn.UserEmail);
                        writer.WriteAttributeString("fileName", Path.GetFileName(file.FileName));
                        writer.WriteAttributeString("contentType", file.ContentType);
                        writer.WriteAttributeString("contentLength", file.ContentLength.ToString());
                        writer.WriteAttributeString("value", Regex.Replace(this.Value, "^.+?\\|([\\w,]+?)_Annotation_Attachment(New|\\w+)(.+)$", String.Format("1|$1_Annotation_Attachment{0}$3", Path.GetFileNameWithoutExtension(fileName))));
                        writer.WriteEndElement();
                        fileName = ((Path.GetFileNameWithoutExtension(fileName) + "_") 
                                    + Path.GetExtension(file.FileName));
                        file.SaveAs(Path.Combine(p, fileName));
                    }
                    finally
                    {
                        writer.Close();
                    }
                }
            }
            return true;
        }
        
        public override void LoadFile(Stream stream)
        {
            string p = AnnotationPlugIn.GenerateDataRecordPath();
            // t|1|OrderDetails,_Annotation_Attachment20091219164153Z|10248|11
            Match m = Regex.Match(this.Value, "_Annotation_Attachment(\\w+)\\|");
            string fileName = Path.Combine(p, (m.Groups[1].Value + ".xml"));
            XPathNavigator nav = new XPathDocument(fileName).CreateNavigator().SelectSingleNode("/*");
            fileName = Path.Combine(p, ((Path.GetFileNameWithoutExtension(fileName) + "_") 
                            + Path.GetExtension(nav.GetAttribute("fileName", String.Empty))));
            if (!(this.Value.StartsWith("t|")))
            {
                this.ContentType = nav.GetAttribute("contentLength", String.Empty);
                HttpContext.Current.Response.ContentType = this.ContentType;
            }
            this.FileName = nav.GetAttribute("fileName", String.Empty);
            Stream input = File.OpenRead(fileName);
            try
            {
                byte[] buffer = new byte[(1024 * 64)];
                long offset = 0;
                long bytesRead = input.Read(buffer, 0, buffer.Length);
                while (bytesRead > 0)
                {
                    stream.Write(buffer, 0, Convert.ToInt32(bytesRead));
                    offset = (offset + bytesRead);
                    bytesRead = input.Read(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                input.Close();
            }
        }
    }
}
