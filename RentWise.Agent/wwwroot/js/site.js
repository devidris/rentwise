function buildQueryParams(key, value, currentUrl = new URL(location.href), deleteIfNotExist = false) {
    if (key && value) {
        // Check if the 'key' parameter already exists
        if (currentUrl.searchParams.has(key)) {
            // If it exists, update its value
            currentUrl.searchParams.set(key, value);
        } else {
            // If it doesn't exist, add the 'value' parameter
            currentUrl.searchParams.append(key, value);
        }
    }
    if (deleteIfNotExist) {
        if (!key && !value) {
            // If it does not exists, delete its value
            currentUrl.searchParams.delete(key, value);
        }
    }
    return currentUrl;
}

function contact() {
    // Capture the form data
    var formData = {
        firstName: $('.firstname').val(),
        lastName: $('.lastname').val(),
        email: $('.email').val(),
        message: $('.message').val()
    };

    if (formData.firstName === null || formData.firstName.trim() === "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'First name is required. Please enter a value.'
        });
        return;
    }

    if (formData.lastName === null || formData.lastName.trim() === "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Last name is required. Please enter a value.'
        });
        return;
    }

    if (formData.email === null || formData.email.trim() === "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Email is required. Please enter a value.'
        });
        return;
    }

    if (formData.message === null || formData.message.trim() === "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Message is required. Please enter a value.'
        });
        return;
    }
    $.ajax({
        type: 'POST',
        url: 'https://localhost:7192/page/ContactAdmin',
        data: formData,
        success: function (data) {
            // Display SweetAlert alert on success
            $('.firstname').val(""),
                $('.lastname').val(""),
                $('.email').val(""),
                $('.message').val("")
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Form Submitted!',
                    text: 'Thank you for your submission.We will get back as soon as possible',
                    confirmButtonText: 'OK'
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong! Please try again.',
                    confirmButtonText: 'OK'
                });
            }
        },
        error: function (error) {
            // Display SweetAlert alert on error
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Something went wrong! Please try again.',
                confirmButtonText: 'OK'
            });
        }
    });
}
function openModal(name, isId = true) {
    if (isId) {
        $('#' + name).modal('show')
    } else {
        $('.' + name).modal('show')
    }
}
function closeModal(name, isId = true) {
    if (isId) {
        $('#' + name).modal('hide')
    } else {
        $('.' + name).modal('hide')
    }
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
                initMap()
            } else {
                console.error('No results found');
            }
        } else {
            console.error('Geocoder failed due to: ' + status);
        }
    });
}
function shareOrCopy(name, link) {
    // Check if the browser supports the Web Share API
    if (navigator.share) {
        // Use Web Share API to share
        navigator.share({
            title: name,
            text: 'Check out this rentals on rentwisegh',
            url: link ?? location.href
        })
            .then(() => {
                console.log('Shared successfully');
            })
            .catch((error) => {
                console.error('Error sharing:', error);
            });
    } else {
        // Fallback for browsers that do not support Web Share API
        console.log('Web Share API not supported, using fallback');
        // Copy link to clipboard
        var dummy = document.createElement("input");
        document.body.appendChild(dummy);
        dummy.setAttribute("id", "dummy_id");
        document.getElementById("dummy_id").value = "https://www.example.com";
        dummy.select();
        document.execCommand("copy");
        document.body.removeChild(dummy);
        // Show toastr notification
        toastr.success('Link copied to clipboard!');
    }
};

// Function to show loading animation
function showLoading() {
    const loadingOverlay = document.querySelector('.loading');
    loadingOverlay.classList.remove('hidden');

}

// Function to hide loading animation
function hideLoading() {
    const loadingOverlay = document.querySelector('.loading');
    loadingOverlay.classList.add('hidden');
}

try {
    $(document).ready(function () {
        const JSONstates = JSON.parse($('.jsonstate').text());
        $(".jsonstate").remove();

        // Set up a change listener for the state dropdown to populate cities
        $('#stateDropdown').change(function () {
            var stateName = $(this).val();
            var selectedState = JSONstates.find(state => state.Name === stateName);
            $('#cityDropdown').empty().append(new Option('Select a City', ''));

            if (selectedState && selectedState.Cities.length > 0) {
                selectedState.Cities.forEach(function (city) {
                    $('#cityDropdown').append(new Option(city.Name, city.Name));
                });
                $('#cityDropdown').prop('disabled', false);
                $('#cityDropdown').addClass('text-capitalize');
            } else {
                $('#cityDropdown').prop('disabled', true);
                $('#cityDropdown').addClass('text-capitalize');
            }
        });

    });
} catch (err) { }