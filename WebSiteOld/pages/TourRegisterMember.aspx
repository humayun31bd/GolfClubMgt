<!doctype html>
<html lang="en" ng-app="tournamentRegistration" ng-cloak>

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"
        crossorigin="anonymous">
    <link rel="stylesheet" href="../css/paynowstyle.css">
    <!-- <link rel="stylesheet" href="lib/css/w3.css"> -->
    <!-- Latest compiled and minified CSS -->
    <title>Golf</title>
</head>

<body class="container-fluid" ng-controller="TournamentRegistrationCtrl">
    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid" style="padding-top: 30px;">
                <a class="btn btn-sm btn-primary" href="/pages/Home.aspx">Back Home</a>
                <button class="btn btn-primary pull-right" style="margin: 0 10px;">Print</button>
                <%--<label class="col-sm-2 no-padding">Tee</label>--%>
                <select class="form-control pull-right" style="width: 10%;" ng-model="teeId" ng-change="getInitialData()">
                    <option ng-repeat="tee in teeList" value="{{tee.TeeID}}">{{tee.TeeName}}</option>
                </select>
                <h4>Tournament Booking &amp; Registration</h4>
                <hr>
                <div class="col-sm-12 no-padding">
                    <div class="col-sm-6 no-padding">
                        <label class="col-sm-2 no-padding">Tournament</label>
                        <input class="st-padding form-control col-sm-5" style="width: 30%; margin-top: -1%;" type="text" placeholder="Search by M.No">
                    </div>
                    <div class="col-sm-3 no-padding">
                        <label>Date</label>
                    </div>
                    <div class="col-sm-3 no-padding">
                        <label>Category</label>
                    </div>
                </div>
                <div class="col-sm-12 no-padding mt-5">
                    <div class="col-sm-6 no-padding">
                        <select class="form-control" style="width: 90%;" ng-model="tournamentId">
                            <option ng-repeat="t in tournamentList" value="{{t.TournamentID}}">{{t.TournamentName}}</option>
                        </select>
                    </div>
                    <div class="col-sm-3 no-padding">
                        <input class="st-padding-cs form-control" type="date" style="width: 90%;" ng-model="regDate" >
                    </div>
                    <div class="col-sm-3 no-padding">
                        <select class="form-control" style="width: 90%;" ng-model="categoryId" ng-change="tourList(tournamentId,categoryId,teeId)">
                            <option ng-repeat="cate in categoryList" value="{{cate.m_GameCategoryID}}">
                                {{cate.m_GameCategoryName}}
                            </option>
                        </select>
                    </div>
                </div>
                <div class="col-sm-12 no-padding mt-5">
                    <div class="table-responsive" style="padding: 3px; border: 1px solid #d6d6d6">
                        <table class="table table-striped table-hover cs-table">
                            <thead class="thead-dark border-right-dark" style="width: 100%">
                                <!-- <th class="text-center">Sl. No</th> -->
                                <th class="text-center">Time</th>
                                <th class="text-center" width="500">Name</th>

                                <th class="text-center">Slots</th>
                            </thead>
                            <tbody class="border-right">
                                <tr ng-repeat="plist in playerList">
                                    <td>
                                        <a ng-click="open(regDate,plist.TournamentFlightSchID,plist.TournamentID,plist.GameCateoryID,plist.TeeID,plist.StartTime)">{{plist.StartTime}}-{{plist.EndTime}}</a>
                                    </td>
                                    <td class="text-center">
                                        <table class="table table-striped no-border">
                                            <tr ng-repeat=" p in plist.Players">
                                                <td>{{p.MemberCode}}</td>
                                                <td>{{p.MemberName}}</td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>{{plist.NumberOfPlayer}}/{{plist.MaxPlayer}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <br>


    <!-- Modal -->
    <script type="text/ng-template" id="registration.html">
        <!-- <div id="registration" class="modal fade" role="dialog" > -->


        <!-- <div class="modal-dialog" style="width: 95%; margin-left: 20px;"> -->
        <!-- Modal content-->
        <div class="modal-content font-sm">
            <div class="modal-header header-dark">
                <button type="button" class="close" ng-click="close()">&times;</button>
                <h4 class="modal-title">
                    <b>Registration</b>
                </h4>
            </div>
            <div class="modal-body no-padding">
                <div class="container-fluid">
                    <div class="col-sm-12">
                        <div class="table-responsive col-sm-7 no-padding" style="border: 1px solid #d6d6d6">
                            <table class="table table-condensed cs-table2">
                                <tbody class="border-right2">
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Registration Form</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Membership Number</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="text" placeholder="" ng-model="info" ng-change="memberinfo()">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Registration Date</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="date" placeholder="" ng-model="regDate">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Play Date</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="date" placeholder="" ng-model="pDate">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Holes</td>
                                        <td class="all-uppercase">
                                            <table>
                                                <tr>
                                                    <td ng-repeat="h in holeList">
                                                        <input type="radio" name="tt" ng-model="holeId" ng-value="{{h.m_HoleTypeID}}" ng-change="tournamnetFee(holeId)"> {{h.m_HoleTypeDesc}}
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Estimated Tee of time</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="time" placeholder="">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Flight Serial</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="text" placeholder="">
                                        </td>
                                    </tr>
                                    <tr>
                                        <table class="table cs-table bordered-div col-sm-12 st-table">
                                            <tr style="border-top: none !important;">
                                                <td>
                                                    <input type="radio" name="paymentType" ng-model="payment" ng-value="1" ng-change="clickvalue()">
                                                    <label class="label-ch">Cash</label>
                                                </td>
                                                <td>
                                                    <input type="radio" name="paymentType" ng-model="payment" ng-value="2" ng-change="clickvalue()">
                                                    <label class="label-ch">Check</label>
                                                </td>
                                                <td>
                                                    <input type="radio" name="paymentType" ng-model="payment" ng-value="4" ng-change="clickvalue()">
                                                    <label class="label-ch">Bank Card</label>
                                                </td>
                                                <td>
                                                    <input type="radio" name="paymentType" ng-model="payment" ng-value="3" ng-change="clickvalue()">
                                                    <label class="label-ch">Card</label>
                                                </td>
                                                <td ng-show="paymentType.check">
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
                                                <td ng-show="paymentType.card">
                                                    <div class="col-sm-12">
                                                        <label class="label-text col-sm-4">Number</label>
                                                        <input class="st-padding form-control fc-auto col-sm-8" type="text" ng-model="paymentType.checkNumber">
                                                    </div>
                                                    <div class="col-sm-12">
                                                        <label class="label-text col-sm-4">Pincode</label>
                                                        <input class="st-padding form-control fc-auto col-sm-8" type="password" ng-model="paymentType.pincode">

                                                    </div>
                                                    <div class="col-sm-12">
                                                        <label class="label-text col-sm-4"></label>
                                                        <button class="col-sm-2 btn btn-sm btn-success mt-4" ng-click="isValidate()">Validate</button>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="table-responsive col-sm-5 no-padding" style="border: 1px solid #d6d6d6">
                            <table class="table table-condensed cs-table2">
                                <tbody class="border-right2">
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Member information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Name</td>
                                        <td class="all-uppercase" width="70%">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.MemberName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Club Name</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>KGC</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Category</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.MemberCategoryName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Status</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.MemberStatusName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Handicap</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.MemberName}}</span>
                                        </td>
                                    </tr>
                                    
                               
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Fee information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Fee</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{memberfee.m_MemberFee}}</span>
                                        </td>
                                    </tr>
                                    
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer" style="margin-top: 10px;">
                <div class="col-sm-12">
                    <div class="col-sm-6">
                        <button type="button" class="btn btn-default" ng-click="payNow(holeId)">Booking</button>
                        <button type="button" class="btn btn-default"ng-click="payNow()" >Registration</button>
                        <button type="button" class="btn btn-default" data-dismiss="modal" ng-click="close()">Cancel</button>
                    </div>
                    <div class="col-sm-6">
                        <h3>Total
                            <span>
                                {{grandTotal}}
                            </span>/= BDT</h3>
                    </div>
                </div>
            </div>
        </div>
        <!-- </div> -->

        <!-- </div> -->
    </script>
    <!-- Modal Ends-->



