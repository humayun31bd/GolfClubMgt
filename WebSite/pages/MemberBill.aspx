<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberBill.aspx.cs" Inherits="Pages_MemberBill"  Title="Bill Approval Pending" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Bill Approval Pending</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Member Bill Approval">
      <div id="view1" data-controller="MemberBill" data-view="grid1" data-show-in-summary="true" data-page-size="25" data-selection-mode="multiple" data-tags="modal-never" data-show-description="false"></div>
    </div>
    <div data-activator="Tab|Member Bill Approved">
      <div id="view2" data-controller="MemberBillApproved" data-view="grid1" data-selection-mode="multiple" data-tags="modal-never"></div>
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