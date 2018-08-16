<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberCardTransactions.aspx.cs" Inherits="Pages_MemberCardTransactions"  Title="My Card" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">My Card</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="column" style="padding-top:8px" data-width="49%">
    <div id="view1" data-controller="MemberCardByMember"></div>
  </div>
  <div data-flow="column" data-width="50%">
    <div data-activator="Tab|Card Deposited">
      <div id="view2" data-controller="MemberCardTransactionReportByMember"></div>
    </div>
  </div>
</asp:Content>