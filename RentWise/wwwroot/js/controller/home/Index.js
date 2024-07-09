let lat, lng
const category = $('#category').val()
$('.hr').addClass('d-none')
if (category) {
    $('.' + category).removeClass('d-none')
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

function searchName() {
    const name = $('.name-search').val()
    if (name && name.length > 0) {
        addToQueryString({name},"/store/category")
    } else {
        toastr.error("Please enter a name to search");
    }
}

// Function to update dropdown toggle text and data-value attribute
function updateDropdownToggle(element) {
    var newText = $(element).text().trim(); // Get the text content of clicked item
    var newValue = $(element).data('value'); // Get the data-value attribute of clicked item
    var dropdownToggle = $(element).closest('.dropdown').find('.dropdown-toggle span');

    dropdownToggle.text(newText); // Update dropdown toggle text with new content
    dropdownToggle.attr('data-value', newValue); // Update data-value attribute with new value
}

// Function to handle search button click
// Function to handle search button click
function handleSearch() {
    var categoryValue = $('.dropdown-category .dropdown-toggle span').attr('data-value');
    var cityValue = $('.dropdown-city .dropdown-toggle span').attr('data-value');
    var priceRangeValue = $('.dropdown-price .dropdown-toggle span').attr('data-value');
    var daysValue = $('.dropdown-days .dropdown-toggle span').attr('data-value');

    var selectedValues = {};

    // Add category if defined
    if (categoryValue) {
        selectedValues.Category = categoryValue;
    }

    // Add city if defined
    if (cityValue) {
        selectedValues.City = cityValue;
    }

    // Add price range if defined
    if (priceRangeValue) {
        var priceRange = priceRangeValue.split('-');
        selectedValues.MinPrice = priceRange[0];
        selectedValues.MaxPrice = priceRange[1] ?? 0;
    }

    // Add days range if defined
    if (daysValue) {
        var daysRange = daysValue.split('-');
        selectedValues.MinDays = parseInt(daysRange[0]);
        selectedValues.MaxDays = parseInt(daysRange[1]) ? parseInt(daysRange[1]) :  0;
    }
   addToQueryString(selectedValues, "/store/category")
}

localStorage.setItem('canStore', true)
if (!localStorage.getItem('canStore') || localStorage.getItem('canStore').length < 1) {
    window.location.href = "/store/category"
}