
function filterCategory(lkpCategory) {
    if (lkpCategory == 0) {
        location.href = `/Store/Index`
    } else {
        location.href = `/Store/Index/${lkpCategory}`
    }
    }
    const lkpCategory = location.href.split('/').reverse()[0]
    if (lkpCategory.toLowerCase() == 'store' || lkpCategory.toLowerCase() == 'index') {
        $('.0').prop('checked', true)
    } else {
        $('.' + lkpCategory).prop('checked', true)
}
function search() {
    const name = $('.product-search').val()
    location.href = buildQueryParams('name',name)
}