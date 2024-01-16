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
function chooseCurrentLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            (position) => {
                const userLocation = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                getAddress(userLocation.lat, userLocation.lng);
            },
            (error) => {
                // Error callback
                console.error("Error getting user location:", error.message);
            }
        );
    } else {
        alert("Geolocation is not supported by your browser.");
    }
}

function getAddress(lat, lng) {
    const geocoder = new google.maps.Geocoder();

    geocoder.geocode({ 'location': { lat, lng } }, (results, status) => {
        if (status === 'OK') {
            if (results[0]) {
                document.getElementById('autocomplete-input').value = results[0].formatted_address;
                getLatLng()
            } else {
                console.error('No results found');
            }
        } else {
            console.error('Geocoder failed due to: ' + status);
        }
    });
}