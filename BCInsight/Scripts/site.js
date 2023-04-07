//Reset all validation message and filds 14-sep-2017 
$("input[type=reset]").click(function () {

    // get the form inside we are working - change selector to your form as needed
    var $form = $("form");

    // get validator object
    var $validator = $form.validate();

    // get errors that were created using jQuery.validate.unobtrusive
    var $errors = $form.find(".field-validation-error span");

    // trick unobtrusive to think the elements were succesfully validated
    // this removes the validation messages
    $errors.each(function () { $validator.settings.success($(this)); });

    // clear errors from validation
    $validator.resetForm();
});

//For Display Notification Messages
function Notyfication(type, message, layout) {
    var n = noty({
        text: message,
        type: type,
        dismissQueue: true,
        timeout: 10000,
        closeWith: ['click'],
        layout: layout,
        theme: 'defaultTheme',
        maxVisible: 10
    });
    console.log('html: ' + n.options.id);
}

function Site() {
    var self = this;
    self.Notification = function () {
        var $notificationBox = $('#NotificationBox');
        if ($notificationBox.length) {
            $("#NotificationBox").fadeIn(1000);
            $("#NotificationBox").delay(1000).fadeOut(1000);
        }
    }
}

var $site = new Site();
$(function () {
    callAllMethods($site);
});

function callAllMethods(obj) {
    // call every method of the given object
    for (var method in obj) {
        if (obj.hasOwnProperty(method) && typeof (obj[method]) === "function") {
            obj[method]();
        }
    }
}

$('#salestable').DataTable(
    {
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "cellspacing": 0,
        "align": "center",
        "width": "100%",       
        "columnDefs": [
            { "width": 100,"targets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18], className: "text-center" }
        ],
        "language":
        {
            "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>",
            "emptyTable": "sales not found"
        },
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": "/SalesMgmt/GetData",
            "type": "POST",
            "dataType": "JSON"
        },
        "columns": [
            {
                "data": "saleDate",
                "render": function (data) {
                    var dateString = data.substr(6);
                    var currentTime = new Date(parseInt(dateString));
                    var month = currentTime.getMonth() + 1;
                    var day = currentTime.getDate();
                    var year = currentTime.getFullYear();
                    if (month < 10) {
                        month = "0" + month;
                    }
                    if (day < 10) {
                        day = "0" + day;
                    }
                    var date = day + "/" + month + "/" + year;                    
                    return '<a class="popup" href="/SalesMgmt/ExportExcel?date=' + date + '">' + date + '</a>';
                   // return '<a class="popup" onclick="Exporttoexcelsale(' + this.data +')">' + date + '</a>';
                }
            },
            { "data": "billNo" },
            { "data": "department" },
            { "data": "product" },
            { "data": "brandName" },
            { "data": "cat6" },
            { "data": "cat2" },
            { "data": "salesPersonNo" },
            { "data": "saleQty" },
            { "data": "mrpAmt" },
            { "data": "netAmt" },
            { "data": "PrmoAmt" },
            { "data": "ItemDesc6" },
            { "data": "ItemDiscountAmt" },
            { "data": "BillDiscountAmt" },
            { "data": "LPDiscountAmt" },
            { "data": "ExTaxAmtFactor" },
            { "data": "pvt_label_group_id" },
            { "data": "vendorName" }            
        ]
    });

$('#stocktable').DataTable(
    {
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "cellspacing": 0,
        "align": "center",
        "width": "100%",
        "columnDefs": [
            { "width": 100, "targets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16], className: "text-center" }
        ],
        "language":
        {
            "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>",
            "emptyTable": "Stock not found"
        },
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": "/StockManagement/GetData",
            "type": "POST",
            "dataType": "JSON"
        },
        "columns": [
            { "data": "stock_id"},
            { "data": "barcode" },
            { "data": "site" },
            { "data": "division" },
            { "data": "section" },
            { "data": "department" },
            { "data": "brand" },
            { "data": "styleCode" },
            { "data": "color" },
            { "data": "size" },
            { "data": "fit" },
            { "data": "closingTotal" },
            { "data": "category2" },
            { "data": "desc4" },
            { "data": "desc6" },
            { "data": "mrp" },
            { "data": "siteCuid" }
        ]
    });

$('#slabstable').DataTable(
    {
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "cellspacing": 0,
        "align": "center",
        "width": "100%",
        "columnDefs": [
            { "width": 100, "targets": [0, 1, 2, 3, 4, 5], className: "text-center" }
        ],
        "language":
        {
            "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>",
            "emptyTable": "Slab not found"
        },
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": "/SlabMgmt/GetData",
            "type": "POST",
            "dataType": "JSON"
        },
        "columns": [            
            { "data": "salesPersonNo" },
            { "data": "finYear" },
            { "data": "qtrNo" },
            { "data": "slabNo" },
            { "data": "slabAmount" },
            { "data": "monthlyIncentive" }
        ]
    });

