function navigate(id) {
    const loadingOverlay = document.querySelector('.loading');
    loadingOverlay.classList.remove('hidden');
    let link = `https://${location.host}/Store/View/${id}`
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('onesignalId') && urlParams.get('onesignalId') != "") {

        link += '?onesignalId=' + urlParams.get('onesignalId');
    }
    location.href = link
}