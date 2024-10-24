﻿var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/contact/GetAll"
        },
        "columns": [
            { "data": "name", "width": "5%" },
            { "data": "map", "width": "5%" },
            { "data": "address", "width": "5%" },
            { "data": "email", "width": "5%" },
            { "data": "phone", "width": "55%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Admin/Contact/Upsert?id=${data}" class="btn btn-success text-white" style="cursor:pointer">
                           Edit
                        </a>
                        <a onclick=Delete('/Admin/Contact/Delete?id=${data}') class="btn btn-danger text-white" style="cursor:pointer">
                          Delete
                        </a>
                    </div>
                    `;
                },
                "width": "5%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You will not be able to recover this record!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Yes, delete it!",
        closeOnConfirm: false
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