</body>

 <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa"
    crossorigin="anonymous"></script> 
<script src="../scripts/angular.min.js"></script>

<script src="../scripts/ui-bootstrap-tpls-2.5.0.js"></script>

<script>
    angular.module('tournamentRegistration', ['ui.bootstrap'])
        .controller('TournamentRegistrationCtrl', function ($scope, $http, $uibModal, $log, $document, $templateCache) {
            //var self = this;
            /* Bank Data load*/


            //$scope.regDate = new Date();
            
            /*For First Time Load*/
            $scope.getInitialData = function () {

                ///$scope.regDate = new format(Date(), 'MM/dd/yyyy').toString();
                ///@scopre.teeId = 1;
                $http.get('http://localhost:2997/api/Game/GetTees').then(function (response) {

                    if (response.data) {
                        $scope.teeList = response.data;
                        //console.log($scope.teeList)
                    }
                }, function (response) {
                    $scope.data = response.data || 'Request failed';
                    $scope.status = response.status;
                });

                $http.get('http://localhost:2997/api/Game/GeTournaments').then(function (response) {

                    if (response.data) {
                        $scope.tournamentList = response.data;
                        //console.log($scope.tournamentList)
                    }
                }, function (response) {
                    $scope.data = response.data || 'Request failed';
                    $scope.status = response.status;
                });

                $http.get('http://localhost:2997/api/Game/GetGameCategory').then(function (response) {

                    if (response.data) {
                        $scope.categoryList = response.data;
                    }
                    //self.status = response.status;
                }, function (response) {
                    $scope.data = response.data || 'Request failed';
                    $scope.status = response.status;
                });
            };


            $scope.getInitialData();

            $scope.tourList = function (TournamentID, GameCateoryID,TeeID) {
                
                var pdate = format($scope.regDate, 'MM/dd/yyyy').toString();
                var b = TournamentID;
                var a = GameCateoryID;
                var aTeeID = TeeID;

                $http.get('http://localhost:2997/api/Game/TournamentFlightSch?pTournamentID=' + b +
                    '&pGameCateoryID=' + a + '&pTeeID=' + aTeeID +
                    '&pTodate=' + "'" + pdate + "'" + '&pTodate1=' + "'" + pdate + ' 23:59:59'+"'").then(function (response) {

                        if (response.data) {
                            $scope.playerList = response.data;
                        }

                    }, function (response) {
                        $scope.data = response.data || 'Request failed';
                        $scope.status = response.status;
                    });

            };

            $scope.open = function (regDate,TournamentFlightSchID,TournamentID,GameCateoryID,TeeID,StartTime) {


                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'registration.html',
                    controller: ModalInstanceCtrl,
                    backdrop: true,
                    size: 'lg',
                    resolve: {
                        tId: function () {
                            return TournamentID;
                        },
                        rDate: function () {
                            return regDate;
                        },
                        tourF: function () {
                            return TournamentFlightSchID;
                        },
                        gameCat: function () {
                            return GameCateoryID;
                        },
                        teeId: function () {
                            return TeeID;
                        },
                        startTime: function () {
                            return StartTime;
                        },
                    }
                });

                modalInstance.result.then(function (selectedItem) {
                    $scope.playerList = selectedItem;

                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });

                function ModalInstanceCtrl($scope, $uibModalInstance, tId, $http, rDate, tourF, gameCat, teeId, startTime) {

                   
                    $scope.regDate = rDate;
                    $scope.pDate = rDate;                    
                    $scope.grandTotal = 0;
                  
                    $scope.calculateTotalAmount = function () {

                        $scope.grandTotal = 0;

                        if ($scope.minfo.MemberOfType) {
                            if ($scope.minfo.MemberOfType === "Full Member") {
                                $scope.grandTotal += $scope.memberfee.m_MemberFee

                            }
                            if ($scope.minfo.MemberOfType === "Spouse Of Member") {
                                $scope.grandTotal += $scope.memberfee.m_SpouseFee
                            }
                            if ($scope.minfo.MemberOfType === "Children Of Member") {
                                $scope.grandTotal += $scope.memberfee.m_ChildrenFee
                            }
                        }




                    };
                    $scope.isValidate = function () {
                        $http.get('http://localhost:2997/api/Member/GetMemberCardIsActive?sCardNumber=' +
                            $scope.paymentType
                            .checkNumber + '&spin=' + $scope.paymentType.pincode + '&pamount=' + $scope
                            .grandTotal).then(
                            function (response) {

                                if (response.data.m_IsCardActive === true) {
                                    self.message = "Your Card is Validated.";
                                    self.mclass = "alert-success";
                                } else {
                                    self.message =
                                        "Your Card is not Validated or Amount not Avilable. Total Balance:  " +
                                        response.data.m_CardBalance;
                                }

                            },
                            function (response) {
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }

                    $scope.paymentType = {
                        "cash": true,
                        "check": "",
                        "card": "",
                        "bcard": "",
                        "checkNumber": "",
                        "date": "",
                        "paidFromAddAmount": ""
                    };

                    $scope.clickvalue = function () {

                        if ($scope.payment === 1) {
                            $scope.paymentType.cash = true
                            $scope.paymentType.check = false
                            $scope.paymentType.card = false
                            $scope.paymentType.bcard = false
                        }
                        if ($scope.payment === 2) {
                            $scope.paymentType.check = true
                            $scope.paymentType.card = false
                            $scope.paymentType.cash = false
                            $scope.paymentType.bcard = false
                        }
                        if ($scope.payment === 3) {

                            $scope.paymentType.card = true
                            $scope.paymentType.check = false
                            $scope.paymentType.cash = false
                            $scope.paymentType.bcard = false
                        }
                        if ($scope.payment === 4) {

                            $scope.paymentType.card = false
                            $scope.paymentType.check = false
                            $scope.paymentType.cash = false
                            $scope.paymentType.bcard = true
                        }

                        $scope.calculateTotalAmount();
                    }


                    $http.get('http://localhost:2997/api/Game/GetHoleTypes').then(function (response) {

                        if (response.data) {
                            $scope.holeList = response.data;
                        }

                    }, function (response) {
                        self.data = response.data || 'Request failed';
                        self.status = response.status;
                    });





                    $scope.memberinfo = function () {

                        $http.get('http://localhost:2997/api/Member/bycode/' + $scope.info).then(function (
                            response) {

                            if (response.data) {
                                $scope.minfo = response.data;
                            }

                          

                        }, function (response) {
                            self.data = response.data || 'Request failed';
                            self.status = response.status;
                        });

                        $http.get('http://localhost:2997/api/MemberBill/GetBanks').then(function (
                            response) {

                            if (response.data) {
                                $scope.banks = response.data;
                            }


                        }, function (response) {
                            $scope.data = response.data || 'Request failed';
                            $scope.status = response.status;
                        });

                    }



                    $scope.tournamnetFee = function (holeId) {
                      
                       
                        $http.get('http://localhost:2997/api/Game/GeTournamentFees?pTournamentID=' + tId +
                            '&pMemberGroupID=' + $scope.minfo.MemberGroupID + '&pHoleTypeID=' + holeId
                        ).then(
                            function (response) {

                                if (response.data) {
                                    $scope.memberfee = response.data;
                                }
                                $scope.holeId = holeId;
                                $scope.calculateTotalAmount();
                            },
                            function (response) {
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }



                    $scope.payNow = function () {


                        var obj = {
                            "Master": {
                                "MemberID": $scope.minfo.MemberID,
                                "GameCategoryID": gameCat,
                                "TeeID": teeId,
                                "TournamentID": tId,
                                "HoleTypeID":$scope.holeId,
                                "PlayDate": format(new Date(rDate),
                                    "MM/dd/yyyy").toString(),
                                "RegDate": format(new Date(rDate),
                                    "MM/dd/yyyy").toString(),
                                "CreatedBY": "Admin",
                                "IsBooking": 0,
                                "isRegister": 1,
                                "TournamentFeeAmount": $scope.grandTotal,
                                "HandiCap": 21,
                                "TournamentFlightSchID": tourF,
                                "cash": $scope.paymentType.cash ? $scope.paymentType.cash : "",
                                "check": $scope.paymentType.check ? $scope.paymentType.check : "",
                                "card": $scope.paymentType.card ? $scope.paymentType.card : "",
                                "chequeNo": $scope.paymentType.checkNumber ? $scope.paymentType.checkNumber :
                                    "",
                                "chequeDT": $scope.paymentType.date ? format(new Date($scope.paymentType.date),
                                    "MM/dd/yyyy").toString() : format(new Date(rDate),
                                    "MM/dd/yyyy").toString(),
                                "bankid": $scope.bankId ? $scope.bankId : 0,


                            }
                        };

                      


                        $http({
                            method: "POST",
                            url: "http://localhost:2997/api/Game/CreateMemberTournamentPay",
                            data: obj
                        }).then(function (response) {
                            //window.location =
                            //    '/Pages/ClubReport.aspx?_ReportID=2&TourRegisterID=' +
                            //    response.data;

                        }, function (response) {
                            self.data = response.data || 'Request failed';
                            self.status = response.status;
                        });
                    };

                    $scope.ok = function () {
                        $uibModalInstance.close();
                    };
                    $scope.close = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                }



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