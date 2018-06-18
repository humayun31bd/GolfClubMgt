<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberBillCollection.aspx.cs" Inherits="Pages_MemberBillCollection"  Title="Member Bill Collection" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Member Bill Collection</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Bill Collection Summary">
      <div id="view1" data-controller="MemberBillCollectionSummary" data-view="grid1" data-refresh-interval="30" data-show-quick-find="false"></div>
    </div>
    <div data-activator="Tab|Member Bill Collection">
      <div id="view2" data-controller="MemberBillCollectionStatement" data-view="grid1" data-refresh-interval="5" data-show-quick-find="false"></div>
    </div>
  </div>
</asp:Content>