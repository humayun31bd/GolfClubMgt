﻿<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberServiceDueDetail.aspx.cs" Inherits="Pages_MemberServiceDueDetail"  Title="Member Service Due Detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Member Service Due Detail</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="MemberServiceDueDetail" data-view="grid1" data-show-in-summary="true" data-show-modal-forms="true"></div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows member service due detail management.</div>
    </div>
  </div>
</asp:Content>