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