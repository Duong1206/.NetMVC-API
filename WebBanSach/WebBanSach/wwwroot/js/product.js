var dataTable;
$(document).ready(function () {
    $('#tblData').DataTable(
        {
            "ajax": {
                "url": "/admin/product/getall",
                "type": "GET",
                "datatype": "json",
            },
            "columns": [
                { "data": "title",width:"15%" },
                { "data": "description",width: "15%" },
                { "data": "isbn" ,width: "15%" },
                { "data": "price50", width: "15%" },
                { "data": "price100" ,width: "15%" },
                { "data": "author", width: "15%" },
                { "data": "category.name", width: "15%" }
            ]
        }
    )
});