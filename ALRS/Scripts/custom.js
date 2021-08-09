$.ajaxSetup({ cache: false }); 

$(document).ready(function () {

    $("form").submit(function () {
        $('input[type=submit]', this).attr('disabled', 'disabled');
    });

    $("form").submit(function () {
        $('input[type=btn-primary]', this).attr('disabled', 'disabled');
    });

    $(function () {
        $("#ALRSApproverComments").focus();
    });

    $(function () {

        var options = {
            "appIconUrl": '../Content/images/ALRS.PNG', //"@ViewBag.SPHostUrl/siteassets/doj_logo.png",
            "appTitle": "Additional Leave Request System",
            //"appHelpPageUrl": "Help.html?"
            //+ document.URL.split("?")[1],
            "settingsLinks": [
                //{
                //    "linkUrl": "Account.html?"
                //        + document.URL.split("?")[1],
                //    "displayName": "Account settings"
                //},
                //{
                //    "linkUrl": "Contact.html?"
                //        + document.URL.split("?")[1],
                //    "displayName": "Contact us"
                //}
            ]
        };


        var nav = new SP.UI.Controls.Navigation("chrome_ctrl_container", options);
        nav.setVisible(true);
    });

    //$(".openPopup").live("click", function (e) { 
    //    e.preventDefault(); 
    //    $('<div></div><p>') 

    //        .addClass("dialog") 
    //        .attr("id", $(this) 
    //        .attr("data-dialog-id")) 
    //        .appendTo("body") 
    //        .dialog({ 
    //            title: $(this).attr("data-dialog-title"), 
    //            close: function () { $(this).remove(); }, 
    //            modal: true, 
    //            height: 250, 
    //            width: 900, 
    //            left: 0 
    //        }) 

    //        .load(this.href); 
    //}); 

    //$(".close").live("click", function (e) { 
    //    e.preventDefault(); 
    //    $(this).closest(".dialog").dialog("close"); 
    //}); 


    $("#AssociateApproverDataList").click(function () {

            $.getJSON($('#AssociateApprover').attr("data-autocompleteme"), function (data) {
            var items;
                $.each(data, function (i, alrs) {
                items += "<option>" + data[i] + "</option>";
            });
            $('#AssociateApprover').html(items);
        });

    });

    $("#TipStaffApproverDataList").click(function () {

        $.getJSON($('#TipStaffApprover').attr("data-autocompleteme"), function (data) {
            var items;
            $.each(data, function (i, alrs) {
                items += "<option>" + data[i] + "</option>";
            });
            $('#TipStaffApprover').html(items);
        });

    });

    $("#UsersDataList").click(function () {

        $.getJSON($('#UserName').attr("data-autocompleteme"), function (data) {
            var items;
            $.each(data, function (i, alrs) {
                items += "<option>" + data[i] + "</option>";
            });
            $('#UserName').html(items);
        });

    });

    $("#ALRSAlternateSelect").change(function () {
        $('#ALRSName').val($(this).val());
        $('#ALRSAlternateSelect').val('Select');
        document.getElementById('ALRSStartDate').focus();
    });


    TableTools.BUTTONS.Back = $.extend(true, TableTools.buttonBase, {
        "sAction": "div",
        "sTag": "default",
        "sToolTip": "Back to main list",
        "sNewLine": " ",
        "sButtonText": "Back to List",
        "fnClick": function (nButton, oConfig) {
            document.location.href = window.location.href;  //window.location.href.split('?')[0];
        }
    });

    // ************* Datatables Server Side ********************/
    var oTable = $('#example').dataTable({

        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).on('click', function () {
                //document.location.href = window.location.toLocaleString() + "/ALRS/Details/" + aData[6];
                document.location.href = window.location.href.split('?')[0] + "/ALRS/Details/" + aData[6] + "?" + window.location.href.split('?')[1];
            })
        },

        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "aButtons": [
                    {
                        "sExtends": "Back",
                        "sButtonText": "Back to List"
                    },
            ]

        },

        "bLengthChange": false,
        "bPaginate": false,
        "bServerSide": true,
        "sAjaxSource": "Home/GetAjaxData",
        "bProcessing": true,
        "bFilter": true,
        "aoColumn": [

        { "sName": "Name" },
        { "sName": "Status" },
        { "sName": "Leave Type" },
        { "sName": "Start Date" },
        { "sName": "End Date" },
        { "sName": "Duration" },
        {"sName:":"Id", "bSearchable": false, "bVisible": false }

        ]
    })
      .rowGrouping({
          iGroupingColumnIndex2: 1,
      });

    $('#example_filter input').unbind();

    $('#example_filter input').bind('keyup', function (e) {
        if ($(this).val().length > 2) {
            oTable.fnFilter(this.value);
        }
    });

    //$('#example tbody').on('click', 'td.group', function () {
    $('#example tbody').on('click', 'td.subgroup', function () {
        oSettings = oTable.fnSettings();

        value2 = $(this).html().toLowerCase();
        test5 = value2.replace(/ /g, "").replace(/-/g, "");
        test1 = $(this).attr("class");
        test3 = test1.replace(/(subgroup|-)/g, "");
        test4 = test3.replace(test5, "");
        value1 = test4.replace(/ /g, "").replace(/-/g, "");


        if (oSettings != null) {
            oSettings.aoServerParams.push({
                "sName": "user",
                "fn": function (aoData) {
                    aoData.push(
                        { "name": "first_data", "value": value1 },
                        { "name": "second_data", "value": value2 }
                        );
                }
            });

            oTable.fnDraw();
        }
    });

    //Tipstaff Table
    var oTableTip = $('#exampletip').dataTable({

        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).on('click', function () {
                //document.location.href = window.location.href.split('?')[0] + "/ALRS/Details/" + aData[6]; 
                //document.location.href = window.location.toLocaleString() + "/ALRS/Details/" + aData[6];
                //var pathArray = window.location.pathname.split('/');
                //document.location.href = pathArray[0] + "/" + pathArray[1] + "/ALRS/Details/" + aData[6];
                dochref = window.location.href.replace(/HomeTipStaff/g, "/ALRS/Details/" + aData[6]);
                document.location.href = dochref;
            })
        },

        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "aButtons": [
                    {
                        "sExtends": "Back",
                        "sButtonText": "Back to List"
                    },
            ]

        },

        "bLengthChange": false,
        "bPaginate": false,
        "bServerSide": true,
        "sAjaxSource": "HomeTipStaff/GetAjaxDataTip",
        "bProcessing": true,
        "bFilter": true,
        "aoColumn": [

        { "sName": "Name" },
        { "sName": "Status" },
        { "sName": "Leave Type" },
        { "sName": "Start Date" },
        { "sName": "End Date" },
        { "sName": "Duration" },
        { "sName:": "Id", "bSearchable": false, "bVisible": false }

        ]
    })
      .rowGrouping({
          iGroupingColumnIndex2: 1,
      });

    $('#exampletip_filter input').unbind();

    $('#exampletip_filter input').bind('keyup', function (e) {
        if ($(this).val().length > 2) {
            oTableTip.fnFilter(this.value);
        }
    });

    $('#exampletip tbody').on('click', 'td.subgroup', function () {
        oSettings = oTableTip.fnSettings();

        value2 = $(this).html().toLowerCase();
        test5 = value2.replace(/ /g, "").replace(/-/g, "");
        test1 = $(this).attr("class");
        test3 = test1.replace(/(subgroup|-)/g, "");
        test4 = test3.replace(test5, "");
        value1 = test4.replace(/ /g, "").replace(/-/g, "");


        if (oSettings != null) {
            oSettings.aoServerParams.push({
                "sName": "user",
                "fn": function (aoData) {
                    aoData.push(
                        { "name": "first_data", "value": value1 },
                        { "name": "second_data", "value": value2 }
                        );
                }
            });

            oTableTip.fnDraw();
        }
    });

    //Leave Totals Table
    var oTableLeaveTotals = $('#exampleleavetotals').dataTable({

        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).on('click', function () {
                //var pathArray = window.location.href.split('/');
                //document.location.href = pathArray[0] + "//" + pathArray[2] + "/ALRS/ALRS/Details/" + aData[6];
                dochref = window.location.href.replace(/LeaveTotals/g, "/ALRS/Details/" + aData[7]);
                document.location.href = dochref;
            })
        },

        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "aButtons": [
                    {
                        "sExtends": "Back",
                        "sButtonText": "Back to List"
                    },
            ]

        },

        "bLengthChange": false,
        "bPaginate": false,
        "bServerSide": true,
        "sAjaxSource": "LeaveTotals/GetAjaxData",
        "bProcessing": true,
        "bFilter": true,
        "aoColumn": [

        { "sName": "Year" },
        { "sName": "Status" },
        { "sName": "Leave Type" },
        { "sName": "Start Date" },
        { "sName": "End Date" },
        { "sName": "Duration" },
        { "sName:": "Id", "bSearchable": false, "bVisible": false }

        ]
    })
      .rowGrouping({
          iGroupingColumnIndex2: 1,
      });

    $('#exampleleavetotals_filter input').unbind();

    $('#exampleleavetotals_filter input').bind('keyup', function (e) {
        if ($(this).val().length > 2) {
            oTableLeaveTotals.fnFilter(this.value);
        }
    });

    $('#exampleleavetotals tbody').on('click', 'td.subgroup', function () {
        oSettings = oTableLeaveTotals.fnSettings();

        value2 = $(this).html().toLowerCase();
        test5 = value2.replace(/ /g, "").replace(/-/g, "");
        test1 = $(this).attr("class");
        test3 = test1.replace(/(subgroup|-)/g, "");
        test4 = test3.replace(test5, "");
        value1 = test4.replace(/ /g, "").replace(/-/g, "");


        if (oSettings != null) {
            oSettings.aoServerParams.push({
                "sName": "user",
                "fn": function (aoData) {
                    aoData.push(
                        { "name": "first_data", "value": value1 },
                        { "name": "second_data", "value": value2 }
                        );
                }
            });

            oTableLeaveTotals.fnDraw();
        }
    });

    //$('#example').dataTable
    //    ({
    //        "bLengthChange": false,
    //        "bPaginate": true,
    //        //"sScrollY": 400,
    //        //"bJQueryUI": true,
    //        //"sPaginationType": "full_numbers",
   
    //    })


    //    .rowGrouping({
    //        iGroupingColumnIndex: 0,
    //        iGroupingOrderByColumnIndex: 0,
    //        iGroupingColumnIndex2: 1,
    //        iGroupingOrderByColumnIndex: 0,
    //        bExpandableGrouping: true,
    //        iExpandGroupOffset: 1,
    //        bExpandableGrouping2: true,
    //        iExpandGroupOffset: 2,
    //    });

    $('#pendingapproval').dataTable
        ({
            "bLengthChange": false,
            "bPaginate": true,
            "sScrollY": 400,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",

        });
   
    $('#accessgroups').dataTable
        ({
            "bLengthChange": false,
            "bPaginate": true,
            "sScrollY": 400,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",

         })
            .rowGrouping({
                iGroupingColumnIndex: 0,
                iGroupingOrderByColumnIndex: 0,
                //iGroupingColumnIndex2: 1,
                iGroupingOrderByColumnIndex: 0,
                bExpandableGrouping: true,
                iExpandGroupOffset: 1,
                bExpandableGrouping2: true,
                iExpandGroupOffset: 2,
        });

    //$(".datepicker").datepicker({
    //    dateFormat: "dd-MM-yy",
    //    timeFormat: "hh:mm tt"
    //}).change(function () {
    //    if (this.id == 'ALRSStartDate') {
    //        var startValue = $('#ALRSStartDate').val();
    //        $('#ALRSStartDate').val(startValue);
    //    }
    //    else
    //        if (this.id == 'ALRSEndDate') {
    //            var endValue = $('#ALRSEndDate').val();;
    //            $('#ALRSEndDate').val(endValue);
    //        }

    //    if (this.id == 'ALRSEndDate') {
    //        if ($('#ALRSEndDate').val() < $('#ALRSStartDate').val()) {
    //            var startValue1 = $('#ALRSStartDate').val();
    //            $('#ALRSEndDate').val(startValue1);
    //        }
    //    }
    //});


    $(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true,
        showotherMonths: true,
        selectotherMonths: true,
        firstDay: 1,
        dateFormat: "dd/mm/yy",
        timeFormat: "hh:mm tt"
    }).change(function () {
        if (this.id == 'ALRSStartDate') {

            if ($('#ALRSStartDate').val() > $('#ALRSEndDate').val()) {
                var startValue1 = $('#ALRSStartDate').val();
                $('#ALRSEndDate').val(startValue1);
            }

            var startVal = $(this).val().substring(0, 10)
            var endValue = $('#ALRSEndDate').val().substring(0, 10)

            var startDateSplit = startVal.split("/");
            var endDateSplit = endValue.split("/");
            var stDate = new Date(startDateSplit[2], startDateSplit[1] - 1, startDateSplit[0]);
            var enDate = new Date(endDateSplit[2], endDateSplit[1] - 1, endDateSplit[0]);
            var difference = Math.round((enDate.getTime() - stDate.getTime()) / (1000 * 60 * 60 * 24) + 1, 2);
            $('#ALRSDuration').val(difference.toString()) //+ " day(s)");
            //alert($('#ALRSStartDate').val())
        }
        else
            if (this.id == 'ALRSEndDate') {

                if ($('#ALRSEndDate').val() < $('#ALRSStartDate').val()) {
                    var startValue1 = $('#ALRSStartDate').val();
                    $('#ALRSEndDate').val(startValue1);
                }

                var startVal = $('#ALRSStartDate').val().substring(0, 10)
                var endValue = $(this).val().substring(0, 10)

                if (endValue < startVal)
                {
                    $('#ALRSEndDate').text(startVal);
                }

                var startDateSplit = startVal.split("/");
                var endDateSplit = endValue.split("/");
                var stDate = new Date(startDateSplit[2], startDateSplit[1] - 1, startDateSplit[0]);
                var enDate = new Date(endDateSplit[2], endDateSplit[1] - 1, endDateSplit[0]);
                var difference = Math.round((enDate.getTime() - stDate.getTime()) / (1000 * 60 * 60 * 24) + 1, 2);
                $('#ALRSDuration').val(difference.toString()) // + " day(s)");
                $('#ALRSEndDate').val()
                //alert($('#ALRSEndDate').val())
            }
    });

    });