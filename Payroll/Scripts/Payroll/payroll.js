$(function () {
    function handleFormSubmit() {
        $('.js-message').toggleClass('hidden');
    };

    function init() {
        $('form').on('submit', handleFormSubmit);
    };

    init();
});