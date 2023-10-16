//================common.js is common for all the pages. Common function will be written here===================
$(document).ready(function () {
    $('.otp-box').hide();
});
//to get the value from the query string
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}
// To convert any date format to dd-MMM-yyyy
Date.prototype.toShortFormat = function () {

    let monthNames = ["Jan", "Feb", "Mar", "Apr",
        "May", "Jun", "Jul", "Aug",
        "Sep", "Oct", "Nov", "Dec"];

    let day = this.getDate();

    let monthIndex = this.getMonth();
    let monthName = monthNames[monthIndex];

    let year = this.getFullYear();
    if (day.toString().length == 1) {
        return `${"0" + day}-${monthName}-${year}`;
    }
    else {
        return `${day}-${monthName}-${year}`;
    }
}
function sendOTP() {
    if (validate()) {
        
        $.ajax({
            type: "GET",
            url: "/BDOData/SendOTPToLoggedInUser",
            success: function (result) {
                if (result != '0') {
                    $('#hdnotp').val(result);
                    $('.otp-box').toggle();
                    $(this).slideUp();
                    $('#btnConfirm').hide();
                }
                else {
                    bootbox.alert("Couldn't send the OTP due to server side error!", function () { });
                }
            },
            error: function (e, ts, et) {
                alert(ts);
            }
        });
    }
}
function isValidOTP() {
    if ($("#txtotp").val() == "") {
        bootbox.alert("Please Enter OTP!");
        $("#txtotp").focus();
        return false;
    }
    else if ($("#txtotp").val() != $("#hdnotp").val()) {
        bootbox.alert("Please enter a valid 4 digit OTP!");
        $("#txtotp").focus();
        return false;
    }
    else {
        return true;
    }
}
function reSendOTP() {
    var count = 0;
    var a = parseInt($("#checkotpcount").val());
    count++;
    $("#checkotpcount").val(count + a);
    
    if (a == 1) {
        $("#otpcount").hide();
        $.ajax({
            type: "GET",
            url: "/BDOData/SendOTPToLoggedInUser",
            success: function (result) {
                if (result != '0') {
                    $('#hdnotp').val(result);
                    bootbox.alert("OTP Sent Successfully One More Time!");
                }
                else {
                    bootbox.alert("Couldn't send the OTP due to server side error!", function () { });
                }
            },
            error: function (e, ts, et) {
                alert(ts);
            }
        });



    }
    else if (validate()) {
        $.ajax({
            type: "GET",
            url: "/BDOData/SendOTPToLoggedInUser",
            success: function (result) {
                if (result != '0') {
                    $('#hdnotp').val(result);
                    bootbox.alert("OTP Sent Successfully One More Time!");
                    
                }
                else {
                    bootbox.alert("Couldn't send the OTP due to server side error!", function () { });
                }
            },
            error: function (e, ts, et) {
                alert(ts);
            }
        });
    }

    else {

        $("#otpcount").hide();


    }
}
