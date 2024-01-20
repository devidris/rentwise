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
    if ('Notification' in window) {
        Notification.requestPermission().then(function (permission) {
            if (permission === 'granted') {
                // Create a notification
                var notification = new Notification(message, {
                    body: 'This is a notification message.',
                    icon: 'https://rentwisegh.com/img/logo.png'
                });
                //notification.onclick = function () {
                //    console.log('Notification clicked!');
                //};
            } else {
                console.warn('Notification permission denied');
            }
        });
    } else {
        console.warn('Notification API not supported');
    }
    }

});

connection.start().catch(err => console.error(err));