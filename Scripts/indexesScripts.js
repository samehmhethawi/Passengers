$(document).ready(function () {
    //$("#NAME").on('input', function () {
    //    var val = this.value;
    //    alert(val);
    //    jQuery.trim(val);
    //    alert(val);
    //    return val;
    //});
    //$("#NAME").bind("change", function () {
    //    $(this).val(function (_, v) {
    //        alert(v);
    //        jQuery.trim(v);
    //        alert(v);
    //        return v;
    //    });
    //});
    //$('form').on('submit', function (event) {
    //    event.preventDefault();
    //    var str = $("#NAME").val();
    //    alert(str);
    //});
    //$("#modal .save-btn").on("click", function () {
    //    var str = $("#NAME").val();
    //    alert(str);
    //}); 
    //$("#modal").submit(function (event) {
    //    alert("Handler for .submit() called.");
    //    event.preventDefault();
    //});
    
    var hideSearchPanel = $("#HideSearchPanel").val();
    if (hideSearchPanel == "True") {
        $("#IndexSearchPanel").hide();
        $("#TitleDiv").hide();
    }
    else {
        $("#IndexSearchPanel").show();
        $("#TitleDiv").show();
        setTimeout(function () {
            $("#grid").data("kendoGrid").dataSource.read();
        }, 500);
    }
    $("#AddNewBtn").click(function () {
        $('#loadingContainer').fadeIn();
        //var url = $(this).data('url');
        var url = $("#myUrlForCreate").val();
        $.ajax({
            type: "GET",
            url: url,
            data: {},
            success: function (data) {
                $('#modalContent').html(data);
                setTimeout(function () {
                    $('#modalContent').find('.modal').modal();
                }, 500);
            }
        });
    });
    $("#AddNewForDetialBtn").click(function() {
        $('#loadingContainer').fadeIn();
        //var url = $(this).data('url');
        var url = $("#myUrlForCreateForDetail").val();
        $.ajax({
            type: "GET",
            url: url,
            data: {},
            success: function(data) {
                $('#modalContent').html(data);
                setTimeout(function() {
                    $('#modalContent').find('.modal').modal();
                }, 500);
            }
        });
    });
    $filter_main = new Array();
    $("#btn-search").click(function () {
        var Name = $("#type_name").val();
        var kindNb = $("#KindNNb").val();
        var docNo = $("#docNo").val();
        var docDate = $("#SDECDATE").val();
        var EName = $("#S_ENAME").val();
        var SNattion = $("#S_NATTION").val();
        var SRegNb = $("#RegNNb").val();
        var SAttribNb = $("#AttribNNb").val();
        var SCarTegoryNb = $("#CATEGORYNNB").val();
        var STYP = $("#STYP").val();
        var SOwnerTyp = $("#OwneKindNb").val();
        var SONNB = $("#ONNB").val(); 
        var SGNNB = $("#SGNNB").val();
        var SAMOUNT = $("#SAMOUNT").val();
        var SIEZTYP = $("#SIEZTYP").val();
        var SCITYNB = $("#SCITYNB").val();
        $filter_main = new Array();
        if (Name != "" && Name != null && Name != undefined) {
            $filter_main.push({ field: "NAME", operator: "contains", value: Name });
        }
        if (EName != "" && Name != null && EName != undefined) {
            $filter_main.push({ field: "ENAME", operator: "contains", value: EName });
        }
        if (SNattion != "" && SNattion != null && SNattion != undefined) {
            $filter_main.push({ field: "NATNB", operator: "equal", value: SNattion });
        }
        if (kindNb != "" && kindNb != null && kindNb != undefined) {
            $filter_main.push({ field: "KINDNB", operator: "equal", value: kindNb });
        }
        if (docNo != "" && docNo != null && docNo != undefined) {
            $filter_main.push({ field: "DECNO", operator: "contains", value: docNo });
        }
        if (docDate != "" && docDate != null && docDate != undefined) {
            $filter_main.push({ field: "DECDATE", operator: "equal", value: docDate });
        }
        if (SRegNb != "" && SRegNb != null && SRegNb != undefined) {
            $filter_main.push({ field: "ZREGNB", operator: "equal", value: SRegNb });
        }
        if (SAttribNb != "" && SAttribNb != null && SAttribNb != undefined) {
            $filter_main.push({ field: "ATTRIBNB", operator: "equal", value: SAttribNb });
        }
        if (SCarTegoryNb != "" && SCarTegoryNb != null && SCarTegoryNb != undefined) {
            $filter_main.push({ field: "CATEGORYNB", operator: "equal", value: SCarTegoryNb });
        }
        if (STYP != "" && STYP != null && STYP != undefined) {
            $filter_main.push({ field: "TYP", operator: "equal", value: STYP });
        }
        if (SOwnerTyp != "" && SOwnerTyp != null && SOwnerTyp != undefined) {
            $filter_main.push({ field: "KIND", operator: "equal", value: SOwnerTyp });
        }
        if (SONNB != "" && SONNB != null && SONNB != undefined) {
            $filter_main.push({ field: "ONB", operator: "equal", value: SONNB });
        }
        if (SGNNB != "" && SGNNB != null && SGNNB != undefined) {
            $filter_main.push({ field: "GNB", operator: "equal", value: SGNNB });
        } 
        if (SAMOUNT != "" && SAMOUNT != null && SAMOUNT != undefined) {
            $filter_main.push({ field: "AMOUNT", operator: "equal", value: SAMOUNT });
        } 
        if (SIEZTYP != "" && SIEZTYP != null && SIEZTYP != undefined) {
            $filter_main.push({ field: "SIEZSTAUS", operator: "equal", value: SIEZTYP });
        } 
        if (SCITYNB != "" && SCITYNB != null && SCITYNB != undefined) {
            $filter_main.push({ field: "CITYNB", operator: "equal", value: SCITYNB });
        } 
        $("#grid").data("kendoGrid").dataSource.filter($filter_main);
        $("#grid").data("kendoGrid").dataSource.read();
    });

    $("#btn-clear").click(function () {
        $("#type_name").val("");
        $("#docNo").val("");
        $("#SDECDATE").val("");
        $("#S_ENAME").val("");
        $("#STYP").val("");
        $("#SAMOUNT").val("");
        $('#SGNNB').val('').trigger('chosen:updated');
        $('#KindNNb').val('').trigger('chosen:updated');
        $('#S_NATTION').val('').trigger('chosen:updated');
        $('#RegNNb').val('').trigger('chosen:updated');
        $('#AttribNNb').val('').trigger('chosen:updated');
        $('#CATEGORYNNB').val('').trigger('chosen:updated');
        $('#OwneKindNb').val('').trigger('chosen:updated');
        $('#ONNB').val('').trigger('chosen:updated');
        $('#SIEZTYP').val('').trigger('chosen:updated');
        $('#SCITYNB').val('').trigger('chosen:updated');
        });
   
   

})
$(function () {
    $(".save-btn").click(function () {
        var val = $("#NAME").val();
        $("#NAME").val(jQuery.trim(val));
    });
    try {
        $("#KindNNb").chosen({ "width": "300px", no_results_text: "لم يتم العثور على النوع الرئيسي" });
        $("#KINDNB").chosen({ "width": "300px", no_results_text: "لم يتم العثور على النوع الرئيسي" });
        $("#S_NATTION").chosen({ "width": "200px", no_results_text: "لم يتم العثور على الجنسية" });
        $("#NATNB").chosen({ "width": "200px", no_results_text: "لم يتم العثور على الجنسية" });
        $("#RegNNb").chosen({ "width": "200px", no_results_text: "لم يتم العثور على الفئة الرئيسية" });
        $("#AttribNNb").chosen({ "width": "400px", no_results_text: "لم يتم العثور على المواصفة" });
        $("#CATEGORYNNB").chosen({ "width": "200px", no_results_text: "لم يتم العثور على فئةالمواصفة" });
        $("#OwneKindNb").chosen({ "width": "200px", no_results_text: "لم يتم العثور على النوع الرئيسي" });
        $("#ONNB").chosen({ "width": "300px", no_results_text: "لم يتم العثور على الجهة" });
        $("#SGNNB").chosen({ "width": "300px", no_results_text: "لم يتم العثور على مجموعة الجهة" });
        $("#SCITYNB").chosen({ "width": "200px", no_results_text: "لم يتم العثور على المحافظة" });
    } catch (e) {

    }
});
function Edit(e) {
    var tr = $(e.target).closest("tr");    // get the current table row (tr)
    var item = this.dataItem(tr);          // get the date of this row
    var id = item.NB;
    var url = $("#myUrlForEdite").val();
    $.ajax({
        type: "GET",
        url: url,
        data: { id: id },
        success: function (data) {
            try {
                $("#modalContent").html(data);
                setTimeout(function () {
                    $("#modalContent").find(".modal").modal();
                }, 500);
            } catch (ee) {
            }
        },
    });
}
function showDetails(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var url = $("#myUrlDetails").val() + "/" + dataItem.NB;
    window.open(url);
}
function showDetailss(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var url = $("#myUrlDetailss").val() + "/" + dataItem.NB;
    window.open(url);
}
function showDetailsForZownerDetials(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var url = $("#myUrlForZownerDetails").val() + "/" + dataItem.NB;
    window.open(url);
}
function showDetailsForZgroupSidesDetails(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var url = $("#myUrlForZgroupSidesDetails").val() + "/" + dataItem.NB;
    window.open(url);
}
function CopyToOldCarChange(e) {
    e.preventDefault();
    var tr = $(e.target).closest("tr");    // get the current table row (tr)
    var item = this.dataItem(tr);          // get the date of this row
    var id = item.NB;
    var url = $("#myUrlForCopyToOldCarChange").val();
    $.ajax({
        type: "GET",
        url: url,
        data: { id: id },
        success: function (data) {
            try {
                $("#modalContent").html(data);
                setTimeout(function () {
                    $("#modalContent").find(".modal").modal();
                }, 500);
            } catch (ee) {
            }
        },
    });
}
function Delete(e) {
    e.preventDefault();
    var tr = $(e.target).closest("tr");    // get the current table row (tr)
    var item = this.dataItem(tr);          // get the date of this row
    var id = item.NB;
    var url = $("#myUrlForDelete").val();
    $.ajax({
        type: "GET",
        url: url,
        data: { id: id },
        success: function (data) {
            try {
                $("#modalContent").html(data);
                setTimeout(function () {
                    $("#modalContent").find(".modal").modal();
                }, 500);
            } catch (ee) {
            }
        },
    });
}
function Copy(e) {
    e.preventDefault();
    var tr = $(e.target).closest("tr");    // get the current table row (tr)
    var item = this.dataItem(tr);          // get the date of this row
    var id = item.NB;
    var url = $("#myUrlForCopy").val();
    $.ajax({
        type: "GET",
        url: url,
        data: { id: id },
        success: function (data) {
            try {
                $("#modalContent").html(data);
                setTimeout(function () {
                    $("#modalContent").find(".modal").modal();
                }, 500);
            } catch (ee) {
            }
        },
    });
}
function setIcons() {
    
    $(".k-grid-Copy").addClass("btn btn-success hvr-box-shadow-outset");
    $(".k-grid-Delete").addClass("btn btn-danger hvr-box-shadow-outset");
    $(".k-grid-Details").addClass("btn btn-info hvr-box-shadow-outset");
    $(".k-grid-Edit").addClass("btn btn-success hvr-box-shadow-outset");
    $(".k-grid-content").find("table").addClass("table-striped table-hover");
    $(".k-grid-Details2").addClass("btn btn-primary hvr-box-shadow-outset");
    $(".k-grid-DetailsForZowners").addClass("btn btn-info hvr-box-shadow-outset");
    $(".k-grid-DetailsForZgroupSidesDetails").addClass("btn btn-info hvr-box-shadow-outset");
    $(".k-grid-Delete").kendoButton({
        icon: "k-icon k-i-trash"
    });

    $(".k-grid-Edit").kendoButton({
        icon: "k-icon k-i-edit"
    });

    $(".k-grid-Copy").kendoButton({
        icon: "k-icon k-i-copy"
    });
    $(".k-grid-Details").kendoButton({
        icon: "k-icon k-i-info"
    });
    $(".k-grid-DetailsForZowners").kendoButton({
        icon: "k-icon k-i-info"
    });
    $(".k-grid-DetailsForZgroupSidesDetails").kendoButton({
        icon: "k-icon k-i-info"
    });
    $(".k-grid-Details2").kendoButton({
        icon: "k-icon k-i-cog"
    });
    $(".k-grid-Details2").kendoTooltip({
        content: "تفاصيل المواصفات الجديدة للتبدل",
        position: "left",
        autoHide: true,
    });

    $(".k-grid-Edit").kendoTooltip({
        content: "تعديل",
        position: "left",
        autoHide: true,
    });
    $(".k-grid-Delete").kendoTooltip({
        content: "حذف",
        position: "left",
        autoHide: true,
    });

    $(".k-grid-Details").kendoTooltip({
        content: "تفاصيل المواصفات  للتبدل",
        position: "left",
        autoHide: true,
    });
    $(".k-grid-Copy").kendoTooltip({
        content: "نسخ  التبدل الفني",
        position: "left",
        autoHide: true,
    });
    $(".k-grid-DetailsForZgroupSidesDetails").kendoTooltip({
        content: "جهات المجموعة",
        position: "left",
        autoHide: true,
    });
    $(".k-grid-DetailsForZowners").kendoTooltip({
        content: "تصنيف المالكين لهذا النوع",
        position: "left",
        autoHide: true,
    });
    var grid = $("#grid").data("kendoGrid");
    var dataSource = grid.dataSource;
    var totalRecords = dataSource.total();
    document.getElementById('ROWSCOUNTLABEL').innerHTML = "عدد السجلات : ";
    document.getElementById('ROWSCOUNT').innerHTML = totalRecords;
    $(".k-button").removeClass("k-button-icontext");
    if ($(".k-button").hasClass("btn-info")) {
        $(".k-button.btn-info").removeClass("btn-info").addClass("btn-secondary");
    }
    //var columnsWidth = 0;
    //for (var i = 0; i < grid.columns.length; i++) {
    //    if (i < grid.columns.length - 1) { grid.autoFitColumn(i); }
    //    if (i < grid.columns.length - 1) { columnsWidth += grid.columns[i].width; }
    //    if (i == grid.columns.length - 1) {
    //        var wdt = window.innerWidth;
    //        grid.columns[i].width = wdt - columnsWidth;
    //    }
    //}
}
function hideIdField(e) {
    e.container.find("label[for=ORDR]").parent(".editor-label").hide();
    e.container.find("#ORDR").parent(".editor-field").hide();
    $("#ORDR").closest(".k-widget").hide();
}
function sync_handler() {

    this.read();
}
$(function () {
    $(".search-btn1").click(function () {
        try {
            $("#grid").data("kendoGrid").dataSource.page(1);//.read();
            $("#pageHasFilteredIco").fadeIn();
        } catch (e) {
            console.log(e);
        }
    });
    $("#type_name").off("keydown");
    $("#type_name").keydown(function (e) {
        if (e.keyCode == 13) { //13: Enter Key
            $(".search-btn1").trigger("click");
            $("#pageHasFilteredIco").fadeIn();
        }
    });
});