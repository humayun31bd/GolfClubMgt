﻿<!doctype html>

<html lang="en" ng-app="gameRegistrationMember"  ng-cloak>

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="../scripts/bootstrap332.min.css">
    <link rel="stylesheet" href="../scripts/paynowstyle.css">
    <!-- <link rel="stylesheet" href="../css/angular-virtual-keyboard.css"> -->
    <!-- Latest compiled and minified CSS -->
    <title>Golf</title>
</head>

<body class="container-fluid" ng-controller="gameRegistrationMemberCtrl" ng-init="getInitialData()" style="background: #ececec;">
    

    <section class="row">
        <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
            <div class="container-fluid" style="padding-top: 30px;">
                <div class="col-sm-2">
                    <a class="btn btn-sm btn-primary" href="/pages/Home.aspx">Back Home</a>
                </div>
                <div class="col-sm-6">
                    <h4>Tee Off Time Booking &amp; Registration</h4>
                </div>
                <div class="col-sm-3 text-right">
                    <div class="col-sm-4 no-padding" style="margin-top: 7px;">
                        <label>Date</label>
                    </div>
                    <div class="col-sm-8">
                        <input class="st-padding-cs form-control" type="date" style="width: 90%;" ng-model="regDate" ng-change="getInitialData()" disabled>
                    </div>
                </div>
