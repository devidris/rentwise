﻿document.addEventListener('DOMContentLoaded', function () {
    // Get the current path from the window location and normalize it
    const path = window.location.pathname.toLowerCase();

    // Select all elements with the class 'nav-link'
    const navLinks = document.querySelectorAll('.nav-link');

    // Iterate over each navigation link
    navLinks.forEach(function (link) {
        const href = link.getAttribute('href');

        // Ensure href exists and is not just a placeholder like "#"
        if (href && href !== '#') {
            // Convert href to lower case to match case-insensitive paths
            const normalizedHref = href.toLowerCase();

            // Check if the normalized href matches the path or is part of it
            if (normalizedHref === path) {
                // Add 'active-nav' class and remove 'not-active-nav' class if they match
                link.classList.add('active-nav');
                link.classList.remove('not-active-nav');
            } else {
                // Otherwise, remove 'active-nav' class and ensure 'not-active-nav' is added
                link.classList.remove('active-nav');
                link.classList.add('not-active-nav');
            }
        }
    });

    // Check if the path includes '/store/' and add 'active-nav' to '.category'
    if (path.includes('/store')) {
        const categoryElement = document.querySelector('.category-list');
        if (categoryElement) {
            categoryElement.classList.add('active-nav');
            categoryElement.classList.remove('not-active-nav');
        }
    }

    // Check if the path includes '/admin/' and add 'active-nav' to '.admin'
    if (path.includes('/admin')) {
        const adminElement = document.querySelector('.admin');
        if (adminElement) {
            adminElement.classList.add('active-nav');
            adminElement.classList.remove('not-active-nav');
        }
    }
});
