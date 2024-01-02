
const statusValue = $('.status').val()
if (statusValue != 0) {
    Swal.fire('Payment Successful', statusValue, 'success');
}
let pageLink = location.href
let type = ''
let orderId = 0;
function togglePaymentOption(option) {
    type = option
    $(".payment-option").hide(); // Hide all payment options

    // Show the selected payment option
    $("#" + option + "Option").show();
}

function pay(id) {
    orderId = id
    openModal('paymentModal')
}

function payNow() {

    if (type == "") {
        toastr.error("Please select a payment method")
        return
    }
    $.ajax({
        url: '/Store/Pay',
        type: 'POST',
        data: {
            orderId,
            type,
            pageLink
        },
        success: function (data) {
            if (data.success) {
                if (type == 'cash') {
                    Swal.fire({
                        title: 'Payment Successful',
                        icon: 'success',
                        confirmButtonText: 'OK',
                    }).then((result) => {
                        if (result.isConfirmed) {
                            location.reload();
                        }
                    });
                } else {
                    const message = JSON.parse(data.data)
                    const content = JSON.parse(message.Content)
                    location.href = content.data.checkoutUrl;
                    Swal.fire('Success', 'Your order will be marked as paid as soon as it is confirmed.', 'info');
                    closeModal('paymentModal')
                }
            }
            else {
                Swal.fire('Error', 'Something went wrong', 'error');
            }
        },
        error: function (data) {
            Swal.fire('Error', 'Something went wrong', 'error');
        }
    })
}
