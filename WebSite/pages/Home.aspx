<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Pages_Home"  Title="^HomeTitle^Start^HomeTitle^" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">^HomeTitle^Start^HomeTitle^</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="MemberDashBoard" data-roles="Administrators,Users"></div>
  </div>
  <div data-flow="row">
    <div id="view2" data-controller="MemberInfoProfileUpdate" data-view="grid1" data-roles="Members"></div>
  </div>
</asp:Content>