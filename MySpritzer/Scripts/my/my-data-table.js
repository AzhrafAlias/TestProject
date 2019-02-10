$(document).ready(function () {
    //PO Detail DataTable
    
    $('#poDetailTable').DataTable({
        //"dom": '<"top"iflp<"clear">>rt<"bottom"iflp<"clear">>',
        "dom": '<"top"iflp<"clear">>',
        bLengthChange: false,
        scrollX: true,
        searching: false,
        iDisplayLength:5,
        aLengthMenu:[[5],[5]]
    });

    $('#palletListTable').DataTable({
        searching: false, paging: false, info: false
    });

    $('#WOLPSODetailTable').DataTable({
        //"dom": '<"top"iflp<"clear">>rt<"bottom"iflp<"clear">>',
        //dom: '<"top"iflp<"clear">>',
        //bLengthChange: false,
        //scrollX: true,
        //searching: false,
        //iDisplayLength: 5,
        //aLengthMenu: [[5], [5]]
        //Ramesh
        paging: true,
        sort: true,
        searching: false,
        pageLength: 5,
        bLengthChange: false,
        dom: '<"top"i>rt<"bottom"flp><"clear">',
    });

    $('#WOLPSOTable').DataTable({
        //"dom": '<"top"iflp<"clear">>rt<"bottom"iflp<"clear">>',
        //AMIN
        //dom: '<"top"iflp<"clear">>',
        //bLengthChange: false,
        //scrollX: true,
        //searching: false,
        //iDisplayLength: 8,
        //aLengthMenu: [[8], [8]]

        //Ramesh
        paging: true,
        sort: true,
        searching: false,
        pageLength: 8,
        bLengthChange: false,
        dom: '<"top"i>rt<"bottom"flp><"clear">',
        order: [[1, "asc"]] //or asc 
        //columnDefs: [{ "targets": 1, "type": "date-eu" }]
    });



});