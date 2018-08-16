<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberInfo.aspx.cs" Inherits="Pages_MemberInfo"  Title="Member Registration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Member Registration</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Registrated Member">
      <div id="view1" data-controller="MemberInfo" data-view="grid1" data-selection-mode="multiple" data-tags="modal-never"></div>
    </div>
    <div data-activator="Tab|Spouse ">
      <div id="view3" data-controller="MemberInfoSpouse" data-view="grid1" data-tags="modal-never
"></div>
    </div>
    <div data-activator="Tab|Children">
      <div id="view2" data-controller="MemberInfoChild" data-view="grid1" data-tags="modal-never
"></div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows member information management.</div>
    </div>
  </div>
</asp:Content>