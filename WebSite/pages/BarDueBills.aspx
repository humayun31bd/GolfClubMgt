<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="BarDueBills.aspx.cs" Inherits="Pages_BarDueBills"  Title="Bar Due Bills" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Bar Due Bills</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Due Bills">
      <div id="view1" data-controller="MemberRestBarBillBaronly"></div>
    </div>
    <div data-activator="Tab|Collections">
      <div id="view2" data-controller="MemberRestBarBillCollectionBar" data-refresh-interval="30"></div>
    </div>
  </div>
</asp:Content>