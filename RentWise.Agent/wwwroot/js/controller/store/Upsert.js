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
    "Games",
    "Hotels",
    "Guest House",
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
                 //$('#allimages')[0].files = fileList.files;
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
    //if (fileList.files.length + OldImageCount < 4) {
    //    toastr.error('Minimum of 4 images')
    //    return
    //}
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

function capitalizeFirstWords(input) {
    return input.replace(/\b\w/g, function (char) {
        return char.toUpperCase();
    });
}
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
                $('#cityDropdown').append(new Option(capitalizeFirstWords(city.Name), capitalizeFirstWords(city.Name)));
            });
            $('#cityDropdown').prop('disabled', false);
            $('#cityDropdown').addClass('text-capitalize');
        } else {
            $('#cityDropdown').prop('disabled', true);
        }
    });

});