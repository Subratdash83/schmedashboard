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

//================Map and Unmap indicator With DataPoint=================
// Onchange of Sector
$("#ddlSector").change(function () {
    bindIndicatorAndDataPoint($(this).val());
});
$("#ddlIndicator").change(function () {
    if ($("#ddlSector").val() == 0) {
        bootbox.alert("Please Select a Sector!");
    }
    else if ($(this).val() == 0) {
        bootbox.alert("Please Select a Indicator!");
    }
    else {
        bindIndicatorMappingsBySectorAndIMId($("#ddlSector").val(), $(this).val());
    }
});
//is census datapoint binding.

$('input[name="checkResp"]').click(function () {
    var check = $('#iscensus').is(":checked");
    if (check == true) {
        $.ajax({

            url: "/IndicatorMapping/GetIsCensusBySectorId?SectorId=" + $("#ddlSector").val(),
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (res) {
                var htmln = '';
                $.each(res.datapoints, function (key, item) {
                    htmln += "<input type='radio' id=" + item.DATAPOINTID + " name='nRadio' class='magic-checkbox chk' value=" + item.DATAPOINTID + " onChange='handleChecked(" + item.DATAPOINTID + ")'>"
                        + "<label for=" + item.DATAPOINTID + ">" + item.DATAPOINTNAME + "</label>"
                });
                $('#data2').hide();
                $('#data1').hide();
                $('#data').show();
                $('#mainDivN1').html(htmln);
            }
        });
    }
    else {

    }


});
//}



//Binding Indicatormapping data through sector id for mapping and unmapping purpose

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
                var htmld = '';
                var htmln = '';
                $.each(res.datapoints, function (key, item) {
                    htmld += "<input type='radio' id='dpd-inline-radio-" + item.DATAPOINTID + "' name='dRadio' class='magic-checkbox chk' value=" + item.DATAPOINTID + " onChange='handleChecked(" + item.DATAPOINTID + ")'>"
                        + "<label for='dpd-inline-radio-" + item.DATAPOINTID + "'>" + item.DATAPOINTNAME + "</label>"
                });
                $('#mainDivD').html(htmld);
                $.each(res.datapoints, function (key, item) {
                    htmln += "<input type='radio' id='dpn-inline-radio-" + item.DATAPOINTID + "' name='nRadio' class='magic-checkbox chk' value=" + item.DATAPOINTID + " onChange='handleChecked(" + item.DATAPOINTID + ")'>"
                        + "<label for='dpn-inline-radio-" + item.DATAPOINTID + "'>" + item.DATAPOINTNAME + "</label>"
                });
                $('#mainDivN').html(htmln);
            }
        });
    }
    else {
        $('#mainDivD').empty();
        $('#mainDivN').empty();
    }
}

function handleChecked(DATAPOINTID) {
    //$('input:radio').removeAttr('disabled');
    if ($("#dpd-inline-radio-" + DATAPOINTID + "").prop("checked")) {
        enableRadioButtonsN();
        $("#dpn-inline-radio-" + DATAPOINTID + "").prop("disabled", true);
    }
    else if ($("#dpn-inline-radio-" + DATAPOINTID + "").prop("checked")) {
        enableRadioButtonsD();
        $("#dpd-inline-radio-" + DATAPOINTID + "").prop("disabled", true);
    }
}
function enableRadioButtonsD() {
    $('#mainDivD :input').each(function () {
        var type = $(this).prop("type");
        if (type == "checkbox" || type == "radio") {
            $(this).prop("disabled", false);
        }
    });
}
function enableRadioButtonsN() {
    $('#mainDivN :input').each(function () {
        var type = $(this).prop("type");
        if (type == "checkbox" || type == "radio") {
            $(this).prop("disabled", false);
        }
    });
}
function getAllCheckedValuesD() {
    var inputValues = $('#mainDivD :input').map(function () {
        var type = $(this).prop("type");

        // checked radios/checkboxes
        if ((type == "checkbox" || type == "radio") && this.checked === true) {
            return $(this).val();
        }
        else {
            return 0;
        }
        // all other fields, except buttons
        //else if (type != "button" && type != "submit") {
        //    return $(this).val();
        //}
    });
    //var values = inputValues.join(",");
    //alert(inputValues);
    return inputValues;
}
function getAllCheckedValuesN() {
    var inputValues = $('#mainDivN :input').map(function () {
        var type = $(this).prop("type");

        // checked radios/checkboxes
        if ((type == "checkbox" || type == "radio") && this.checked === true) {
            return $(this).val();
        }
        else {
            return 0;
        }
        // all other fields, except buttons
        //else if (type != "button" && type != "submit") {
        //    return $(this).val();
        //}
    });
    //var values = inputValues.join(",");
    //alert(inputValues);
    return inputValues;
}


