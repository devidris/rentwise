function setCategory(lkpcategory) {
    $('.category-input').val(lkpcategory)
    $('.categories').text(categories[lkpcategory - 1])
    onToggle(2)
}
function onToggle(contentNo) {
    document.querySelectorAll(".content").forEach((content) => {
        content.classList.add("d-none");
    })

    document.querySelector(".content-" + contentNo).classList.remove("d-none");
}
const categories = [
    "Construction Equipments",
    "Vehicle Rentals",
    "Office Items/Personal Items",
    "Events/ Equipment Rentals",
    "Sales Of Vehicles Trackers",
    "Bill BOard",
    "Shot Stay Rooms",
    "Motel",
    "Boat/Yacht",
    "Games"
];


const fileList = new DataTransfer();

$("#images").change(function (e) {
    Array.from(e.target.files).forEach((file) => {
        fileList.items.add(file)
        displayImages(file)
    });

    $('#allimages')[0].files = fileList.files
})

function displayImages(file) {
    if (file && file.type.startsWith('image/')) {
        const reader = new FileReader();

        reader.onload = function () {
            const image = new Image();
            image.src = reader.result;
            image.style.width = '200px';
            image.style.height = '200px';

            // Create a container for the image and delete icon
            const container = document.createElement('div');
            container.classList.add('image-container');

            // Create the delete icon
            const deleteIcon = document.createElement('span');
            deleteIcon.classList.add('delete-icon');
            deleteIcon.innerHTML = '<i class="bi bi-trash3-fill"></i>'; // Use '×' for a close symbol

            // Append the image and delete icon to the container
            container.appendChild(image);
            container.appendChild(deleteIcon);

            // Append the container to the main container
            const mainContainer = document.getElementById('imageContainer');
            mainContainer.insertBefore(container, mainContainer.firstChild);

            // Handle the click event on the delete icon
            deleteIcon.addEventListener('click', function () {
                // Remove the corresponding file from fileList
                container.remove();
                Array.from(fileList.files).forEach((f, index) => {
                    if (f === file) {
                        fileList.items.remove(index);
                    }
                });

                // Update the input files
                // $('#allimages')[0].files = fileList.files;
            });
        }

        reader.readAsDataURL(file);
    }
}
let includesNo = 0
function addIncludes(value = false) {
    if (includesNo > 10) {
        toastr.error("Can add more than 10 product includes")
        return
    }
    let input = `<input type="text" placeholder="Your product include" class="form-control mb-4 include-input include-input-${includesNo}"/>`
    if (value) {
        input = `<input type="text" placeholder="Your product include" class="form-control mb-4 include-input include-input-${includesNo}" value="${value}"/>`
    }
    $('.includesContainer').prepend(input)
    if(!value) {
        $(`.include-input-${includesNo}`).focus()
    }
    includesNo++
}
function declareIncludes() {
    const includes = $('.includes').val().split(',,')
    $('.includes').remove()
    if (includes.length > 0) {
        includes.forEach(include => {
            if (include.length > 0) {
                addIncludes(include)
            }
        })
    }
}
let rulesNo = 0
function addRules(value) {
    if (rulesNo > 10) {
        toastr.error("Can add more than 10 product rules")
        return
    }
    let input = `<input type="text" placeholder="Rule" class="form-control mb-4 rule-input  rule-input-${rulesNo}"/>`
    if (value) {
        input = `<input type="text" placeholder="Rule" class="form-control mb-4 rule-input rule-input-${rulesNo}" value="${value} "/>`
    } 
    $('.rulesContainer').prepend(input)
    if(!value) {
        $(`.rule-input-${rulesNo}`).focus()
    }
    rulesNo++
}
function declareRules() {
    const rules = $('.rules').val().split(',,')
    $('.rules').remove()
    if (rules.length > 0) {
        rules.forEach(rule => {
            if (rule.length > 0) {
            addRules(rule)
            }
        })
    }
}

function declareLocation() {
    const location = $('.location').val()
    $('.location').remove()
    if (location) {
        $('#autocomplete-input').val(location)
        getLocationDetails()
    }
}
let OldImageCount = $('.product-image-count').val()
function saveChanges() {
    if (fileList.files.length + OldImageCount < 4) {
        toastr.error('Minimum of 4 images')
        return
    }
    let concatenatedInputIncludes = ''
    $('.include-input').each(function () {
        if (!$(this).val() || $(this).val() == '') return
        concatenatedInputIncludes += $(this).val() + ',,rw,,';
    });
    $('.includes-value').val('')
    $('.includes-value').val(concatenatedInputIncludes)

    let concatenatedInputRules = ''
    $('.rule-input').each(function () {
        if (!$(this).val() || $(this).val() == '') return
        concatenatedInputRules += $(this).val() + ',,rw,,';
    });
    $('.rules-value').val('')
    $('.rules-value').val(concatenatedInputRules)

    $('.save').click()

}

function displayImage(input) {
    $('#main-image').removeClass('d-none')
    var reader = new FileReader();

    reader.onload = function (e) {
        $('#main-image').attr('src', e.target.result);
    };

    reader.readAsDataURL(input.files[0]);
}

$('#main-image-input').on('change', function () {
    var fileInput = $(this)[0];

    if (fileInput.files.length > 0) {
        displayImage(fileInput);
    }
});

// Initialize the Places Autocomplete
const autocomplete = new google.maps.places.Autocomplete(document.getElementById('autocomplete-input'));

// Set the types to restrict the search to addresses only
autocomplete.setTypes(['address']);

// Listen for the event when a place is selected
autocomplete.addListener('place_changed', getLocationDetails);

function getLocationDetails() {
    const place = autocomplete.getPlace();
    $('.store-address').val(place.formatted_address)
    if (place.geometry && place.geometry.location) {
        const latitude = place.geometry.location.lat();
        const longitude = place.geometry.location.lng();
        $('.latitude').val(latitude)
        $('.longitude').val(longitude)

        // Construct the request URL for reverse geocoding
        const geocodingApiUrl = 'https://maps.googleapis.com/maps/api/geocode/json?latlng=' + latitude + ',' + longitude + '&key=AIzaSyCM2yIul54qUYlxHgjTmvp52goQdqPcUwA';

        // Make a request to the Geocoding API
        fetch(geocodingApiUrl)
            .then(response => response.json())
            .then(data => {
                // Check if the response contains results
                if (data.results && data.results.length > 0) {
                    // Extract country and state from the first result
                    const components = data.results[0].address_components;

                    components.forEach(function (component) {
                        if (component.types.includes('country')) {
                            const country = component.long_name;
                            $('.country').val(country)
                        }
                        if (component.types.includes('administrative_area_level_1')) {
                            const state = component.long_name;
                            $('.state').val(state)
                        }

                    });
                } else {
                    console.log("No results found for reverse geocoding");
                }
            })
            .catch(error => {
                console.error("Error during reverse geocoding request:", error);
            });

    }
}

function deleteImage(id) {
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
            $.ajax({
                method: "DELETE",
                url: "/Store/DeleteImage?Id=" + id,
                success: function (res) {
                    Swal.fire('Success', 'You have successfully deleted image.', 'success');
                    $('#' + id).remove()
                    OldImageCount--
                },
                error: function (err) {
                    Swal.fire('Error', 'Something went wrong', 'error');
                }

            })
        }
    });
}
// Main 
declareIncludes()
declareRules()
declareLocation()