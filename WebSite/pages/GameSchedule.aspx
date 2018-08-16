<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="GameSchedule.aspx.cs" Inherits="Pages_GameSchedule"  Title="Flight Schedule Create" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Flight Schedule Create</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Flight Schedule">
      <div id="view1" data-controller="GameFlightSchedule" data-view="grid1" data-page-size="120"></div>
    </div>
  </div>
</asp:Content>