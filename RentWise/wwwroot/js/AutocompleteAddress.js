
// Initialize the Places Autocomplete
const autocomplete = new google.maps.places.Autocomplete(document.getElementById('autocomplete-input'));

// Set the types to restrict the search to addresses only
autocomplete.setTypes(['address']);

// Listen for the event when a place is selected
autocomplete.addListener('place_changed', getLatLng);

function getLatLng() {
    const place = autocomplete.getPlace();
    if (place.geometry && place.geometry.location) {
        const latitude = place.geometry.location.lat();
        const longitude = place.geometry.location.lng();
        lat = latitude
        lng = longitude
    }
}