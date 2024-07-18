
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

function boostNow(id) {
    let pageLink = location.href
    $.ajax({
        url: '/Store/BoostNow/'+id,
        type: 'POST',
        data: {
            pageLink
        },
        success: function (data) {
            if (data.success) {
                const message = JSON.parse(data.data)
                const content = JSON.parse(message.Content)
                location.href = content.data.checkoutUrl;
                Swal.fire('Boost Your Product', 'Boosting your product can significantly increase its reach and visibility to a wider audience. Click Yes to proceed with the payment.', 'info');
            }
            else {
                Swal.fire('Error', 'Something went wrong', 'error');
            }
        },
        error: function (data) {
            Swal.fire('Error', 'Something went wrong', 'error');
        }
    })
}