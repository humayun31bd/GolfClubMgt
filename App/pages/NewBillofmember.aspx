<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="NewBillofmember.aspx.cs" Inherits="Pages_NewBillofmember"  Title="New Bill of member" %>
<%@ Register Src="../Controls/MemberBillCollectionNew.ascx" TagName="MemberBillCollectionNew"  TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">New Bill of member</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row"><uc:MemberBillCollectionNew ID="control1" runat="server"></uc:MemberBillCollectionNew></div>
</asp:Content>