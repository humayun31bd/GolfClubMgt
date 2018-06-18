<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemberBillCollectionNew.ascx.cs" Inherits="Controls_MemberBillCollectionNew" %>

<link rel="stylesheet" href="../style.css">
<link rel="stylesheet" href="../css/w3.css">


<!-- 
    This section provides a sample markup for Desktop user inteface.
-->
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div style="margin: 2px; border: solid 1px silver; padding: 8px;">uc:MemberBillCollectionNew</div>
    </ContentTemplate>
</asp:UpdatePanel>
<!-- 
    This section provides a sample markup for Touch UI user interface. 
-->
<div id="MemberBillCollectionNew" data-app-role="page" data-activator="Button|MemberBillCollectionNew">

    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="w3-container">

                <section class="w3-container w3-padding-large">
                    <div class="information-wrap">
                        <div class="w3-container w3-card textTransform" style="padding: 10px">
                            <h4>Collection Information</h4>
                            <div class="w3-row w3-border">
                                <div class="w3-col m8">
                                    <div class="w3-row w3-border-bottom">
                                        <div class="w3-col w3-border-right m5" style="margin: auto">
                                            <p style="padding-left: 10px; padding-right: 5px">Membership No</p>
                                        </div>
                                        <div class="w3-col m3" style="padding-top: 6px; padding-left: 5px; padding-right: 5px">
                                            <asp:TextBox ID="txtMemberID" runat="server" AutoPostBack="true" OnTextChanged="txtMemberID_TextChanged"></asp:TextBox>
                                            <asp:Label ID="lblMemberName" runat="server"></asp:Label>
                                            <%--                                    <input class="w3-input w3-border" type="text" placeholder="Membership Number"                                         
                                        ng-model="ctrl.member.memberId" 
                                        ng-change="ctrl.getMemberInfo()">--%>
                                        </div>

                                        <div class="w3-col w3-border-left w3-border-right m1">
                                            <p style="padding-left: 10px; padding-right: 5px">Date</p>
                                        </div>
                                        <div class="w3-col m3" style="padding-top: 6px; padding-left: 5px; padding-right: 5px">
                                            <%--<input class="w3-input w3-border" type="date" ng-model="ctrl.member.date">--%>
                                        </div>
                                    </div>
                                    <div class="w3-row w3-border-bottom">
                                        <div class="w3-col w3-border-right w3-border-top m5">
                                            <p style="padding-left: 10px; padding-right: 5px">Received With Thanks From</p>
                                        </div>
                                        <div class="w3-col w3-border-top m7">
                                            <%--<p style="padding-left: 10px; padding-right: 5px">{{ctrl.member.fullName}}--%><!--Brig Gen Obaidul Haque (Retd)--></p>
                                        </div>
                                    </div>
                                    <div class="w3-row">
                                        <div class="w3-col w3-border-right m5">
                                            <p style="padding-left: 10px; padding-right: 5px">Category</p>
                                        </div>
                                        <div class="w3-col m7">
                                            <p style="padding-left: 10px; padding-right: 5px">
                                                <asp:Label ID="lblCategory" runat="server"></asp:Label>
                                                <%--{{ctrl.member.category}}--%><!--Defence Service Officer-->
                                            </p>
                                        </div>
                                    </div>
                                </div>
                                <div class="w3-col m4">
                                    <table class="w3-table w3-bordered w3-padding">
                                        <tr style="border-top: none !important;">
                                            <td>Additional Amount</td>
                                            <td>: <%--{{ctrl.member.additionalAmount}}--%></td>
                                        </tr>
                                        <tr>
                                            <td>Annual Amount</td>
                                            <td>: <%--{{ctrl.member.annualDue}}--%></td>
                                        </tr>
                                        <tr>
                                            <td>Monthly Due</td>
                                            <td>: <%--{{ctrl.member.monthlyDue}}--%></td>
                                        </tr>
                                        <tr>
                                            <td>Contribution(wf) Due</td>
                                            <td>:<%-- {{ctrl.member.contributionDue}}--%></td>
                                        </tr>
                                        <tr>
                                            <td>Locker Due</td>
                                            <td>: <%--{{ctrl.member.lockerDue}}--%></td>
                                        </tr>
                                        <tr>
                                            <td>Tournament Due</td>
                                            <td>: <%--{{ctrl.member.tournamentDue}}--%></td>
                                        </tr>
                                        <tr style="border-bottom: none !important;">
                                            <td>Service Due</td>
                                            <td>: <%--{{ctrl.member.serviceDue}}--%></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
                <section class="w3-container w3-padding-large">
                    <div class="service-wrap">
                        <div class="w3-container w3-card textTransform" style="padding: 10px">
                            <h4>Other service</h4>
                            <div class="w3-row w3-border">
                                <div class="w3-col m12" style="overflow-y: scroll; height: 200px;">
                                    <table class="w3-table w3-bordered">
                                        <thead>
                                            <tr style="border-top: none !important;">
                                                <th>NO</th>
                                                <th>Name</th>
                                                <th>Price</th>
                                                <th>Quantity</th>
                                                <th>Total Amount</th>
                                            </tr>
                                        </thead>
                                        <%--<tbody>
                                    <tr ng-repeat="s in ctrl.services track by $index">
                                        <td>
                                            <input class="w3-check" type="checkbox" ng-model="ctrl.serviceItem.id[$index]" ng-true-value="{{s.id}}" ng-false-value="0" ng-change="ctrl.setTotalServiceAmount()">
                                        </td>
                                        <td>{{s.name}}</td>
                                        <td>{{s.price}}
                                            <input type="hidden" ng-model="ctrl.serviceItem.price[$index]" ng-init="ctrl.serviceItem.price[$index] = s.price" ng-value="s.price">
                                        </td>
                                        <td>
                                            <input class="w3-input" type="number" ng-init="ctrl.serviceItem.qty[$index] = 1" ng-model="ctrl.serviceItem.qty[$index]" min="1" ng-change="ctrl.setTotalServiceAmount()">
                                        </td>
                                        <td>
                                            <input class="w3-input" type="number" ng-model="ctrl.serviceItem.amountTotal[$index]" min="0" value="{{ctrl.serviceItem.qty[$index] * s.price}}" disabled>
                                        </td>
                                    </tr>
                                </tbody>--%>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
                <section class="w3-container w3-padding-large">
                    <div class="account-wrap">
                        <div class="w3-container w3-card textTransform" style="padding: 10px">
                            <h4>On Account of</h4>
                            <div class="w3-row w3-border">
                                <div class="w3-col m12">
                                    <table class="w3-table w3-bordered">
                                        <tr style="border-top: none !important;">
                                            <td>A)</td>
                                            <td>
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.isAddSubscription" ng-true-value="true" ng-false-value="false" ng-click="ctrl.calculateTotalAmount()">--%>
                                                <label>Subscription</label>
                                            </td>
                                            <td>
                                                <label class="labelLeft">From</label>
                                                <%--<input class="w3-input inputWidth" type="date" ng-model="ctrl.subscriptionFrom">--%>
                                            </td>
                                            <td>
                                                <label class="labelLeft">TO</label>
                                                <%--<input class="w3-input inputWidth labelLeft" type="date" ng-model="ctrl.subscriptionTo">--%>
                                                <%--<button class="w3-button w3-ripple w3-tiny" ng-click="ctrl.subscriptionFeeTotal = ctrl.subscriptionFee * ctrl.getTotalMonth(ctrl.subscriptionFrom, ctrl.subscriptionTo)">Add</button>--%>
                                            </td>
                                            <td>
                                                <%--<input class="w3-input" type="text" ng-model="ctrl.subscriptionFeeTotal" disabled></td>--%>
                                        </tr>
                                        <tr>
                                            <td>B)</td>
                                            <td>
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.isAddWelFareFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">--%>
                                                <label>Welfare Contribution (new)</label>
                                            </td>
                                            <td>
                                                <label class="labelLeft">From</label>
                                                <%--<input class="w3-input inputWidth" type="date" ng-model="ctrl.welFareFrom">--%>
                                            </td>
                                            <td>
                                                <label class="labelLeft">TO</label>
                                                <%--<input class="w3-input inputWidth labelLeft" type="date" ng-model="ctrl.welFareTo">--%>
                                                <%--<button class="w3-button w3-ripple w3-tiny" ng-click="ctrl.welFareContributionTotal = ctrl.welFareContribution * ctrl.getTotalMonth(ctrl.welFareFrom, ctrl.welFareTo)">Add</button>--%>
                                            </td>
                                            <td>
                                                <%--<input class="w3-input" type="text" ng-model="ctrl.welFareContributionTotal" disabled></td>--%>
                                        </tr>
                                        <tr>
                                            <td>C)</td>
                                            <td>
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.isAddLockerFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">--%>
                                                <label>Locker</label>
                                            </td>
                                            <td>
                                                <label class="labelLeft">From</label>
                                                <%--<input class="w3-input inputWidth" type="date" ng-model="ctrl.lockerFeeFrom">--%>
                                            </td>
                                            <td>
                                                <label class="labelLeft">To</label>
                                                <%--<input class="w3-input inputWidth labelLeft" type="date" ng-model="ctrl.lockerFeeTo">--%>
                                                <%--<button class="w3-button w3-ripple w3-tiny" ng-click="ctrl.lockerFeeTotal = ctrl.lockerFee * ctrl.getTotalMonth(ctrl.lockerFeeFrom, ctrl.lockerFeeTo)">Add</button>--%>
                                            </td>
                                            <td>
                                                <%--<input class="w3-input" type="text" ng-model="ctrl.lockerFeeTotal" disabled></td>--%>
                                        </tr>
                                        <tr style="border-bottom: none !important;">
                                            <td>D)</td>
                                            <td>
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.isAddAnnualFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">--%>
                                                <label>Annual fee</label>
                                            </td>
                                            <td style="border-right: none !important;"></td>
                                            <td></td>
                                            <td>
                                                <%--<input class="w3-input" type="text" ng-model="ctrl.annualFee" disabled></td>--%>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
                <section class="w3-container w3-padding-large">
                    <div class="account-wrap">
                        <div class="w3-container w3-card textTransform" style="padding: 10px">
                            <div class="w3-row w3-border">
                                <div class="w3-col m12">
                                    <table class="w3-table w3-bordered">
                                        <tr style="border-top: none !important;">
                                            <td style="width: 10%; vertical-align: middle">
                                                <label>Cash</label>
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.paymentType.cash">--%>
                                            </td>
                                            <td style="width: 10%; vertical-align: middle">
                                                <label>Check</label>
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.paymentType.check">--%>
                                            </td>
                                            <td style="width: 10%; vertical-align: middle">
                                                <label>Card</label>
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.paymentType.card">--%>
                                            </td>
                                            <td style="width: 40%" <%--ng-show="ctrl.paymentType.check"--%>>
                                                <label class="labelLeft">Number</label>
                                                <%--<input class="w3-input w3-border" style="width: 35%" type="text" ng-model="ctrl.paymentType.checkNumber">--%>
                                                <hr>
                                                <label class="labelLeft">Bank</label>
                                                <%--<select class="w3-border" ng-model="ctrl.bankName" style="width: 35%">
                                            <option value="" selected disabled>- Choose One -</option>
                                            <option value="Bank Asia">Bank Asia</option>
                                            <option value="Islami Bank">Islami Bank</option>
                                        </select>--%>
                                                <hr>
                                                <label class="labelLeft">Date</label>
                                                <%--<input type="date" class="w3-border" ng-model="ctrl.paymentType.date">--%>
                                            </td>
                                            <td style="vertical-align: middle">
                                                <%--<input class="w3-check" type="checkbox" ng-model="ctrl.paymentType.paidFromAddAmount">--%>
                                                <label>Paid From additional amount</label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="w3-table">
                                        <tr>
                                            <td style="width: 75%;">
                                                <%--<button class="w3-btn w3-ripple w3-blue" ng-click="ctrl.payNow();">Pay Now</button>--%>
                                            </td>
                                            <td style="padding-top: 17px;">
                                                <%--<label>Total = {{ctrl.grandTotal}}</label>--%>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>

            </div>


        </ContentTemplate>
    </asp:UpdatePanel>
</div>

<script type="text/javascript">
    (function () {
        if ($app.touch)
            $(document).one('start.app', function () {
                // The page of Touch UI application is ready to be configured
            });
    })();
</script>
