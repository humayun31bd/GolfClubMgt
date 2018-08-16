<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="TournamentGameFilter.aspx.cs" Inherits="Pages_TournamentGameFilter"  Title="Tournament Game Filter" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Tournament Game Filter</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="TournamentGameFilter" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple"></div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows tournament game filter management.</div>
    </div>
  </div>
</asp:Content>