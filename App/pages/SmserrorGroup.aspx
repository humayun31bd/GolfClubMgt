﻿<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="SmserrorGroup.aspx.cs" Inherits="Pages_SmserrorGroup"  Title="Smserror Group" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Smserror Group</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="SmserrorGroup" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple" data-show-modal-forms="true"></div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows smserror group management.</div>
    </div>
  </div>
</asp:Content>