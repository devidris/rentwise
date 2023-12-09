let lat, lng
const category = $('#category').val()
$('.hr').addClass('d-none')
if (category) {
    $('.' + category).removeClass('d-none')
} else {
    $('.2').removeClass('d-none')
}

function selectCategory(lkpCategory) {
    location.href = buildQueryParams('Category', lkpCategory).toString();
}

function filter() {
    const min = $('.min').val()
    const max = $('.max').val()
    if (min >= max) {
        toastr.error("Mimimum price must be more than or equal to maximum price");
        return;
    }
    // Get the current URL
    const currentUrl = new URL(location.href);

    let url;

    if (min) {
        url = buildQueryParams('min', min)
    }

    if (max) {
        url = buildQueryParams('max', max, url)
    }
    if (lat && lng) {
        url = buildQueryParams('lat', lat, url, true)
        url = buildQueryParams('lng', lng, url, true)
    }

    // Set the updated URL
    location.href = url.toString();
}
