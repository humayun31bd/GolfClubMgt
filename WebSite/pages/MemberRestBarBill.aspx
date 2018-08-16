<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberRestBarBill.aspx.cs" Inherits="Pages_MemberRestBarBill"  Title="Restaurent/Bar Due Bills" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Restaurent/Bar Due Bills</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Dues">
      <div id="view1" data-controller="MemberRestBarBill" data-view="grid1" data-show-in-summary="true"></div>
    </div>
    <div data-activator="Tab|Collections">
      <div id="view2" data-controller="MemberRestBarBillCollection"></div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarPlaceHolder" runat="Server">
  <div class="TaskBox About">
    <div class="Inner">
      <div class="Header">About</div>
      <div class="Value">This page allows member rest bar bill management.</div>
    </div>
  </div>
</asp:Content>