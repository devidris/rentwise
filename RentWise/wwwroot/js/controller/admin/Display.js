$(document).ready(function () {
    $('#fileInput').on('change', function () {
        if ($(this).val()) {
            $('#uploadButton').prop('disabled', false);
        } else {
            $('#uploadButton').prop('disabled', true);
        }
    });

    $('#fileServiceInput').on('change', function () {
        if ($(this).val()) {
            $('#uploadServiceButton').prop('disabled', false);
        } else {
            $('#uploadServiceButton').prop('disabled', true);
        }
    });
});