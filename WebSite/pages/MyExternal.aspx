<!doctype html>
<html lang="en" ng-app="payNow" ng-cloak>

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="../scripts/bootstrap332.min.css">
    <link rel="stylesheet" href="../scripts/paynowstyle.css">
    <!-- <link rel="stylesheet" href="lib/css/w3.css"> -->
    <!-- Latest compiled and minified CSS -->
    <title>Member Bill Collection</title>
</head>
<style>
    .p0 {
        padding: 0;
    }

    .text-center {
        text-align: center;
    }

    .fc-110 {
        width: 110px !important;
    }

    .bg-org {
        background: #ffbe00;
    }
</style>

<body class="container-fluid" ng-controller="PayNowCtrl as ctrl">


    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid custom-container">
                <div class="col-sm-11 text-center p0">
                    <h4>Collection Information</h4>
                </div>
                <div class="col-sm-1 text-right">
                    <a class="btn btn-sm btn-primary" href="/pages/Home.aspx">Back Home</a>
                </div>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-8 col-md-8 bordered-div no-padding nb-right">
                        <div class="bordered-div col-sm-12 no-padding nb-left nb-top nb-right">
                            <div class="col-sm-5 bordered-div st-height nb-top nb-left nb-bottom">
                                <span class="st-padding table-heading-cs">Membership No</span>
                            </div>
                            <div class="col-sm-3 bordered-div st-height nb-top nb-left nb-bottom">
                                <input class="st-padding cs-input form-control" type="text" placeholder="Membership Number" ng-model="ctrl.kaisar"
                                    ng-change="ctrl.getMemberInfo(ctrl.kaisar)">
                            </div>
                            <div class="col-sm-1 bordered-div st-height nb-top nb-left nb-bottom">
                                <span class="st-padding table-heading-cs" style="margin-left: -5px;">Date</span>
                            </div>
                            <div class="col-sm-3 bordered-div st-height nb-top nb-left nb-bottom nb-right">
                                <input class="st-padding-cs cs-input form-control" type="date" ng-model="ctrl.member.date">
                            </div>
                        </div>
                        <div class="bordered-div col-sm-12 no-padding nb-left nb-top nb-right">
                            <div class="col-sm-5 bordered-div st-height nb-left nb-top nb-bottom">
                                <span class="st-padding table-heading-cs">Received With Thanks From</span>
                            </div>
                            <div class="col-sm-7 bordered-div st-height nb-left nb-top nb-bottom nb-right">                                
                                <span class="st-padding table-heading-cs">{{ctrl.member.fullName}}: {{ctrl.member.MemberStatus}}</span>
                            </div>
                        </div>
                        <div class="bordered-div col-sm-12 no-padding nb-left nb-top nb-bottom nb-right">
                            <div class="col-sm-5 bordered-div st-height nb-left nb-top nb-bottom">
                                <span class="st-padding table-heading-cs">Category</span>
                            </div>
                            <div class="col-sm-7 bordered-div st-height nb-left nb-top nb-bottom nb-right">
                                <span class="st-padding table-heading-cs">{{ctrl.member.category}}</span>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4 col-md-4 bordered-div no-padding nb-left nb-top nb-right nb-bottom">
                        <table class="table cs-table bordered-div col-sm-12">
                            <tr>
                                <td>Additional Amount</td>
                                <td>: {{ctrl.member.additionalAmount}}</td>
                            </tr>
                            <tr>
                                <td>Annual Amount</td>
                                <td>: {{ctrl.member.annualDue}}</td>
                            </tr>
                            <tr>
                                <td>Monthly Due</td>
                                <td>: {{ctrl.member.monthlyDue}}</td>
                            </tr>
                            <tr>
                                <td>Contribution(wf) Due</td>
                                <td>: {{ctrl.member.contributionDue}}</td>
                            </tr>
                            <tr>
                                <td>Locker Due</td>
                                <td>: {{ctrl.member.lockerDue}}</td>
                            </tr>
                            <tr>
                                <td>Tournament Due</td>
                                <td>: {{ctrl.member.tournamentDue}}</td>
                            </tr>
                            <tr style="border-bottom: none !important;">
                                <td>Service Due</td>
                                <td>: {{ctrl.member.serviceDue}}</td>
                            </tr>
                            <tr>
                                <td>Restaurant Due</td>
                                <td>: {{ctrl.member.RestDueAmount}}</td>
                            </tr>
                            <tr style="border-bottom: none !important;">
                                <td>Bar Due</td>
                                <td>: {{ctrl.member.BarDueAmount}}</td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="col-sm-12 col-xs-12 col-md-6 col-lg-6 p0">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12 p0">
            <div class="container-fluid custom-container" style="padding-bottom: 20px;">
                <h4>Other service</h4>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top table-responsive" style="overflow-y: scroll; height: 434px;">
                        <table class="table cs-table bordered-div col-sm-12 st-table">
                            <thead>
                                <tr style="border-top: none !important;">
                                    <th>No.</th>
                                    <th>Name</th>
                                    <th>Price</th>
                                    <th>Quantity</th>
                                    <th>Total Amount</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr ng-repeat="s in ctrl.services track by $index">
                                    <td>
                                        <input type="checkbox" ng-model="ctrl.serviceItem.id[$index]" ng-true-value="{{s.ServiceID}}" ng-false-value="0" ng-change="ctrl.setTotalServiceAmount()">
                                    </td>
                                    <td width="200">{{s.ServiceName}}</td>
                                    <td>{{s.ServiceFee}}
                                        <input type="hidden" ng-model="ctrl.serviceItem.price[$index]" ng-init="ctrl.serviceItem.price[$index] = s.ServiceFee" ng-value="s.ServiceFee">
                                    </td>
                                    <td>
                                        <input class="form-control" type="number" ng-init="ctrl.serviceItem.qty[$index] = 1" ng-model="ctrl.serviceItem.qty[$index]"
                                            min="1" ng-change="ctrl.setTotalServiceAmount()">
                                    </td>
                                    <td>
                                        <input class="form-control" type="number" ng-model="ctrl.serviceItem.amountTotal[$index]" min="0" value="{{ctrl.serviceItem.qty[$index] * s.ServiceFee}}"
                                            disabled>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="col-sm-12 col-xs-12 col-md-6 col-lg-6 p0">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12 p0">
            <div class="container-fluid custom-container" style="padding-bottom: 20px;">
                <h4>On Account of</h4>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top nb-right table-responsive">
                        <table class="table cs-table bordered-div col-sm-12 st-table">
                            <tr style="border-top: none !important;" class="text-center">
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddSubscription" ng-true-value="true" ng-false-value="false" ng-click="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Subscription</label>
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">From</label>
                                    <input class="st-padding form-control fc-auto col-sm-12 text-center p0" type="date" ng-model="ctrl.subscriptionFrom" >
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">TO</label>
                                    <input class="st-padding form-control fc-auto col-sm-12 text-center p0" type="date" ng-model="ctrl.subscriptionTo" ng-change="ctrl.subscriptionFeeTotal = ctrl.getsubsidueAmount(ctrl.subscriptionFrom, ctrl.subscriptionTo)">
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">Total</label>
                                    <input class="st-padding form-control fc-auto fc-110" type="text" ng-model="ctrl.subscriptionFeeTotal" disabled>
                                </td>
                            </tr>
                            <tr class="text-center">
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddWelFareFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Welfare Contribution (new)</label>
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">From</label>
                                    <input class="st-padding form-control fc-auto col-sm-12 text-center p0" type="date" ng-model="ctrl.welFareFrom">
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">TO</label>
                                    <input class="st-padding form-control fc-auto col-sm-12 text-center p0" type="date" ng-model="ctrl.welFareTo" ng-change="ctrl.welFareContributionTotal = ctrl.getcontrwfdueAmount(ctrl.welFareFrom, ctrl.welFareTo)">
                                    <%--<button class="col-sm-6 btn btn-sm btn-success mt-4 text-center" >Add</button>--%>
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">Total</label>
                                    <input class="st-padding form-control fc-auto fc-110" type="text" ng-model="ctrl.welFareContributionTotal">
                                </td>
                            </tr>
                            <tr class="text-center">
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddLockerFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Locker</label>
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">From</label>
                                    <input class="st-padding form-control fc-auto col-sm-12 text-center p0" type="date" ng-model="ctrl.lockerFeeFrom" 
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">To</label>
                                    <input class="st-padding form-control fc-auto col-sm-12 text-center p0" type="date" ng-model="ctrl.lockerFeeTo" ng-change="ctrl.lockerFeeTotal = ctrl.getlockerdueAmount(ctrl.lockerFeeFrom, ctrl.lockerFeeTo)">
                                    <%--<button class="col-sm-6 btn btn-sm btn-success mt-4" ng-click="ctrl.lockerFeeTotal = ctrl.getlockerdueAmount(ctrl.lockerFeeFrom, ctrl.lockerFeeTo)">Add</button>--%>
                                </td>
                                <td>
                                    <label class="label-text col-sm-12 text-center">Total</label>
                                    <input class="st-padding form-control fc-auto fc-110" type="text" ng-model="ctrl.lockerFeeTotal" disabled>
                                </td>
                            </tr>
                            <tr style="border-bottom: none !important;" class="text-center">
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddAnnualFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Annual fee</label>
                                </td>
                                <td style="border-right: none !important;"></td>
                                <td></td>
                                <td>
                                    <input class="st-padding form-control fc-auto fc-110" type="text" ng-model="ctrl.annualFee">
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="col-sm-12 col-xs-12 col-md-12 col-lg-12 p0" ng-show="ctrl.hidetournament">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12 p0">
            <div class="container-fluid custom-container" style="padding-bottom: 20px;">
                <h4>Member Tournament</h4>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top nb-right table-responsive">
                        <table class="table cs-table bordered-div col-sm-12 st-table">
                            <thead>
                                <th>Flight No.</th>
                                <th>Game</th>
                                <th>Tournament</th>
                                <th>Flight Date</th>
                                <th>Start From</th>
                                <th>Total Due</th>
                            </thead>
                            <tr style="border-top: none !important;">
                                <td>
                                    <label class="label-ch">{{ctrl.member.tDue[0].FlightNo}}</label>
                                </td>
                                <td>
                                    <span>{{ctrl.member.tDue[0].GameType}}</span>
                                </td>
                                <td>
                                    <span>{{ctrl.member.tDue[0].TournamentName}}</span>
                                </td>
                                <td>
                                    <span>{{ctrl.member.tDue[0].FlightDate | date:'dd-MM-yyyy'}}</span>
                                </td>
                                <td class="bg-org">
                                    <span>{{ctrl.member.tDue[0].StartFrom}}</span>
                                </td>
                                <td>
                                    <span>{{ctrl.member.tDue[0].TotalDue}}</span>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="col-sm-12 col-xs-12 col-md-12 col-lg-12 p0" ng-show="ctrl.hideservicedue">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12 p0">
            <div class="container-fluid custom-container" style="padding-bottom: 20px;">
                <h4>Due services</h4>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top nb-right table-responsive">
                        <table class="table cs-table bordered-div col-sm-12 st-table">
                            <thead>
                                <th>No.</th>
                                <th>Name</th>
                                <th>Price</th>
                                <th>Quantity</th>
                                <th>Total Amount</th>
                            </thead>

                            <tr style="border-top: none !important;" ng-repeat="sd in ctrl.member.sDue track by $index">

                                <td>

                                    <input type="checkbox" ng-model="ctrl.serviceItemdue.id[$index]" ng-true-value="{{sd.ServiceID}}" ng-false-value="0">
                                    <input type="hidden" ng-model="ctrl.serviceItemdue.MemberBillID[$index]" ng-init="ctrl.serviceItemdue.MemberBillID[$index] = sd.MemberBillID" ng-value="sd.MemberBillID">
                                </td>
                                <td>
                                    <span>{{sd.ServiceName}}</span>
                                </td>
                                <td>
                                    <span>{{sd.Price}}</span>

                                </td>
                                <td>
                                    <span>{{sd.Quantity}}</span>

                                </td>
                                <td>
                                    <span>{{sd.TotalAmount}}</span>

                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid custom-container" style="padding: 20px">
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top table-responsive">
                        <table class="table cs-table bordered-div col-sm-12 st-table">
                            <tr style="border-top: none !important;">
                                <td>

                                    <input type="radio" name="ctrl.paymentType" ng-model="ctrl.payment" ng-value="1" ng-change="ctrl.clickvalue()">
                                    <label class="label-ch">Cash</label>
                                </td>
                                <td>

                                    <input type="radio" name="ctrl.paymentType" ng-model="ctrl.payment" ng-value="2" ng-change="ctrl.clickvalue()">
                                    <label class="label-ch">Checque</label>
                                </td>
                                <td>

                                    <input type="radio" name="ctrl.paymentType" ng-model="ctrl.payment" ng-value="4" ng-change="ctrl.clickvalue()">
                                    <label class="label-ch">Bank Card</label>
                                </td>
                                <td>

                                    <input type="radio" name="ctrl.paymentType" ng-model="ctrl.payment" ng-value="3" ng-change="ctrl.clickvalue()">
                                    <label class="label-ch">Member Card</label>
                                </td>
                                <td ng-show="ctrl.paymentType.check">
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Number</label>
                                        <input class="st-padding form-control fc-auto col-sm-8" type="text" ng-model="ctrl.paymentType.checkNumber">
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Bank</label>
                                        <select n class="st-padding form-control fc-auto col-sm-8" ng-model="ctrl.bankId">
                                            <option value="" selected disabled>- Choose One -</option>
                                            <option ng-repeat="b in ctrl.banks" value="{{b.BankID}}">{{b.BankName}}</option>

                                        </select>
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Date</label>
                                        <input type="date" class="st-padding form-control fc-auto col-sm-8" ng-model="ctrl.paymentType.date">
                                    </div>
                                </td>
                                <td ng-show="ctrl.paymentType.bcard">
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Number</label>
                                        <input class="st-padding form-control fc-auto col-sm-8" type="text" ng-model="ctrl.paymentType.checkNumber">
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Bank Card</label>
                                        <select n class="st-padding form-control fc-auto col-sm-8" ng-model="ctrl.paymentType.BankCard">
                                            <option value="0" selected disabled>- Choose One -</option>
                                            <option value="1">Debit Card</option>
                                            <option value="2">Visa Card</option>
                                            <option value="3">Master Card</option>
                                            <option value="4">Amex Card</option>
                                        </select>
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Bank</label>
                                        <select class="st-padding form-control fc-auto col-sm-8" ng-model="ctrl.bankId">
                                            <option value="" selected disabled>- Choose One -</option>
                                            <option ng-repeat="b in ctrl.banks" value="{{b.BankID}}">{{b.BankName}}</option>

                                        </select>
                                    </div>
                                    <div class="col-sm-12" style="visibility: hidden">
                                        <label class="label-text col-sm-4">Date</label>
                                        <input type="date" class="st-padding form-control fc-auto col-sm-8" ng-model="ctrl.paymentType.date">
                                    </div>
                                </td>

                                <td style="vertical-align: middle">
                                    <input type="checkbox" ng-model="ctrl.paymentType.paidFromAddAmount">
                                    <label class="label-ch">Paid From additional amount</label>
                                </td>
                            </tr>
                            <!-- </table>
                    <table class="table cs-table bordered-div col-sm-12 st-table"> -->
                            <tr>
                                <td colspan="4">
                                    <button class="btn btn-sm btn-primary" ng-click="ctrl.payNow();">Pay Now</button>
                                </td>
                                <td colspan="4">
                                    <a class="btn btn-sm btn-primary" href="/pages/Home.aspx">Back Home</a>
				</td>
                                <td style="padding-top: 17px;">
                                    <label>Total = {{ctrl.grandTotal}}</label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <br>
