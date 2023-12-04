
let noOfProduct = 1;
let datesSelected = {}
function calculatePrice(selectedDates) {
    datesSelected = selectedDates
    let weekdaysCount = 0;
    let weekendsCount = 0;
    let weeksCount
    let firstDate = null;
    let lastDate = null;

    // Assuming a simple price calculation based on the number of selected dates
    var days = Object.keys(selectedDates).reduce(function (total, year) {
        return total + Object.keys(selectedDates[year]).reduce(function (yearTotal, month) {
            return yearTotal + selectedDates[year][month].reduce(function (monthTotal, day) {
                var iterDate = new Date(year, month, day);
                var dayOfWeek = iterDate.getDay();

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
    if (days > $('.maxRentalDay').val()) {
        toastr.error($('.maxRentalDay').val() + "days is the maximum allowed rental date")
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

    $('.total-price').text('$' + (totalDayPrice + totalWeekendPrice + totalWeekPrice))
    $('.start-date').text(formatDate(firstDate))
    $('.end-date').text(formatDate(lastDate))
}
function setNoOfProduct(action) {
    if (action == 'add') {
        noOfProduct++
    } else {
        if (noOfProduct - 1 < 0) {

        } else {
            noOfProduct
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

    var monthNames = [
        "January", "February", "March",
        "April", "May", "June", "July",
        "August", "September", "October",
        "November", "December"
    ];

    var month = monthNames[date.getMonth()];
    var day = date.getDate();
    var year = date.getFullYear();

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
        const ratingDescription = $("textarea").val();

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
