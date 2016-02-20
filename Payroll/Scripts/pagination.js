$(function() {
    // ===== Private Methods
    function handlePaginationClick() {
        var url = window.location.href;
        var pageNumber = $(this).data('page');

        url = updateQueryStringParameter(url, "Page", pageNumber);
        window.location = url;
    }

    function updateQueryStringParameter(uri, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        }
        else {
            return uri + separator + key + "=" + value;
        }
    }

    // ===== Init
    function init() {
        $('.js-pagination a').on('click', handlePaginationClick);
    }

    init();

});
	