﻿<!doctype html>
<html lang="en" ng-app="payNow" ng-cloak>

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="../css/paynowstyle.css">
    <!-- <link rel="stylesheet" href="lib/css/w3.css"> -->
    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"
        crossorigin="anonymous">
    <title>Pay Now</title>
</head>

<body class="container-fluid" ng-controller="PayNowCtrl as ctrl">
    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid custom-container">
                <h4>Collection Information</h4>
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
                                <span class="st-padding table-heading-cs">{{ctrl.member.fullName}}</span>
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
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid custom-container" style="padding-bottom: 20px;">
                <h4>Other service</h4>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top table-responsive" style="overflow-y: scroll; height: 250px;">
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
                                    <td width="400">{{s.ServiceName}}</td>
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
    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid custom-container" style="padding-bottom: 20px;">
                <h4>On Account of</h4>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top table-responsive">
                        <table class="table cs-table bordered-div col-sm-12 st-table">
                            <tr style="border-top: none !important;">
                                <td>A)</td>
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddSubscription" ng-true-value="true" ng-false-value="false" ng-click="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Subscription</label>
                                </td>
                                <td>
                                    <label class="label-text col-sm-3">From</label>
                                    <input class="st-padding form-control fc-auto col-sm-6" type="date" ng-model="ctrl.subscriptionFrom" disabled>
                                </td>
                                <td>
                                    <label class="label-text col-sm-3">TO</label>
                                    <input class="st-padding form-control fc-auto col-sm-6" type="date" ng-model="ctrl.subscriptionTo">
                                    <button class="col-sm-2 btn btn-sm btn-success mt-4" ng-click="ctrl.subscriptionFeeTotal = ctrl.subscriptionFee * ctrl.getTotalMonth(ctrl.subscriptionFrom, ctrl.subscriptionTo)">Add</button>
                                </td>
                                <td>
                                    <input class="st-padding form-control fc-auto" type="text" ng-model="ctrl.subscriptionFeeTotal" disabled>
                                </td>
                            </tr>
                            <tr>
                                <td>B)</td>
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddWelFareFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Welfare Contribution (new)</label>
                                </td>
                                <td>
                                    <label class="label-text col-sm-3">From</label>
                                    <input class="st-padding form-control fc-auto col-sm-6" type="date" ng-model="ctrl.welFareFrom" disabled>
                                </td>
                                <td>
                                    <label class="label-text col-sm-3">TO</label>
                                    <input class="st-padding form-control fc-auto col-sm-6" type="date" ng-model="ctrl.welFareTo">
                                    <button class="col-sm-2 btn btn-sm btn-success mt-4" ng-click="ctrl.welFareContributionTotal = ctrl.welFareContribution * ctrl.getTotalMonth(ctrl.welFareFrom, ctrl.welFareTo)">Add</button>
                                </td>
                                <td>
                                    <input class="st-padding form-control fc-auto" type="text" ng-model="ctrl.welFareContributionTotal" disabled>
                                </td>
                            </tr>
                            <tr>
                                <td>C)</td>
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddLockerFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Locker</label>



                                </td>
                                <td>
                                    <label class="label-text col-sm-3">From</label>
                                    <input class="st-padding form-control fc-auto col-sm-6" type="date" ng-model="ctrl.lockerFeeFrom" disabled>
                                </td>
                                <td>
                                    <label class="label-text col-sm-3">To</label>
                                    <input class="st-padding form-control fc-auto col-sm-6" type="date" ng-model="ctrl.lockerFeeTo">
                                    <button class="col-sm-2 btn btn-sm btn-success mt-4" ng-hide="ctrl.hidelocaker" ng-click="ctrl.lockerFeeTotal = ctrl.lockerFee * ctrl.getTotalMonth(ctrl.lockerFeeFrom, ctrl.lockerFeeTo)">Add</button>
                                </td>
                                <td>
                                    <input class="st-padding form-control fc-auto" type="text" ng-model="ctrl.lockerFeeTotal" disabled>
                                </td>
                            </tr>
                            <tr style="border-bottom: none !important;">
                                <td>D)</td>
                                <td>
                                    <input type="checkbox" ng-model="ctrl.isAddAnnualFee" ng-true-value="true" ng-false-value="false" ng-change="ctrl.calculateTotalAmount()">
                                    <label class="label-ch">Annual fee</label>
                                </td>
                                <td style="border-right: none !important;"></td>
                                <td></td>
                                <td>
                                    <input class="st-padding form-control fc-auto" type="text" ng-model="ctrl.annualFee" disabled>
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
                                    <label class="label-ch">Check</label>
                                </td>
                                <td>

                                    <input type="radio" name="ctrl.paymentType" ng-model="ctrl.payment" ng-value="3" ng-change="ctrl.clickvalue()">
                                    <label class="label-ch">Card</label>
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
                                <td ng-show="ctrl.paymentType.card">
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Number</label>
                                        <input class="st-padding form-control fc-auto col-sm-8" type="text" ng-model="ctrl.paymentType.checkNumber">
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Pincode</label>
                                        <input class="st-padding form-control fc-auto col-sm-8" type="password" ng-model="ctrl.paymentType.pincode">
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4"></label>
                                        <button class="col-sm-2 btn btn-sm btn-success mt-4" ng-click="ctrl.isValidate()">Validate</button>
                                    </div>
                                </td>
                                <td ng-show="paymentType.bcard">
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Number</label>
                                        <input class="st-padding form-control fc-auto col-sm-8" type="text" ng-model="paymentType.checkNumber">
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Bank</label>
                                        <select n class="st-padding form-control fc-auto col-sm-8" ng-model="bankId">
                                            <option value="" selected disabled>- Choose One -</option>
                                            <option ng-repeat="b in banks" value="{{b.BankID}}">{{b.BankName}}</option>

                                        </select>
                                    </div>
                                    <div class="col-sm-12">
                                        <label class="label-text col-sm-4">Date</label>
                                        <input type="date" class="st-padding form-control fc-auto col-sm-8" ng-model="paymentType.date">
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
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <a class="btn btn-sm btn-primary" href="/pages/Home.aspx">Back Home</a>
                                </td>
                                <td colspan="4"></td>
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

