//================Map and Unmap Block================================
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
//JavaScript Method to get week By Year
Date.prototype.getWeek = function () {
    var date = new Date(this.getTime());
    date.setHours(0, 0, 0, 0);
    // Thursday in current week decides the year.
    date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);
    // January 4 is always in week 1.
    var week1 = new Date(date.getFullYear(), 0, 4);
    // Adjust to Thursday in week 1 and count number of weeks from date to week1.
    return 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000 - 3 + (week1.getDay() + 6) % 7) / 7);
}
//hiding all before unchange of sector
$("#ddlSector").change(function () {
    $("#frequency").show();
    $('#table').hide();
    $('#button').hide();
    $("#weekly").hide();
    $("#monthly").hide();
    $("#quarterly").hide();
    $("#halfyearly").hide();
    $("#annually").hide();
});
//On Onchange of frequency hiding and showing the fields 
$("#ddlFrequency").change(function () {
    bindDataPoint($("#ddlSector").val(), $(this).val());
    var currentDate = new Date();
    //if ($(this).val() == "1") {
    //    $("#monthly").hide();
    //    $("#quarterly").hide();
    //    $("#halfyearly").hide();
    //    var currentYear = new Date().getFullYear();
    //    $('#ddlYear').val(currentYear);
    //    $('#annually').show();
    //    $('#weekly').show();
    //}
    //else
    if ($(this).val() == "2") {
        $("#quarterly").hide();
        $("#halfyearly").hide();
        $('#weekly').hide();
        var currentYear = new Date().getFullYear();
        var currentDay = new Date().getDay();
        var startTargetDate = new Date("01-Jan-" + currentYear);
        var targetDate = new Date("15-Jan-" + currentYear);
        var prevYear = currentYear - 1;
        if (currentDate <= targetDate && currentDate >= startTargetDate) {
            $('#ddlYear').val(prevYear);
        }
        else {
            $('#ddlYear').val(currentYear);
        }
        $('#annually').show();
        var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        var prev = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
        var toBeInsertMonth = months[prev.getMonth()];
        if (currentDay <= 15) {
            $('#ddlMonth').val(toBeInsertMonth);
        }
        $('#monthly').show();
    }
    else if ($(this).val() == "3") {
        $("#halfyearly").hide();
        $('#weekly').hide();
        $('#monthly').hide();
        var currentYear = new Date().getFullYear();
        var startTargetDate = new Date("01-Jan-" + currentYear);
        var targetDate = new Date("15-Jan-" + currentYear);
        var prevYear = currentYear - 1;
        if (currentDate <= targetDate && currentDate >= startTargetDate) {
            $('#ddlYear').val(prevYear);
        }
        else {
            $('#ddlYear').val(currentYear);
        }
        $('#annually').show();
        var firstQuarterTargetDateStart = new Date("01-Apr-" + currentYear);
        var firstQuarterTargetDate = new Date("15-Apr-" + currentYear);
        var secondQuarterTargetDateStart = new Date("01-Jul-" + currentYear);
        var secondQuarterTargetDate = new Date("15-Jul-" + currentYear);
        var thirdQuarterTargetDateStart = new Date("01-Oct-" + currentYear);
        var thirdQuarterTargetDate = new Date("15-Oct-" + currentYear);
        var fourthQuarterTargetDateStart = new Date("01-Jan-" + currentYear);
        var fourthQuarterTargetDate = new Date("15-Jan-" + currentYear);
        if (currentDate <= firstQuarterTargetDate && currentDate >= firstQuarterTargetDateStart) {
            $('#ddlQuarter').val("Q1");
        }
        else if (currentDate <= secondQuarterTargetDate && currentDate >= secondQuarterTargetDateStart) {
            $('#ddlQuarter').val("Q2");
        }
        else if (currentDate <= thirdQuarterTargetDate && currentDate >= thirdQuarterTargetDateStart) {
            $('#ddlQuarter').val("Q3");
        }
        else if (currentDate <= fourthQuarterTargetDate && currentDate >= fourthQuarterTargetDateStart) {
            $('#ddlQuarter').val("Q4");
        }
        else { }
        $('#quarterly').show();
    }
    else if ($(this).val() == "4") {
        $('#weekly').hide();
        $('#monthly').hide();
        $('#quarterly').hide();
        var currentYear = new Date().getFullYear();
        var startTargetDate = new Date("01-Jan-" + currentYear);
        var targetDate = new Date("15-Jan-" + currentYear);
        var prevYear = currentYear - 1;
        if (currentDate <= targetDate && currentDate >= startTargetDate) {
            $('#ddlYear').val(prevYear);
        }
        else {
            $('#ddlYear').val(currentYear);
        }
        $('#annually').show();
        var firstHalfTargetDateStart = new Date("01-Jul-" + currentYear);
        var firstHalfTargetDate = new Date("15-Jul-" + currentYear);
        var secondHalfTargetDateStart = new Date("01-Jan-" + currentYear);
        var secondHalfTargetDate = new Date("15-Jan-" + currentYear);
        if (currentDate <= firstHalfTargetDate && currentDate >= firstHalfTargetDateStart) {
            $('#ddlHalfyear').val("H1");
        }
        else if (currentDate <= secondHalfTargetDate && currentDate >= secondHalfTargetDateStart) {
            $('#ddlHalfyear').val("H2");
        }
        else { }
        $('#halfyearly').show();
    }
    else {
        $("#halfyearly").hide();
        $('#weekly').hide();
        $('#monthly').hide();
        $('#quarterly').hide();
        var currentYear = new Date().getFullYear();
        var startTargetDate = new Date("01-Jan-" + currentYear);
        var targetDate = new Date("15-Jan-" + currentYear);
        var prevYear = currentYear - 1;
        if (currentDate <= targetDate && currentDate >= startTargetDate) {
            $('#ddlYear').val(prevYear);
        }
        else { }
        $('#annually').show();
    }
});

