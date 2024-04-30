const fullMessage = JSON.parse($('.full-message').val());
const chatSummaries = JSON.parse($('.chat-summaries').val());
//const userId = $('.user-id').val();
const receipientId = $('.receipient-id').val();

function displayProfiles() {
    let chatList = "";
    chatSummaries.forEach(chat => {
        chatList += ` <div class="chat cursor-pointer" onclick="navigate('${chat.UserId}')">
                    <img src="${chat.ProfilePicture}" alt="Profile Image" />
                    <p><b>${chat.User.UserName.split('@')[0]}</b></p>
                    <p class="${chat.UserId}-lastmessage">${chat.LastMessage.Message}</p>
                    <p>${chat.LastChat}</p>
                    <p class="bg-primary rounded-circle text-center text-white ${chat.UnreadMessageCount > 0 ? '' : 'd-none'}">${chat.UnreadMessageCount}</p>
                </div>
                <hr />`
    })
    $('.chats').html(chatList);

}
displayProfiles()

function displayMessagePage() {
    if (! receipientId) return
    const receipient = chatSummaries.find(chat => chat.UserId == receipientId);
    console.log(receipient)
    const receipientProfile = `
     <div class="chat-profile bg-white">
         <a class="btn btn-primary" href="/page/chat"><i class="bi bi-arrow-left-short"></i>Back</a>
                <img src="${receipient.ProfilePicture}" alt="" width="50" height="50" class="rounded-circle" />
                <span class="ml-3">${receipient.User.UserName.split('@')[0]}</span>
            </div>`;

    $('.chat-area').append(receipientProfile);
    displayChat()
}
const data = [
];
function displayChat(message = null,position = "left") {
    try {
        const gridContainer = document.getElementById('grid-container');
        if (message) {
            const messageProp = [{ content: message, class: position }]
            messageProp.forEach(item => {
                const gridItem = document.createElement('div');
                gridItem.classList.add('grid-item');

                const widthDiv = document.createElement('div');
                widthDiv.classList.add('width', item.class);
                widthDiv.textContent = item.content;

                gridItem.appendChild(widthDiv);
                gridContainer.appendChild(gridItem);
            });

            data.push(messageProp)
        } else {
            fullMessage.forEach(message => {
                const messageProp = { content: message.Message }
                if (message.IsOrder) {
                    messageProp.class = 'center'
                }
                else if (message.FromUserId == userId) {
                    messageProp.class = 'left'
                }
                else {
                    messageProp.class = 'right'
                }
                data.push(messageProp)
                data.forEach(item => {
                    const gridItem = document.createElement('div');
                    gridItem.classList.add('grid-item');

                    const widthDiv = document.createElement('div');
                    widthDiv.classList.add('width', item.class);
                    widthDiv.textContent = item.content;

                    gridItem.appendChild(widthDiv);
                    gridContainer.appendChild(gridItem);
                });
            });
        }
    } catch (err) { }
   
}
displayMessagePage()
function navigate(id) {
    showLoading()
    location.href = `/Page/Chat/${id}`
}

function send() {
    const message = $('#message').val();
    console.log(message)
    if (!message || message == "") {
        toastr.error("Message cannot be empty");
        return
    }
    $('.' + receipientId + '-lastmessage').text(message)

    $.ajax({
        url: "/Store/SendMessage",
        type: "POST",
        data: {
            receipient: receipientId,
            message
        },
        success: function (response) {
            if (response.success) {
                $('#message').val("");
                displayChat(message)
                const scrollingDiv = $('#grid-container');
                scrollingDiv.scrollTop(scrollingDiv[0].scrollHeight);
            } else {
                if (response.statusCode == 401) {
                    toastr.error("Please login to like this product");
                    location.href = '/Auth/Login'
                } else {
                    toastr.error("Something went wrong");
                }
            }
        },
        error: function (error) {
            console.log(error)
        }
    })
}

// Scroll to bottom of the chat
$(document).ready(function () {
    // Select the div with the overflow set to scroll
    const scrollingDiv = $('#grid-container');

    if (scrollingDiv) {
        // Set the scrollTop property to the maximum scroll height
        scrollingDiv.scrollTop(scrollingDiv[0].scrollHeight);
    }


    const textarea = document.getElementById('message');
    const lineHeight = parseInt(window.getComputedStyle(textarea).lineHeight);

    // Set initial height
    const initialRows = 1.6; // Set the initial number of rows
    textarea.style.height = initialRows * lineHeight + 'px';
    $('.chat-send').css('height', initialRows * lineHeight + 'px');

    // Add event listener
    textarea.addEventListener('input', function () {
        const rows = Math.min(this.scrollHeight / lineHeight, 4); // Limit to 4 lines
        this.style.height = rows * lineHeight + 'px'; // Set the height of textarea based on content, up to 4 lines
        $('.chat-send').css('height', rows * lineHeight + 'px'); // Set the height of .chat-send
    });

});

