$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#myTable').DataTable({
        "ajax": { url:'/admin/product/getall' },
        "columns": [
            { data: 'name',"width":"10%" },
            { data: 'brand', "width": "10%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id', "render": function (data) {
                    return `<div class="w-75 btn btn-group " role="group">  
                   <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit </a>
                   <a href="/admin/product/delete?id=${data}" class="btn btn-danger mx-2"><i class="bi bi-trash"></i> Delete </a>
                    </div >`


                },
                
                "width": "20%"
            }
        ]
    });

};


    

                

