
let noOfProduct = 1;
let datesSelected = {}
let days = 0
let totalPrice = 0
function calculatePrice(selectedDates) {
    datesSelected = selectedDates
    let weekdaysCount = 0;
    let weekendsCount = 0;
    let weeksCount
    let firstDate = null;
    let lastDate = null;

    // Assuming a simple price calculation based on the number of selected dates
     days = Object.keys(selectedDates).reduce(function (total, year) {
        return total + Object.keys(selectedDates[year]).reduce(function (yearTotal, month) {
            return yearTotal + selectedDates[year][month].reduce(function (monthTotal, day) {
                let iterDate = new Date(year, month, day);
                let dayOfWeek = iterDate.getDay();

                if (firstDate === null || iterDate < firstDate) {
                    firstDate = iterDate;
                }
                if (lastDate === null || iterDate > lastDate) {
                    lastDate = iterDate;
                }

                if (dayOfWeek >= 1 && dayOfWeek <= 5) {
                    // Weekday (Monday to Friday)
                    weekdaysCount++;
                } else {
                    // Weekend (Saturday or Sunday)
                    weekendsCount++;
                }

                return monthTotal + 1; // Increment the total count
            }, 0);
        }, 0);
    }, 0);
    if (days > 0 && days > $('.maxRentalDay').val()) {
        toastr.error($('.maxRentalDay').val() + " days is the maximum allowed rental date")
        return
    }
    weeksCount = Math.floor(weekdaysCount / 5);
    weekdaysCount = weekdaysCount - (weeksCount * 5)

    const priceDay = $('.price-day').val()
    const totalDayPrice = priceDay * weekdaysCount * noOfProduct
    const calDaysPrice = `
                <p>
                $${priceDay}/Day * ${weekdaysCount} Day * ${noOfProduct} Product
                </p>
                <p>
                $${totalDayPrice}
                </p>
                `
    $('.day').html(calDaysPrice)


    const priceWeekend = $('.price-weekend').val()
    const totalWeekendPrice =
        priceWeekend * weekendsCount * noOfProduct
    const calWeekendPrice = `
                    <p>
                        $${priceWeekend}/Day * ${weekendsCount} Weekend * ${noOfProduct} Product
                    </p>
                    <p>
                        $${totalWeekendPrice}
                    </p>
                    `
    $('.weekend').html(calWeekendPrice)

    const priceWeek = $('.price-week').val()
    const totalWeekPrice = priceWeek * weeksCount * noOfProduct
    const calWeekPrice = `
                        <p>
                            $${priceWeek}/Day * ${weeksCount} Week * ${noOfProduct} Product
                        </p>
                        <p>
                            $${totalWeekPrice}
                        </p>
                        `
    $('.week').html(calWeekPrice)
    totalPrice = totalDayPrice + totalWeekendPrice + totalWeekPrice
    $('.total-price').text('$' + totalPrice)
    $('.start-date').text(formatDate(firstDate))
    $('.end-date').text(formatDate(lastDate))
}
function setNoOfProduct(action) {
    if (action == 'add') {
        noOfProduct++
    } else {
        if (noOfProduct - 1 < 0) {
            toastr.error("No of product cannot be less than 0")

        } else {
            noOfProduct--
        }
    }
    $('.no-of-product').text(noOfProduct)
    $('.no-of-product-2').text(`(${noOfProduct} item(s) added)`)
    calculatePrice(datesSelected)


}
// Function to format date as "Month Day, Year" (e.g., "May 6, 2023")
function formatDate(date) {
    if (date === null) {
        return "N/A";
    }

    let monthNames = [
        "January", "February", "March",
        "April", "May", "June", "July",
        "August", "September", "October",
        "November", "December"
    ];

    let month = monthNames[date.getMonth()];
    let day = date.getDate();
    let year = date.getFullYear();

    return month + ' ' + day + ', ' + year;
}

$('.insert-rating').starRating(
    {
        starSize: 1,
        showInfo: false
    });

$(document).on('change', '.insert-rating',
    function (e, stars, index) {
        $('.rating-value').val(stars * 2);
    });
$(document).ready(function () {
    $("form").submit(function (event) {
        const ratingValue = $(".rating-value").val();
        const ratingDescription = $(".review").val();

        if (ratingValue === "0" || ratingDescription.trim() === "" || ratingDescription.split(' ').length > 250) {
            toastr.error("Please provide a valid rating and description (not more than 250 words).");
            event.preventDefault(); // Prevent form submission
        }
    });
});

function toggleLike() {
    let type = 'unlike'
    if ($('#like').hasClass('bi-heart')) {
        $('#like').removeClass('bi-heart ').addClass('bi-heart-fill')
        type = 'like'
    } else if ($('#like').hasClass('bi-heart-fill')) {
        $('#like').removeClass('bi-heart-fill').addClass('bi-heart')
        type = 'unlike'
    }
    return type
}

function like() {
   let type = toggleLike()
    $('#like').off('click')

    $.ajax({
        url: '/Store/Like',
        method: 'POST',
        data: {
            productId: $('.product-id').val(),
            type
        },
        success: function (result) {
            if (result.statusCode == 401) {
                toastr.error("Please login to like this product");
                location.href = '/Auth/Login'
            } else if (result.statusCode == 200) {

            } else {
                toggleLike()
                toastr.error("Something went wrong");
            }
            $('#like').on('click', like)
        }, erorr: function (e) {
            $('#like').on('click', like)
            toggleLike()
        }
    })
}
function bookNow() {
    if (days == 0 || totalPrice == 0) {
        Swal.fire('Error', 'Please select a date', 'error');
        return
    }

    if (days > 0 && days > $('.maxRentalDay').val()) {
        Swal.fire('Maximum rental days exceded',$('.maxRentalDay').val() + " day(s) is the maximum allowed rental date",'error')
        return
    }
    const orderMessage = "Pleaced an order for " + noOfProduct + " " + $('.product-name').val() + " product(s) for " + days + " day(s) at $" + totalPrice + " from " + $('.start-date').text() + " to " + $('.end-date').text() + "."
    const alertMessage = "You are about to place an order for " + noOfProduct + " " + $('.product-name').val() + " product(s) for " + days + " day(s) at ₵" + totalPrice + " from " + $('.start-date').text() + " to " + $('.end-date').text() + "."
    // Show SweetAlert confirmation
    Swal.fire({
        title: 'Confirm Order',
        text: alertMessage,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'Yes, place order',
        cancelButtonText: 'No, cancel',
    }).then((result) => {
        if (result.isConfirmed) {
$.ajax({
                url: '/Store/Book',
                method: 'POST',
                data: {
                    productId: $('.product-id').val(),
                    productQuantity: noOfProduct,
                    startDate: $('.start-date').text(),
                    endDate: $('.end-date').text(),
                    totalPrice,
                    agentId: $('.agent-id').val(),
                    message:orderMessage
                },
                success: function (result) {
                    if (result.statusCode == 401) {
                        toastr.error("Please login to place an order");
                        location.href = '/Auth/Login'
                    } else if (result.statusCode == 200) {
                        Swal.fire('Success', 'Your order has been placed successfully.', 'success');
                    } else {
                        Swal.fire('Error', 'Something went wrong', 'error');
                    }
                }, erorr: function (e) {
                    Swal.fire('Error', 'Something went wrong', 'error');
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire('Cancelled', 'Your request has been cancelled.', 'info');
        }
    });


}