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
function saveAsDraftForm() {
    
    //Getting all ids of all the tbodys to get all the row data for the insertion 
    var Ids = []; //array declaring to store the ids of all the tbodys
    $('tbody').each(function () {
        var itm = {};
        itm.id = $(this).attr('id');
        Ids.push(itm);
    });
    var Items = []; //array declaring to store the records to send it to controller
    var count = 0;
    for (var j = 0; j < Ids.length; j++) {
        var tbl = document.getElementById(Ids[j].id);
        for (var i = 0; i < tbl.rows.length; i++) {
            var item1 = {};
            if (tbl.rows[i].children[3].getElementsByTagName('input').length != 0 && $('#txtRemark-' + Ids[j].id.split("-")[1]).val()!="") {
                item1.BDODATAID = parseInt(tbl.rows[i].children[1].textContent);
                item1.DATAPOINTVALUE = parseFloat(tbl.rows[i].children[3].children[0].value);
                item1.COLLECTORREMARK = $('#txtRemark-'+Ids[j].id.split("-")[1]).val();
                count++;
            }
            Items.push(item1);
        }
    }
    //checking wheither for all the villages the source village have been selected or not
    if (count == 0) {
        bootbox.alert('Plese enter values and remark against every data points', function () {
        });
        return false;
    }
    else {
        bootbox.confirm({
            size: "medium",
            message: "Are you sure , you want to Submit?",
            callback: function (result) {
                if (result === true) {
                    $(".my-loader").show();
                    $.ajax({
                        url: "/CollectorData/SaveAsDraftCollectorData",
                        data: JSON.stringify(Items),
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataApproval?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        },
                        error: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataApproval?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        }
                    });
                }
            }
        });
    }
};

function acceptForm() {
  
    //Getting all ids of all the tbodys to get all the row data for the insertion 
    var Ids = []; //array declaring to store the ids of all the tbodys
    $('tbody').each(function () {
        var itm = {};
        itm.id = $(this).attr('id');
        Ids.push(itm);
    });
    var Items = []; //array declaring to store the records to send it to controller
    var count = 0;
    for (var j = 0; j < Ids.length; j++) {
        var tbl = document.getElementById(Ids[j].id);
        for (var i = 0; i < tbl.rows.length; i++) {
            var item1 = {};
            if (tbl.rows[i].children[3].getElementsByTagName('input').length != 0 && $('#txtRemark-' + Ids[j].id.split("-")[1]).val() != "") {
                item1.BDODATAID = parseInt(tbl.rows[i].children[1].textContent);
                item1.DATAPOINTVALUE = parseFloat(tbl.rows[i].children[3].children[0].value);
                item1.COLLECTORREMARK = $('#txtRemark-' + Ids[j].id.split("-")[1]).val();
                count++;
            }
            Items.push(item1);
        }
    }
    //checking wheither for all the villages the source village have been selected or not
    if (count == 0) {
        bootbox.alert('Plese enter values and remark against every data points', function () {
        });
        return false;
    }
    else {
        bootbox.confirm({
            size: "medium",
            message: "Are you sure , you want to Submit?",
            callback: function (result) {
                if (result === true) {
                    $(".my-loader").show();
                    $.ajax({
                        url: "/CollectorData/AcceptCollectorData",
                        data: JSON.stringify(Items),
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataApproval?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        },
                        error: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataApproval?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        }
                    });
                }
            }
        });
    }
};
function rejectForm() {
 
    //Getting all ids of all the tbodys to get all the row data for the insertion 
    var Ids = []; //array declaring to store the ids of all the tbodys
    $('tbody').each(function () {
        var itm = {};
        itm.id = $(this).attr('id');
        Ids.push(itm);
    });
    var Items = []; //array declaring to store the records to send it to controller
    var count = 0;
    for (var j = 0; j < Ids.length; j++) {
        var tbl = document.getElementById(Ids[j].id);
        for (var i = 0; i < tbl.rows.length; i++) {
            var item1 = {};
            if (tbl.rows[i].children[3].getElementsByTagName('input').length != 0 && $('#txtRemark-' + Ids[j].id.split("-")[1]).val() != "") {
                item1.BDODATAID = parseInt(tbl.rows[i].children[1].textContent);
                item1.DATAPOINTVALUE = parseFloat(tbl.rows[i].children[3].children[0].value);
                item1.COLLECTORREMARK = $('#txtRemark-' + Ids[j].id.split("-")[1]).val();
                count++;
            }
            Items.push(item1);
        }
    }
    //checking wheither for all the villages the source village have been selected or not
    if (count == 0) {
        bootbox.alert('Plese enter values and remark against every data points', function () {
        });
        return false;
    }
    else {
        bootbox.confirm({
            size: "medium",
            message: "Are you sure , you want to Submit?",
            callback: function (result) {
                if (result === true) {
                    $(".my-loader").show();
                    $.ajax({
                        url: "/CollectorData/RejectCollectorData",
                        data: JSON.stringify(Items),
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataApproval?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        },
                        error: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataApproval?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        }
                    });
                }
            }
        });
    }
};
//function hideAcceptButton() {
//    $("#btnAccept").hide();
//}
//$("#ddlBlock").change(function () {
//    $(".my-loader").show();
//    if ($('#ddlBlock').val() == "0") {
//        window.location.href = "/CollectorData/CollectorDataApproval?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
//    }
//    else {
//        $(".my-loader").hide();
//        window.location.href = "/CollectorData/GetCollectorDataByBlockId?BlockId=" + $('#ddlBlock').val()+ "&Glink="+decodeURI(getUrlVars()["Glink"])+"&Plink="+decodeURI(getUrlVars()["Plink"]);
//    }
//});

//function bindDataPointsByBlockId(blockId) {
//    if (distCode != 0) {
//        $.ajax({
            
//            type: "GET",
//            contentType: "application/json;charset=utf-8",
//            dataType: "json",
//            success: function (res) {

//            }
//        });
//    }
//    else {
//        $('#mainDiv').empty();
//    }
//}
$('#ddlBlock').change(function () {
    $(".my-loader").show();
    $.ajax({
        type: "GET",
        url: "/CollectorData/GetCollectorDataByBlockId?BlockId=" + blockId + "&Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]),
        contentType: "application/json;charset=utf-8",
        dataType: 'Json',
        success: function (response) {
            $(".my-loader").hide();
            window.location.reload();
        },
        error: function (e, ts, et) {
            alert(ts);
        }
    });
});