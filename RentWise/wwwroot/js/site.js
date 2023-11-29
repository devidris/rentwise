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

function openModel(name, isId = true) {
    if (isId) {
        $('#' + name).modal('show')
    } else {
        $('.' + name).modal('show')
    }
}
function closeModel(name, isId = true) {
    if (isId) {
        $('#' + name).modal('hide')
    } else {
        $('.' + name).modal('hide')
    }
}