function getAllCheckedValuecheckiscensus() {
    
    var inputValues = $('#mainDivN1 :input').map(function () {
        var type = $(this).prop("type");

        // checked radios/checkboxes
        if ((type == "checkbox" || type == "radio") && this.checked === true) {
            return $(this).val();
        }
        else {
            return 0;
        }
        // all other fields, except buttons
        //else if (type != "button" && type != "submit") {
        //    return $(this).val();
        //}
    });
    //var values = inputValues.join(",");
    //alert(inputValues);
    return inputValues;
}


$("#btnSubmit").click(function () {
    
    var cnt = 0; var count = 0; var countcensus=0; var inputCheckedValuesN; var inputCheckedValuesD; var inputCheckedValueincensus;
    if ($('#ddlSector').val() === "0") {
        bootbox.alert("Please Select a Sector!");
    }
    else if ($('#ddlIndicator').val() === "0")
    {
        bootbox.alert("Please Select a Indicator!");
    }

    else {
        $('#mainDivN :input').map(function () {
            if (this.checked === true) {
                inputCheckedValuesN = $(this).val();
                cnt++;
            }
        });
        $('#mainDivD :input').map(function () {
            if (this.checked === true) {
                inputCheckedValuesD = $(this).val();
                count++;
            }
        });
        $('#mainDivN1 :input').map(function () {

            if (this.checked == true) {
                inputCheckedValueincensus = $(this).val();
                countcensus++;
                //count++;
            }

        });


        //$('#mainDivN1 :input').map(function () {

        //    //if (this.checked == false) {
        //    //    bootbox.alert('Choose any Datapoint!');
        //    //}
        //});

        //var inputCheckedValuesD = $.map(getAllCheckedValuesD(), function (value, index) {//Getting checked values from the check boxes in an array to send it to controller for mapping or unmapping purpose
        //    if (value != 0) {
        //        cnt++;

        //    }

        //        return [value];
        //});
        //var inputCheckedValuesN = $.map(getAllCheckedValuesN(), function (value, index) {//Getting unchecked values from the check boxes in an array to send it to controller for mapping or unmapping purpose
        //    if (value != 0) {
        //        count++;

        //    }

        //        return [value];

        //});
        ////Putting all the values into a variable as per dto(Data Transmission Object) defined in the controller
        var check = $('#iscensus').is(":checked");
     
        var isdata;
        // var inputCheckincense = 0;
        if (check == true) {
            isdata = 1;

        }
        else {
            isdata = 0;

        }
       
        if (countcensus == 0 && isdata == 1) {
            bootbox.alert("Please Select one  Datapoint Mapping!");
            return false;
        }
        if (cnt == 0 && check != true) {
          
            bootbox.alert("Please Select one  Datapoint Mapping(Numerator)!");

        }
        else if (count == 0 && check != true) {
            bootbox.alert("Please Select one  Datapoint Mapping(Denominator)!");

        }

        else {
            var InicatorMappingObj = {

                CheckedValuesD: inputCheckedValuesD,
                CheckedValuesN: inputCheckedValuesN,
                CheckedValuesIncenus: inputCheckedValueincensus,
                SectorId: $("#ddlSector").val(),
                IndicatorMappingId: $('#hdnIMId').val(),
                IndicatorId: $("#ddlIndicator").val(),
                Formula: $("#txtFormula").val(),
                iscensus: isdata,
            };
            var imId = getUrlVars()["ID"];
            if (imId != undefined || $('#hdnIMId').val() != "0") {
                msg = "Are you sure ,you want to Update?";
            }
            else {
                msg = "Are you sure ,you want to Submit?";
            }
            bootbox.confirm({
                size: "medium",
                message: msg,
                callback: function (result) {
                   
                    if (result === true)
                    {

                        $.ajax({
                            url: "/IndicatorMapping/AddIndicatorMapping",
                            data: JSON.stringify(InicatorMappingObj),
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            type: "POST",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            success: function (result) {
                                var INDICATORMAPPINGID = getUrlVars()["ID"];
                                if (INDICATORMAPPINGID != undefined || $('#hdnIMId').val() != "0") {
                                    bootbox.alert(result, function () {  //Alert showing through bootbox for a better look and fill
                                        window.location.href = "/IndicatorMapping/ViewIndicatorMapping?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                                    });
                                }
                                else {
                                    bootbox.alert(result, function () {
                                        window.location.href = "/IndicatorMapping/AddIndicatorMapping?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                                    });
                                }
                            },
                            error: function (result) {
                                alert(result);
                            }
                        });
                    }
                }
            });
        }
    }
});
//Sending the data to the add page for updating purpose

function bindIndicatorMappingsById(imId) {
    $.ajax({
        url: "/IndicatorMapping/IndicatorMappingGateById?IMId=" + imId,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=UTF-8",
        success: function (data) {  //Filling the values in appropriate field for updating purpose       
            $('#hdnIMId').val(data[0].INDICATORMAPPINGID);
            $("#ddlSector option[value=" + data[0].SECTORID + "]").prop('selected', true);
            bindIndicatorAndDataPointAndAll(data);
            $('#txtFormula').val(data[0].FORMULA);
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            bootbox.alert("Error : " + jsonValue);
        }
    });
}
function bindIndicatorAndDataPointAndAll(data) {
    $.ajax({
        url: "/IndicatorMapping/GetIndicatorAndDataPointsBySectorId?SectorId=" + data[0].SECTORID,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (res) {
            var item = "<option value='0'>---Select---</option>";
            $("#ddlIndicator").empty();
            $.each(res.indicators, function (i, dat) {
                item += '<option value="' + dat.INDICATORID + '">' + dat.INDICATORNAME + '</option>'
            });
            $("#ddlIndicator").html(item);
            $("#ddlIndicator option[value=" + data[0].INDICATORID + "]").prop('selected', true);
            var htmld = '';
            var htmln = '';
            $.each(res.datapoints, function (key, item) {
                htmld += "<input type='radio' id='dpd-inline-radio-" + item.DATAPOINTID + "' name='dRadio' class='magic-checkbox chk' value=" + item.DATAPOINTID + " onChange='handleChecked(" + item.DATAPOINTID + ")'>"
                    + "<label for='dpd-inline-radio-" + item.DATAPOINTID + "'>" + item.DATAPOINTNAME + "</label>"
            });
            $('#mainDivD').html(htmld);
            $.each(res.datapoints, function (key, item) {
                htmln += "<input type='radio' id='dpn-inline-radio-" + item.DATAPOINTID + "' name='nRadio' class='magic-checkbox chk' value=" + item.DATAPOINTID + " onChange='handleChecked(" + item.DATAPOINTID + ")'>"
                    + "<label for='dpn-inline-radio-" + item.DATAPOINTID + "'>" + item.DATAPOINTNAME + "</label>"
            });
            $('#mainDivN').html(htmln);
            $("#dpd-inline-radio-" + data[0].DDATAPOINTID + "").prop('checked', true);
            handleChecked(data[0].DDATAPOINTID);
            $("#dpn-inline-radio-" + data[0].NDATAPOINTID + "").prop('checked', true);
            handleChecked(data[0].NDATAPOINTID);
        }
    });
}
//Resetting all the fields
$('#btnCancel').click(function () {
    var imId = getUrlVars()["ID"];
    if (imId != undefined) {
        location.href = "/IndicatorMapping/ViewIndicatorMapping?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    }
    else {
        $('#hdnIMId').val("0");
        $('#ddlSector').val("0");
        $('#ddlIndicator').val("0");
        $('#txtFormula').val("");
        $('#mainDivD').empty();
        $('#mainDivN').empty();
        $('#btnConfirm').show();
    }

});
//===============View And Delete Indicator Mapping===================

// Data table required for many purposes like pagination,searching printing the data showing in the grid etc.
function getDataTable() {
    $('#tblIndicatorMapping').DataTable({
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
function deleteIndicatorMapping(id) {

    bootbox.confirm({
        size: "medium",
        message: "Are you sure you want to delete ?",
        callback: function (result) {

            if (result === true) {
                $.ajax({
                    url: "/IndicatorMapping/DeleteIndicatorMapping?id=" + id,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;charset=UTF-8",
                    success: function (data) {
                        if (data == "3") {
                            bootbox.alert("Indicator Mapping Deleted Successfully", function () {
                                //window.location.href = "/IndicatorMapping/ViewIndicatorMapping";
                                window.location.reload();

                            });
                        }
                        else if (data == "4") {
                            bootbox.alert("Indicator Formula Already In Use!", function () {
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
function bindIndicatorMappingsBySectorAndIMId(sectorId, imId) {
    $.ajax({
        url: "/IndicatorMapping/IndicatorMappingGateBySectorAndImId?SectorId=" + sectorId + "&IMId=" + imId,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=UTF-8",
        success: function (data) {  //Filling the values in appropriate field for updating purpose
            $("#ddlSector option[value=" + sectorId + "]").prop('selected', true);
            $("#ddlIndicator option[value=" + imId + "]").prop('selected', true);
            if (data.length > 0) {
                $('#hdnIMId').val(data[0].INDICATORMAPPINGID);
                bindIndicatorAndDataPointAndAll(data);
                $('#txtFormula').val(data[0].FORMULA);
                $("#btnSubmit").html("Update");
                $("#btnCancel").html("Cancel");
            }
            else {
                $('#hdnIMId').val("0");
                $('#mainDivN :input').each(function () {
                    this.checked = false;
                });
                $('#mainDivD :input').each(function () {
                    this.checked = false;
                });
                $("#btnSubmit").html("Submit");
                $("#btnCancel").html("Reset");
            }
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            bootbox.alert("Error : " + jsonValue);
        }
    });
}
function validate() {
    var cnt = 0; var count = 0; var inputCheckedValuesN; var inputCheckedValuesD;
    if ($('#ddlSector').val() === "0") {
        bootbox.alert("Please Select a Sector!");
    }
    else if ($('#ddlIndicator').val() === "0") {
        bootbox.alert("Please Select a Indicator!");
    }

    else {
        $('#mainDivN :input').map(function () {
            if (this.checked === true) {
                inputCheckedValuesN = $(this).val();
                cnt++;
            }
        });
        $('#mainDivD :input').map(function () {
            if (this.checked === true) {
                inputCheckedValuesD = $(this).val();
                count++;
            }
        });
      
        if (inputCheckedValuesN == 0) {
            
            bootbox.alert("Please Select one  Datapoint Mapping(Numerator)!");

        }
        else if (inputCheckedValuesD == 0) {
            bootbox.alert("Please Select one  Datapoint Mapping(Denominator)!");

        }
        else {
            $("#btnCancel").hide();
            return true;
        }
    }
}