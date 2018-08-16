<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="CardTransactions.aspx.cs" Inherits="Pages_CardTransactions"  Title="Card Transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Card Transactions</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row" style="padding-top:8px">
    <div data-activator="Tab|Daily Transactions">
      <div id="view2" data-controller="MemberCardTransactionReport"></div>
    </div>
    <div data-activator="Tab|Card Statement Print">
      <div id="view1" data-controller="MemberCardStatementReport" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>