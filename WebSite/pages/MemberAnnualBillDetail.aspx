<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberAnnualBillDetail.aspx.cs" Inherits="Pages_MemberAnnualBillDetail"  Title="Annual Due Create" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Annual Due Create</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Subscription Due Create">
      <div id="view2" data-controller="MemberSubsDueDetail" data-view="grid1" data-roles="Administrators,DueAdmin"></div>
    </div>
    <div data-activator="Tab|Member Contricution Due Create">
      <div id="view3" data-controller="MemberConWfdueDetail" data-view="grid1" data-roles="Administrators,DueAdmin"></div>
    </div>
    <div data-activator="Tab|Locker Due Bill Create">
      <div id="view4" data-controller="MemberLockerBillDetail" data-view="grid1" data-roles="Administrators,DueAdmin"></div>
    </div>
    <div data-activator="Tab|Annual Due Manage">
      <div id="view1" data-controller="MemberAnnualBillDetail" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows member annual bill detail management.</div>
    </div>
  </div>
</asp:Content>