$(document).ready(function () {
    $("#image").change(function () {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (event) {
                $(".image-display").attr("src", event.target.result);
            };
            reader.readAsDataURL(file);
        }
    });
});

function deleteAccount() {
    const userId = $('.userId').val()
    Swal.fire({
        title: "Delete Account",
        text: "Are you sure you want to delete your account",
        icon: "info",
        showCancelButton: true,
        confirmButtonText: 'Yes, place reservation',
        cancelButtonText: 'No, cancel',
    }).then((result) => {
        if (result.isConfirmed) {
            location.href = "/Auth/Delete?userId=" + userId
        }
    })
}