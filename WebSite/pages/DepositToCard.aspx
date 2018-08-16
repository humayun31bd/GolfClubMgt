<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="DepositToCard.aspx.cs" Inherits="Pages_DepositToCard"  Title="Recharge Card" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Recharge Card</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Recharge Card">
      <div id="view1" data-controller="MemberCardTransactionDeposit" data-view="grid1" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>