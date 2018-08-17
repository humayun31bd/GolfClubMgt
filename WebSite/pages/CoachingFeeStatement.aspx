<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="CoachingFeeStatement.aspx.cs" Inherits="Pages_CoachingFeeStatement"  Title="Coaching Fee Statement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Coaching Fee Statement</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div id="view1" data-controller="CoachFeeReport" data-view="grid1"></div>
  </div>
</asp:Content>