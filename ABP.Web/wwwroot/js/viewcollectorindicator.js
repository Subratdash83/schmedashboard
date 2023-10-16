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
//On Onchange of frequency hiding and showing the fields 
//$("#ddlFrequency").change(function () {
//    if ($(this).val() == "2") {
//        $("#quarterly").hide();
//        $("#quarterlyinput").hide();
//        $("#halfyearly").hide();
//        $("#halfyearlyinput").hide();
//        var currentYear = new Date().getFullYear();
//        $('#ddlYear').val(currentYear);
//        $('#annually').show();
//        $("#annuallyinput").show();
//        $('#monthly').show();
//        $("#monthlyinput").show();
//    }
//    else if ($(this).val() == "3") {
//        $("#halfyearly").hide();
//        $("#halfyearlyinput").hide();
//        $('#monthly').hide();
//        $("#monthlyinput").hide();
//        var currentYear = new Date().getFullYear();
//        $('#ddlYear').val(currentYear);
//        $('#annually').show();
//        $("#annuallyinput").show();
//        $('#quarterly').show();
//        $("#quarterlyinput").show();
//    }
//    else if ($(this).val() == "4") {
//        $('#monthly').hide();
//        $("#monthlyinput").hide();
//        $('#quarterly').hide();
//        $("#quarterlyinput").hide();
//        var currentYear = new Date().getFullYear();
//        $('#ddlYear').val(currentYear);
//        $('#annually').show();
//        $("#annuallyinput").show();
//        $('#halfyearly').show();
//        $("#halfyearlyinput").show();
//    }
//    else {
//        $("#halfyearly").hide();
//        $("#halfyearlyinput").hide();
//        $('#monthly').hide();
//        $("#monthlyinput").hide();
//        $('#quarterly').hide();
//        $("#quarterlyinput").hide();
//        var currentYear = new Date().getFullYear();
//        $('#ddlYear').val(currentYear);
//        $('#annually').show();
//        $("#annuallyinput").show();
//    }
//});
// Data table required for many purposes like pagination,searching printing the data showing in the grid etc.
function getDataTable() {
    $('#tblCollectorIndicator').DataTable({
        "searching": true,
        //"bStateSave": true,
        "dom": 'Bfrtip',
        "autoWidth": false,
        "buttons": [
            {
                extend: 'pageLength'

            }
        ],
        "lengthMenu": [
            [10, 25, 50, 100, 1000 - 1],
            ['10 rows', '25 rows', '50 rows', '100 rows', '1000 rows', 'Show all']
        ]

    });
}
//$('#btnSearch').click(function () {
//    var year = $('#ddlYear').val();
//    selectedText = $('#ddlYear').val();
//    fromDate = "01-Jan-" + year;
//    toDate = "31-Dec-" + year;
//    searchRecords($('#ddlIndicator').val(), $("#ddlFrequency").val(), $('#ddlYear').val(), fromDate, toDate);
//});

