//=============Add And Update Indicator========================
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
        var deptId = getUrlVars()["ID"];
        if (deptId != undefined) {
            msg = "Are you sure ,you want to Update?";
        }
        else {
            msg = "Are you sure ,you want to Submit?";
        }
        bootbox.confirm({
            size: "medium",
            message: msg,
            callback: function (result) {
                if (result === true) {
                    var fileData = new FormData();  //Creating FormData Object to fill the values entered by the  user for insert into the database
                    fileData.append("ID", $('#hdnDeptId').val());
                    fileData.append("SECTORID", $('#ddlSector').val());
                    fileData.append("DEPTID", $('#ddlDepartment').val());
                    fileData.append("DESCRIPTION", $('#txtDescription').val());
                    addDepartmentData(fileData);
                }
            }
        });
    }

});
//Validations for the fields in the form
function validate() {

    if ($('#ddlDepartment').val() == "0") {
        bootbox.alert("Please select department name!");
        return false;
        $('#ddlDepartment').focus();
    }
    else if ($('#ddlSector').val() == "0") {
        bootbox.alert("Please select sector!");
        $('#ddlSector').focus();
        return false;
    }
    else
        return true;
}
//Post Call to the controller for data insertion 
function addDepartmentData(Data) {
    
    $.ajax({
        type: "POST",
        url: "/Department/AddDepartment",//Controller and Method(/Controller/Method)
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: Data,
        contentType: false, // Not to set any content header=================
        processData: false, // Not to process data
        success: function (res) {
            var DEPTID = getUrlVars()["ID"];
            if (DEPTID != undefined) {
                bootbox.alert(res, function () {  //Alert showing through bootbox for a better look and fill
                    window.location.href = "/Department/ViewDepartment?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                });
            }
            else {
                bootbox.alert(res, function () {  //Alert showing through bootbox for a better look and fill
                    window.location.href = "/Department/AddDepartment?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"])
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

function bindDepartmentById(deptId) {
    $.ajax({
        url: "/Department/DepartmentGateById?DeptId=" + deptId,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=UTF-8",
        success: function (data) {  //Filling the values in appropriate field for updating purpose
            $('#hdnDeptId').val(data[0].ID);
            $('#ddlSector').val(data[0].SECTORID);
            $('#ddlDepartment').val(data[0].DEPTID);
            $('#txtDescription').val(data[0].DESCRIPTION);
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            bootbox.alert("Error : " + jsonValue);
        }
    });
}
//Resetting all the fields
$('#btnCancel').click(function () {
    var deptId = getUrlVars()["ID"];
    if (deptId != undefined) {
        location.href = "/Department/ViewDepartment?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    }
    else {
        $('#hdnDeptId').val("0");
        $('#ddlSector').val("0");
        $('#ddlDepartment').val("0");
        $('#txtDescription').val("");
    }    
});
//===============View And Delete Indicator===================

// Data table required for many purposes like pagination,searching printing the data showing in the grid etc.
function getDataTable() {
    $('#tblDepartment').DataTable({
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
function deleteDepartment(id) {
    bootbox.confirm({
        size: "medium",
        message: "Are you sure you want to delete ?",
        callback: function (result) {

            if (result === true) {
                $.ajax({
                    url: "/Department/DeleteDepartment?id=" + id,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;charset=UTF-8",
                    success: function (data) {
                        if (data == "3") {
                            bootbox.alert("Data Deleted Successfully", function () {
                               // window.location.href = "/Department/ViewDepartment";
                                window.location.reload();
                            });
                        }
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

//$("#btnSearch").click(function () {
//    if ($('#ddlDepartment').val() == "0") {
//        bootbox.alert("Please select department name!");
//        return false;
//        $('#ddlDepartment').focus();
//    }
//    else if ($('#ddlSector').val() == "0") {
//        bootbox.alert("Please select sector!");
//        $('#ddlSector').focus();
//        return false;
//    }
//    else
//        return true;
//});
