<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="EmailSMS.aspx.cs" Inherits="Pages_EmailSMS"  Title="Email/SMS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitleContentPlaceHolder" runat="Server">Email/SMS</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentPlaceHolder" runat="Server">
  <div data-flow="row">
    <div data-activator="Tab|Due Member Send Email">
      <div id="view2" data-controller="MemberServiceBillDue" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Sending SMS">
      <div id="view3" data-controller="MemberSendSMS" data-view="AllMembers" data-show-in-summary="true" data-selection-mode="multiple"></div>
    </div>
    <div data-activator="Tab|Send Due SMS">
      <div id="view1" data-controller="MemberAll_DueStatementAsOnSendSMS" data-view="grid1" data-show-in-summary="true" data-selection-mode="multiple"></div>
    </div>
  </div>
</asp:Content>