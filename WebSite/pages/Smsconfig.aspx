<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="Smsconfig.aspx.cs" Inherits="Pages_Smsconfig"  Title="Smsconfig" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Smsconfig</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Card Verification SMS">
      <div id="view1" data-controller="Smsconfig" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple" data-start-command-name="Edit"></div>
    </div>
    <div data-activator="Tab|SMS Balance">
      <div id="view2" data-controller="Smsbalance" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple" data-show-action-bar="false" data-show-description="false" data-show-view-selector="false" data-show-quick-find="false" data-show-search-bar="false"></div>
    </div>
    <div data-activator="Tab|SMS Purchased">
      <div id="view3" data-controller="Smspurchase" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple" data-show-action-bar="false" data-show-description="false" data-show-view-selector="false" data-show-quick-find="false" data-show-search-bar="false"></div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows smsconfig management.</div>
    </div>
  </div>
</asp:Content>