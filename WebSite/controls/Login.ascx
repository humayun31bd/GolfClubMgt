<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Login.ascx.cs" Inherits="Controls_Login" %>

<link href="../Scripts/bootstrap332.min.css" rel="stylesheet" />
<%--<link href="../LoginCSS.css" rel="stylesheet" />--%>
<style type="text/css">
    @import url("http://fonts.googleapis.com/css?family=Open+Sans:300italic,400italic,700italic,400,300,700");

    body {
        font-family: Open Sans;
        font-size: 14px;
        line-height: 1.42857;
        background: #333333;
        height: Auto;
        padding: 0;
        margin: 0;
    }

    .container {
        text-align: center !important;
    }

    .container-login {
        min-height: 0;
        width: 90%;
        color: #333333;
        margin-top: 0px;
        padding: 0;
    }

    .center-block {
        display: block;
        margin-left: auto;
        margin-right: auto;
    }

    .container-login > section {
        margin-left: 0;
        margin-right: 0;
        padding-bottom: 10px;
    }

    #top-bar {
        display: inherit;
    }

    .nav-tabs.nav-justified {
        border-bottom: 0 none;
        width: 100%;
    }

        .nav-tabs.nav-justified > li {
            display: table-cell;
            width: 1%;
            float: none;
        }

    .container-login .nav-tabs.nav-justified > li > a,
    .container-login .nav-tabs.nav-justified > li > a:hover,
    .container-login .nav-tabs.nav-justified > li > a:focus {
        background: #ea533f;
        border: medium none;
        color: #ffffff;
        margin-bottom: 0;
        margin-right: 0;
        border-radius: 0;
    }

    .container-login .nav-tabs.nav-justified > .active > a,
    .container-login .nav-tabs.nav-justified > .active > a:hover,
    .container-login .nav-tabs.nav-justified > .active > a:focus {
        background: #ffffff;
        color: #333333;
    }

    .container-login .nav-tabs.nav-justified > li > a:hover,
    .container-login .nav-tabs.nav-justified > li > a:focus {
        background: #de2f18;
    }

    .tabs-login {
        background: #ffffff;
        border: medium none;
        margin-top: -1px;
        padding: 10px 30px;
    }

    .container-login h2 {
        color: #ea533f;
    }

    .form-control {
        background-color: #ffffff;
        /*background-image: url('../images/IMG-loginBack.jpg');*/
        background-image: none;
        border: 1px solid #999999;
        border-radius: 0;
        box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset;
        color: #333333;
        display: block;
        font-size: 14px;
        height: 34px;
        line-height: 1.42857;
        padding: 6px 12px;
        transition: border-color 0.15s ease-in-out 0s, box-shadow 0.15s ease-in-out 0s;
        width: 100%;
    }

    .container-login .checkbox {
        margin-top: -15px;
    }

    .panel-heading {
        text-align: center !important;
    }

    .panel-title {
        padding-right: 10px !important;
    }

    .container-login button {
        background-color: #478d41;
        border-color: #478d41;
        color: #ffffff;
        border-radius: 0;
        font-size: 18px;
        line-height: 1.33;
        padding: 10px 16px;
        width: 100%;
    }

        .container-login button:hover,
        .container-login button:focus {
            background: #478d41;
            border-color: #478d41;
        }
</style>
<div data-app-role="page" data-content-framework="bootstrap">        
        <article class="container-login center-block">
            <div class="tab-content tabs-login col-lg-12 col-md-12 col-sm-12 cols-xs-12">                
                    <div id="login-access" class="tab-pane fade active in">
                        <div class="col-md-12">
                            <div class="modal-dialog" style="margin-bottom: 0">
                                <div class="modal-content">
                                    <div class="row">
                                        <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4"></div>
                                        <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4  text-center">
                                            <img src="../images/logo-inverse.png" alt="Golf Club" width="100px;" height="auto;" />
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4"></div>
                                    </div>
                                    <div class="panel-heading">
                                        <h3><i class="glyphicon glyphicon-log-in panel-title"></i>Login </h3>
                                    </div>
                                    <div class="panel-body">
                                        <form role="form">
                                            <fieldset>
                                                <div class="form-group">
                                                    <input class="form-control" id="login-user-name" type="text" name="login-user-name"
                                                        placeholder="User Name" tabindex="1" autofocus="">
                                                </div>
                                                <div class="form-group">
                                                    <input class="form-control" type="password" name="login-password" id="login-password"
                                                        placeholder="Password" value="" tabindex="2">
                                                </div>
                                                <div class="checkbox">
                                                    <label>
                                                        <input name="remember" type="checkbox" value="Remember Me">Remember Me                                   
                                                    </label>
                                                </div>
                                                <button type="submit" name="login-button" id="login-button" tabindex="5" class="btn btn-lg btn-primary">Submit</button>
                                            </fieldset>
                                        </form>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                
            </div>
        </article>
</div>


<script type="text/javascript">
    (function () {
        var resources = Web.MembershipResources.Messages;

        function performLogin(username, password) {

            var userNameElem = $('#login-user-name'),
                passwordElem = $('#login-password');

            if (!username)
                username = userNameElem.val();
            if (!password)
                password = passwordElem.val();

            if (!username)
                $app.alert(resources.BlankUserName, function () {
                    userNameElem.focus();
                });
            else if (!password)
                $app.alert(resources.BlankPassword, function () {
                    passwordElem.focus();
                });
            else
                $app.login(username, password, true,
                    function () {
                        SetCookie("UserSettings", username, 1);
                        setTimeout(function () {
                            $app._navigated = true;
                            var returnUrl = window.location.href.match(/\?ReturnUrl=(.+)$/);
                            window.location.replace(returnUrl && decodeURIComponent(returnUrl[1]) || __baseUrl);
                        });
                    },
                    function () {
                        $app.alert(resources.InvalidUserNameAndPassword, function () {
                            userNameElem.focus();
                        });
                    });
            return false;
        }

        function SetCookie(cookieName, cookieValue, nDays) {
            var today = new Date();
            var expire = new Date();
            if (nDays == null || nDays == 0) nDays = 1;
            expire.setTime(today.getTime() + 3600000 * 24 * nDays);
            document.cookie = cookieName + "=" + escape(cookieValue)
                            + ";expires=" + expire.toGMTString();
        }

        $(document)
            .on('click', '#login-button', function () {
                performLogin();
            })
            .on('click', '#admin-login', function () {
                performLogin('admin', 'admin123%');
            })
            .on('click', '#user-login', function () {
                performLogin('user', 'user123%');
            })
            .on('keydown', 'input', function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                    performLogin();
                    return false;
                }
            });
    })();
</script>

