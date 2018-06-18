<%@ Page Title="Help System" Language="C#" MasterPageFile="Help.master" %>

<script runat="server">
    void Page_Load(object sender, EventArgs e)
    {
        string topic = Request.Params["topic"];
        string pagePath = Server.MapPath(topic);
        if (System.IO.File.Exists(pagePath))
            Response.Redirect(topic);
        else
        {
            if (!Request.IsLocal)
                pagePath = pagePath.Replace(Server.MapPath("~/"), @"&lt;Root&gt;\");
            TopicPath.Text = pagePath;
        }
    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    This is a generic help page.<br />
    <br />
    Create a dedicated help page at
    <asp:Label ID="TopicPath" runat="server" Font-Bold="true" />
    to replace the generic content.
</asp:Content>
