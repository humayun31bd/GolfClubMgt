﻿<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="GameType.aspx.cs" Inherits="Pages_GameType"  Title="Game Type" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Game Type</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="GameType" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple"></div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows game type management.</div>
    </div>
  </div>
</asp:Content>