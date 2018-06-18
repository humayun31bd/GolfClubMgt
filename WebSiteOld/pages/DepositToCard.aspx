<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="DepositToCard.aspx.cs" Inherits="Pages_DepositToCard"  Title="Deposit To Card" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Deposit To Card</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Deposit to Card">
      <div id="view1" data-controller="MemberCardTransactionDeposit" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Deposit Approved">
      <div id="view2" data-controller="MemberCardDepositApproved" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>