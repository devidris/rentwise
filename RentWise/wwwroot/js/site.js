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
function sendMessage(receipient, messageInput) {
    const message = $('#'+messageInput).val();
    if (!message || message == "") {
        toastr.error("Message cannot be empty");
        return
    }
    $.ajax({
        url: "/Store/SendMessage",
        type: "POST",
        data: {
            receipient,
            message
        },
        success: function (response) {
            if (response.success) {
                $('#' + messageInput).val("");
                if (messageInput == "messageSellar") {
                toastr.success(response.message);
                closeModal("chatModal");
                }
                
            } else {
                if (response.statusCode == 401) {
                    toastr.error("Please login to like this product");
                    location.href = '/Auth/Login'
                } else {
                    toastr.error("Sonething went wrong");
                }
            }
        },
        error: function (error) {
            toastr.error(error.message);
        }
    })
}


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalhub")
    .build();



connection.on("ReceiveMessage", (user, message) => {
    const userId = $('.user-id').val()
    if (userId == user || user == "all") { 
        const path = location.href.split("/")
        if (path.includes("Chat")) {
        displayChat(message, "right")
        const scrollingDiv = $('#grid-container');
        scrollingDiv.scrollTop(scrollingDiv[0].scrollHeight);
        }
        //connection.invoke("SendToken", user, userId).catch(function (err) {
        //    return console.error(err.toString());
        //});
    }

});

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
    Swal.fire({
        title: 'Submitting...',
        allowOutsideClick: false,
        onBeforeOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: 'POST',
        url: 'https://localhost:7192/page/ContactAdmin',
        data: formData,
        success: function (data) {
            // Display SweetAlert alert on success
            Swal.close();
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
            Swal.close();
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Something went wrong! Please try again.',
                confirmButtonText: 'OK'
            });
        }
    });
}
connection.start().catch(err => console.error(err));
function shareOrCopy(name,link) {
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

function addToQueryString(params, pathname = window.location.pathname, loadPage = true) {
    console.log(pathname)
    const queryString = Object.keys(params)
        .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
        .join('&');

    if (loadPage) {
        const newUrl = `${window.location.origin}${pathname}?${queryString}`;
        window.location.href = newUrl;
    } else {
        return queryString;
    }
}
function removeParamsFromUrl(paramsToRemove, url = window.location.href) {
    // Parse the URL
    const urlObj = new URL(url);
    const searchParams = urlObj.searchParams;

    // Remove specified parameters
    paramsToRemove.forEach(param => searchParams.delete(param));

    // Update the URL with the new query string
    urlObj.search = searchParams.toString();
    return urlObj.toString();
}

function sortDropdownToggle(element) {
    const value = $(element).data('value');
    window.location.href = buildQueryParams("sort", value).href
}