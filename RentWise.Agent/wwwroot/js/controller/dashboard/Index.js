function onToggle(contentNo,link = false) {
        if(link){
        window.location.href = link
            return
        }
    // Get the current URL
    const url = new URL(window.location.href);
    console.log(url.searchParams.has('filterDate'),contentNo)
    // Check if 'featuredDate' query parameter exists
    if (url.searchParams.has('filterDate') && contentNo != 1) {
        // Remove the 'featuredDate' query parameter
        url.searchParams.delete('filterDate');
    // Set the 'content' query parameter with the specified value
    url.searchParams.set('active', contentNo);
    // Redirect to the updated URL
    window.location.href = url.href;
    return; // Exit the function to prevent further execution
        }

        // Hide all content sections
        document.querySelectorAll(".content").forEach((content) => {
        content.classList.add("display-none");
        });
    // Display the selected content section
    document.querySelector(".content-" + contentNo).classList.remove("display-none");

    // Update sidebar active class
    $(".sidebar").removeClass("active");
    $(".sidebar-" + contentNo).addClass("active");

    }

function onToggleSidebar() {
        const url = new URL(window.location.href);
    const urlParams = new URLSearchParams(url.search);
    const activeValue = urlParams.get('active');

        if (activeValue && activeValue.length > 0) {
        onToggle(activeValue);
        } else {
            const action = $(".action").val();
    onToggle(action);
        }
    }
onToggleSidebar()
