<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemberSMSPanel.ascx.cs" Inherits="Controls_MemberSMSPanel" %>
<!-- 
    This section provides a sample markup for Desktop user inteface.
-->
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div style="margin: 2px; border: solid 1px silver; padding: 8px;">uc:MemberSMSPanel</div>
    </ContentTemplate>
</asp:UpdatePanel>
<!-- 
    This section provides a sample markup for Touch UI user interface. 
-->
<div id="MemberSMSPanel" data-app-role="page" data-activator="Button|Member SMS Panel">
    <div data-flow="row">
      <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12 uc_fev">
        <table class="uc_fevtb" style="width: 100%">
            <tr>
                <td>
                    <table style="width: 30%; text-align: left">
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblSmSQty" runat="server" Text=""></asp:Label>
                                &amp;                               
                                    <asp:Label ID="lblSMSBalance" runat="server" Text="" ></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>SMS Body :
                            </td>
                            <td>
                                <asp:TextBox TextMode="MultiLine" ID="txtSMSBody" runat="server"
                                    Width="100%" Height="50px" placeholder="Enter your Message here"
                                    class="form-control ui-btn ui-icon-carat-d ui-btn-icon-right ui-corner-all ui-shadow">
                                </asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <h2>
                            <asp:Label ID="lblMsg" runat="server" Style="color: red;"></asp:Label></h2>
                    </div>
                </td>
            </tr>
            <tr>
                <td>&nbsp;
                </td>
            </tr>
            <tr>
                <div id="pnl_Member" visible="true" runat="server">
                    <table style="width: 98%;">
                        <tr>
                            <td>
                                <div id="Div1" runat="server">
                                    <div>
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <table style="width: 319px">
                                                        <tr>
                                                            <td valign="top">
                                                                <asp:Button CssClass="Button btn btn-primary btn-block" ID="btnMemberSendSMS" runat="server"                                                                    
                                                                    Text="Send SMS"
                                                                    Width="122px"
                                                                    OnClick="btnMemberSendSMS_Click" />
                                                            </td>
                                                            <td>&nbsp; &nbsp; &nbsp;</td>
                                                            <td valign="top">
                                                                <asp:Button CssClass="Button btn btn-primary btn-block" ID="btnSendSMSCancel" runat="server"
                                                                    Text="Cancle"
                                                                    Width="122px"
                                                                    OnClick="btnSendSMSCancel_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td></td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div>
                                        <asp:GridView ID="Member_grid" runat="server" AutoGenerateColumns="False"
                                            CellPadding="3" CellSpacing="2"         
                                            Visible="true"                                  
                                            Width="98%" AllowPaging="True" PageSize="50"
                                            EmptyDataText="There are no Members !!!"
                                            CssClass="mGrid"
                                            PagerStyle-CssClass="pgr"
                                            OnPageIndexChanging="Member_grid_PageIndexChanging"
                                            AlternatingRowStyle-CssClass="alt">
                                            <AlternatingRowStyle CssClass="alt" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Select All" HeaderStyle-HorizontalAlign="Center" 
                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chk_select" runat="server"
                                                            AutoPostBack="True"
                                                            OnCheckedChanged="chk_select_CheckedChanged" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkRowMember" runat="server" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="MemberID" HeaderText="MemberID" ItemStyle-Width="5%">
                                                    <ItemStyle CssClass="boundfield-hidden" />
                                                    <HeaderStyle CssClass="boundfield-hidden" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="MemberCode" HeaderText="Member Code" ItemStyle-Width="15%"></asp:BoundField>
                                                <asp:BoundField DataField="NameOfMember" HeaderText="Member Name" ItemStyle-Width="35%"></asp:BoundField>
                                                <asp:BoundField DataField="MobileNo" HeaderText="Mobile No" ItemStyle-Width="25%"></asp:BoundField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                        </asp:GridView>
                                    </div>
                                    <br />

                                    <table style="width: 98%">
                                        <tr>
                                            <td>
                                                <table style="width: 319px">
                                                    <tr>
                                                        <td>
                                                            <asp:Button CssClass="Button btn btn-primary btn-block" ID="btnMemberSendSMS1"
                                                                runat="server"
                                                                Text="Send SMS" Width="122px"
                                                                OnClick="btnMemberSendSMS_Click" />
                                                        </td>
                                                        <td>&nbsp; &nbsp; &nbsp;</td>
                                                        <td>
                                                            <asp:Button CssClass="Button btn btn-primary btn-block" ID="btnSendSMSCancel1"
                                                                runat="server" Text="Cancle"
                                                                Width="122px"
                                                                OnClick="btnSendSMSCancel_Click" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </tr>
        </table>
    </div>

  </div>
</div>

<%--<script type="text/javascript">
    (function () {
        if ($app.touch)
            $(document).one('start.app', function () {
                // The page of Touch UI application is ready to be configured
            });
    })();
</script>--%>
