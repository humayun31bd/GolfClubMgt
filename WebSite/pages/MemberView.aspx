<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberView.aspx.cs" Inherits="Pages_MemberView"  Title="Member View" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Member View</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|All Member View">
      <div id="view1" data-controller="MemberInfoView" data-view="grid1" data-page-size="150" data-selection-mode="multiple" data-show-pager="top-and-bottom"></div>
    </div>
  </div>
</asp:Content>