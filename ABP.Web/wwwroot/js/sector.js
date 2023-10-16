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

//======Add And Update Sector Master========
$('#btnSubmit').click(function () {
    if (validate()) {
        var secId = getUrlVars()["ID"];
        if (secId != undefined) {
            msg = "Are you sure ,you want to Update?";
        } else {
            msg = "Are you sure ,you want to Submit?";
        }
        bootbox.confirm({
            size: "medium",
            message: msg,
            callback: function (result) {
                if (result === true) {
                    var fileData = new FormData(); //Creating FormData Object to fill the values entered by the  user for insert into the database
                    fileData.append("SECTORID", $('#hdnSectorId').val());
                    fileData.append("SECTORNAME", $('#txtSectorName').val());
                    fileData.append("DEPTID", $('#ddlDepartment').val());
                    //fileData.append("CSSCLASS", $('#txtcssclass').val());
                    if (window.FormData !== undefined) {
                        var fileUpload = $("#fuUploadPoc").get(0);

                        var files = fileUpload.files;
                        if (files.length > 0) {
                            for (var i = 0; i < files.length; i++) {
                                fileData.append(files[i].name, files[i]);
                            }
                        }
                        else {
                            fileData.append("CSSCLASS", $('#fuUploadPocPrev')[0].innerText);
                        }
                    }
                    addSectorData(fileData);
                }
            }
        });
    }

});
$('#fuUploadPoc').on('change', function (e) {
    var file = $('#fuUploadPoc')[0].files[0].name;
    $('#fuUploadPocPrev').text(file);
    if (!ValidateFile('fuUploadPoc', 'Valid Document')) {
        return false;
    };
    if (!CheckFileType('fuUploadPoc', '2')) {
        return false;
    };
});
function ValidateFile(cntr, strText) {
    var strValue = $('#' + cntr).get(0).files.length;
    if (strValue == "0") {
        bootbox.alert("Please upload " + strText);
        $('#fuUploadPocPrev').text("Choose File");
        return false;
    }
    else
        return true;
}
function CheckFileType(cntr, ftype) {
    var userImg = '@Url.Content("~/imgs/no_user.png")';
    // Get the file upload control file extension
    var extn = $('#' + cntr).val().split('.').pop().toLowerCase();
    if (extn != '') {

        // Create array with the files extensions to upload
        var fileListToUpload;
        if (parseInt(ftype) == 1)
            fileListToUpload = new Array('pdf', 'jpg', 'jpeg');
        else if (parseInt(ftype) == 2)
            fileListToUpload = new Array('svg');
        else
            fileListToUpload = new Array('pdf');

        //Check the file extension is in the array.
        var isValidFile = $.inArray(extn, fileListToUpload);

        // isValidFile gets the value -1 if the file extension is not in the list.
        if (isValidFile == -1) {
            if (parseInt(ftype) == 1) {
                bootbox.alert('Please upload a valid document of type pdf/jpg/jpeg!!!');
                $('#' + cntr).replaceWith($('#' + cntr).val('').clone(true));
                $('#' + cntr).focus();
            }
            else if (parseInt(ftype) == 2) {
                bootbox.alert('Please upload a valid svg document.!!!');
                $('#fuUploadPocPrev').text("Choose file");
                $('#showPhoto').attr('src', userImg);
                $('#' + cntr).replaceWith($('#' + cntr).val('').clone(true));
                $('#' + cntr).focus()
            }
            else if (parseInt(ftype) == 3) {
                bootbox.alert("<strong>Please upload a valid pdf file</strong>");
                $('#fuUploadPocPrev').text("Choose File");
                $('#' + cntr).replaceWith($('#' + cntr).val('').clone(true));
                $('#' + cntr).focus();
            }
            else {
                bootbox.alert('Please Upload a valid document !!!');
                $('#' + cntr).replaceWith($('#' + cntr).val('').clone(true));
                $('label[id*="' + cntr + '"]').text('');
                $('#' + cntr).focus();
            }
        }
        else {
            // Restrict the file size to 2MB(1024 * 5120;)
            if ($('#' + cntr).get(0).files[0].size > (5242880)) {
                bootbox.alert("<strong>Proceeding file size should not exceed 5MB.!!!</strong>");
                $('#fuUploadPocPrev').text("Choose File");
                $('#' + cntr).replaceWith($('#' + cntr).val('').clone(true));
                $('#' + cntr).focus();
            }
            if ($('#' + cntr).get(0).files[0].name.length > 100) {
                bootbox.alert("<strong>Proceeding file Name should be maximum 100 Characters!</strong>");
                $('#fuUploadPocPrev').text("Choose File");
                $('#' + cntr).replaceWith($('#' + cntr).val('').clone(true));
                $('label[id*="' + cntr + '"]').text('');
                $('#' + cntr).focus();
            }
            else
                return true;
        }
    }
    else
        return true;
}
//Validations for the fields in the form
function validate() {
    var d = new Date();
      if ($('#ddlDepartment').val() == "0") {
        bootbox.alert("Please select department name!");
        return false;
        $('#ddlDepartment').focus();
    }
      else if ($('#txtSectorName').val() == "") {
        bootbox.alert("Please enter sector name!");
        $('#txtSectorName').focus();
        return false;
    }
  

    else {
        $("#btnCancel").hide();
        return true;
    }
        
}
//Post Call to the controller for data insertion 
function addSectorData(Data) {
    $.ajax({
        type: "POST",
        url: "/Sector/AddSector", //Controller and Method(/Controller/Method)
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: Data,
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        success: function (res) {
            var SECTORID = getUrlVars()["ID"];
            if (SECTORID != undefined) {
                bootbox.alert(res, function () {  //Alert showing through bootbox for a better look and fill
                    window.location.href = "/Sector/ViewSector?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                });
            }
            else {
                bootbox.alert(res, function () { //Alert showing through bootbox for a better look and fill
                    window.location.href = "/Sector/AddSector?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
                    //window.location.reload();
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
function bindSectorById(secId) {
    $.ajax({
        url: "/Sector/SectorGateById?SectorId=" + secId,
        type: "GET",
        dataType: "json",
        contentType: "application/json;charset=UTF-8",
        success: function (data) {
            console.log(data);
            //Filling the values in appropriate field for updating purpose
            $('#hdnSectorId').val(data[0].SECTORID);
            $('#txtSectorName').val(data[0].SECTORNAME);
            /*$('#ddlDepartment').val(data[0].Deptid);*/
            //$('#txtcssclass').val(data[0].CSSCLASS);
            $('#fuUploadPocPrev')[0].innerText = data[0].CSSCLASS;
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            bootbox.alert("Error : " + jsonValue);
        }
    });
}
//Resetting all the fields
$('#btnCancel').click(function () {
    var secId = getUrlVars()["ID"];
    if (secId != undefined) {
        location.href = "/Sector/ViewSector?Glink=" + decodeURI(getUrlVars()["Glink"]) + "&Plink=" + decodeURI(getUrlVars()["Plink"]);
    } else {
        $('#ddlDepartment').val("0");
        $('#txtSectorName').val("");
        $('#txtDate').val("");
        $('#fuUploadPocPrev').text("Choose file");
        $('#txtcssclass').val("");
        $('#btnConfirm').show();
    }
    
});
//================View And Delete Sector===================

// Data table required for so many purposes like pagination,searching printing the data showing in the grid etc.
function getDataTable() {
    $('#tblSector').DataTable({
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
function deleteSector(id) {
    bootbox.confirm({
        size: "medium",
        message: "Are you sure you want to delete ?",
        callback: function (result) {

            if (result === true) {
                $.ajax({
                    url: "/Sector/DeleteSector?id=" + id,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;charset=UTF-8",
                    success: function (data) {
                        if (data == "3") {
                            bootbox.alert("Sector Deleted Successfully", function () {
                                //window.location.href = "/Sector/ViewSector";
                                window.location.reload();
                            });

                        }
                        else if (data == "4") {
                            bootbox.alert("Sector Already In Use!", function () {
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