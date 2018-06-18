<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="NonMemberRegistration.aspx.cs" Inherits="Pages_NonMemberRegistration"  Title="Non Member Registration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Non Member Registration</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="MemberInfoNonMember" data-view="grid1" data-tags="modal-never"></div>
  </div>
</asp:Content>