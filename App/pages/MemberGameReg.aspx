<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberGameReg.aspx.cs" Inherits="Pages_MemberGameReg"  Title="Game Payment Approval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Game Payment Approval</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Game Registration Approval">
      <div id="view1" data-controller="MemberGameRegApproval" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Game Registration Approved">
      <div id="view2" data-controller="MemberGameRegApproved"></div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows member game reg management.</div>
    </div>
  </div>
</asp:Content>