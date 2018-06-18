<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="Caddie.aspx.cs" Inherits="Pages_Caddie"  Title="Caddie" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Caddie</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Caddie">
      <div id="view1" data-controller="Caddie" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Ball Boy">
      <div id="view2" data-controller="BallBoy" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows caddie management.</div>
    </div>
  </div>
</asp:Content>