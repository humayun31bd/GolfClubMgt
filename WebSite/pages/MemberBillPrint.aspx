<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberBillPrint.aspx.cs" Inherits="Pages_MemberBillPrint"  Title="Member Bill Print" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Member Bill Print</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Member Bill Print">
      <div id="view4" data-controller="MemberBillPaidApproved" data-view="grid1" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>