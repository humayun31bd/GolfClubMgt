<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="MemberFeeReport.aspx.cs" Inherits="Pages_MemberFeeReport"  Title="Member Fee Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Member Fee Report</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Member Fees">
      <div id="view1" data-controller="MemberFeeReport" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Absentee Members">
      <div id="view2" data-controller="AbsenteeMemberReport" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Member Information">
      <div id="view3" data-controller="MemberInformationReport" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>