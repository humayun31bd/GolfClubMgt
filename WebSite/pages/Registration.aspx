<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="Registration.aspx.cs" Inherits="Pages_Registration"  Title="Registration Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Registration Report</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Daily Registration">
      <div id="view1" data-controller="DailyRegistration" data-view="grid1" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>