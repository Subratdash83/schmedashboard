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
    debugger;
    if (validate()) {
        var DATAPOINTID = getUrlVars()["ID"];
        if (DATAPOINTID != undefined) {
            msg = "Are you sure ,you want to Update?";
        } else {
            msg = "Are you sure ,you want to Submit?";
        }
        bootbox.confirm({
            size: "medium",
            message: msg,
            callback: function (result) {
                var radioValue = $("input[name='YEARTYPE']:checked").val();
                if (result === true) {
                    debugger;

                    var check = $('#iscensus').is(":checked");
                    var fileData = new FormData();  //Creating FormData Object to fill the values entered by the  user for insert into the database
                    fileData.append("DATAPOINTID", $('#hdnDATAPOINTID').val());
                    fileData.append("SECTORID", $('#ddlSector').val());
                    fileData.append("DATAPOINTDATE", $('#txtDATAPOINTDATE').val());
                    fileData.append("DEPTID", $('#ddlDepartment').val());
                    fileData.append("PROVIDERID", $('#ddlProvider').val());
                    fileData.append("DATAPOINTNAME", $('#txtDATAPOINTNAME').val());
                    fileData.append("TYPE", $('#ddlIndicatorType').val());
                    fileData.append("YEARTYPE", radioValue);
                    fileData.append("FREQUENCYID", $('#ddlFrequency').val());
                    fileData.append("MONTHNO", $('#ddlFrequencyM').val());
                    fileData.append("DESCRIPTION", $('#txtDescription').val());
                    fileData.append("UNIT", $('#ddlUnit').val());
                    if (check == true) {
                        fileData.append("iscensus", 1);
                    }
                    else {
                        fileData.append("iscensus", 0);
                    }
                   
                    addDataPointData(fileData);
                }
            }
        });
    }

});
//Validations for the fields in the form
function validate() {
    var radioValue = $("input[name='YEARTYPE']:checked").val();
    //var d = new Date();
    //var currentDate = d.toShortFormat();
    if ($('#ddlDepartment').val() == "0") {
        bootbox.alert("Please select Department!");
        $('#ddlDepartment').focus();
        return false;
    }
    else if ($('#ddlProvider').val() == "0") {
        bootbox.alert("Please select Data Provider!");
        $('#ddlProvider').focus();
        return false;
    }
    else if ($('#ddlSector').val() == "0") {
        bootbox.alert("Please select sector!");
        $('#ddlSector').focus();
        return false;
    }
    else if ($('#txtDATAPOINTNAME').val() == "") {
        bootbox.alert("Please enter data point name!");
        $('#txtDATAPOINTNAME').focus();
        return false;
    }
    else if (radioValue == undefined) {
        bootbox.alert("Please select Collection Mode!");
        return false;
        //$('#ddlDepartment').focus();
    }
    else if ($('#ddlFrequency').val() == "0") {
        bootbox.alert("Please select frequency!");
        $('#ddlFrequency').focus();
        return false;
    }
    else if ($('#ddlUnit').val() == "Select") {
        bootbox.alert("Please select Unit!");
        $('#ddlUnit').focus();
        return false;
    }
    else if ($('#ddlIndicatorType').val() == "") {
        bootbox.alert("Please select Type!");
        $('#ddlIndicatorType').focus();
        return false;
    }
    else if ($('#txtDATAPOINTDATE').val() == "") {
        bootbox.alert("Please enter Effective date!");
        return false;
        $('#txtDate').focus();
    }
    else { 
        $("#btnCancel").hide();
        return true;
    }
}
//Post Call to the controller for data insertion 
function addDataPointData(Data) {
    $.ajax({
        type: "POST",
        url: "/DataPoint/AddDataPoint",//Controller and Method(/Controller/Method)
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: Data,
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        success: function (res) {
            var DATAPOINTID = getUrlVars()["ID"];
            if (DATAPOINTID != undefined) {
                bootbox.alert(res, function () {  //Alert showing through bootbox for a better look and fill
                    window.location.href = "/DataPoint/ViewDataPoint?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                });
            }
            else {
                bootbox.alert(res, function () {  //Alert showing through bootbox for a better look and fill
                    window.location.href = "/DataPoint/AddDataPoint?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
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

function bindDataPointById(DATAPOINTID) {
    $.ajax({
        url: "/DataPoint/DataPointGateById?DATAPOINTID=" + DATAPOINTID,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=UTF-8",
        success: function (data) {
            debugger;//Filling the values in appropriate field for updating purpose
            $('#hdnDATAPOINTID').val(data[0].DATAPOINTID);
            $('#ddlSector').val(data[0].SECTORID);
            $('#txtDATAPOINTNAME').val(data[0].DATAPOINTNAME);
            $('#ddlDepartment').val(data[0].DEPTID);
            $('#ddlProvider').val(data[0].PROVIDERID);
            $('#txtDATAPOINTDATE').val(data[0].DATAPOINTDATE);
            $('#ddlFrequency').val(data[0].FREQUENCYID);
            $('#txtDescription').val(data[0].DESCRIPTION);
            
            if (data[0].FREQUENCYID == 5) {
                $('#ddlFrequencyM').val(data[0].MONTHNO);
                $("#divFrequencyMon").show();
            }
            else {
                
                $("#divFrequencyMon").hide();
            }

            
            $('#ddlIndicatorType').val(data[0].TYPE);
            if (data[0].YEARTYPE == 1) {
                $('#type1').attr('checked', true);
            }
            else {
                $('#type2').attr('checked', true);
            }
            $('#ddlUnit').val(data[0].UNIT);
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            bootbox.alert("Error : " + jsonValue);
        }
    });
}
//Resetting all the fields
$('#btnCancel').click(function () {
    var DATAPOINTID = getUrlVars()["ID"];
    if (DATAPOINTID != undefined) {
        location.href = "/DataPoint/ViewDataPoint?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    }
    else {
        $('#hdnDATAPOINTID').val("0");
        $('#ddlSector').val("0");
        $('#txtDATAPOINTNAME').val("");
        $('#ddlFrequency').val("0");
        $('#txtDATAPOINTDATE').val("");
        $('#btnConfirm').show();
    }
    
});
//===============View And Delete Indicator===================

// Data table required for many purposes like pagination,searching printing the data showing in the grid etc.
function getDataTable() {
    $('#tblDataPoint').DataTable({
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
function deleteDataPoint(id) {
    bootbox.confirm({
        size: "medium",
        message: "Are you sure you want to delete ?",
        callback: function (result) {

            if (result === true) {
                $.ajax({
                    url: "/DataPoint/DeleteDataPoint?id=" + id,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;charset=UTF-8",
                    success: function (data) {
                        if (data == "3") {
                            bootbox.alert("DataPoint Deleted Successfully", function () {
                                //window.location.href = "/DataPoint/ViewDataPoint";
                                window.location.reload();
                            });
                        } else if (data == "4") {
                            bootbox.alert("DataPoints Already In Use!", function () {
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
                $.each(res.datapoints, function (key, data) {
                    item += '<option value="' + data.DATAPOINTID + '">' + data.DATAPOINTNAME + '</option>'
                });
                $("#ddlDatapoint").html(item);                
            }
        });
    }    
}

//$("#btnSearch").click(function () {
//    if ($('#ddlSector').val() == "0") {
//        bootbox.alert("Please Select a Sector!");
//        return false;
//    }
//    else {
//        return true;
//    }
//});