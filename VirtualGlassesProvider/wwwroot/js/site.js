// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


setTimeout(function () {
    var addedToCartMessageElement = document.getElementById('addedToCartMessage');
    if (addedToCartMessageElement) {
        addedToCartMessageElement.remove();
    }
}, 2000);

setTimeout(function () {
    var addedToWishlistMessageElement = document.getElementById('addedToWishlistMessage');
    if (addedToWishlistMessageElement) {
        addedToWishlistMessageElement.remove();
    }
}, 2000);