<%--                <div class="col-sm-1">
                    <button class="btn btn-primary pull-right" style="margin: 0 10px;">Print</button>
                </div>--%>
                
                
             
                <hr class="col-sm-12" style="border-bottom: 1px solid #363636; margin-left: -15px;">
                <div class="col-sm-12 col-md-12 col-lg-12 col-xs-12">
                     <div class="col-sm-6 no-padding mt-5">
                                    <h4 align="center" style="font-weight: 900;font-size: 26px;color: #000;">Front 9</h4>
                          <form name="outerForm1" class="tab-form-demo">
                                    <div class="table-responsive" style="padding: 3px; border: 1px solid #d6d6d6">
                                        <table class="table table-striped table-hover cs-table table-bordered ">
                                            <thead class="thead-dark border-right-dark" style="width: 100%; background: #4CAF50; color: #fff;">
                                                <!-- <th class="text-center">Sl. No</th> -->
                                                <th class="text-center" width="15%">Time</th>
                                                <th width="25%">&nbsp;&nbsp;&nbsp;MembershipNo</th>
                                                <th class="text-center" width="50%">Name</th>
                                                <%--<th class="text-center" width="10%">Slots</th>--%>
                                            </thead>
                                            <tbody class="border-right">                                             
                                                <tr ng-repeat="plist in schyList.Tee1">
                                                    <td class="text-center">
                                                        <a ng-click="open('1',regDate,plist.StartTime,plist.FlightSchID,plist.FlightSchNo,plist.StartTime,ssUserName,plist.NumberOfPlayer,plist.MaxMemberCount) ">{{plist.StartTime}}</a>
                                                    </td>
                                                    <td class="text-center" colspan="2" width="100%">
                                                        <table class="table table-bordered">
                                                            <tr ng-repeat=" p in plist.RegPlayers">                                                              
                                                                <td width="45px"><a ng-click="show('3',regDate,plist.StartTime,plist.FlightSchID,plist.FlightSchNo,plist.StartTime,ssUserName)">{{p.m_MemberCode}}</a></td>
                                                                <td>{{p.m_MemberName}}</td>
                                                                <%--<td><a ng-click="show('1',regDate,p.StartTime,p.FlightSchID,p.FlightSchNo,p.StartTime,p.m_MemberCode,p,ssUserName)">{{p.m_MemberName}}</a></td>--%>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <%--<td class="text-center">{{plist.NumberOfPlayer}}/{{plist.MaxMemberCount}}</td>--%>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    </form>
                     </div>
                     <div class="col-sm-6 no-padding mt-5">
                                     <h4 align="center" style="font-weight: 900;font-size: 26px;color: #000;">Back 9</h4>
                          <form name="outerForm" class="tab-form-demo">
                                    <div class="table-responsive" style="padding: 3px; border: 1px solid #d6d6d6">
                                        <table class="table table-striped table-hover cs-table table-bordered">
                                            <thead class="thead-dark border-right-dark" style="width: 100%; background: #337ab7;color:#fff">
                                                <!-- <th class="text-center">Sl. No</th> -->
                                                <th class="text-center" width="15%">Time</th>
                                                <th width="25%">&nbsp;&nbsp;&nbsp;MembershipNo</th>
                                                <th class="text-center" width="50%">Name</th>
                                                <%--<th class="text-center" width="10%">Slots</th>--%>
                                            </thead>
                                            <tbody class="border-right">
                                                <tr ng-repeat="plist2 in schyList.Tee2">
                                                    <td class="text-center">
                                                        <a ng-click="open('2',regDate,plist2.StartTime,plist2.FlightSchID,plist2.FlightSchNo,plist2.StartTime,ssUserName,plist2.NumberOfPlayer,plist2.MaxMemberCount)">{{plist2.StartTime}}</a>
                                                    </td>
                                                    <td class="text-center" colspan="2" width="100%">
                                                        <table class="table table-bordered">
                                                            <tr ng-repeat=" p2 in plist2.RegPlayers">
                                                                <td width="45px">{{p2.m_MemberCode}}</td>
                                                                <td>{{p2.m_MemberName}}</td>
                                                                    <%--<a ng-click="show('2',regDate,p2.StartTime,p2.FlightSchID,p2.FlightSchNo,p2.StartTime,p2.m_MemberCode,p2,ssUserName)">{{p2.m_MemberName}}</a></td>--%>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <%--<td class="text-center">{{plist2.NumberOfPlayer}}/{{plist2.MaxMemberCount}}</td>--%>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                            </form>
                                    
                    </div>


                </div>
               
            </div>
        </div>
    </section>
    <br>


    <!-- Modal -->
        <script type="text/ng-template" id="registration">
        <!-- <div id="registration" class="modal fade" role="dialog" > -->


        <!-- <div class="modal-dialog" style="width: 95%; margin-left: 20px;"> -->
        <!-- Modal content-->
        <div class="modal-content font-sm">
            <div class="modal-header header-dark">
                <button type="button" class="close" ng-click="close()">&times;</button>
                <h4 class="modal-title">
                    <b>Registration - {{estime}}</b>
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
                                            <input class="st-padding form-control" id="myInput" type="text" placeholder="Scan Your Member Card" ng-model="infocard" ng-change="memberinfo()" ng-virtual-keyboard="{kt: 'Numerico'}">
                                            <input  class="st-padding form-control" id="myInput" type="hidden" placeholder="" ng-model="info" ng-change="memberinfo()" ng-virtual-keyboard="{kt: 'Numerico'}">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Registration Date</td>
                                        <td class="all-uppercase">
                                            <%--<input class="st-padding form-control" type="date" placeholder="" ng-model="regDate">--%>
                                            <input class="st-padding form-control" type="date" placeholder="" ng-model="regDate" disabled>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Holes</td>
                                        <td class="all-uppercase">
                                            <table>
                                                <tr>
                                                    <td ng-repeat="h in holeList">
                                                        <%--<input type="radio" name="tt" ng-model="hole_checked" ng-value="{{h.m_HoleTypeID}}" ng-click="onCheck()" ng-change="gamefee(holeId)" ng-disabled="!checked"> {{h.m_HoleTypeDesc}}--%>
                                                        <input type="radio" name="tt" ng-model="holeId" ng-value="{{h.m_HoleTypeID}}" ng-click="gamefee(holeId)"> {{h.m_HoleTypeDesc}}
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Caddie Number</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="text" placeholder="Enter Caddie Number Here" ng-model="cadeinfo" ng-change="cadeeinfo()">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Ballboy Number</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="text" placeholder="Enter Ballboy Number Here" ng-model="bboyinfo" ng-change="ballboyinfo()">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Need Golf Cart: </td>
					<td class="all-uppercase vam text-left"><input type="checkbox" ng-true-value="1" ng-false-value="0" name="needcart" ng-model="needgcart" ng-click="GolfCartFeeNeed()"></td>
                                    </tr>
				<tr>
					<td class="all-uppercase vam text-left">Golf Cart Code: </td>
				      <td>
                                        <input class="st-padding form-control col-sm-3" type="text" placeholder="Enter Cart Code" style="width: 50%;" ng-model="needgcartID">
					</td>
				</tr>
				    <tr >
					<td class="all-uppercase vam text-left">
                                        <td class="all-uppercase vam text-left">
                                                <input type="radio" name="8ball" ng-model="ball8" ng-value="1" ng-click="GolfCartFeeNeed()"> 9 Holes
                                                <input type="radio" name="8ball" ng-model="ball8" ng-value="2" ng-click="GolfCartFeeNeed()"> 18 Holes
                                        </td>
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
                                        <td class="all-uppercase vam text-left">Status</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.MemberStatusName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Handicap</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.HandiCap}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Caddie information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Name</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{cadee.m_CaddieName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Ball boy information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Name</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{ballboy.m_BallBoyName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Fee information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left" colspan="2">
                                            <table class="table">
                                                <tbody>
                                                    <tr>
                                                        <td>Green</td>
                                                        <td>:{{fee.m_GreenFee}}</td>
                                                        <td>Golfcart</td>
                                                        <td>:{{fee.m_GolfCartFee}}</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Caddie</td>
                                                        <td>:{{fee.m_CaddieFee}}</td>
                                                        <td>Subsidie</td>
                                                        <td>:{{fee.m_CaddieSubsidy}}</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Ballboy</td>
                                                        <td>:{{fee.m_BallBoyFee}}</td>
                                                        <td>Subsidie</td>
                                                        <td>:{{fee.m_BallBoySubsidy}}</td>
                                                    </tr>
                                                </tbody>
                                            </table>
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
                        <button type="button" class="btn btn-default" ng-click="payNow()">Registration</button>
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

    <script type="text/ng-template" id="editregistration">
        <!-- <div id="registration" class="modal fade" role="dialog" > -->


        <!-- <div class="modal-dialog" style="width: 95%; margin-left: 20px;"> -->
        <!-- Modal content-->
        <div class="modal-content font-sm">
            <div class="modal-header header-dark">
                <button type="button" class="close" ng-click="close()">&times;</button>
                <h4 class="modal-title">
                    <b>Registration - {{estime}}</b>
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
                                            <input class="st-padding form-control" id="myInput" type="text" placeholder="Scan Your Member Card" ng-model="infocard" ng-change="memberinfo()" ng-virtual-keyboard="{kt: 'Numerico'}">
                                            <input  class="st-padding form-control" id="myInput" type="hidden" placeholder="" ng-model="info" ng-change="memberinfo()" ng-virtual-keyboard="{kt: 'Numerico'}">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Registration Date</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="date" placeholder="" ng-model="regDate">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Holes</td>
                                        <td class="all-uppercase">
                                            <table>
                                                <tr>
                                                    <td ng-repeat="h in holeList">
                                                        <input type="radio" name="tt" ng-model="hole_checked" ng-value="{{h.m_HoleTypeID}}" ng-click="onCheck()" ng-change="gamefee(holeId)" ng-disabled="!checked"> {{h.m_HoleTypeDesc}}
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Caddie Number</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="text" placeholder="" ng-model="cadeinfo" ng-change="cadeeinfo()">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Ballboy Number</td>
                                        <td class="all-uppercase">
                                            <input class="st-padding form-control" type="text" placeholder="" ng-model="bboyinfo" ng-change="ballboyinfo()">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Need Golf Cart</td>
                                        <td class="all-uppercase">
                                            <p>
                                                <input type="checkbox" name="needcart" ng-model="needgcart" ng-click="calculateTotalAmount()">
                                            </p>
                                            <p>
                                                <input class="st-padding form-control col-sm-3" type="hidden" ng-click="gcartFee()" placeholder="" style="width: 20%;" ng-model="needgcartID">
                                                <input type="radio" name="8ball" ng-model="ball8" ng-value="1" ng-click="gcartFee()"> 9 Holes
                                                <input type="radio" name="8ball" ng-model="ball8" ng-value="2" ng-click="gcartFee()"> 18 Holes
                                            </p>
                                        </td>
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
                                        <td class="all-uppercase vam text-left">Status</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.MemberStatusName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Handicap</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{minfo.HandiCap}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Caddie information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Name</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{cadee.m_CaddieName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Ball boy information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left">Name</td>
                                        <td class="all-uppercase">:&nbsp;&nbsp;&nbsp;
                                            <span>{{ballboy.m_BallBoyName}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="all-uppercase vam text-left" style="background-color: #b7b7b7; border: 3px solid #FFF;">Fee information</td>
                                    </tr>
                                    <tr>
                                        <td class="all-uppercase vam text-left" colspan="2">
                                            <table class="table">
                                                <tbody>
                                                    <tr>
                                                        <td>Green</td>
                                                        <td>:{{fee.m_GreenFee}}</td>
                                                        <td>Golfcart</td>
                                                        <td>:{{fee.m_GolfCartFee}}</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Caddie</td>
                                                        <td>:{{fee.m_CaddieFee}}</td>
                                                        <td>Subsidie</td>
                                                        <td>:{{fee.m_CaddieSubsidy}}</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Ballboy</td>
                                                        <td>:{{fee.m_BallBoyFee}}</td>
                                                        <td>Subsidie</td>
                                                        <td>:{{fee.m_BallBoySubsidy}}</td>
                                                    </tr>
                                                </tbody>
                                            </table>
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
                        <button type="button" class="btn btn-default" ng-click="payNow()">Registration</button>
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


    

</body>


<script src="../scripts/angular.min.js"></script>
<script src="../scripts/ui-bootstrap-tpls-2.5.0.js"></script>

<script>
    angular.module('gameRegistrationMember', ['ui.bootstrap'])
        .controller('gameRegistrationMemberCtrl', function ($scope, $http, $uibModal, $log, $document, $templateCache) {
            //var self = this;
            /* Bank Data load*/
            $scope.regDate = new Date();
            $scope.ssUserName = "";
            /*For First Time Load*/
            $scope.getInitialData = function () {

                var cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)UserSettings\s*\=\s*([^;]*).*$)|^.*$/, "$1");
                $scope.ssUserName = cookieValue;


                var pdate = $scope.regDate ? format($scope.regDate, 'MM/dd/yyyy').toString() : format(new Date(), 'MM/dd/yyyy').toString();
                $http.get('http://localhost:2997/api/Game/GameFlightSchedules?pGameDate=' + "'" + pdate + "'" +
                    '&pGameDate1=' + "'" + pdate + ' 23:59:59' + "'").then(function (response) {

                        if (response.data) {
                            $scope.schyList = response.data;

                        }
                        
                        console.log('schedule', $scope.schyList);
                        
                    }, function (response) {
                        $scope.data = response.data || 'Request failed';
                        $scope.status = response.status;
                    });
            };

            //self.getInitialData();


            $scope.open = function (b, regDate, regTime, fSchID, fSchno, starttime, ssUserName, pNumberOfPlayer,pMaxMemberCount) {

                if (pNumberOfPlayer > 0 && pMaxMemberCount > 0)
                {
                    if (pNumberOfPlayer >= pMaxMemberCount)
                    {
                        alert('Maximum Player registration is complete....');
                        return;
                    }
                }

                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'registration',
                    controller: ModalInstanceCtrl,
                    backdrop: false,
                    size: 'lg',
                    resolve: {
                        tId: function () {
                            return b;
                        },
                        rDate: function () {
                            return regDate;
                        },
                        rTime: function () {
                            return regTime;
                        },
                        fSchID: function () {
                            return fSchID;
                        },
                        fSchno: function () {
                            return fSchno;
                        },
                        starttime: function () {
                            return starttime;
                        },
                        ssUserName: function () {
                            return ssUserName;
                        },
                    }
                });


                modalInstance.result.then(function (selectedItem) {
                    $scope.schyList = selectedItem;
                }, function (selectedItem) {
                    $log.info('Modal dismissed at: ' + new Date());
                });

                function ModalInstanceCtrl($scope, $uibModalInstance, tId, $http, rDate, rTime, fSchID, starttime, fSchno, ssUserName) {

                    $scope.fSchID = fSchID;
                    $scope.fSchNo = fSchno;
                    $scope.playDate = rDate;
                    $scope.estime = starttime;
                    $scope.regDate = rDate;
                    $scope.ssUserName = ssUserName;
                    $scope.disablecadee = false;
                    $scope.holeId = 1;
                    $scope.payment = 1;
                    $scope.ball8 = 1;



                    $scope.getInitialData = function () {
                        var pdate = format($scope.regDate, 'MM/dd/yyyy').toString();
                        $http.get('http://localhost:2997/api/Game/GameFlightSchedules?pGameDate=' + "'" + pdate + "'" +
                            '&pGameDate1=' + "'" + pdate + ' 23:59:59' + "'").then(function (response) {
                                if (response.data) {
                                    $scope.schyList = response.data;
                                }

                                //self.status = response.status;
                            }, function (response) {
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });
                    };



                    $scope.calculateTotalAmount = function () {



                        $scope.grandTotal = 0;
                        if ($scope.fee.m_GreenFee) {
                            $scope.grandTotal = $scope.fee.m_GreenFee;
                        }
                        else {
                            $scope.grandTotal += 0;
                        }
                        console.log("Click", $scope.grandTotal);

                        if ($scope.bboyinfo) {

                            if ($scope.bboyinfo > 0) {
                                $scope.grandTotal += $scope.fee.m_BallBoyFee;
                                $scope.grandTotal -= $scope.fee.m_BallBoySubsidy;
                            } else {
                                $scope.grandTotal += 0;
                                $scope.bboyinfo = false;
                            }
                        }
                        if ($scope.cadeinfo) {
                            if ($scope.cadeinfo > 0) {
                                $scope.grandTotal += $scope.fee.m_CaddieFee;
                                $scope.grandTotal -= $scope.fee.m_CaddieSubsidy;
                            } else {
                                $scope.grandTotal += 0;
                                $scope.cadeinfo = false;
                            }
                        }
                        if ($scope.needgcart) {
                            if ($scope.needgcart) {
                                $scope.grandTotal += $scope.fee.m_GolfCartFee;
                            } else {
                                $scope.grandTotal +=0;
                                $scope.gameFee = false;
                            }
                        }
                        if ($scope.pbcheck === 1) {
                            console.log('caddie fee', $scope.fee.m_CaddieFee, 'subsidy', $scope.fee.m_CaddieSubsidy);

                            $scope.grandTotal -= $scope.fee.m_CaddieFee;
                            $scope.grandTotal += $scope.fee.m_CaddieSubsidy;
                        } else {
                            $scope.grandTotal += 0;
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
                                //self.status = response.status;
                                alert(self.message);
                            },
                            function (response) {
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }

                    $scope.paymentType = {
                        "cash": "",
                        "check": "",
                        "card": "1",
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

                            $scope.paymentType.bcard = true
                            $scope.paymentType.card = false
                            $scope.paymentType.check = false
                            $scope.paymentType.cash = false
                        }

                        $scope.calculateTotalAmount();
                    }


                    $http.get('http://localhost:2997/api/Game/GetHoleTypes').then(function (response) {

                        if (response.data) {
                            $scope.holeList = response.data;
                            console.log('hole list', response.data);
                        }

                    }, function (response) {
                        self.data = response.data || 'Request failed';
                        self.status = response.status;
                    });


                    $scope.memberinfo = function () {

                        $http.get('http://localhost:2997/api/Member/GetByCard?MemberCard=' + $scope.infocard + '&pHashCode=INTOIT').then(function (
                            response) {

                            if (response.data) {
                                $scope.minfo = response.data;
                                console.log('memberinfo',response.data);
				$scope.gamefee(1);
                            }

                        }, function (response) {
                            $scope.calculateTotalAmount();
                            self.data = response.data || 'Request failed';
                            self.status = response.status;
                        });

                        $http.get('http://localhost:2997/api/MemberBill/GetBanks').then(function (
                            response) {

                            if (response.data) {
                                $scope.banks = response.data;
                            }

                            console.log($scope.banks);
                            //self.status = response.status;
                        }, function (response) {
                            $scope.calculateTotalAmount();
                            $scope.data = response.data || 'Request failed';
                            $scope.status = response.status;
                        });

                    }

                    $scope.cadeeinfo = function () {

                        $http.get('http://localhost:2997/api/Game/GetCaddiebyID?ID=' + $scope.cadeinfo).then(
                            function (response) {

                                /*console.log(response.data);*/

                                if (response.data) {
                                    $scope.cadee = response.data;
                                    if ($scope.cadee.m_IsAvailable === false) {
                                        /*alert("Caddie is not Available");
                                        return;*/
                                    }
                                } else {
                                    alert("Caddie is not found");
                                    return;
                                }
                                $scope.calculateTotalAmount();
                            },
                            function (response) {
                                $scope.calculateTotalAmount();
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }

                    $scope.ballboyinfo = function () {

                        $http.get('http://localhost:2997/api/Game/GetBallBoybyID?ID=' + $scope.bboyinfo).then(
                            function (response) {

                                if (response.data) {
                                    $scope.ballboy = response.data;
                                    if ($scope.ballboy.m_IsAvailable === false) {
                                        /*alert("Ballboy is not Available");
                                        return;*/
                                    }
                                } else {
                                    alert("Ballboy is not found");
                                    return;
                                }

                                $scope.calculateTotalAmount();
                            },
                            function (response) {
                                $scope.calculateTotalAmount();
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }

                    $scope.onCheck = function () {
                        $scope.hole_checked = false;
                    }

                    $scope.gamefee = function (hole) {


                        $scope.memberfee = {};
                        console.log("hole", hole, '$scope.infocard', $scope.infocard, $scope.playDate)
                        var memcardid = $scope.infocard;
                        $http.get('http://localhost:2997/api/Game/GetGameFeeByCard?pMemberCard=' + memcardid +
                            '&pHoleTypeID=' + hole + '&pGameDate=' + format($scope.playDate, 'MM/dd/yyyy').toString()).then(
                            function (response) {

                                console.log("Game fee res", response.data);

                                if (response.data) {
                                    $scope.fee = response.data;
                                }
                                console.log("Game fee", $scope.fee, hole, memcardid, $scope.grandTotal);

                                $scope.calculateTotalAmount();
                                console.log(' scope hole Id' + hole);
                            },
                            function (response) {
                                $scope.calculateTotalAmount();
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }
                    $scope.GolfCartFeeNeed = function () {

             		

			console.log($scope.needgcart);

                        if($scope.needgcart === 1){

				
			   $scope.grandTotal -=$scope.Preball8?$scope.Preball8:0;
 			$scope.Preball8 = $scope.Preball8 ? 0: $scope.Preball8;
				console.log("$scope.Preball8",$scope.Preball8);
                            if ($scope.ball8 === 1) {

                                $scope.grandTotal += 1000;
                                $scope.Preball8 = 1000;
				console.log("$scope.Preball82nd",$scope.Preball8);

                            } else {
                               
                            		if ($scope.ball8 === 2) {
                            		    $scope.grandTotal += 2000;
                            		    $scope.Preball8 = 2000;
                                
                          		  } else {
                                                            
                           		     
                          		   $scope.Preball8 = 0;
					}
                            }

                        }else{
				$scope.grandTotal -=$scope.Preball8?$scope.Preball8:0; 
		        	$scope.Preball8 = 0;
				
			
			}
                        
                    }



                    $scope.parmanantCadii = function () {
                        if ($scope.pbcheck === true) {
                            $scope.fee.m_CaddieSubsidy = 0;
                            $scope.fee.m_BallBoySubsidy = 0;
                            $scope.fee.m_CaddieFee = 0;
                            $scope.fee.m_BallBoyFee = 0;

                        }
                    }



                    $scope.payNow = function () {

                        if ($scope.infocard === null)
                        {
                            alert('You card is blank....');
                            return;
                        }

                        var obj = {
                            "Master": {
                                "MemberID": $scope.minfo.MemberID,
                                "MemberCard": $scope.infocard,
                                "HandiCap": 21,
                                "FlightSchID": fSchID,
                                "IsSinglePlayer": 1,
                                "IsGroupPlayer": 0,
                                "BallBoyID": $scope.bboyinfo,
                                "BallBoyFee": $scope.pbcheck ? 0 : $scope.fee.m_BallBoyFee,
                                "CaddieFee": $scope.pbcheck ? 0 : $scope.fee.m_CaddieFee,
                                "CaddieID": $scope.cadeinfo,
                                "CaddieCode": $scope.cadeinfo,
                                "CaddiePermanent": $scope.pbcheck?$scope.pbcheck:0,
                                "NeedGolfCart": $scope.needgcart?$scope.needgcart:0,
                                "GolfCartID": $scope.needgcartID?$scope.needgcartID:0,
                                "GolfCartFee": $scope.fee.m_GolfCartFee,
                                "GolfCartHoleTypeID": $scope.ball8,
                                "GreenFee": $scope.fee.m_GreenFee,
                                "CaddieSubsidy": $scope.pbcheck ? 0 : $scope.fee.m_CaddieSubsidy,
                                "BallBoySubsidy": $scope.pbcheck ? 0 : $scope.fee.m_BallBoySubsidy,
                                "GreenSubsidy": $scope.fee.m_GreenSubsidy,
                                "HoleTypeID": $scope.fee.m_HoleTypeID,
                                "IsBooking": 0,
                                "BookingDate": $scope.regDate ? format(new Date($scope.regDate),
                                    "MM/dd/yyyy").toString() : '',
                                "PlayDate": $scope.playDate ? format(new Date($scope.playDate),
                                    "MM/dd/yyyy").toString() : '',
                                "RegDate": $scope.regDate ? format(new Date($scope.regDate),
                                    "MM/dd/yyyy").toString() : '',
                                "TeeID": tId,
                                "isRegister": 1,
                                "CreatedBY": $scope.ssUserName,
                                "TotalBill": $scope.grandTotal,

                                "NextTeeID": 2,
                                "NextFlightSchID": 0,
                                "cash": "",
                                "check":  "",
                                "card": true,
                                "bcard":  "",
                                "checkNumber": "",
                                "chequedt": $scope.paymentType.date ? format(new Date($scope.paymentType.date),
                                    "MM/dd/yyyy").toString() : format(new Date($scope.playDate),
                                    "MM/dd/yyyy").toString(),
                                "bankid": 0


                            }
                        };


			console.log(obj);

                        $http({
                            method: "POST",
                            url: "http://localhost:2997/api/Game/CreateMemberGamePayByMemberNew",
                            data: obj
                        }).then(function (response) {

                            console.log(response.data);
                            if (response.data.ErrorCode > 0) {
                                /*console.log('response..error > 0', response.data);*/
                                alert("Registration failed. call support team-" + response.data.ErrorMessage);
                            }


                            var pdate = format(new Date(response.data.RegDate), 'MM/dd/yyyy').toString();

				

                            $http.get('http://localhost:2997/api/Game/GameFlightSchedules?pGameDate=' + "'" + pdate + "'" +
                                '&pGameDate1=' + "'" + pdate + ' 23:59:59' + "'").then(function (response) {
                                    if (response.data) {
                                        $scope.schyList = response.data;
                                    }
                                    $uibModalInstance.close($scope.schyList);
                                    //self.status = response.status;
                                }, function (response) {
                                    self.data = response.data || 'Request failed';
                                    self.status = response.status;
                                });
                        }, function (response) {
                            self.data = response.data || 'Request failed';
                            self.status = response.status;
                        });
                    };

                    $scope.ok = function () {

                        $uibModalInstance.close();
                    };
                    $scope.close = function () {
                        $scope.items = {
                            ka: "JJJ",
                            pa: "88888888888888"
                        }
                        $uibModalInstance.dismiss($scope.items);
                    };
                }





            };

            $scope.show = function (b, regDate, regTime, fSchID, fSchno, starttime, mId, p) {


                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'editregistration',
                    controller: ModalInstanceCtrl,
                    backdrop: false,
                    size: 'lg',
                    resolve: {
                        tId: function () {
                            return b;
                        },
                        rDate: function () {
                            return regDate;
                        },
                        rTime: function () {
                            return regTime;
                        },
                        fSchID: function () {
                            return fSchID;
                        },
                        fSchno: function () {
                            return fSchno;
                        },
                        starttime: function () {
                            return starttime;
                        },
                        mId: function () {
                            return mId;
                        },
                        p: function () {
                            return p;
                        },

                    }
                });


                modalInstance.result.then(function (selectedItem) {
                    $scope.schyList = selectedItem;
                }, function (selectedItem) {
                    $log.info('Modal dismissed at: ' + new Date());
                });

                function ModalInstanceCtrl($scope, $uibModalInstance, tId, $http, rDate, rTime, fSchID, starttime, fSchno, mId, p) {

                    console.log(p);


                    $scope.fee = {};

                    $scope.previousfee = {};

                    $scope.grandTotal = 0;

                    $scope.previousfee.grandTotal = 0;

                    $scope.MemberName = p.m_MemberName;
                    $scope.fSchID = fSchID;
                    $scope.fSchNo = fSchno;
                    $scope.playDate = rDate;
                    $scope.estime = starttime;
                    $scope.regDate = rDate;
                    $scope.info = mId,
                    $scope.holeId = p.m_HoleTypeID,
                    $scope.grandTotal = p.m_TotalBill;
                    $scope.fee.m_GreenFee = p.m_GreenFeep
                    $scope.fee.m_GreenSubsidy = p.m_GreenSubsidy
                    $scope.fee.m_CaddieFee = p.m_CaddieFee
                    $scope.fee.m_CaddieSubsidy = p.m_CaddieSubsidy
                    $scope.fee.m_BallBoyFee = p.m_BallBoyFee
                    $scope.fee.m_BallBoySubsidy = p.m_BallBoySubsidy
                    $scope.fee.m_GolfCartFee = p.m_GolfCartFee

                    $scope.bboyinfo = p.m_BallBoyID
                    $scope.cadeinfo = p.m_CaddieID
                    $scope.needgcart = p.m_NeedGolfCart


                    $scope.previousfee.fSchID = fSchID;
                    $scope.previousfee.fSchNo = fSchno;
                    $scope.previousfee.playDate = rDate;
                    $scope.previousfee.estime = starttime;
                    $scope.previousfee.regDate = rDate;
                    $scope.previousfee.info = mId,
                    $scope.previousfee.holeId = p.m_HoleTypeID,
                    $scope.previousfee.grandTotal = p.m_TotalBill;
                    $scope.previousfee.m_GreenFee = p.m_GreenFee
                    $scope.previousfee.m_GreenSubsidy = p.m_GreenSubsidy
                    $scope.previousfee.m_CaddieFee = p.m_CaddieFee
                    $scope.previousfee.m_CaddieSubsidy = p.m_CaddieSubsidy
                    $scope.previousfee.m_BallBoyFee = p.m_BallBoyFee
                    $scope.previousfee.m_BallBoySubsiDy = p.m_BallBoySubsidy
                    $scope.previousfee.m_GolfCartFee = p.m_GolfCartFee



                    $scope.getInitialData = function () {
                        var pdate = format($scope.regDate, 'MM/dd/yyyy').toString();
                        $http.get('http://localhost:2997/api/Game/GameFlightSchedules?pGameDate=' + "'" + pdate + "'" +
                            '&pGameDate1=' + "'" + pdate + ' 23:59:59' + "'").then(function (response) {
                                if (response.data) {
                                    $scope.schyList = response.data;
                                }

                                //self.status = response.status;
                            }, function (response) {
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });
                    };

                    $scope.calculateTotalAmount = function () {



                        $scope.grandTotal = 0;
                        if ($scope.fee.m_GreenFee) {
                            $scope.grandTotal = $scope.fee.m_GreenFee;
                        }
                        else {
                            $scope.grandTotal += 0;
                        }
                        

                        if ($scope.bboyinfo) {

                            if ($scope.bboyinfo > 0) {
                                $scope.grandTotal += $scope.fee.m_BallBoyFee;
                                $scope.grandTotal -= $scope.fee.m_BallBoySubsidy;
                            } else {
                                $scope.grandTotal = $scope.fee.m_GreenFee;
                                $scope.bboyinfo = false;
                                alert("Please give date first")
                            }
                        }
                        if ($scope.cadeinfo) {
                            if ($scope.cadeinfo > 0) {
                                $scope.grandTotal += $scope.fee.m_CaddieFee;
                                $scope.grandTotal -= $scope.fee.m_CaddieSubsidy;
                            } else {
                                $scope.grandTotal = $scope.fee.m_GreenFee;
                                $scope.cadeinfo = false;
                                alert("Please give date first")
                            }
                        }
                        if ($scope.needgcart) {
                            if ($scope.needgcart) {
                                $scope.grandTotal += $scope.fee.m_GolfCartFee;
                            } else {
                                $scope.grandTotal += 0;
                                $scope.gameFee = false;
                                
                            }
                        }


                    };


                    $scope.paymentType = {
                        "cash": "",
                        "check": "",
                        "card": "true",
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

                            $scope.paymentType.bcard = true
                            $scope.paymentType.card = false
                            $scope.paymentType.check = false
                            $scope.paymentType.cash = false
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

                        // $scope.addVKI();
                        console.log('game edit', $scope.minfo);

                        $http.get('http://localhost:2997/api/Member/bycode/' + $scope.info).then(function (
                            response) {

                            if (response.data) {
                                $scope.minfo = response.data;
                            }
                            console.log('game edit', $scope.minfo);

                        }, function (response) {
                            $scope.calculateTotalAmount();
                            self.data = response.data || 'Request failed';
                            self.status = response.status;
                        });

                        $http.get('http://localhost:2997/api/MemberBill/GetBanks').then(function (
                            response) {

                            if (response.data) {
                                $scope.banks = response.data;
                            }

                            console.log($scope.banks);
                            //self.status = response.status;
                        }, function (response) {
                            $scope.calculateTotalAmount();
                            $scope.data = response.data || 'Request failed';
                            $scope.status = response.status;
                        });

                    }

                    $scope.cadeeinfo = function () {

                        $http.get('http://localhost:2997/api/Game/GetCaddiebyID?ID=' + $scope.cadeinfo).then(
                            function (response) {

                                if (response.data) {
                                    $scope.cadee = response.data;
                                }
                                $scope.calculateTotalAmount();
                            },
                            function (response) {
                                $scope.calculateTotalAmount();
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }

                    $scope.ballboyinfo = function () {

                        $http.get('http://localhost:2997/api/Game/GetBallBoybyID?ID=' + $scope.bboyinfo).then(
                            function (response) {

                                if (response.data) {
                                    $scope.ballboy = response.data;
                                }

                                $scope.calculateTotalAmount();
                            },
                            function (response) {
                                $scope.calculateTotalAmount();
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }

                    $scope.gamefee = function (hole) {


                        $scope.memberfee = {};

                        $http.get('http://localhost:2997/api/Game/GetGameFee?pMemberCode=' + $scope.info +
                            '&pHoleTypeID=' + hole + '&pGameDate=' + format($scope.playDate, 'MM/dd/yyyy').toString()).then(
                            function (response) {

                                if (response.data) {
                                    $scope.fee = response.data;

                                    $scope.cartfee = response.data;
                                }
                                var cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)UserSettings\s*\=\s*([^;]*).*$)|^.*$/, "$1");
                                var ssUserName = cookieValue;

                                $scope.calculateTotalAmount();

                            },
                            function (response) {
                                $scope.calculateTotalAmount();
                                self.data = response.data || 'Request failed';
                                self.status = response.status;
                            });

                    }

                    $scope.parmanantCadii = function () {
                        if ($scope.pbcheck === true) {
                            $scope.fee.m_CaddieSubsidy = 0;
                            $scope.fee.m_BallBoySubsidy = 0;
                            $scope.fee.m_CaddieFee = 0;
                            $scope.fee.m_BallBoyFee = 0;

                        }
                    }

                    $scope.payNow = function () {


                        var obj = {
                            "Master": {
                                //  "MemberID": $scope.minfo.MemberID,
                                "MemberCode": $scope.info,
                                "HandiCap": 21,
                                "FlightSchID": p.m_FlightSchID,
                                "GRegisterID": p.m_GRegisterID,
                                "IsSinglePlayer": 1,
                                "IsGroupPlayer": 0,
                                "BallBoyID": $scope.bboyinfo,
                                "BallBoyFee": $scope.pbcheck ? 0 : $scope.fee.m_BallBoyFee - $scope.previousfee.m_BallBoyFee,
                                "CaddieFee": $scope.pbcheck ? 0 : $scope.fee.m_CaddieFee - $scope.previousfee.m_CaddieFee,
                                "CaddieID": $scope.cadeinfo,
                                "CaddiePermanent": $scope.pbcheck?$scope.pbcheck:0,
                                "NeedGolfCart": $scope.needgcart?$scope.needgcart:0,
                                "GolfCartID": $scope.needgcartID,
                                "GolfCartFee": $scope.fee.m_GolfCartFee - $scope.previousfee.m_GolfCartFee,
                                "GolfCartHoleTypeID": $scope.ball8,
                                "GreenFee": $scope.fee.m_GreenFee - $scope.previousfee.m_GreenFee,
                                "CaddieSubsidy": $scope.pbcheck ? 0 : $scope.fee.m_CaddieSubsidy - $scope.previousfee.m_CaddieSubsidy,
                                "BallBoySubsidy": $scope.pbcheck ? 0 : $scope.fee.m_BallBoySubsidy - p.m_BallBoySubsidy,
                                "GreenSubsidy": $scope.fee.m_GreenSubsidy - $scope.previousfee.m_GreenSubsidy,
                                "HoleTypeID": $scope.fee.m_HoleTypeID,
                                "IsBooking": 0,
                                "BookingDate": $scope.regDate ? format(new Date($scope.regDate),
                                    "MM/dd/yyyy").toString() : '',
                                "PlayDate": $scope.playDate ? format(new Date($scope.playDate),
                                    "MM/dd/yyyy").toString() : '',
                                "RegDate": $scope.regDate ? format(new Date($scope.regDate),
                                    "MM/dd/yyyy").toString() : '',
                                "TeeID": tId,
                                "isRegister": 1,
                                "TotalBill": $scope.grandTotal - $scope.previousfee.grandTotal,

                                "NextTeeID": 2,
                                "NextFlightSchID": 0,
                                "cash": $scope.paymentType.cash ? $scope.paymentType.cash : "",
                                "check": $scope.paymentType.check ? $scope.paymentType.check : "",
                                "card": $scope.paymentType.card ? $scope.paymentType.card : "",
                                "bcard": $scope.paymentType.bcard ? $scope.paymentType.bcard : "",
                                "checkNumber": $scope.paymentType.checkNumber ? $scope.paymentType.checkNumber :
                                    "",
                                "chequedt": $scope.paymentType.date ? format(new Date($scope.paymentType.date),
                                    "MM/dd/yyyy").toString() : format(new Date($scope.playDate),
                                    "MM/dd/yyyy").toString(),
                                "bankid": $scope.bankId ? $scope.bankId : 0,
                                "CreatedBy": $scope.ssUserName

                            }
                        };


                        console.log(obj, $scope.fee.m_BallBoySubsidy, p.m_BallBoySubsidy);


                        $http({
                            method: "POST",
                            url: "http://localhost:2997/api/Game/CreateMemberGameExtraPay",
                            data: obj
                        }).then(function (response) {

                            var pdate = response.data.RegDate;
                            $http.get('http://localhost:2997/api/Game/GameFlightSchedules?pGameDate=' + "'" + pdate + "'" +
                                '&pGameDate1=' + "'" + pdate + ' 23:59:59' + "'").then(function (response) {
                                    if (response.data) {
                                        $scope.schyList = response.data;
                                    }
                                    $uibModalInstance.close($scope.schyList);
                                    //self.status = response.status;
                                }, function (response) {
                                    self.data = response.data || 'Request failed';
                                    self.status = response.status;
                                });





                        }, function (response) {
                            self.data = response.data || 'Request failed';
                            self.status = response.status;
                        });
                    };

                    $scope.ok = function () {

                        $uibModalInstance.close();
                    };
                    $scope.print = function () {
                        console.log('p.m_GRegisterID', p.m_GRegisterID);
                        window.location = '/Pages/ClubReport.aspx?_ReportID=2&_GregisterID=' + p.m_GRegisterID;

                    }
                    $scope.close = function () {
                        $scope.items = {
                            ka: "JJJ",
                            pa: "88888888888888"
                        }
                        $uibModalInstance.dismiss($scope.items);
                    };

                }

            }




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