$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#myTable').DataTable({
        "ajax": { url:'/admin/company/getall' },
        "columns": [
            { data: 'name',"width":"10%" },
            { data: 'streetAddress', "width": "10%" },
            { data: 'city', "width": "10%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id', "render": function (data) {
                    return `<div class="w-75 btn btn-group " role="group">  
                   <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit </a>
                   <a href="/admin/company/delete?id=${data}" class="btn btn-danger mx-2"><i class="bi bi-trash"></i> Delete </a>
                    </div >`


                },
                
                "width": "20%"
            }
        ]
    });

};


    

                

