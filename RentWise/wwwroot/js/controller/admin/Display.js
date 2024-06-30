$(document).ready(function () {
    $('#fileInput').on('change', function () {
        if ($(this).val()) {
            $('#uploadButton').prop('disabled', false);
        } else {
            $('#uploadButton').prop('disabled', true);
        }
    });
});