
var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/product/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "quantity", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
        <div class="text-center">
            <a href="/Admin/Product/Detail?id=${data}" class="btn btn-info text-white" style="cursor:pointer">
                <i class="fas fa-eye"></i>
            </a>
            <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-success text-white" style="cursor:pointer">
                <i class="fas fa-edit"></i>
            </a>
            <a onclick=Delete('/Admin/Product/Delete?id=${data}') class="btn btn-danger text-white" style="cursor:pointer">
                <i class="fas fa-trash-alt"></i>
            </a>
        </div>
        `;
                },
                "width": "15%"
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