//$('#btnSearch').click(function () {
//    var projectInstallYear = "2020";
//    var currentYear = new Date().getFullYear();
//    if ($('#ddlFrequency').val() == "0") {
//        if ($('#ddlYear').val() == "0") {
//            fromDate = "0";
//            toDate = "0";
//        }
//        else {
//            fromDate = "01-Jan-" + projectInstallYear;
//            toDate = "31-Dec-" + currentYear;
//        }
//        searchRecords($('#ddlIndicator').val(), $('#ddlYear').val(), $("#ddlFrequency").val(),"0", fromDate, toDate);
//    }
//    else if ($('#ddlFrequency').val() == "2") {
//        var year = $('#ddlYear').val();
//        if ($('#ddlMonth').val() == 0) {
//            fromDate = "01-Jan-" + year;
//            toDate = "31-Dec-" + year;
//        }
//        else {
//            var value = $('#ddlMonth').val();
//            selectedText = $('#ddlMonth').val();
//            fromDate = "01-" + value + "-" + year;
//            if (value == "Jan") {
//                toDate = "31-" + value + "-" + year;
//            }
//            else if (value == "Feb") {
//                if (isLeapYear(value)) {
//                    toDate = "29-" + value + "-" + year;
//                }
//                else {
//                    toDate = "28-" + value + "-" + year;
//                }
//            }
//            else if (value == "Mar") {
//                toDate = "31-" + value + "-" + year;
//            }
//            else if (value == "Apr") {
//                toDate = "30-" + value + "-" + year;
//            }
//            else if (value == "May") {
//                toDate = "31-" + value + "-" + year;
//            }
//            else if (value == "Jun") {
//                toDate = "30-" + value + "-" + year;
//            }
//            else if (value == "Jul") {
//                toDate = "31-" + value + "-" + year;
//            }
//            else if (value == "Aug") {
//                toDate = "31-" + value + "-" + year;
//            }
//            else if (value == "Sep") {
//                toDate = "30-" + value + "-" + year;
//            }
//            else if (value == "Oct") {
//                toDate = "31-" + value + "-" + year;
//            }
//            else if (value == "Nov") {
//                toDate = "30-" + value + "-" + year;
//            }
//            else if (value == "Dec") {
//                toDate = "31-" + value + "-" + year;
//            }
//            else { }
//        }
//        searchRecords($('#ddlIndicator').val(), $('#ddlYear').val(), $("#ddlFrequency").val(), $('#ddlMonth').val() , fromDate, toDate);
//    }
//    else if ($('#ddlFrequency').val() == "3") {
//        var year = $('#ddlYear').val();
//        if ($('#ddlQuarter').val() == 0) {
//            fromDate = "01-Jan-" + year;
//            toDate = "31-Dec-" + year;
//        }
//        else {
//            var year = $('#ddlYear').val();
//            var value = $('#ddlQuarter').val();
//            selectedText = $('#ddlQuarter').val();
//            if (value == "Q1") {
//                fromDate = "01-Jan-" + year;
//                toDate = "31-Mar-" + year;
//            }
//            else if (value == "Q2") {
//                fromDate = "01-Apr-" + year;
//                toDate = "30-Jun-" + year;
//            }
//            else if (value == "Q3") {
//                fromDate = "01-Jul-" + year;
//                toDate = "30-Sep-" + year;
//            }
//            else if (value == "Q4") {
//                fromDate = "01-Oct-" + year;
//                toDate = "31-Dec-" + year;
//            }
//            else { }
//        }
//        searchRecords($('#ddlIndicator').val(), $('#ddlYear').val(), $("#ddlFrequency").val(), $('#ddlQuarter').val(), fromDate, toDate);
//    }
//    else if ($('#ddlFrequency').val() == "4") {
//        var year = $('#ddlYear').val();
//        if ($('#ddlHalfyear').val() == 0) {
//            fromDate = "01-Jan-" + year;
//            toDate = "31-Dec-" + year;
//        }
//        else {
//            var value = $('#ddlHalfyear').val();
//            selectedText = $('#ddlHalfyear').val();
//            if (value == "H1") {
//                fromDate = "01-Jan-" + year;
//                toDate = "30-Jun-" + year;
//            }
//            else if (value == "H2") {
//                fromDate = "01-Jul-" + year;
//                toDate = "31-Dec-" + year;
//            }
//            else { }
//        }
//        searchRecords($('#ddlIndicator').val(), $('#ddlYear').val(), $("#ddlFrequency").val(), $('#ddlHalfyear').val(), fromDate, toDate);
//    }
//    else if ($('#ddlFrequency').val() == "5") {
//        var year = $('#ddlYear').val();
//        selectedText = $('#ddlYear').val();
//        fromDate = "01-Jan-" + year;
//        toDate = "31-Dec-" + year;
//        searchRecords($('#ddlIndicator').val(), $('#ddlYear').val(),$("#ddlFrequency").val(), $('#ddlYear').val(), fromDate, toDate);
//    }
//    else { }
//});
function searchRecords(indicatorId, year, frequencyId, frequencyValue, fromDate, toDate) {
    
    $('#table').hide();
    $.ajax({
        url: "/CollectorData/SearchCollectorIndicator?IndicatorId=" + indicatorId + "&Year=" + year + "&FrequencyId=" + frequencyId + "&FrequencyValue=" + frequencyValue + "&FromDate=" + fromDate + "&ToDate=" + toDate + "&Glink = " + decodeURI(getUrlVars()["Glink"]) + "& Plink=" + decodeURI(getUrlVars()["Plink"]),
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (res) {
            if (res.length != 0) {
                $('#table').show();
                $('#tbody').empty();
                $('.norecord').hide();
                var html = '';
                var count = 1;
                $.each(res, function (key, item) {
                    html += '<tr>';
                    html += '<td>' + count + '</td>';
                    html += '<td>' + item.YEAR + '</td>';
                    html += '<td>' + item.FREQUENCYVALUE + '</td>';
                    html += '<td>' + item.INDICATORNAME + '</td>';
                    html += '<td>' + item.INDICATORVALUE + '</td>';
                    html += '</tr>';
                    count++;
                });
                $('#tblCollectorIndicator').DataTable().destroy();
                $('#tbody').html(html);
                getDataTable();
            }
            else {
                $('#tbody').empty();
                $('#table').hide();
                $('.norecord').show();
            }
        }
    });
}