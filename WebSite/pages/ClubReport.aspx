<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="ClubReport.aspx.cs" Inherits="Pages_ClubReport"  Title="Club Report" %>
<%@ Register Src="../Controls/ReportViewer.ascx" TagName="ReportViewer"  TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Club Report</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row"><uc:ReportViewer ID="control1" runat="server"></uc:ReportViewer></div>
</asp:Content>