</body>

<%--<script src="../js/jquery.min.js"></script>--%>
<script src="../scripts/jquery.min.js"></script>
<script src="../scripts/bootstrap.min.js"></script>
<script src="../scripts/angular.min.js"></script>



<script>
    angular.module('payNow', [])
        .controller('PayNowCtrl', function ($http, $templateCache) {
            var self = this;
            self.member = {
                "memberId": "",
                "fullName": "",
                "category": "",
                "date": "",
                "additionalAmount": 0,
                "annualDue": 0,
                "monthlyDue": 0,
                "contributionDue": 0,
                "lockerDue": 0,
                "tournamentDue": 0,
                "serviceDue": 0,
                "RestDueAmount": 0,
                "MemberStatus":"",
                "BarDueAmount": 0
            };

            self.hidelocaker = false;
            self.hidetournament = false;
            self.hideservicedue = false;
            self.subscriptionFrom = new Date();
            self.subscriptionTo = new Date();

            self.welFareFrom = new Date();
            self.welFareTo = new Date();

            self.lockerFeeFrom = new Date();
            self.lockerFeeTo = new Date();
            self.BankCard = "0";
            self.services = [];
            self.bank = {};
            self.serviceItem = {};
            self.serviceItemdue = {};
            self.paymentType = {
                "cash": true,
                "check": "",
                "card": "",
                "bcard": "",
                "checkNumber": "",
		"BankCard":"",
                "date": "",
                "paidFromAddAmount": ""
            };
            var cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)UserSettings\s*\=\s*([^;]*).*$)|^.*$/, "$1");

            /*console.log("KKKKKKKKKK", cookieValue.substr(5, cookieValue.length));*/
            /*console.log("user Name", cookieValue);*/

            /*var ssUserName = cookieValue.substr(5, cookieValue.length);*/
            var ssUserName = cookieValue;

            self.getMemberInfo = function (mID) {

                //console.log("yyy", self.member.memberIde, mID);
                $http({
                    method: "GET",
                    url: "http://api.kgc-bd.com/api/MemberCurrentDue/bymembercode/" + mID,
                    //cache: $templateCache
                }).then(function (response) {



                    //self.status = response.status;
                    if (response.data) {

                        self.serviceItemdue = response.data.ServicesDue;
                        self.member = {
                            id: response.data.MemberID,
                            "memberId": response.data.MemberCode,
                            "memberCode": response.data.MemberCode,
                            "fullName": response.data.MemberName,
                            "category": response.data.MemberCategory,
                            "date": new Date(),
                            "additionalAmount": response.data.AdditionalValue,
                            "annualDue": response.data.AnnualValue,
                            "monthlyDue": response.data.MonthlyValue,
                            "contributionDue": response.data.ContributionValue,
                            "lockerDue": response.data.LockerValue,
                            "tournamentDue": response.data.TournamentValue,
                            "serviceDue": response.data.ServiceValue,
                            "tDue": response.data.ToursDue,
                            "sDue": response.data.ServicesDue,
                            "RestDueAmount": response.data.RestDueAmount,
                            "BarDueAmount": response.data.BarDueAmount,
                            "MemberStatus": response.data.MemberStatus

                        }



                        self.hidetournament = false;
                        if (response.data.TournamentValue > 0) {
                            self.hidetournament = true;
                        }
                        self.hideservicedue = false;
                        if (response.data.ServiceValue > 0) {
                            self.hideservicedue = true;
                        }

                        self.member.memberId = response.data.MemberCode;


                        self.subscriptionFrom = new Date(response.data.LastPaySubsDate);
                        self.welFareFrom = new Date(response.data.LastPayContDate);
                        //*self.subscriptionTo = new Date(response.data.LastPaySubsDate);*/
                        /*self.welFareTo = new Date(response.data.LastPayContDate);*/
                        self.lockerFeeFrom = response.data.LastPayLockerDate ? new Date(response.data.LastPayLockerDate) :
                            new Date();
                        if (response.data.LastPayLockerDate === null) {
                            self.hidelocaker = true;
                        }

                        /*self.BankCard = response.data;*/

                        self.subscriptionFee = response.data.MonthlySubcriptionFee;
                        self.welFareContribution = response.data.MonthlyContributionFee;
                        self.lockerFee = response.data.MonthlyLockerFee;
                        self.annualFee = response.data.AnnualValue;



                        self.calculateTotalAmount();
                    }
                }, function (response) {
                    self.data = response.data || 'Request failed';
                    self.status = response.status;
                });
            };

            /* Bank Data load*/
            self.clickvalue = function () {

                if (self.payment === 1) {
                    self.paymentType.cash = true
                    self.paymentType.check = false
                    self.paymentType.card = false
                    self.paymentType.bcard = false
                }
                if (self.payment === 2) {
                    self.paymentType.check = true
                    self.paymentType.card = false
                    self.paymentType.cash = false
                    self.paymentType.bcard = false
                }
                if (self.payment === 3) {

                    self.paymentType.card = true
                    self.paymentType.check = false
                    self.paymentType.cash = false
                    self.paymentType.bcard = false
                }
                if (self.payment === 4) {

                    self.paymentType.card = false
                    self.paymentType.check = false
                    self.paymentType.cash = false
                    self.paymentType.bcard = true
                }
            }


            /*For First Time Load*/
            self.getInitialData = function () {

                $http.get('http://api.kgc-bd.com/api/Service/All').then(function (response) {

                    if (response.data) {
                        self.services = response.data;
                    }
                    //self.status = response.status;

                }, function (response) {
                    self.data = response.data || 'Request failed';
                    self.status = response.status;
                });

                $http.get('http://api.kgc-bd.com/api/MemberBill/GetBanks').then(function (response) {

                    if (response.data) {
                        self.banks = response.data;
                    }
                    //self.status = response.status;
                }, function (response) {
                    self.data = response.data || 'Request failed';
                    self.status = response.status;
                });
            };
            self.getInitialData();
            self.serviceAmount = 0;
            self.setTotalServiceAmount = function () {

                self.serviceAmount = 0;
                angular.forEach(self.serviceItem['id'], function (item, key) {
                    if (item !== 0) {
                        self.serviceAmount += self.serviceItem['price'][key] * self.serviceItem['qty'][key];
                    }
                });
                self.calculateTotalAmount();
            };

            self.grandTotal = 0;
            self.subscriptionFeeTotal = 0;
            self.lockerFeeTotal = 0;
            self.welFareContributionTotal = 0;
            self.annualFee = 0;
            self.BankCard = "";
            self.isAddSubscription = false;
            self.isAddWelFareFee = false;
            self.isAddLockerFee = false;
            self.isAddAnnualFee = false;

            self.calculateTotalAmount = function () {

                self.grandTotal = self.member.serviceDue + self.member.tournamentDue + self.serviceAmount;
                if (self.isAddSubscription) {
                    if (self.subscriptionFeeTotal > 0) {
                        self.grandTotal += self.subscriptionFeeTotal;
                    } else {
                        self.grandTotal += 0;
                        self.isAddSubscription = false;
                        /*alert("Please give date first")*/
                    }
                }
                if (self.isAddWelFareFee) {
                    if (self.welFareContributionTotal > 0) {
                        self.grandTotal += self.welFareContributionTotal;
                    } else {
                        self.grandTotal += 0;
                        self.isAddWelFareFee = false;
                        /*alert("Please give date first")*/
                    }
                }

                if (self.isAddLockerFee) {
                    if (self.lockerFeeTotal > 0) {
                        self.grandTotal += self.lockerFeeTotal;
                    } else {
                        self.grandTotal += 0;
                        self.isAddLockerFee = false;
                        /*alert("Please give date first")*/
                    }
                }
                if (self.isAddAnnualFee) {
                    if (self.annualFee > 0) {
                        self.grandTotal += self.annualFee;
                    }
                    else {
                        self.grandTotal += 0;
                    }
                }
            };

            self.getsubsidueAmount = function (from, to) {
                if (from && to) {

                    var subsidueamount = 0;
                    /*api.kgc-bd.com*/
                    $http.get('http://api.kgc-bd.com/api/MemberBill/GetSubscriptionDueBill?pMemberCode=' + self.kaisar + "&&pFromDate=" + format(new Date(from), "MM/dd/yyyy").toString() + "&pToDate=" + format(new Date(to), "MM/dd/yyyy").toString()).then(function (response) {
                        subsidueamount = response.data.MemberSubscriptionFeeDue;
                        self.subscriptionFeeTotal = subsidueamount;
                        self.calculateTotalAmount();
                    })

                    return subsidueamount;
                }
                return 0;
            };

            self.getcontrwfdueAmount = function (from, to) {
                if (from && to) {
                    var subsidueamount = 0;
                    /*api.kgc-bd.com*/
                    $http.get('http://api.kgc-bd.com/api/MemberBill/GetContributionDueBill?pMemberCode=' + self.kaisar + "&&pFromDate=" + format(new Date(from), "MM/dd/yyyy").toString() + "&pToDate=" + format(new Date(to), "MM/dd/yyyy").toString()).then(function (response) {
                        subsidueamount = response.data.MemberContributionFeeDue;
                        self.welFareContributionTotal = subsidueamount;
                        self.calculateTotalAmount();
                    })

                    return subsidueamount;
                }
                return 0;
            };

            self.getlockerdueAmount = function (from, to) {
                if (from && to) {

                    var subsidueamount = 0;
                    /*api.kgc-bd.com*/
                    $http.get('http://api.kgc-bd.com/api/MemberBill/getMemberLockerBillDue?pMemberCode=' + self.kaisar + "&&pFromDate=" + format(new Date(from), "MM/dd/yyyy").toString() + "&pToDate=" + format(new Date(to), "MM/dd/yyyy").toString()).then(function (response) {
                        console.log('MemberLockerFeeDue', response.data);
                        subsidueamount = response.data.MemberLockerFeeDue;
                        self.lockerFeeTotal = subsidueamount;
                        self.calculateTotalAmount();
                    })

                    return subsidueamount;
                }
                return 0;
            };

            self.payNow = function () {
                var serviceItem = [];
                angular.forEach(self.serviceItem['id'], function (item, key) {
                    if (item !== 0) {
                        serviceItem.push({
                            "id": self.serviceItem['id'][key],
                            "qty": self.serviceItem['qty'][key],
                            "unitprice": self.serviceItem['price'][key]
                        });
                    }
                });

                var serviceItemdue = [];
                console.log("bulbu", self.serviceItemdue, self.member.sDue);
                // console.log(self.serviceItemdue);
                angular.forEach(self.member.sDue, function (item, key) {
                    console.log(item, key);
                    if (item !== 0) {
                        serviceItemdue.push({
                            "id": self.member.sDue[key]['ServiceID'],
                            "qty": self.member.sDue[key]['Quantity'],
                            "unitprice": self.member.sDue[key]['Price'],
                            "memberbillid": self.member.sDue[key]['MemberBillID']
                        });
                    }
                });


                var obj = {
                    "Master": {
                        "MemberCode": self.member.memberId,
                        "BillDate": format(new Date(), 'MM/dd/yyyy').toString(),
                        "subcriptionamount": self.subscriptionFeeTotal ? self.subscriptionFeeTotal : 0,
                        "subcriptionfromdate": self.subscriptionFrom ? format(new Date(self.subscriptionFrom),
                            "MM/dd/yyyy").toString() : '',
                        "subcriptiontodate": self.subscriptionTo ? format(new Date(self.subscriptionTo),
                            "MM/dd/yyyy").toString() : '',
                        "contributionamount": self.welFareContributionTotal ? self.welFareContributionTotal :
                            0,
                        "contributionfromdate": self.welFareFrom ? format(new Date(self.welFareFrom),
                            "MM/dd/yyyy").toString() : '',
                        "contributiontodate": self.welFareTo ? format(new Date(self.welFareTo),
                            "MM/dd/yyyy").toString() : '',
                        "lockeramount": self.lockerFeeTotal ? self.lockerFeeTotal : 0,
                        "lockerfromdate": self.lockerFeeFrom ? format(new Date(self.lockerFeeFrom),
                            "MM/dd/yyyy").toString() : '',
                        "lockertodate": self.lockerFeeTo ? format(new Date(self.lockerFeeTo), "MM/dd/yyyy")
                            .toString() : '',
                        "annual": self.isAddAnnualFee ? self.annualFee : 0,
                        "cash": self.paymentType.cash ? self.paymentType.cash : "",
                        "check": self.paymentType.check ? self.paymentType.check : "",
                        "card": self.paymentType.card ? self.paymentType.card : "",
                        "bcard": self.paymentType.bcard ? self.paymentType.bcard : "",
                        "chequeNo": self.paymentType.checkNumber ? self.paymentType.checkNumber : "",
                        "ChequeDT": self.paymentType.date ? format(new Date(self.paymentType.date),
                            "MM/dd/yyyy").toString() : '',
                        "date": self.paymentType.date ? format(new Date(self.paymentType.date),
                            "MM/dd/yyyy").toString() : '',
                        "bankid": self.bankId ? self.bankId : 0,
                        "tournamentDue": self.member.tDue[0].TotalDue ? self.member.tDue[0].TotalDue : 0,
                        "tourRegisterID": self.member.tDue[0].TournamentRegisterID ? self.member.tDue[0].TournamentRegisterID : 0,
                        "CreatedBy": ssUserName,
                        "BankCard": self.paymentType.BankCard ? self.paymentType.BankCard : 0,
                        "PaidAmount": self.grandTotal
                    },
                    "ServiceItems": serviceItem,
                    "ServiceItemsDue": serviceItemdue,

                };

                console.log('Pay now', obj);

                $http({
                    method: "POST",
                    url: "http://api.kgc-bd.com/api/MemberBill/CreateMemberPayNew",
                    data: obj
                }).then(function (response) {
                    ////self.member.memberId means membercode
			
                    console.log('submit response', response.data);
                    
                    var error = response.data.ErrorCode;
                    console.log('ErrorCode', error);
                    if (error > 0) {
                        /*console.log('response..error > 0', response.data);*/
                        alert("Bill Collection failed." + response.data.ErrorMessage);
                    }
                    var pBillNo = response.data.BillNo;

                    console.log('BillNo', pBillNo);

			/*self.paymentType.checkNumber = "";
			self.BankCard = 0;*/
			
                    alert("Successfully Saved Data.")
                    
                    window.location =
                        '/Pages/ClubReport.aspx?_ReportID=1&_MemberBillID=' +
                        pBillNo + '&_MemberCode=' + self.kaisar;
                        


                }, function (response) {
                    console.log(response.data);
                    self.data = response.data || 'Request failed';
                    self.status = response.status;
                    console.log('submit response Failed..', response.data);
                    alert(response.data.ErrorMessage);
                });
            };
        });


    var format = function date2str(x, y) {
        var z = {
            M: x.getMonth() + 1,
            d: x.getDate(),
            h: x.getHours(),
            m: x.getMinutes(),
            s: x.getSeconds()
        };
        y = y.replace(/(M+|d+|h+|m+|s+)/g, function (v) {
            return ((v.length > 1 ? "0" : "") + eval('z.' + v.slice(-1))).slice(-2)
        });

        return y.replace(/(y+)/g, function (v) {
            return x.getFullYear().toString().slice(-v.length)
        });
    }
</script>

</html>
