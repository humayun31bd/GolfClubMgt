<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberBillCollection.aspx.cs" Inherits="Pages_MemberBillCollection"  Title="Member Bill Collection" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Member Bill Collection</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="DailyRegistration" data-view="grid1"></div>
  </div>
</asp:Content>