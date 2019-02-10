//Trim Bin Code before "-" and auto fill that value to WH
function TrimBinCode(binCode) {
    var whCode = "";
    var n = binCode.indexOf('-');
    //whCode = binCode.substring(0, n != -1 ? n : 0);
    whCode = binCode.substring(0, n != -1 ? n : n.Length);
    return whCode;
}

//https://stackoverflow.com/questions/19233415/how-to-make-type-number-to-positive-numbers-only
$(document).ready(function () {
    Calculate();

    $('#QtyPerContainer').change(function () {
        // Calculate Number of Tags.
        // NumberOfTags = TagQty(Fix) / QtyPerContainer(Enable modifying)
        // Round to higher int
        Calculate();

    });

    function Calculate(data) {
        var maxQtyVal = parseFloat($('#MaxQty').val());
        var qtyPerContainer = $("#QtyPerContainer").val();
        var tagQty = $("#TagQty").val();
        var numberOfTags = $("#NumberOfTags").val();

        if ((qtyPerContainer != "") && (qtyPerContainer > 0) && (qtyPerContainer <= maxQtyVal)) {
            numberOfTags = tagQty / qtyPerContainer;
            numberOfTags = Math.ceil(numberOfTags);
            $("#NumberOfTags").val(numberOfTags);
        }
        //else {
        //    //alert("MaxQty: " + maxQtyVal + "qtyPerContainer: " + qtyPerContainer);
        //    $('#alertModal').modal();
        //}
    }

    /*
    //https:/ / stackoverflow.com / questions / 23505985 / asp - net - mvc - success - alert - after - form - submit
    $('#PrintTagSubmit').click(function () {
        alert("submit");
        var form = $('#PrintTagForm');
        $.ajax({
            type: "POST",
            url: "POReceipt/Update",
            data: form.serialize(),
            success: function (data) {
                alert(data);
                alert(data.Message);
                //https://stackoverflow.com/questions/1960240/jquery-ajax-submit-form
                //alert('Result3: ' + data);
            },

            error: function () {
                alert("Something went wrong. Maybe the server is offline?");
            }
        });
    });
    */


    //$('#PrintTagSubmit').click(function () {
    //    alert("Click");
    //    var printTagObj = {
    //        Id: 1,
    //        Company: "A",
    //        PONum: "1",
    //        POLine: "1",
    //        QtyPerContainer: "10",
    //        Uom: "A",
    //        TransactionQty: "1.0",
    //        TagQty: "1.0",
    //        NumberOfTags: "1"
    //    }

    //    $.ajax({
            
    //        url: 'POReceipt/Update',
    //        type: 'post',
    //        data: JSON.stringify(printTagObj),
    //        success: function (data) {
    //            alert(data);
    //            alert(data.msg);
    //        },
            
    //    });

    //    alert("End");
    //});

    //Ramesh 

    $("#pickRawMtrlDtlTable").DataTable(
        {
            paging: true,
            sort: true,
            searching: false,
            "pageLength": 5,
            "bLengthChange": false,
            "dom": '<"top"i>rt<"bottom"flp><"clear">'
            //scrollY: 200

        }
    );
    $("#warehseTable").DataTable(
        {
            paging: true,
            sort: true,
            searching: false,
            "pageLength": 5,
            "bLengthChange": false,
            "dom": '<"top"i>rt<"bottom"flp><"clear">'
            //scrollY: 200

        }
    );
    //$("#FGTopUpTable").DataTable(
    //    {
    //        paging: true,
    //        sort: true,
    //        searching: false,
    //        //"pageLength": 5,
    //        "bLengthChange": false,
    //        "dom": '<"top"i>rt<"bottom"flp><"clear">'
    //        //scrollY: 200

    //    }
    //);
    //Ramesh 

/********************WO LOOSE PICKING JS***********************/
//https://stackoverflow.com/questions/4220126/run-javascript-function-when-user-finishes-typing-instead-of-on-key-up

    $('#WOLPSOTable').on('click', '.SOProcessingModalCall', function () {
        var soNum = $(this).val();
        var msg = soNum + " is currently in picking process."
        var model = $('#alertModal').modal();
        model.find('.modal-title').text("Warning");
        model.find('.modal-body').text(msg);
    });


    $('#WOLPSODetailTable').on('click', '.LineProcessingButton', function () {
        var line = $(this).val();
        var msg = "This line (" + line + ") is currently processing."
        var model = $('#alertModal').modal();
        model.find('.modal-title').text("Warning");
        model.find('.modal-body').text(msg);
    });

    $('#CloseScanning').click(function () {
        var partNum = $(this).val();
        window.location.href = "PalletSelection?id=" + partNum;
    });

    $('#SaveScanning').click(function () {

    });

});

//TEST EXPORT CSV
//https://www.codexworld.com/export-html-table-data-to-csv-using-javascript/
function downloadCSV(csv, filename) {
    var csvFile;
    var downloadLink;

    // CSV file
    csvFile = new Blob([csv], { type: "text/csv" });

    // Download link
    downloadLink = document.createElement("a");

    // File name
    downloadLink.download = filename;

    // Create a link to the file
    downloadLink.href = window.URL.createObjectURL(csvFile);

    // Hide download link
    downloadLink.style.display = "none";

    // Add the link to DOM
    document.body.appendChild(downloadLink);

    // Click download link
    downloadLink.click();
}

function exportTableToCSV(filename) {
    var csv = [];
    var rows = document.querySelectorAll("table tr");

    for (var i = 0; i < rows.length; i++) {
        var row = [], cols = rows[i].querySelectorAll("td, th");

        for (var j = 0; j < cols.length; j++)
            row.push(cols[j].innerText);

        csv.push(row.join(","));
    }

    // Download CSV file
    downloadCSV(csv.join("\n"), filename);
}
//TEST EXPORT CSV//