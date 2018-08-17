<!doctype html>
<html lang="en" ng-app="payNow" ng-cloak>

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="../scripts/bootstrap332.min.css">
    <link rel="stylesheet" href="../scripts/paynowstyle.css">
    <title>Member Bill Due Create</title>
</head>
<body class="container-fluid" ng-controller="PayNowCtrl as ctrl">
    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid custom-container">
                <h4>Member Bill Service Due Create</h4>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 col-md-12 bordered-div no-padding nb-right">
                        <div class="bordered-div col-sm-12 no-padding nb-left nb-top nb-right">
                            <div class="col-sm-2 bordered-div st-height nb-top nb-left nb-bottom">
                                <span class="st-padding table-heading-cs">Membership No</span>
                            </div>
                            <div class="col-sm-1 bordered-div st-height nb-top nb-left nb-bottom">
                                <input class="st-padding cs-input form-control" type="text" placeholder="Membership Number" ng-model="ctrl.kaisar"
                                    ng-change="ctrl.getMemberInfo(ctrl.kaisar)">
                            </div>
                            <div class="col-sm-4 bordered-div st-height nb-top nb-left nb-bottom">
                                <span class="st-padding table-heading-cs">{{ctrl.member.fullName}}</span>
                            </div>
                            <div class="col-sm-2 bordered-div st-height nb-top nb-left nb-bottom">
                                <span class="st-padding table-heading-cs" style="margin-left: -5px;">Date</span>
                            </div>
                            <div class="col-sm-3 bordered-div st-height nb-top nb-left nb-bottom nb-right">
                                <input class="st-padding-cs cs-input form-control" type="date" ng-model="ctrl.member.date">
                            </div>
                        </div>
                        <div class="bordered-div col-sm-12 no-padding nb-left nb-top nb-right">
                            <div class="col-sm-2 bordered-div st-height nb-left nb-top nb-bottom">
                                <span class="st-padding table-heading-cs">Category</span>
                            </div>
                            <div class="col-sm-1 bordered-div st-height nb-left nb-top nb-bottom nb-right">
                            </div>
                            <div class="col-sm-4 bordered-div st-height nb-left nb-top nb-bottom nb-right">
                                <span class="st-padding table-heading-cs">{{ctrl.member.category}}</span>
                            </div>
                            <div class="col-sm-2 bordered-div st-height nb-top nb-left nb-bottom">
                                <span class="st-padding table-heading-cs" style="margin-left: -5px;">Service Due</span>
                            </div>
                            <div class="col-sm-3 bordered-div st-height nb-left nb-top nb-bottom nb-right">
                                <span class="st-padding table-heading-cs" style="margin-left: -5px;">: {{ctrl.member.serviceDue}}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="col-sm-12 col-xs-12 col-md-12 col-lg-12 p0">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12 p0">
            <div class="container-fluid custom-container" style="padding-bottom: 10px;">
                <h4>Service list</h4>
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
                                    <td><!-- {{s.ServiceFee}} -->
                                        <input class="form-control" type="number" string-to-number ng-model="ctrl.serviceItem.price[$index]"ng-init="ctrl.serviceItem.price[$index] = s.ServiceFee" ng-value="s.ServiceFee" ng-change="ctrl.setTotalServiceAmount()">
                                    </td>
                                    <td>
                                        <input class="form-control" type="number" ng-init="ctrl.serviceItem.qty[$index] = 1.00" string-to-number ng-model="ctrl.serviceItem.qty[$index]"
                                            min=".00" ng-change="ctrl.setTotalServiceAmount()">
                                    </td>
                                    <td>
                                        <input class="form-control" type="number" string-to-number ng-model="ctrl.serviceItem.amountTotal[$index]" min="0" value="{{ctrl.serviceItem.qty[$index] * ctrl.serviceItem.price[$index]}}"
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
            <div class="container-fluid custom-container" style="padding: 10px">
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-12 bordered-div no-padding nb-left nb-bottom nb-top table-responsive">
                        <table class="table cs-table bordered-div col-sm-12 st-table">
                            <tr>
                                <td colspan="4">
                                    <button class="btn btn-sm btn-primary" ng-click="ctrl.payNow();">Create Due</button>
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


</body>


<script src="../scripts/jquery.min.js"></script>
<script src="../scripts/bootstrap.min.js"></script>
<script src="../scripts/angular.min.js"></script>

<script>
    angular.module('payNow', [])
    .directive('stringToNumber', function() {
          return {
            require: 'ngModel',
            link: function(scope, element, attrs, ngModel) {
              ngModel.$parsers.push(function(value) {
                return '' + value;
              });
              ngModel.$formatters.push(function(value) {
                return parseFloat(value);
              });
            }
          };
        })
        .controller('PayNowCtrl', function ($http, $templateCache) {
            var self = this;
            self.member = {
                "memberId": "",
                "fullName": "",
                "category": "",
                "date": "",
                "serviceDue": 0
            };

            self.services = [];
            self.serviceItem = {};
            self.paymentType = {
                "credit": true
            };


            self.getMemberInfo = function (mID) {

                //console.log("yyy", self.member.memberId, mID);
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
                            "memberCode": response.data.MemberCode,
                            "fullName": response.data.MemberName,
                            "category": response.data.MemberCategory,
                            "date": new Date(),
                            "serviceDue": response.data.ServiceValue
                        }

                        self.member.memberId = response.data.MemberCode;

                        console.log("yyyUpdtCode", response.data.MemberCode, mID);


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
                if (self.payment === 5) {

                    self.paymentType.card = true
                    self.paymentType.check = false
                    self.paymentType.cash = false
                }
            }
            /*For First Time Load*/
            self.getInitialData = function () {

                $http.get('http://localhost:2997/api/Service/All').then(function (response) {
                    if (response.data) {
                        self.services = response.data;
                    }

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
            self.calculateTotalAmount = function () {

                self.grandTotal = self.member.serviceDue + self.serviceAmount;

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
                        "MemberCode": self.member.memberId,
                        "BillDate": format(new Date(), 'MM/dd/yyyy').toString(),
                        "credit": self.paymentType.credit ? self.paymentType.credit : "",
                        "PaidAmount": self.grandTotal
                    },
                    "ServiceItems": serviceItem,
                };


                $http({
                    method: "POST",
                    url: "http://localhost:2997/api/Service/CreateMemberServiceDue",
                    data: obj
                }).then(function (response) {
                    ////self.member.memberId means membercode
                    //window.location =
                    //    '/Pages/ClubReport.aspx?_ReportID=1&_MemberBillID=' +
                    //    response.data + '&_MemberCode=' + self.member.memberId;
                    alert("Save Success..");

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
