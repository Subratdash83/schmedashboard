//=============Add And Update Indicator===================
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
$('#btnSubmit').click(function () {
    if (validate()) {
        var indicatorId = getUrlVars()["ID"];
        if (indicatorId != undefined) {
            msg = "Are you sure ,you want to Update?";
        }
        else {
            msg = "Are you sure ,you want to Submit?";
        }
        bootbox.confirm({
            size: "medium",
            message: msg,
            callback: function (result) {
                /*var radioValue = $("input[name='IndType']:checked").val();*/
                if (result === true) {

                    var fileData = new FormData();  //Creating FormData Object to fill the values entered by the  user for insert into the database
                    fileData.append("INDICATORID", $('#hdnIndicatorId').val());
                    fileData.append("SECTORID", $('#ddlSector').val());
                    fileData.append("INDICATORNAME", $('#txtIndicatorName').val());
                    fileData.append("INDICATORTYPE", $('#ddlIndicatorType').val());
                    fileData.append("FREQUENCYID", $('#ddlFrequency').val());
                    fileData.append("DESCRIPTION", $('#txtDescription').val());
                    fileData.append("INDICATORDATE", $('#txtDate').val());
                    fileData.append("TARGETVALUE", $('#txttargetvalue').val());
                    fileData.append("UNIT", $('#ddlunit').val());
                    fileData.append("ID", $('#hdnDeptId').val());
                    fileData.append("DEPTID", $('#ddlDepartment').val());
                    addIndicatorData(fileData);
                }
            }
        });
    }
});
//Validations for the fields in the form
function validate() {

    //var d = new Date();
    //var currentDate = d.toShortFormat();

    var radioValue = $("input[name='IndType']:checked").val();

    //var inputDate = $('#txtDate').val();
    //if ($('#ddlDepartment').val() == "0") {
    //    bootbox.alert("Please select department name!");
    //    return false;
    //    $('#ddlDepartment').focus();
    //}

    if ($('#ddlSector').val() == "0") {
        bootbox.alert("Please select sector!");
        $('#ddlSector').focus();
        return false;
    }
    else if ($('#txtIndicatorName').val() == "") {
        bootbox.alert("Please enter indicator name!");
        $('#txtIndicatorName').focus();
        return false;
    }
    else if ($('#ddlIndicatorType').val() == "") {
        bootbox.alert("Please select Indicator Type!");
        $('#ddlIndicatorType').focus();
        return false;
    }
    else if ($('#ddlFrequency').val() == "0") {
        bootbox.alert("Please select Frequency!");
        $('#ddlFrequency').focus();
        return false;
    }
    else if ($('#ddlIndicatorType').val() == '0' && parseFloat($("#txttargetvalue").val()) > parseFloat(0)) {
        bootbox.alert("Target value for negative indicator can't be greater than zero!");
        $('#txttargetvalue').focus();
        return false;
    }
    else if ($('#ddlIndicatorType').val() == '1' && parseFloat($("#txttargetvalue").val()) < parseFloat(100)) {
        bootbox.alert("Target value for positive indicator can't be less than hundred!");
        $('#txttargetvalue').focus();
        return false;
    }
    else if ($('#ddlunit').val() == "") {
        bootbox.alert("Please select Unit !");
        $('#ddlunit').focus();
        return false;
    }
    //else if ($('#txtDescription').val() == "") {
    //    bootbox.alert("Please enter description!");
    //    return false;
    //    $('#txtDescription').focus();
    //}
    //else if (radioValue == undefined) {
    //    bootbox.alert("Please select Type!");
    //    return false;
    //    //$('#ddlDepartment').focus();
    //}
    else if ($('#txtDate').val() == "") {
        bootbox.alert("Please enter Effective date!");
        return false;
        $('#txtDate').focus();
    }

    //else if (currentDate > $('#txtDate').val()) {
    //    bootbox.alert("Please enter date, greater than current date!");
    //    return false;
    //    $('#txtDate').focus();
    //}
    else {
        $("#btnCancel").hide();
        return true;
    }

}
//Post Call to the controller for data insertion 
function addIndicatorData(Data) {
    $.ajax({
        type: "POST",
        url: "/Indicator/AddIndicator",//Controller and Method(/Controller/Method)
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: Data,
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        success: function (res) {
            var INDICATORID = getUrlVars()["ID"];
            if (INDICATORID != undefined) {
                bootbox.alert(res, function () {  //Alert showing through bootbox for a better look and fill
                    window.location.href = "/Indicator/ViewIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                });
            }
            else {
                bootbox.alert(res, function () {  //Alert showing through bootbox for a better look and fill
                    window.location.href = "/Indicator/AddIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                });
            }
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
}
//Sending the data to the add page for updating purpose

function bindIndicatorById(indicatorId) {

    $.ajax({
        url: "/Indicator/IndicatorGateById?IndicatorId=" + indicatorId,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=UTF-8",
        success: function (data) {

            //Filling the values in appropriate field for updating purpose

            $('#hdnIndicatorId').val(data[0].INDICATORID);
            $('#hdnDeptId').val(data[0].ID);
            $('#ddlSector').val(data[0].SECTORID);
            $('#txtIndicatorName').val(data[0].INDICATORNAME);
            $('#ddlIndicatorType').val(data[0].INDICATORTYPE);
            $('#txtDescription').html(data[0].DESCRIPTION);
            $('#ddlFrequency').val(data[0].FREQUENCYID);
            $('#ddlDepartment').val(data[0].DEPTID);
            $('#ddlunit').val(data[0].UNIT);
            //var d = new Date(data[0].SDATE);
            //$('#txtDate').val(d.toShortFormat());
            //IndType
            //if (data[0].IndType == 1) {

            //$('#type1').attr('checked', true);
            //}
            //else {

            //$('#type2').attr('checked', true);
            //}

            $('#txtDate').val(data[0].INDICATORDATE);
            $('#txttargetvalue').val(data[0].TARGETVALUE);
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            bootbox.alert("Error : " + jsonValue);
        }
    });
}
//Resetting all the fields
$('#btnCancel').click(function () {
    var indicatorId = getUrlVars()["ID"];
    if (indicatorId != undefined) {
        location.href = "/Indicator/ViewIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    }
    else {
        $('#ddlDepartment').val("0")
        $('#hdnIndicatorId').val("0");
        $('#ddlSector').val("0");
        $('#txtIndicatorName').val("");
        $('#ddlIndicatorType').val("");
        $('#txtDescription').val("");
        $('#txtDate').val("");
        $('#txttargetvalue').val("");
        $('#btnConfirm').show();

        $("input[name='IndType']:checked").each(function () {
            $(this).prop('checked', false);
        });
    }
});
//===============View And Delete Indicator===================

// Data table required for many purposes like pagination,searching printing the data showing in the grid etc.
function getDataTable() {
    $('#tblIndicator').DataTable({
        "searching": true,
        //"bStateSave": true,
        "dom": 'Bfrtip',
        "autoWidth": false,
        "buttons": [
            //{
            //    extend: 'print',
            //    title: 'Manage Block',
            //    exportOptions: {
            //        columns: [0, 1, 2, 3, 4]
            //    },
            //    customize: function (win) { $(win.document.body).css('font-size', '10pt').prepend('<img src="' + origin + '/images/t5-logo-white.png"  style="position:absolute; top:0; left:0;" />'); $(win.document.body).find('table').addClass('compact').css('font-size', 'inherit'); }

            //}
            ,
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
//Deleting record through id(primary key)
function deleteIndicator(id) {
    bootbox.confirm({
        size: "medium",
        message: "Are you sure you want to delete ?",
        callback: function (result) {

            if (result === true) {
                $.ajax({
                    url: "/Indicator/DeleteIndicator?id=" + id,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;charset=UTF-8",
                    success: function (data) {
                        if (data == "3") {
                            bootbox.alert("Indicator Deleted Successfully", function () {
                                //window.location.href = "/Indicator/ViewIndicator";
                                window.location.reload();
                            });
                        }
                        else if (data == "4") {
                            bootbox.alert("Indicator Already In Use!", function () {
                                //window.location.href = "/Sector/ViewSector";
                                window.location.reload();
                            });

                        }
                        else { }
                    },
                    error: function (error) {
                        jsonValue = jQuery.parseJSON(error.responseText);
                        bootbox.alert("Error : " + jsonValue);
                    }
                });
            }
        }
    });
}

$("#ddlSector").change(function () {
    bindIndicatorAndDataPoint($(this).val());
});
function bindIndicatorAndDataPoint(SectorId) {
    if (SectorId != 0) {
        $.ajax({
            url: "/IndicatorMapping/GetIndicatorAndDataPointsBySectorId?SectorId=" + SectorId,
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (res) {
                var item = "<option value='0'>---Select---</option>";
                $("#ddlIndicator").empty();
                $.each(res.indicators, function (i, data) {
                    item += '<option value="' + data.INDICATORID + '">' + data.INDICATORNAME + '</option>'
                });
                $("#ddlIndicator").html(item);
            }
        });
    }
}