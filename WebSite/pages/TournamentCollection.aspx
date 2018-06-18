<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="TournamentCollection.aspx.cs" Inherits="Pages_TournamentCollection"  Title="Tournament Collection" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Tournament Collection</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="TournamentCollection" data-view="grid1"></div>
  </div>
</asp:Content>