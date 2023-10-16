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
function submitForm() {
    var currentDate = new Date();
    var months = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    var prev = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
    var viewingMonth = months[prev.getMonth()];
    //Getting all ids of all the tbodys to get all the row data for the insertion 
    var Ids = [];  //array declaring to store the ids of all the tbodys
    $('tbody').each(function () {
        var itm = {};
        itm.id = $(this).attr('id');
        Ids.push(itm)
    });
    var Items = []; //array declaring to store the records to send it to controller
    for (var j = 0; j < Ids.length; j++) {
        var tbl = document.getElementById(Ids[j].id);
        for (var i = 0; i < tbl.rows.length; i++) {
            var item1 = {};
            if (tbl.rows[i].children[3].getElementsByTagName('input').length != 0 && parseInt(tbl.rows[i].children[4].textContent) == 0 || parseInt(tbl.rows[i].children[4].textContent)==3) {
                item1.BDODATAID = parseInt(tbl.rows[i].children[1].textContent);
                item1.DATAPOINTVALUE = parseFloat(tbl.rows[i].children[3].children[0].value);
                item1.FREQUENCYNO = parseFloat(tbl.rows[i].children[5].textContent);
                item1.DATAPOINTID = parseFloat(tbl.rows[i].children[6].textContent);
                item1.INDICATORID = parseFloat(tbl.rows[i].children[7].textContent);
                item1.SECTORID = parseFloat(tbl.rows[i].children[8].textContent);
                item1.BLOCKID = parseFloat(tbl.rows[i].children[9].textContent);
                item1.YEAR = parseFloat(tbl.rows[i].children[10].textContent);
                item1.FROMDATE = tbl.rows[i].children[11].textContent;
                item1.TODATE = tbl.rows[i].children[12].textContent;
                item1.FREQUENCYVALUE =tbl.rows[i].children[13].textContent;
                item1.FREQUENCYID = parseFloat(tbl.rows[i].children[14].textContent);
                item1.INDICATORMAPPINGID = parseFloat(tbl.rows[i].children[15].textContent);
                item1.DATAID = parseFloat(tbl.rows[i].children[16].textContent);
                item1.MONTHNAME = viewingMonth;
                item1.OTP = $("#txtotp").val();
                Items.push(item1);
            }
            else { }
        }
    }
    //checking wheither for all the villages the source village have been selected or not
    if (Items.length == 0) {
        bootbox.alert("Plese enter values against every data points!");
        return false;
    }
    else if (isValidOTP()) {
        bootbox.confirm({
            size: "medium",
            message: "Are you sure , you want to Submit?",
            callback: function (result) {
                if (result === true) {
                    $(".my-loader").show();
                    $.ajax({
                        url: "/BDOData/BDODataSubmission",
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
                                window.location.href = "/BDOData/BDODataSubmission?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                            });
                        },
                        error: function (result) {
                            $(".my-loader").hide();
                            bootbox.alert(result, function () {
                                window.location.href = "/BDOData/BDODataSubmission?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                            });
                        }
                    });
                }
            }
        });
    }
};
function validate() {
    return true;
}