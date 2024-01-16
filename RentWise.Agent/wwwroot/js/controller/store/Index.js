
function filterCategory(lkpCategory) {
    if (lkpCategory == 0) {
        location.href = `/Store/Index`
    } else {
        location.href = `/Store/Index/${lkpCategory}`
    }
    }
    const lkpCategory = location.href.split('/').reverse()[0]
    if (lkpCategory.toLowerCase() == 'store' || lkpCategory.toLowerCase() == 'index') {
        $('.0').prop('checked', true)
    } else {
        $('.' + lkpCategory).prop('checked', true)
}
function search() {
    const name = $('.product-search').val()
    location.href = buildQueryParams('name',name)
}
function deleteProduct(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You won\'t be able to revert this!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            location.href = "/Store/ModifyProduct?Id="+id+"&type=DELETE"
        }
    });
}

function resumeProduct(id) {
   location.href = "/Store/ModifyProduct?Id=" + id + "&type=ENABLE"
}

function pauseProduct(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You listing will not be visible to potential client until you resume!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#FFD700',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, pause it!'
    }).then((result) => {
        if (result.isConfirmed) {
            location.href = "/Store/ModifyProduct?Id=" + id + "&type=DISABLE"
        }
    });
}