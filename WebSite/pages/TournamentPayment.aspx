<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="TournamentPayment.aspx.cs" Inherits="Pages_TournamentPayment"  Title="Tournament Payment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Tournament Payment</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Tournament Payment Approval">
      <div id="view2" data-controller="TourRegisterMemberApproval" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Tournament Payment Approved">
      <div id="view3" data-controller="TourRegisterMemberApproved"></div>
    </div>
  </div>
</asp:Content>