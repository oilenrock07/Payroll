$(function () {

    function handleSearchKeyDown(e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) { //Enter keycode
            handleSearchClick();
        }
    }

    function handleSearchClick() {
        var criteria = $('.js-searchEmployee').val();
        var params = $('.js-searchEmployeeParams').val();
        params = params != undefined ? params : "";

        window.location = $('.js-searchLocation').val() + '?query=' + criteria + params;
    }

    function init() {
        $('.js-searchEmployee').on('keydown', handleSearchKeyDown);
        $('.js-search').on('click', handleSearchClick);
    }

    init();
})