//Binding datapoints in the editable table to insert the value

function bindDataPoint(sectorId, FREQUENCYID) {
    $.ajax({
        url: "/BDOData/GetDataPointBySectorAndFrequency?SECTORID=" + sectorId + "&FREQUENCYID=" + FREQUENCYID + "&Glink = " + decodeURI(getUrlVars()["Glink"]) + "& Plink=" + decodeURI(getUrlVars()["Plink"]),
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (res) {
            if (res.length != 0) {
                $('#table').show();
                $('#button').show();
                var html = '';
                var count = 1;
                $.each(res, function (key, item) {
                    html += '<tr>';
                    html += '<td>' + count + '</td>';
                    html += '<td style="display:none;">' + item.DATAPOINTID + '</td>'
                    html += '<td>' + item.DATAPOINTNAME + '</td>';
                    html += '<td><input type="text" value=' + item.DATAPOINTVALUE + ' id="demo-text-input" class="form-control" onkeypress="return isNumber(event)"></td>';
                    html += '<td style="display:none;">' + item.INDICATORID + '</td>'
                    html += '</tr>';
                    count++;
                });
                $('#tbody').html(html);
            }
            else {
                $('#weekly').hide();
                $('#monthly').hide();
                $('#quarterly').hide();
                $('#annually').hide();
                $('#halfyearly').hide();
                $('#table').hide();
                $('#button').hide();
            }
        }
    });
}
$('#btnSubmit').click(function () {
    //if ($('#ddlFrequency').val() == "1") {

    //    if ($('#ddlYear').val() == "0") {
    //        bootbox.alert("Please Select a Year!", function () {
    //        });
    //    }
    //    else if ($('#ddlWeek').val() == "0") {
    //        bootbox.alert("Please Select a Week!", function () {
    //        });
    //    }
    //    else {
    //        var year = $('#ddlYear').val();
    //        var value = $('#ddlWeek').val();
    //        selectedText = $('#ddlWeek').val();
    //        var fromDate = getFromDateOfWeek(value, year).toShortFormat();
    //        var toDate = getToDateOfWeek(value, year).toShortFormat();
    //        fromDate = fromDate;
    //        toDate = toDate;
    //        submitRecordsFromTable(fromDate, toDate);
    //    }
    //}
    //else
    if ($('#ddlFrequency').val() == "2") {
        if ($('#ddlYear').val() == "0") {
            bootbox.alert("Please Select a Year!", function () {
            });
        }
        else if ($('#ddlMonth').val() == "0") {
            bootbox.alert("Please Select a Month!", function () {
            });
        }
        else {
            var year = $('#ddlYear').val();
            var value = $('#ddlMonth').val();
            selectedText = $('#ddlMonth').val();
            fromDate = "01-" + value + "-" + year;
            if (value == "Jan") {
                toDate = "31-" + value + "-" + year;
            }
            else if (value == "Feb") {
                if (isLeapYear(value)) {
                    toDate = "29-" + value + "-" + year;
                }
                else {
                    toDate = "28-" + value + "-" + year;
                }
            }
            else if (value == "Mar") {
                toDate = "31-" + value + "-" + year;
            }
            else if (value == "Apr") {
                toDate = "30-" + value + "-" + year;
            }
            else if (value == "May") {
                toDate = "31-" + value + "-" + year;
            }
            else if (value == "Jun") {
                toDate = "30-" + value + "-" + year;
            }
            else if (value == "Jul") {
                toDate = "31-" + value + "-" + year;
            }
            else if (value == "Aug") {
                toDate = "31-" + value + "-" + year;
            }
            else if (value == "Sep") {
                toDate = "30-" + value + "-" + year;
            }
            else if (value == "Oct") {
                toDate = "31-" + value + "-" + year;
            }
            else if (value == "Nov") {
                toDate = "30-" + value + "-" + year;
            }
            else if (value == "Dec") {
                toDate = "31-" + value + "-" + year;
            }
            else { }
            var currentYear = new Date().getFullYear();
            var currentDate = new Date();
            var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var prev = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
            toBeInsertMonth = months[prev.getMonth()];
            startInsertDate = new Date("01-" + months[currentDate.getMonth()] + "-" + currentYear);
            endInsertDate = new Date("15-" + months[currentDate.getMonth()] + "-" + currentYear);
            preViousYear = currentYear - 1;
            //if (value == 'Dec') {
            //    if (year == preViousYear && currentDate >= startInsertDate && currentDate <= endInsertDate) {
            //        submitRecordsFromTable(fromDate, toDate);
            //    }
            //    else {
            //        bootbox.alert("You can only enter the last month's data and before 15th of the current month!");
            //    }
            //}
            //else if (year == currentYear && value == toBeInsertMonth && currentDate >= startInsertDate && currentDate <= endInsertDate) {
            submitRecordsFromTable(fromDate, toDate);
            //}
            //else {
            //    bootbox.alert("You can only enter the last month's data and before 15th of the current month!");
            //}
        }
    }
    else if ($('#ddlFrequency').val() == "3") {
        if ($('#ddlYear').val() == "0") {
            bootbox.alert("Please Select a Year!", function () {
            });
        }
        else if ($('#ddlQuarter').val() == "0") {
            bootbox.alert("Please Select a Quarter!", function () {
            });
        }
        else {
            var year = $('#ddlYear').val();
            var value = $('#ddlQuarter').val();
            selectedText = $('#ddlQuarter').val();
            if (value == "Q1") {
                fromDate = "01-Jan-" + year;
                toDate = "31-Mar-" + year;
            }
            else if (value == "Q2") {
                fromDate = "01-Apr-" + year;
                toDate = "30-Jun-" + year;
            }
            else if (value == "Q3") {
                fromDate = "01-Jul-" + year;
                toDate = "30-Sep-" + year;
            }
            else if (value == "Q4") {
                fromDate = "01-Oct-" + year;
                toDate = "31-Dec-" + year;
            }
            else { }
            var currentYear = new Date().getFullYear();
            var currentDate = new Date();
            var startDateOfFirstQuarter = new Date("01-Jan-" + currentYear);
            var endDateOfFirstQuarter = new Date("31-Mar-" + currentYear);
            var startDateOfSecondQuarter = new Date("01-Apr-" + currentYear);
            var endDateOfSecondQuarter = new Date("30-Jun-" + currentYear);
            var startDateOfThirdQuarter = new Date("01-Jul-" + currentYear);
            var endDateOfThidQuarter = new Date("30-Sep-" + currentYear);
            var startDateOfFourthQuarter = new Date("01-Oct-" + currentYear);
            var endDateOfFourthQuarter = new Date("31-Dec-" + currentYear);
            if (currentDate > startDateOfFirstQuarter && currentDate < endDateOfFirstQuarter) {
                currentQuarter = 'Q1';
                toBeInsertQuarter = 'Q4';
                startInsertDate = new Date("01-Jan-" + currentYear);
                endInsertDate = new Date("15-Jan-" + currentYear);
            }
            else if (currentDate > startDateOfSecondQuarter && currentDate < endDateOfSecondQuarter) {
                currentQuarter = 'Q2';
                toBeInsertQuarter = 'Q1';
                startInsertDate = new Date("01-Apr-" + currentYear);
                endInsertDate = new Date("15-Apr-" + currentYear);
            }
            else if (currentDate > startDateOfThirdQuarter && currentDate < endDateOfThidQuarter) {
                currentQuarter = 'Q3';
                toBeInsertQuarter = 'Q2';
                startInsertDate = new Date("01-Jul-" + currentYear);
                endInsertDate = new Date("15-Jul-" + currentYear);
            }
            else if (currentDate > startDateOfFourthQuarter && currentDate < endDateOfFourthQuarter) {
                currentQuarter = 'Q4';
                toBeInsertQuarter = 'Q3';
                startInsertDate = new Date("01-Oct-" + currentYear);
                endInsertDate = new Date("15-Oct-" + currentYear);
            } else { }
            preViousYear = currentYear - 1;
            //if (value == 'Q4') {
            //    if (year == preViousYear && currentDate >= startInsertDate && currentDate <= endInsertDate) {
            //        submitRecordsFromTable(fromDate, toDate);
            //    }
            //    else {
            //        bootbox.alert("You can only enter the last qurter's data and before 15th of the next month of that quarter!");
            //    }
            //}
            //else if (year == currentYear && value == toBeInsertQuarter && currentDate >= startInsertDate && currentDate <= endInsertDate) {
            submitRecordsFromTable(fromDate, toDate);
            //}
            //else {
            //    bootbox.alert("You can only enter the last qurter's data and before 15th of the next month of that quarter!");
            //}
        }
    }
    else if ($('#ddlFrequency').val() == "4") {
        if ($('#ddlYear').val() == "0") {
            bootbox.alert("Please Select a Year!", function () {
            });
        }
        else if ($('#ddlHalfyear').val() == "0") {
            bootbox.alert("Please Select a Half of year!", function () {
            });
        }
        else {
            var year = $('#ddlYear').val();
            var value = $('#ddlHalfyear').val();
            selectedText = $('#ddlHalfyear').val();
            if (value == "H1") {
                fromDate = "01-Jan-" + year;
                toDate = "30-Jun-" + year;
            }
            else if (value == "H2") {
                fromDate = "01-Jul-" + year;
                toDate = "31-Dec-" + year;
            }
            else { }
            var currentYear = new Date().getFullYear();
            var currentDate = new Date();
            var startDateOfFirstHalf = new Date("01-Jan-" + currentYear);
            var endDateOfFirstHalf = new Date("30-Jun-" + currentYear);
            var startDateOfSecondHalf = new Date("01-Jul-" + currentYear);
            var endDateOfSecondHalf = new Date("30-Dec-" + currentYear);
            if (currentDate >= startDateOfFirstHalf && currentDate <= endDateOfFirstHalf) {
                toBeInsertHalf = 'H2';
                startInsertDate = new Date("01-Jan-" + currentYear);
                endInsertDate = new Date("15-Jan-" + currentYear);
            }
            else if (currentDate >= startDateOfSecondHalf && currentDate <= endDateOfSecondHalf) {
                toBeInsertHalf = 'H1';
                startInsertDate = new Date("01-Jul-" + currentYear);
                endInsertDate = new Date("15-Jul-" + currentYear);
            }
            else { }
            preViousYear = currentYear - 1;
            //if (value == 'H2') {
            //    if (year == preViousYear && currentDate >= startInsertDate && currentDate <= endInsertDate) {
            //        submitRecordsFromTable(fromDate, toDate);
            //    }
            //    else {
            //        bootbox.alert("You can only enter the last half years's data and before 15th of the next month of that half year!");
            //    }
            //}
            //else if (year == currentYear && value == toBeInsertHalf && currentDate >= startInsertDate && currentDate <= endInsertDate) {
            submitRecordsFromTable(fromDate, toDate);
            //}
            //else {
            //    bootbox.alert("You can only enter the last half years's data and before 15th of the next month of that half year!");
            //}
        }
    }
    else if ($('#ddlFrequency').val() == "5") {
        if ($('#ddlYear').val() == "0") {
            bootbox.alert("Please Select a Year!", function () {

            });
        }
        else {
            var year = $('#ddlYear').val();
            selectedText = $('#ddlYear').val();
            fromDate = "01-Jan-" + year;
            toDate = "31-Dec-" + year;
            var currentYear = new Date().getFullYear();
            lastInsertDateTobeDisplay = "15-Jan-" + currentYear;
            var lastInsertDate = new Date("15-Jan-" + currentYear);
            var toBeInsertYear = currentYear - 1;
            var currentDate = new Date();
            //if (year != toBeInsertYear && currentDate > lastInsertDate) {
            //    bootbox.alert("You can only enter the last year's data and before " + lastInsertDateTobeDisplay + "!");
            //}
            //else {
            submitRecordsFromTable(fromDate, toDate);
            //}
        }
    }
    else { }
});
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function submitRecordsFromTable(fromDate, toDate) {

    //var data = Array();
    //$("table tr").each(function (i, v) {
    //    data[i] = Array();
    //    $(this).children('td').each(function (ii, vv) {
    //        data[i][ii] = $(this).text();
    //    });
    //})
    //getting all required data information as per uploaded excel from the table rendered after uploading of the excel sheet
    var tbl = document.getElementById("tbody");
    if (tbl.rows.length != 0) {

        var Items = []; var count = 0;
        for (var i = 0; i < tbl.rows.length; i++) {
            var item1 = {};
            if (tbl.rows[i].children[3].getElementsByTagName('input').length != 0) {
                if (tbl.rows[i].children[3].children[0].value == "") {
                    count++;
                }
                item1.SECTORID = $("#ddlSector").val();
                item1.DATAPOINTID = parseInt(tbl.rows[i].children[1].textContent);
                item1.DATAPOINTVALUE = parseFloat(tbl.rows[i].children[3].children[0].value);
                item1.FROMDATE = fromDate;
                item1.TODATE = toDate;
                item1.FREQUENCYVALUE = selectedText;
                item1.INDICATORID = parseInt(tbl.rows[i].children[4].textContent);
                Items.push(item1);
            }
            else { }
        }
        //checking wheither for all the villages the source village have been selected or not
        if (count > 0) {
            bootbox.alert("Please enter the values against the data points", function () {
            });
            return false;
        }
        else {
            bootbox.confirm({
                size: "medium",
                message: "Are you sure , you want to save as draft?",
                callback: function (result) {
                    if (result == true) {
                        $.ajax({
                            url: "/BDOData/AddBDOData",
                            data: JSON.stringify(Items),
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            type: "POST",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            success: function (result) {
                                bootbox.alert(result, function () {
                                    window.location.href = "/BDOData/AddBDOData?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                                });
                            },
                            error: function (result) {
                                bootbox.alert(result);
                            }
                        });
                    }
                }
            });
        }
    }
    else {
        bootbox.alert("Data Points are unavailable in this sector and frequency", function () {
            window.location.href = "/BDOData/AddBDOData?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
        });
    }
}
function getFromDateOfWeek(w, y) {
    var d = (1 + (w - 1) * 7); // 1st of January + 7 days for each week

    return new Date(y, 0, d);
}
function getToDateOfWeek(w, y) {
    var d = (1 + w * 7); // 1st of January + 7 days for each week

    return new Date(y, 0, d);
}
function isLeapYear(year) {
    return ((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0);
}