$('#Baseqtytable').DataTable(
    {
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "cellspacing": 0,
        "align": "center",
        "width": "100%",
        "columnDefs": [
            { "width": 100, "targets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], className: "text-center" }
        ],
        "language":
        {
            "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>",
            "emptyTable": "Quantity not found"
        },
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": "/QuantityMgmt/GetData",
            "type": "POST",
            "dataType": "JSON"
        },
        "columns": [            
            { "data": "barcode" },
            { "data": "siteCuid" },
            { "data": "division" },
            { "data": "section" },
            { "data": "department" },
            { "data": "brand" },
            { "data": "styleCode" },
            { "data": "color" },
            { "data": "fit" },
            { "data": "size" },
            { "data": "baseQty" },
            { "data": "combination" }
        ]
    });

$('#WeeklyTargetIncentivetable').DataTable(
    {
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "cellspacing": 0,
        "align": "center",
        "width": "100%",
        "columnDefs": [
            { "width": 100, "targets": [0, 1, 2, 3, 4, 5, 6, 7], className: "text-center" }
        ],
        "language":
        {
            "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>",
            "emptyTable": "Weekly Target Incentive not found"
        },
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": "/WeeklyTargetIncentiveMgmt/GetData",
            "type": "POST",
            "dataType": "JSON"
        },
        "columns": [            
            { "data": "salesPerson" },
            { "data": "finYear" },
            { "data": "weekNo" },
            { "data": "weekTargetAmt" },
            { "data": "incentivePercent" },
            { "data": "weekSalesAmt" },
            { "data": "incentiveEarnedAmt" },
            {
                "data": "id",
                "width": "50px",
                "render": function (data) {
                    return '<a class="popup" href="WeeklyTargetIncentiveMgmt/updateTarget/' + data + '"><i class="fas fa-edit"></i></a>';
                },
                "orderable": false
            }
            
        ]
    });

$('#attendancetable').DataTable(
    {
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "cellspacing": 0,
        "align": "center",
        "width": "100%",
        "columnDefs": [
            { "width": 100, "targets": [0, 1, 2, 3, 4, 5, 6, 7], className: "text-center" }
        ],
        "language":
        {
            "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>",
            "emptyTable": "Weekly Target Incentive not found"
        },
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": "/VendorMgmt/GetData",
            "type": "POST",
            "dataType": "JSON"
        },
        "columns": [
            { "data": "salePersonNo" },
            {
                "data": "AttendanceDate",
                "render": function (data) {
                    var dateString = data.substr(6);
                    var currentTime = new Date(parseInt(dateString));
                    var month = currentTime.getMonth() + 1;
                    var day = currentTime.getDate();
                    var year = currentTime.getFullYear();
                    if (month < 10) {
                        month = "0" + month;
                    }
                    if (day < 10) {
                        day = "0" + day;
                    }
                    var date = day + "/" + month + "/" + year;
                    return date;
                }
            },
            { "data": "status" },
            { "data": "firstintime" },
            { "data": "secondouttime" },
            { "data": "thirdintime" },
            { "data": "fourthouttime" },
            { "data": "totalHours" }            
        ]
    });

//$('#tableadminLog').DataTable(
//    {
//        "responsive": true,
//        "lengthChange": true,
//        "autoWidth": false,
//        "cellspacing": 0,
//        "align": "center",
//        "width": "100%",
//        "columnDefs": [
//            { "width": 100, "targets": [0, 1, 2, 3, 4, 5, 6], className: "text-center" }
//        ],
//        "language":
//        {
//            "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>",
//            "emptyTable": "Log not found"
//        },
//        "processing": true,
//        "serverSide": true,
//        "ajax":
//        {
//            "url": "/Home/GetData",
//            "type": "POST",
//            "dataType": "JSON"
//        },
//        "columns": [
//            { "data": "SectionName" }
//            { "data": "ModuleName" },
//            { "data": "Action" },
//            { "data": "Description" },
//            { "data": "UserId" },
//            { "data": "Ip" },
//            {
//                "data": "LogDate",
//                "render": function (data) {
//                    var dateString = data.substr(6);
//                    var currentTime = new Date(parseInt(dateString));
//                    var month = currentTime.getMonth() + 1;
//                    var day = currentTime.getDate();
//                    var year = currentTime.getFullYear();
//                    var date = day + "/" + month + "/" + year;
//                    return date;
//                }
//            }
//        ]
//    });
