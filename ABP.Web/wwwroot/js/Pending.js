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
    var tabIds = []
    $('#tblsSectoWise tbody').each(function () {
        var itm = {};
        itm.id = this.id;
        tabIds.push(itm);
    });
    var IndIds = []; var SecIds = []; var dpIds = []; //array declaring to store the ids of all the tbodys
    for (var l = 0; l < tabIds.length; l++) {
        if (tabIds[l].id.split("-")[1] == "sector") {
            var itm = {};
            itm.id = tabIds[l].id;
            SecIds.push(itm);
        }
        else if (tabIds[l].id.split("-")[1] == "indicator") {
            var itm = {};
            itm.id = tabIds[l].id;
            IndIds.push(itm);
        }
        else if (tabIds[l].id.split("-")[1] == "datapoint") {
            var itm = {};
            itm.id = tabIds[l].id;
            dpIds.push(itm);
        }
        else { }
    }
    //$('.tbodySector').each(function () {
    //    var itm = {};
    //    itm.id = $(this).attr('id');
    //    SecIds.push(itm);
    //});
    //$('.tbodyIndicator').each(function () {
    //    var itm = {};
    //    itm.id = $(this).attr('id');
    //    IndIds.push(itm);
    //});

    var Items = []; //array declaring to store the records to send it to controller
    var count = 0;
    for (var k = 0; k < SecIds.length; k++) {
        var finalindids = [];
        for (var m = 0; m < IndIds.length; m++) {
            if (IndIds[m].id.split("-")[3] == SecIds[k].id.split("-")[2]) {
                var itm = {};
                itm.id = IndIds[m].id;
                finalindids.push(itm);
            }
        }
        for (var j = 0; j < finalindids.length; j++) {
            var tbl = document.getElementById(finalindids[j].id);
            for (var n = 0; n < dpIds.length; n++) {
                if (dpIds[n].id.split("-")[2] == finalindids[j].id.split("-")[2]) {
                    finalDPId = dpIds[n].id;
                }
            }
            var tbl2 = document.getElementById(finalDPId);
            for (var i = 0; i < tbl.rows.length; i++) {
                var item1 = {};
                if (tbl.rows[i].children[4].getElementsByTagName('input').length == 0 && $('#txtRemark-sector-' + SecIds[0].id.split("-")[2]).val() == "") {
                    count++;
                }
                item1.SECTORID = SecIds[k].id.split("-")[2];
                item1.INDICATORID = parseInt(tbl.rows[i].children[2].textContent);
                item1.INDICATORVALUE = parseFloat(tbl.rows[i].children[4].children[0].value.trim("%"));
                item1.FROMDATE = tbl.rows[i].children[5].textContent;
                item1.TODATE = tbl.rows[i].children[6].textContent;
                item1.FREQUENCYVALUE = tbl.rows[i].children[7].textContent;
                item1.FREQUENCYNO = tbl.rows[i].children[8].textContent;
                item1.NBDODATAID = tbl2.rows[0].children[0].textContent;
                item1.DBDODATAID = tbl2.rows[1].children[0].textContent;
                item1.NDATAPOINTVALUE = parseFloat(tbl2.rows[0].children[2].children[0].value);
                item1.DDATAPOINTVALUE = parseFloat(tbl2.rows[1].children[2].children[0].value);
                item1.COLLECTORREMARK = $('#txtRemark-sector-' + SecIds[k].id.split("-")[2]).val();
                item1.BLOCKID = $("#ddlBlock").val();
                Items.push(item1);
            }
        }
    }
    //checking wheither for all the villages the source village have been selected or not
    if (count > 0) {
        bootbox.alert('Plese enter remark against every sector(s)', function () {
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
                        url: "/CollectorData/SaveAsDraftCollectorDataIndicator",
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
                                window.location.href = "/CollectorData/CollectorDataIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        },
                        error: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        }
                    });
                }
            }
        });
    }
};
function acceptForm() {
    var currentDate = new Date();
    var months = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    var prev = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
    var viewingMonth = months[prev.getMonth()]
    //Getting all ids of all the tbodys to get all the row data for the insertion 
    var tabIds = []
    $('#tblsSectoWise tbody').each(function () {
        var itm = {};
        itm.id = this.id;
        tabIds.push(itm);
    });
    var IndIds = []; var SecIds = []; var dpIds = []; //array declaring to store the ids of all the tbodys
    for (var l = 0; l < tabIds.length; l++) {
        if (tabIds[l].id.split("-")[1] == "sector") {
            var itm = {};
            itm.id = tabIds[l].id;
            SecIds.push(itm);
        }
        else if (tabIds[l].id.split("-")[1] == "indicator") {
            var itm = {};
            itm.id = tabIds[l].id;
            IndIds.push(itm);
        }
        else if (tabIds[l].id.split("-")[1] == "datapoint") {
            var itm = {};
            itm.id = tabIds[l].id;
            dpIds.push(itm);
        }
        else { }
    }
    //$('.tbodySector').each(function () {
    //    var itm = {};
    //    itm.id = $(this).attr('id');
    //    SecIds.push(itm);
    //});
    //$('.tbodyIndicator').each(function () {
    //    var itm = {};
    //    itm.id = $(this).attr('id');
    //    IndIds.push(itm);
    //});

    var Items = []; //array declaring to store the records to send it to controller
    var count = 0;
    for (var k = 0; k < SecIds.length; k++) {
        var finalindids = [];
        for (var m = 0; m < IndIds.length; m++) {
            if (IndIds[m].id.split("-")[3] == SecIds[k].id.split("-")[2]) {
                var itm = {};
                itm.id = IndIds[m].id;
                finalindids.push(itm);
            }
        }
        for (var j = 0; j < finalindids.length; j++) {
            var tbl = document.getElementById(finalindids[j].id);
            for (var n = 0; n < dpIds.length; n++) {
                if (dpIds[n].id.split("-")[2] == finalindids[j].id.split("-")[2]) {
                    finalDPId = dpIds[n].id;
                }
            }
            var tbl2 = document.getElementById(finalDPId);
            for (var i = 0; i < tbl.rows.length; i++) {
                var item1 = {};
                if (tbl.rows[i].children[4].getElementsByTagName('input').length == 0 && $('#txtRemark-sector-' + SecIds[0].id.split("-")[2]).val() == "") {
                    count++;
                }
                item1.SECTORID = SecIds[k].id.split("-")[2];
                item1.INDICATORID = parseInt(tbl.rows[i].children[2].textContent);
                item1.INDICATORVALUE = parseFloat(tbl.rows[i].children[4].children[0].value.trim("%"));
                item1.FROMDATE = tbl.rows[i].children[5].textContent;
                item1.TODATE = tbl.rows[i].children[6].textContent;
                item1.FREQUENCYVALUE = tbl.rows[i].children[7].textContent;
                item1.FREQUENCYNO = tbl.rows[i].children[8].textContent;
                item1.NBDODATAID = tbl2.rows[0].children[0].textContent;
                item1.DBDODATAID = tbl2.rows[1].children[0].textContent;
                item1.NDATAPOINTVALUE = parseFloat(tbl2.rows[0].children[2].children[0].value);
                item1.DDATAPOINTVALUE = parseFloat(tbl2.rows[1].children[2].children[0].value);
                item1.COLLECTORREMARK = $('#txtRemark-sector-' + SecIds[k].id.split("-")[2]).val();
                item1.BLOCKID = $("#ddlBlock").val();
                item1.MONTHNAME = viewingMonth;
                Items.push(item1);
            }
        }
    }
    //checking wheither for all the villages the source village have been selected or not
    if (count > 0) {
        bootbox.alert('Plese enter remark against every sector(s)', function () {
        });
        return false;
    }
    else {
        bootbox.confirm({
            size: "medium",
            message: "Are you sure , you want to Accept?",
            callback: function (result) {
                if (result === true) {
                    $(".my-loader").show();
                    $.ajax({
                        url: "/CollectorData/AcceptCollectorDataIndicator",
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
                                window.location.href = "/CollectorData/CollectorDataIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        },
                        error: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        }
                    });
                }
            }
        });
    }
};
function rejectForm(sectorId) {

    var currentDate = new Date();
    var months = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    var prev = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
    var viewingMonth = months[prev.getMonth()];
    //Getting all ids of all the tbodys to get all the row data for the insertion 
    if ($('#txtRemark-sector-' + sectorId).val() == "") {
        bootbox.alert("Please enter remark");
        return false;
    }
    else {
        var tabIds = []
        $('#tblsSectoWise tbody').each(function () {
            var itm = {};
            itm.id = this.id;
            tabIds.push(itm);
        });
        var IndIds = []; var SecIds = []; var dpIds = []; //array declaring to store the ids of all the tbodys
        for (var l = 0; l < tabIds.length; l++) {
            if (tabIds[l].id.split("-")[1] == "sector") {
                var itm = {};
                itm.id = tabIds[l].id;
                SecIds.push(itm);
            }
            else if (tabIds[l].id.split("-")[1] == "indicator") {
                var itm = {};
                itm.id = tabIds[l].id;
                IndIds.push(itm);
            }
            else if (tabIds[l].id.split("-")[1] == "datapoint") {
                var itm = {};
                itm.id = tabIds[l].id;
                dpIds.push(itm);
            }
            else { }
        }
        //$('.tbodySector').each(function () {
        //    var itm = {};
        //    itm.id = $(this).attr('id');
        //    SecIds.push(itm);
        //});
        //$('.tbodyIndicator').each(function () {
        //    var itm = {};
        //    itm.id = $(this).attr('id');
        //    IndIds.push(itm);
        //});

        var Items = []; //array declaring to store the records to send it to controller
        var count = 0;
        var finalindids = [];
        for (var m = 0; m < IndIds.length; m++) {
            if (IndIds[m].id.split("-")[3] == sectorId) {
                var itm = {};
                itm.id = IndIds[m].id;
                finalindids.push(itm);
            }
        }
        for (var j = 0; j < finalindids.length; j++) {
            var tbl = document.getElementById(finalindids[j].id);
            for (var n = 0; n < dpIds.length; n++) {
                if (dpIds[n].id.split("-")[2] == finalindids[j].id.split("-")[2]) {
                    finalDPId = dpIds[n].id;
                }
            }
            var tbl2 = document.getElementById(finalDPId);
            for (var i = 0; i < tbl.rows.length; i++) {
                var item1 = {};
                if (tbl.rows[i].children[4].getElementsByTagName('input').length == 0 && $('#txtRemark-sector-' + sectorId).val() == "") {
                    count++;
                }
                item1.SECTORID = sectorId;
                item1.INDICATORID = parseInt(tbl.rows[i].children[2].textContent);
                item1.INDICATORVALUE = parseFloat(tbl.rows[i].children[4].children[0].value.trim("%"));
                item1.FROMDATE = tbl.rows[i].children[5].textContent;
                item1.TODATE = tbl.rows[i].children[6].textContent;
                item1.FREQUENCYVALUE = tbl.rows[i].children[7].textContent;
                item1.FREQUENCYNO = tbl.rows[i].children[8].textContent;
                item1.NBDODATAID = tbl2.rows[0].children[0].textContent;
                item1.DBDODATAID = tbl2.rows[1].children[0].textContent;
                item1.NDATAPOINTVALUE = parseFloat(tbl2.rows[0].children[2].children[0].value);
                item1.DDATAPOINTVALUE = parseFloat(tbl2.rows[1].children[2].children[0].value);
                item1.COLLECTORREMARK = $('#txtRemark-sector-' + sectorId).val();
                item1.BLOCKID = $("#ddlBlock").val();
                item1.MONTHNAME = viewingMonth;
                Items.push(item1);
            }
        }
    }
    //checking wheither for all the villages the source village have been selected or not
    if (count > 0) {
        bootbox.alert('Plese enter remark!', function () {
        });
        return false;
    }
    else {
        bootbox.confirm({
            size: "medium",
            message: "Are you sure , you want to Reject?",
            callback: function (result) {
                if (result === true) {
                    $(".my-loader").show();
                    $.ajax({
                        url: "/CollectorData/RejectCollectorDataIndicator",
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
                                window.location.href = "/CollectorData/GetIndicatorDataByBlockId?BlockId=" + $("#ddlBlock").val() + "&Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        },
                        error: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/CollectorData/CollectorDataIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
                            });
                        }
                    });
                }
            }
        });
    }
};
$("#ddlBlock").change(function () {
    if ($(this).val() == "0") {
        window.location.href = "/DistrictData/CollectorDataIndicator?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    }
    else {
        window.location.href = "/DistrictData/GetIndicatorDataByBlockId?BlockId=" + $(this).val() + "&Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    }
});
function OpenComponent(id) {
    if ($("#tbl_" + id + "")[0].className == "chartertable table collapsebtn collapsed") {
        $("#tbl_" + id + "").attr('class', 'chartertable table collapsebtn tableactive');
        $("#collapseExample_" + id + "").attr('class', 'compbg collapse show');
    }
    else {
        $("#tbl_" + id + "").attr('class', 'chartertable table collapsebtn collapsed');
        $("#collapseExample_" + id + "").attr('class', 'compbg collapse');
    }
}