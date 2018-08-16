<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Golf Club</title>

    <link href="../css/bootstrap.css" rel="stylesheet">
    <link rel="stylesheet" href="../css/GPstyle.css">
    <link rel="stylesheet" href="../css/cp-style.css">
    <link href="../css/keyboard-basic.css" rel="stylesheet">
    <link rel="stylesheet" href="../css/jquery-ui.css">

    <script src="../js/jquery.min.js"></script>
    <script src="../js/jquery-ui.js"></script>
</head>
<style>
    .mytable a:link {
        color: #fff;
        font-weight: bold;
        text-decoration: none;
    }

    .mytable a:visited {
        color: #fff;
        font-weight: bold;
        text-decoration: none;
    }

    .mytable a:active,
    .mytable a:hover {
        color: #bd5a35;
        text-decoration: underline;
    }

    table.mytable {
        width: 90%;
        font-family: Arial, Helvetica, sans-serif;
        color: #666;
        margin-left: auto;
        margin-right: auto;
        font-size: 12px;
        background: #eaebec;
        border: #ccc 1px solid;
        -moz-border-radius: 3px;
        -webkit-border-radius: 3px;
        border-radius: 3px;
        -moz-box-shadow: 10px 10px 5px #888;
        -webkit-box-shadow: 10px 10px 5px #888;
        box-shadow: 10px 10px 5px #888;
    }

    .mytable th {
        color: #fff;
        padding: 21px 25px 22px 25px;
        border-top: 1px solid #fafafa;
        border-bottom: 1px solid #e0e0e0;
        background: #242424;
    }

    .mytable th:first-child {
        text-align: center;
        padding-left: 20px;
    }

    .mytable tr {
        text-align: center;
        padding-left: 20px;
    }

    .mytable tr td:first-child {
        text-align: center;
        padding-left: 20px;
        border-left: 0;
    }

    .mytable tr td {
        padding: 18px;
        border-top: 1px solid #ffffff;
        border-bottom: 1px solid #e0e0e0;
        border-left: 1px solid #e0e0e0;
        background: #fafafa;
        background: -webkit-gradient(linear, left top, left bottom, from(#fbfbfb), to(#fafa fa));
        background: -moz-linear-gradient(top, #fbfbfb, #fafafa);
    }

    .mytable tr.even td {
        background: #f6f6f6;
        background: -webkit-gradient(linear, left top, left bottom, from(#f8f8f8), to(#f6f6 f6));
        background: -moz-linear-gradient(top, #f8f8f8, #f6f6f6);
    }

    .mytable tr:last-child td {
        border-bottom: 0;
    }

    .mytable tr:last-child td:first-child {
        -moz-border-radius-bottomleft: 3px;
        -webkit-border-bottom-left-radius: 3px;
        border-bottom-left-radius: 3px;
    }

    .mytable tr:last-child td:last-child {
        -moz-border-radius-bottomright: 3px;
        -webkit-border-bottom-right-radius: 3px;
        border-bottom-right-radius: 3px;
    }

    .mytable tr:hover td {
        background: #f2f2f2;
        transform: scale(1.01);
        padding-left: 20px;
        outline: 1px solid #191970;
        -moz-box-shadow: 10px 10px 5px #888;
        -webkit-box-shadow: 10px 10px 5px #888;
        box-shadow: 10px 10px 5px #888;
    }

    .ui-keyboard {
        /* adjust overall keyboard size using "font-size" */
        font-size: 14px;
        text-align: center;
        background: #fefefe;
        border: 1px solid #aaa;
        padding: 4px;

        /* include the following setting to place the
        keyboard at the bottom of the browser window */
        width: 320px;
        height: 100px;
        left: 0px;
        top: auto;
        bottom: 0px;
        position: relative;
        white-space: nowrap;
        overflow-x: auto;
        /* see issue #484 */
        -ms-touch-action: manipulation;
        touch-action: manipulation;
    }

    input[type=checkbox] {
        display: none;
    }

    input[type=checkbox]+label {
        display: inline-block;
        padding: 0 0 0 0px;
        background: url("lib/img/nocaddiee.png") no-repeat;
        height: 35px;
        width: 35px;
        background-size: 100%;
    }

    input[type=checkbox]:checked+label {
        background: url("lib/img/caddieyes.png") no-repeat;
        height: 35px;
        width: 35px;
        display: inline-block;
        background-size: 100%;
    }

    caption {
        padding: 0;
    }
</style>

<body>
    <div class="cp-wrapper">
        <div class="cp-main-content">
            <section class="cp-booking-section cp-booking-section_v2 pd-tb100" style="padding-bottom: 20px;">
                <div class="container">
                    <div style="margin-top: -7%;">
                        <ul class="breadcrumb">
                            <li>
                                <a href="index.html">Home</a>
                            </li>
                            <li class="active">Member Registration
                            </li>
                        </ul>
                    </div>
                    <div class="cp-heading-style1_v1" style="margin-top: -2%;margin-bottom: 10px">
                        <h2 style="font-size: 35px">Member Registration</h2>
                    </div>
                    <div class="row">
                        <div class="col-md-10 col-md-offset-1">
                            <div class="col-sm-12" id="infoMessage" style="font-size: 35px; text-align: center; background-color: #ffffff; color: green">
                            </div>
                            <div class="col-sm-12" id="infoMessage1" style="font-size: 20px; text-align: center; background-color: #ffffff; color: red">
                            </div>
                            <div class="cp-booking-inner">
                                <form class="" action="http://localhost/golf/registration/reg_tee_member" method="POST">
                                    <div class="row">
                                        <div class="col-sm-12 col-xs-12" style=" font-size: 25px;">
                                            <div class="col-sm-6">
                                                <strong>Date:
                                                    <span id="date"></span>
                                                </strong>
                                            </div>
                                            <div class="col-sm-6" style="text-align: right; font-size: 25px;">
                                                <strong>Time:
                                                    <span id="time"></span>
                                                </strong>
                                            </div>
                                        </div>
                                        <div class="col-sm-12 col-xs-12" style=" font-size: 20px;">
                                            <div class="col-sm-6" style="text-align: right; font-size: 20px;">
                                                <strong></strong>
                                            </div>
                                        </div>
                                        <div class="col-md-6 col-sm-6" style="padding-bottom: 10px;">
                                            <div class="continput">
                                                <ul class=new-ul>
                                                    <li class="new-li">
                                                        <input type="radio" class="player_type" name="player" value="1">
                                                        <label class="new-label label-new">Single Player</label>
                                                        <div class="bullet">
                                                            <div class="line zero"></div>
                                                            <div class="line one"></div>
                                                            <div class="line two"></div>
                                                            <div class="line three"></div>
                                                            <div class="line four"></div>
                                                            <div class="line five"></div>
                                                            <div class="line six"></div>
                                                            <div class="line seven"></div>
                                                        </div>
                                                    </li>
                                                    <li class="new-li">
                                                        <input type="radio" class="player_type" name="player" value="2">
                                                        <label class="new-label label-new">Group Player (Maximum of 4 Players)</label>
                                                        <div class="bullet">
                                                            <div class="line zero"></div>
                                                            <div class="line one"></div>
                                                            <div class="line two"></div>
                                                            <div class="line three"></div>
                                                            <div class="line four"></div>
                                                            <div class="line five"></div>
                                                            <div class="line six"></div>
                                                            <div class="line seven"></div>
                                                        </div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                        <div class="col-sm-12 col-md-12" style="">
                                            <div style="display:none;" id="single" class="col-md-12 col-sm-12">
                                                <div class="col-sm-12">
                                                    <div class="col-md-3 col-sm-3 form-group">
                                                        <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 keyboard keymem" id="rfid" name="single_member"
                                                            onkeyup="read_rfid_single()" placeholder="Player" value="" type="text"
                                                            autocomplete="off">
                                                    </div>
                                                    <div class="col-md-2 col-sm-2 form-group">
                                                        <input style="color: #0e0e0e;" class="form-control1 form-control5 cad_cursor keyboard keycad" id="caddie" name="single_caddie"
                                                            placeholder="Caddie" type="text" maxlength="30" autocomplete="off">
                                                    </div>
                                                    <div class="col-md-2 col-sm-2 form-group">
                                                        <input style="color: #0e0e0e; padding: 3px;" class="form-control1 form-control5 cad_cursor keyboard bboy" id="bboy" name="single_ballboy"
                                                            placeholder="Ball Boy" type="text" maxlength="30" autocomplete="off">
                                                    </div>
                                                    <div class="col-md-5 col-sm-5 form-group">
                                                        <div class="col-md-7" style="padding-top: 10px;">
                                                            <label for="cad1" class="label-new no-padding">Personal Caddie</label>
                                                        </div>
                                                        <div class="col-md-5" style="margin-left: -30px;">
                                                            <input type="checkbox" name="" id="cad1" value="1">
                                                            <label for="cad1"></label>
                                                        </div>
                                                    </div>
                                                    <div>
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
                                                            <!-- </table>
                                                    <table class="table cs-table bordered-div col-sm-12 st-table"> -->

                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="group" style="display:none; padding: 0;" class="col-md-12 col-sm-12">
                                                <div class="col-sm-12">
                                                    <div class="col-md-12">
                                                        <div class="col-md-3 col-sm-3 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 rfid keyboard keymem" id="rfid_1" name="mem_0"
                                                                onkeyup="read_rfid('1')" placeholder="Player#1 " type="text"
                                                                autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 cad_cursor_1 keyboard keycad" id="cad1" name="cad_0"
                                                                placeholder="Caddie#1" value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e; padding: 3px;" class="form-control1 form-control5 col-sm-12 keyboard bboy" id="bboy1" name="ball_0"
                                                                placeholder="BallBoy#1" value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-5 col-sm-5 form-group">
                                                            <div class="col-md-7" style="padding-top: 10px;">
                                                                <label for="cad2" class="label-new no-padding">Personal Caddie</label>
                                                            </div>
                                                            <div class="col-md-5" style="margin-left: -30px;">
                                                                <input type="checkbox" name="" id="cad2" value="1">
                                                                <label for="cad2"></label>
                                                            </div>
                                                        </div>
                                                        <div>
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
                                                                <!-- </table>
                                                        <table class="table cs-table bordered-div col-sm-12 st-table"> -->
    
                                                            </table>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-12">
                                                        <div class="col-md-3 col-sm-3 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 rfid keyboard keymem" id="rfid_2" name="mem_1"
                                                                onkeyup="read_rfid('2')" placeholder="Player#2" type="text" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 cad_cursor_2 keyboard keycad" id="cad2" name="cad_1"
                                                                placeholder="Caddie#2 " value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e; padding: 3px;" class="form-control1 form-control5 col-sm-12 keyboard bboy" id="bboy2" name="ball_1"
                                                                placeholder="BallBoy#2" value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-5 col-sm-5 form-group">
                                                            <div class="col-md-7" style="padding-top: 10px;">
                                                                <label for="cad3" class="label-new no-padding">Personal Caddie</label>
                                                            </div>
                                                            <div class="col-md-5" style="margin-left: -30px;">
                                                                <input type="checkbox" name="" id="cad3" value="1">
                                                                <label for="cad3"></label>
                                                            </div>
                                                        </div>
                                                        <div>
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
                                                                <!-- </table>
                                                        <table class="table cs-table bordered-div col-sm-12 st-table"> -->
    
                                                            </table>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-12">
                                                        <div class="col-md-3 col-sm-3 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 rfid keyboard keymem" id="rfid_3" name="mem_2"
                                                                onkeyup="read_rfid('3')" placeholder="Player#3 " type="text"
                                                                autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 cad_cursor_3 keyboard keycad" id="cad3" name="cad_2"
                                                                placeholder="Caddie#3 " value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e; padding: 3px;" class="form-control1 form-control5 col-sm-12 keyboard bboy" id="bboy3" name="ball_2"
                                                                placeholder="BallBoy#3" value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-5 col-sm-5 form-group">
                                                            <div class="col-md-7" style="padding-top: 10px;">
                                                                <label for="cad4" class="label-new no-padding">Personal Caddie</label>
                                                            </div>
                                                            <div class="col-md-5" style="margin-left: -30px;">
                                                                <input type="checkbox" name="" id="cad4" value="1">
                                                                <label for="cad4"></label>
                                                            </div>
                                                        </div>
                                                        <div>
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
                                                                <!-- </table>
                                                        <table class="table cs-table bordered-div col-sm-12 st-table"> -->
    
                                                            </table>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-12">
                                                        <div class="col-md-3 col-sm-3 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 rfid keyboard keymem" id="rfid_4" name="mem_3"
                                                                onkeyup="read_rfid('4')" placeholder="Player#4" type="text" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e;" class="form-control1 form-control5 col-sm-12 cad_cursor_4 keyboard keycad" id="cad4" name="cad_3"
                                                                placeholder="Caddie#4" value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-2 col-sm-2 form-group">
                                                            <input style="color: #0e0e0e; padding: 3px;" class="form-control1 form-control5 col-sm-12 keyboard bboy" id="bboy4" name="ball_3"
                                                                placeholder="BallBoy#4" value="" type="text" maxlength="30" autocomplete="off">
                                                        </div>
                                                        <div class="col-md-5 col-sm-5 form-group">
                                                            <div class="col-md-7" style="padding-top: 10px;">
                                                                <label for="cad5" class="label-new no-padding">Personal Caddie</label>
                                                            </div>
                                                            <div class="col-md-5" style="margin-left: -30px;">
                                                                <input type="checkbox" name="" id="cad5" value="1">
                                                                <label for="cad5"></label>
                                                            </div>
                                                        </div>
														<div>
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
                                                            <!-- </table>
                                                    <table class="table cs-table bordered-div col-sm-12 st-table"> -->

                                                        </table>
                                                    </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-12 col-sm-12" style="padding-bottom: 10px;">
                                            <table class="table table-striped table-condensed" style="border: 1px solid #474747">
                                                <caption>
                                                    <h3 class="label-new" style="text-align: center;font-size:20px; background-color: #b7b7b7; border-radius: 4px; padding: 5px; text-transform: uppercase; border: 1px solid #474747;">Person Waiting</h3>
                                                </caption>
                                                <tr style="font-size: 20px;">
                                                    <th style="text-align: center;">Front 9 Hole</th>
                                                    <th style="text-align: center;">Back 9 Hole</th>
                                                </tr>
                                                <tr style="font-size: 20px">
                                                    <td style="text-align: center;" id="waitninef"></td>
                                                    <td style="text-align: center;" id="waitnineb"></td>
                                                </tr>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="col-sm-12" style="font-size: 25px; text-align: center; background-color: #ffffff; color: red; margin-bottom: -30px">
                                            Please Choose an Option Before Submit !
                                        </div>
                                        <div class="col-md-6 col-sm-6">
                                            <div class="continput">
                                                <ul class=new-ul>
                                                    <li class="new-li">
                                                        <input type="radio" name="game_type" value="1" required="">
                                                        <label class="new-label label-new" style="">Front 9</label>
                                                        <div class="bullet">
                                                            <div class="line zero"></div>
                                                            <div class="line one"></div>
                                                            <div class="line two"></div>
                                                            <div class="line three"></div>
                                                            <div class="line four"></div>
                                                            <div class="line five"></div>
                                                            <div class="line six"></div>
                                                            <div class="line seven"></div>
                                                        </div>
                                                    </li>
                                                    <li class="new-li">
                                                        <input type="radio" name="game_type" value="2" required="">
                                                        <label class="new-label label-new" style="">Back 9</label>
                                                        <div class="bullet">
                                                            <div class="line zero"></div>
                                                            <div class="line one"></div>
                                                            <div class="line two"></div>
                                                            <div class="line three"></div>
                                                            <div class="line four"></div>
                                                            <div class="line five"></div>
                                                            <div class="line six"></div>
                                                            <div class="line seven"></div>
                                                        </div>
                                                    </li>
                                                    <li class="new-li">
                                                        <input type="radio" name="game_type" value="3" required="">
                                                        <label class="new-label label-new" style="padding-bottom: 2px">18 Holes</label>
                                                        <div class="bullet">
                                                            <div class="line zero"></div>
                                                            <div class="line one"></div>
                                                            <div class="line two"></div>
                                                            <div class="line three"></div>
                                                            <div class="line four"></div>
                                                            <div class="line five"></div>
                                                            <div class="line six"></div>
                                                            <div class="line seven"></div>
                                                        </div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                    <div class="cp-booking-bottom" style="padding-right: 500px">
                        <div class="btns-holder">
                            <button class="btn btn-success" id="btnSubmitBook" type="submit">Submit</button>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </div>
</body>

</html>
<script type="text/javascript" src="lib/js/jquery.keyboard.js"></script>
<script>
    $('.keymem').keyboard({
        layout: 'custom',
        customLayout: {
            'normal': [
                '1 2 3 4 5 6 7',
                '8 9 0 S C {bksp}'

            ]
        },
        autoAccept: true,
        usePreview: false
    })
    /*.addTyping({
        showTyping: true,
        delay: 50000
    })
    .addExtender({
        layout: 'numpad',
        showing: false,
        reposition: false
    });*/
</script>
<script>
    $('.keycad').keyboard({
        layout: 'custom',
        customLayout: {
            'normal': [
                '1 2 3 4 5 6',
                '7 8 9 0 {bksp}'
            ]
        },
        autoAccept: true,
        usePreview: false
    });
    $('.bboy').keyboard({
        layout: 'custom',
        customLayout: {
            'normal': [
                '1 2 3 4 5 6',
                '7 8 9 0 {bksp}'
            ]
        },
        autoAccept: true,
        usePreview: false
    });
    /*.addTyping({
        showTyping: true,
                delay: 250
        })
    .addExtender({
        layout: 'numpad',
        showing: false,
        reposition: false
    });*/
</script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
<script>
    window.onload = function () {
        var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September',
            'October', 'November', 'December'
        ];;
        var date = new Date();

        document.getElementById('date').innerHTML = date.getDate() + ' ' + months[date.getMonth()] + ', ' + date.getFullYear();
    };
</script>
<script>
    function checkTime(i) {
        if (i < 10) {
            i = "0" + i;
        }
        return i;
    }

    function startTime() {
        var today = new Date();
        var h = today.getHours();
        var m = today.getMinutes();
        var s = today.getSeconds();
        m = checkTime(m);
        s = checkTime(s);
        document.getElementById('time').innerHTML = h + ":" + m + ":" + s;
        t = setTimeout(function () {
            startTime()
        }, 500);
    }
    startTime();
</script>
<script>
    $(document).ready(function () {
        $('.player_type').on('change', function () {
            if (this.value == '1') {
                $("#single").show();
                $("#rfid").focus();
                $("#caddie").attr("disabled", false);
                $("#rfid").attr("disabled", false);

                $("#group").hide();

            } else if (this.value == '2') {
                $("#single").hide();
                $("#group").show();
                $("#rfid_1").focus();
                $("#rfid").attr("disabled", true);
                if (("#rfid_1").val() == "") {
                    $("#rfid_1").attr("disabled", true);
                    $("#cad1").attr("disabled", true);
                } else
                    $("#rfid_1").attr("disabled", false);

                if (("rfid_2").val() == "") {
                    $("#rfid_2").attr("disabled", true);
                    $("#cad2").attr("disabled", true);
                } else
                    $("#rfid_2").attr("disabled", false);
                if (("rfid_3").val() == "") {
                    $("#rfid_3").attr("disabled", true);
                    $("#cad3").attr("disabled", true);
                } else
                    $("#rfid_3").attr("disabled", false);
                if (("rfid_4").val() == "") {
                    $("#rfid_4").attr("disabled", true);
                    $("#cad4").attr("disabled", true);
                } else
                    $("#rfid_4").attr("disabled", false);
                $("#caddie").attr("disabled", true);

                $("#rfid_1").focus();
            } else {
                $("#single").hide();
                $("#group").hide();
            }
        });
    });
</script>
<script>
    setTimeout(function () {
        $('#infoMessage').fadeOut('fast');
    }, 10000);
</script>
<script>
    setTimeout(function () {
        $('#infoMessage1').fadeOut('fast');
    }, 2000);
</script>
<script>
    $(document).ready(function () {
        $('.keyboard').keypress(function (event) {

            if (event.keyCode == 10 || event.keyCode == 13)
                event.preventDefault();
        });
    });
</script>