<script src="../js/jquery.min.js"></script>
<script src="../js/bootstrap.min.js"></script>
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
                "serviceDue": 0
            };

            self.hidelocaker = false;
            self.subscriptionFrom = new Date();
            self.subscriptionTo = new Date();

            self.welFareFrom = new Date();
            self.welFareTo = new Date();

            self.lockerFeeFrom = new Date();
            self.lockerFeeTo = new Date();

            self.services = [];
            self.bank = {};
            self.serviceItem = {};
            self.paymentType = {
                "cash": true,
                "check": "",
                "card": "",
                "bcard": "",
                "checkNumber": "",
                "date": "",
                "paidFromAddAmount": ""
            };


            self.getMemberInfo = function (mID) {

                console.log("yyy", self.member.memberId, mID);
                $http({
                    method: "GET",
                    url: "http://localhost:2997/api/MemberCurrentDue/bymembercode/" + mID,
                    //cache: $templateCache
                }).then(function (response) {


                    //self.status = response.status;
                    if (response.data) {

                        self.member = {
                            id: response.data.MemberID,
                            "memberId": response.data.MemberCode,
                            "fullName": response.data.MemberName,
                            "category": response.data.MemberCategory,
                            "date": new Date(),
                            "additionalAmount": response.data.AdditionalValue,
                            "annualDue": response.data.AnnualValue,
                            "monthlyDue": response.data.MonthlyValue,
                            "contributionDue": response.data.ContributionValue,
                            "lockerDue": response.data.LockerValue,
                            "tournamentDue": response.data.TournamentValue,
                            "serviceDue": response.data.ServiceValue
                        }


                        self.subscriptionFrom = new Date(response.data.LastPaySubsDate);
                        self.welFareFrom = new Date(response.data.LastPayContDate);
                        self.subscriptionTo = new Date(response.data.LastPaySubsDate);
                        self.welFareTo = new Date(response.data.LastPayContDate);
                        self.lockerFeeFrom = response.data.LastPayLockerDate ? new Date(response.data.LastPayLockerDate) :
                            new Date();
                        if (response.data.LastPayLockerDate === null) {
                            self.hidelocaker = true;
                        }



                        self.subscriptionFee = response.data.MonthlySubcriptionFee;
                        self.welFareContribution = response.data.MonthlyContributionFee;
                        self.lockerFee = response.data.MonthlyLockerFee;
                        self.annualFee = response.data.AnnualValue;



                        //console.log(response.data);
                        //self.subscriptionFrom = new Date(response.data.LastPaySubsDate,"dd-M-yy");

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
                }
                if (self.payment === 2) {
                    self.paymentType.check = true
                    self.paymentType.card = false
                    self.paymentType.cash = false
                }
                if (self.payment === 3) {

                    self.paymentType.card = true
                    self.paymentType.check = false
                    self.paymentType.cash = false
                }
            }

            self.isValidate = function () {
                $http.get('http://localhost:2997/api/Member/GetMemberCardIsActive?sCardNumber=' + self.paymentType
                    .checkNumber + '&spin=' + self.paymentType.pincode + '&pamount=' + self.grandTotal).then(
                    function (response) {

                        if (response.data.m_IsCardActive === true) {
                            self.message = "Your Card is Validated.";
                            self.mclass = "alert-success";
                        } else {
                            self.message =
                                "Your Card is not Validated or Amount not Avilable. Total Balance:  " +
                                response.data.m_CardBalance;


                        }
                        //self.status = response.status;
                        alert(self.message);
                    },
                    function (response) {
                        self.data = response.data || 'Request failed';
                        self.status = response.status;
                    });

            }


            /*For First Time Load*/
            self.getInitialData = function () {

                $http.get('http://localhost:2997/api/Service/All').then(function (response) {

                    if (response.data) {
                        self.services = response.data;
                    }
                    //self.status = response.status;

                }, function (response) {
                    self.data = response.data || 'Request failed';
                    self.status = response.status;
                });

                $http.get('http://localhost:2997/api/MemberBill/GetBanks').then(function (response) {

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
                        self.serviceAmount += self.serviceItem['price'][key] * self.serviceItem['qty'][
                            key
                        ];
                    }
                });
                self.calculateTotalAmount();
            };

            self.grandTotal = 0;
            self.subscriptionFeeTotal = 0;
            self.lockerFeeTotal = 0;
            self.welFareContributionTotal = 0;

            self.isAddSubscription = false;
            self.isAddWelFareFee = false;
            self.isAddLockerFee = false;
            self.isAddAnnualFee = false;

            self.calculateTotalAmount = function () {

                self.grandTotal = self.member.additionalAmount + self.member.annualDue + self.member.monthlyDue +
                    self.member.contributionDue + self.member.lockerDue + self.member.tournamentDue + self.member
                    .serviceDue + self.serviceAmount;
                if (self.isAddSubscription) {
                    if (self.subscriptionFeeTotal > 0) {
                        self.grandTotal += self.subscriptionFeeTotal;
                    } else {
                        self.isAddSubscription = false;
                        alert("Please give date first")
                    }
                }
                else {
                    self.subscriptionFeeTotal = 0;
                }
                if (self.isAddWelFareFee) {
                    if (self.welFareContributionTotal > 0) {
                        self.grandTotal += self.welFareContributionTotal;
                    } else {
                        self.isAddWelFareFee = false;
                        alert("Please give date first")
                    }
                } else {
                    self.welFareContributionTotal = 0;
                }

                if (self.isAddLockerFee) {
                    if (self.lockerFeeTotal > 0) {
                        self.grandTotal += self.lockerFeeTotal;
                    } else {
                        self.isAddLockerFee = false;
                        alert("Please give date first")
                    }
                } else {
                    self.lockerFeeTotal = 0;
                }
                if (self.isAddAnnualFee) {
                    self.grandTotal += self.annualFee;
                } else {
                    self.annualFee = 0
                }
            };

            self.getTotalMonth = function (from, to) {
                if (from && to) {
                    var months;
                    months = (to.getFullYear() - from.getFullYear()) * 12;
                    months -= from.getMonth() + 1;
                    months += to.getMonth() + 1;
                    return months <= 0 ? 1 : months + 1;
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


                var obj = {
                    "Master": {
                        "MemberID": self.member.id,
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
                        "annual": self.annualFee ? self.annualFee : 0,
                        "cash": self.paymentType.cash ? self.paymentType.cash : "",
                        "check": self.paymentType.check ? self.paymentType.check : "",
                        "card": self.paymentType.card ? self.paymentType.card : "",
                        "bcard": self.paymentType.bcard ? self.paymentType.bcard : "",
                        "chequeNo": self.paymentType.checkNumber ? self.paymentType.checkNumber : "",
                        "date": self.paymentType.date ? format(new Date(self.paymentType.date),
                            "MM/dd/yyyy").toString() : '',
                        "bankid": self.bankId ? self.bankId : 0,
                        "tournamentDue": 0

                    },
                    "ServiceItems": serviceItem,

                };


                $http({
                    method: "POST",
                    url: "http://localhost:2997/api/MemberBill/CreateMemberPay",
                    data: obj
                }).then(function (response) {
                    window.location =
                        '/Pages/ClubReport.aspx?_ReportID=1&_MemberBillID=' +
                        response.data + '&_MemberID=' + self.member.id;

                }, function (response) {
                    self.data = response.data || 'Request failed';
                    self.status = response.status;
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
