<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="CardRechargeReport.aspx.cs" Inherits="Pages_CardRechargeReport"  Title="Card Recharge Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Card Recharge Report</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view2" data-controller="MemberCardRechargeReport" data-view="grid1"></div>
  </div>
</asp:Content>