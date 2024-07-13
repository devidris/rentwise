document.addEventListener('DOMContentLoaded', function () {
    const citySearchInput = document.getElementById('citySearch');
    citySearchInput.addEventListener('input', function () {
        const filter = citySearchInput.value.toLowerCase();
        const cityItems = document.querySelectorAll('.dropdown-item.city-item');

        cityItems.forEach(function (item) {
            const text = item.textContent.toLowerCase();
            if (filter === '') {
                item.style.display = 'none';
            } else {
                item.style.display = text.includes(filter) ? '' : 'none';
            }
        });
    });
});

function updateDropdownToggle(element) {
    const dropdownToggle = document.querySelector('.dropdown-city .nav-link .span');
    dropdownToggle.textContent = element.textContent;
}
