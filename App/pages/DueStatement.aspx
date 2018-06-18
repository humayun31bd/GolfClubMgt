<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="DueStatement.aspx.cs" Inherits="Pages_DueStatement"  Title="Due Statement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Due Statement</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Due Statement">
      <div id="view1" data-controller="MemberDueStatement" data-view="grid1" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>