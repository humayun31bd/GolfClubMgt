<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" Title="Login"  %>
<%@ Register Src="Controls/Login.ascx" TagName="Login" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"><script type="text/javascript">
					
        function pageLoad() {
            var inputs = document.getElementsByTagName('input');
            for (var i = 0; i < inputs.length; i++)
                if (inputs[i].id.match(/_UserName/)) {
                inputs[i].focus();
                break;
            }
        }
    
			</script></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">
			Login
		</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server" />
<asp:Content ID="Content4" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <uc1:Login ID="Login1" runat="server" />
</asp:Content>