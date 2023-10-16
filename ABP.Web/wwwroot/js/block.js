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

//================Map and Unmap Block=================
// Onchange of district
$("#ddlDist").change(function () {
    bindBlock($(this).val());
});
//Binding Blocks in the checkboxes for mapping and unmapping purpose

function bindBlock(distCode) {
    if (distCode != 0) {
        $.ajax({
            url: "/Block/GetBlocksByDist?DistCode=" + distCode,
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (res) {
                var html = '';
                $("#blk").show();
                $("#btnConfirm").show();
                $("#btnReset").show();
                getmappedblock(res, distCode);
                //if (res.length === 0) {
                //    html += "<input type='text' id='txtBlock' class='form-control'>"
                //}
                //else {
                //    $.each(res, function (key, item) {
                //        if (item.DISTRICT_CODE != 0) { //Unmapped blocks will view as unchecked check boxes and similarly mapped blocks as checked check boxes
                //            html += "<input id='demo-form-inline-checkbox-" + item.BLOCK_CODE + "' class='magic-checkbox' checked='true' value=" + item.BLOCK_CODE + " type='checkbox'>"
                //                + "<label for='demo-form-inline-checkbox-" + item.BLOCK_CODE + "'>" + item.BLOCK_NAME + "</label> &nbsp;&nbsp;"
                //        }
                //        else {
                //            html += "<input id='demo-form-inline-checkbox-" + item.BLOCK_CODE + "' class='magic-checkbox' value=" + item.BLOCK_CODE + " type='checkbox'>"
                //                + "<label for='demo-form-inline-checkbox-" + item.BLOCK_CODE + "'>" + item.BLOCK_NAME + "</label> &nbsp;&nbsp;"

                //        }
                //    });
                //}

                //$('#mainDiv').html(html);
            }
        });
    }
    else {
        $('#mainDiv').empty();
    }
}
function getmappedblock(res1, distcode) {
    $.ajax({
        url: "/Block/GetMappedBlocksByDist?DistCode=" + distcode,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (ress) {
            var html = '';
            if (ress.length === 0) {
                $.each(res1, function (key, item) {
                    html += "<input id='demo-form-inline-checkbox-" + item.BLOCK_CODE + "' class='magic-checkbox block' value=" + item.BLOCK_CODE + " type='checkbox'>"
                        + "<label for='demo-form-inline-checkbox-" + item.BLOCK_CODE + "'>" + item.BLOCK_NAME + "</label> &nbsp;&nbsp;"
                });
            }
            else {
                $.each(res1, function (key, item) {
                    var count = 0;
                    $.each(ress, function (key1, item1) {
                        if (item1.BLOCK_CODE == item.BLOCK_CODE) { //Unmapped blocks will view as unchecked check boxes and similarly mapped blocks as checked check boxes
                            html += "<input id='demo-form-inline-checkbox-" + item.BLOCK_CODE + "' class='magic-checkbox block' checked='true' value=" + item.BLOCK_CODE + " type='checkbox'>"
                                + "<label for='demo-form-inline-checkbox-" + item.BLOCK_CODE + "'>" + item.BLOCK_NAME + "</label> &nbsp;&nbsp;";
                            count++;
                        }
                        //else {
                        //    html += "<input id='demo-form-inline-checkbox-" + item.BLOCK_CODE + "' class='magic-checkbox' value=" + item.BLOCK_CODE + " type='checkbox'>"
                        //        + "<label for='demo-form-inline-checkbox-" + item.BLOCK_CODE + "'>" + item.BLOCK_NAME + "</label> &nbsp;&nbsp;"

                        //}                        
                    })
                    if (count == 0) {
                        html += "<input id='demo-form-inline-checkbox-" + item.BLOCK_CODE + "' class='magic-checkbox block' value=" + item.BLOCK_CODE + " type='checkbox'>"
                            + "<label for='demo-form-inline-checkbox-" + item.BLOCK_CODE + "'>" + item.BLOCK_NAME + "</label> &nbsp;&nbsp;"
                    }
                });
            }

            $('#mainDiv').html(html);
        }
    });
}
//Submitting the data will map and unmap blocks as per user entered
$("#btnSubmit").click(function () {
    var checkedCount = $.map(getAllCheckedValues(), function (value, index) { return [value]; });;
    if ($('#ddlDist').val() === "") {
        bootbox.alert("Please Select a District!", function () {
            //window.location.href = "/Block/AddBlock";
        });
    }
    else if (checkedCount.length == 0) {
        bootbox.alert("Please Select Blocks");
    }
    else {
        
        var inputCheckedValues = $.map(getAllCheckedValues(), function (value, index) {//Getting checked values from the check boxes in an array to send it to controller for mapping or unmapping purpose
            return [value];
        });
        var inputUnCheckedValues = $.map(getAllUnCheckedValues(), function (value, index) {//Getting unchecked values from the check boxes in an array to send it to controller for mapping or unmapping purpose
            return [value];
        });
        //Putting all the values into a variable as per dto(Data Transmission Object) defined in the controller
        var BlockObj = {
            CheckedValues: inputCheckedValues,
            UnCheckedValues: inputUnCheckedValues,
            DistCode: $("#ddlDist").val()
        };
        var distdata = '';
        for (var i = 0; i < inputCheckedValues.length; i++) {

            // Concat Array value to string variable
            if (distdata == '') {
                distdata += inputCheckedValues[i] + "|" + 0;
            }
            else {
                distdata = distdata + "," + inputCheckedValues[i] + "|" + 0;
            }


        }
        var distCode = getUrlVars()["DistCode"];
        if (distCode != undefined) {
            msg = "Are you sure ,you want to Update?";
        } else {
            msg = "Are you sure , you want to Submit?";
        }
        if (isValidOTP()) {
            bootbox.confirm({
                size: "medium",
                message: msg,
                callback: function (result) {
                    if (result === true) {
                        $.ajax({
                            url: "/Block/MapBlock",
                            data: { BlockData: distdata, DistIdD: BlockObj.DistCode },
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("XSRF-TOKEN",
                                    $('input:hidden[name="__RequestVerificationToken"]').val());
                            },
                            method: "POST",
                            success: function (result) {
                                //bindBlock($("#ddlDist").val());
                                var Glink = 8;
                                var Plink = 32;
                                var DISTID = getUrlVars()["DistCode"];
                                if (DISTID != undefined) {
                                    bootbox.alert("Block Updated successfully", function () {  //Alert showing through bootbox for a better look and fill
                                        window.location.href = "/Block/ViewBlock?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                                    });
                                }
                                else {
                                    bootbox.alert("Block Mapped successfully", function () {
                                        window.location.href = "/Block/AddBlock?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
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

$("#btnReset").click(function () {
    
    var distCode = getUrlVars()["DistCode"];
    if (distCode != undefined) {
        location.href = "/Block/ViewBlock?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    } else {
        $("#ddlDist").val("");
        $("#blk").hide(); 
        $("#txtotp").val("");
        $("#btnReset").hide();
        $("#btnConfirm").hide();
        
        
    }
});
//Finding all checked values inside the main div
function getAllCheckedValues() {
 
    var inputValues = $('#mainDiv :input').map(function () {
        var type = $(this).prop("type");

        // checked radios/checkboxes
        if ((type == "checkbox" || type == "radio") && this.checked) {
            return $(this).val();
        }
        // all other fields, except buttons
        //else if (type != "button" && type != "submit") {
        //    return $(this).val();
        //}
    });
    //alert(inputValues);
    //var values = inputValues.join(",");
    return inputValues;
}
//Finding all unchecked values inside the main div
function getAllUnCheckedValues() {
    var inputValues = $('#mainDiv :input').map(function () {
        var type = $(this).prop("type");

        // checked radios/checkboxes
        if ((type == "checkbox" || type == "radio") && this.checked === false) {
            return $(this).val();
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
//==============View And Deleting Mapping for Districts==================
$("#btnSearch").click(function () {
    getAllBlockByDist($('#ddlDist').val());
});

// Data table required for so many purposes like pagination,searching printing the data showing in the grid etc.
function GetDataTable() {
    $('#tblBlocks').DataTable({
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
//Deleting record through dist id(unmapping all the blocks under the disrtict which is clicked)
function deleteBlock(distCode) {
    bootbox.confirm({
        size: "medium",
        message: "Are you sure , you want to Delete?",
        callback: function (result) {
            if (result === true) {
                $.ajax({
                    url: "/Block/DeleteBlock?DistCode=" + distCode,
                    type: "GET",
                    //data: "BlockCode=" + BLOCK_CODE,
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        bootbox.alert(res, function () {
                            window.location.href = "/Block/ViewBlock?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                        });
                    }
                });
            }
        }
    });
}
function validate() {
    return true;
}