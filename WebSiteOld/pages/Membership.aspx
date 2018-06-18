<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="Membership.aspx.cs" Inherits="Pages_Membership"  Title="^MembershipTitle^Membership Manager^MembershipTitle^" %>
<%@ Register Src="../Controls/MembershipManager.ascx" TagName="MembershipManager"  TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">^MembershipTitle^Membership Manager^MembershipTitle^</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row"><uc:MembershipManager ID="control1" runat="server"></uc:MembershipManager></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">^MembershipAbout^This page allows to manage roles and users.^MembershipAbout^</div>
    </div>
  </div>
</asp:Content>