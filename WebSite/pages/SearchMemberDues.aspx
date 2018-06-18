<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="SearchMemberDues.aspx.cs" Inherits="Pages_SearchMemberDues"  Title="Search Member Dues" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Search Member Dues</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view4" data-controller="MemberSearch" data-view="form1" data-start-command-name="Edit"></div>
  </div>
</asp:Content>