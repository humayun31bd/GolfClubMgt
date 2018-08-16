<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberBill.aspx.cs" Inherits="Pages_MemberBill"  Title="Bill Approval Pending" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Bill Approval Pending</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row" style="padding-top:8px">
    <div data-activator="Tab|Member Payments Approval List">
      <div id="view2" data-controller="MemberBillApproval" data-view="grid1" data-selection-mode="multiple" data-show-quick-find="false"></div>
    </div>
    <div data-activator="Tab|Game Registration Payment Approval List">
      <div id="view1" data-controller="GameRegisterApproval" data-view="grid1" data-selection-mode="multiple" data-show-quick-find="false"></div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows member bill management.</div>
    </div>
  </div>
</asp:Content>