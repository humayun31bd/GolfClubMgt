<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportViewer.ascx.cs" Inherits="Controls_ReportViewer"  %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<div id="PurchaseOrderReport" data-app-role="page" data-activator="Button|Report Viewer">
    <div id="Div1" runat="server">
        <div id="Div3" runat="server">

            <table style="width: 100%; height: 406px;">
                <tr>
                    <td style="vertical-align: top;">
                        <div style="align-content: center">
                            <rsweb:ReportViewer ID="ReportViewer2" runat="server" Font-Names="Verdana" Font-Size="8pt" SizeToReportContent="True" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" AsyncRendering="false" Visible="false">
                                <LocalReport>
                                    <DataSources>
                                        <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                                    </DataSources>
                                </LocalReport>
                            </rsweb:ReportViewer>
                        </div>

                    </td>
                    <td>
                        <embed runat="server" id="report" style="width: 100%;" height="550" name="whatever" type='application/pdf' />
                    </td>
                </tr>
            </table>

        </div>
    </div>
</div>

<script type="text/javascript">
    (function () {
        if ($app.touch)
            $(document).one('start.app', function () {
                // The page of Touch UI application is ready to be configured
            });
    })();
</script>
