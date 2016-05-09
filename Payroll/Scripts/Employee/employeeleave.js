$(function() {

    function handleLeaveHoursChange() {
        var leaveHours = parseInt($(this).val());

        $('.js-leaveSpecifiedHours').addClass('hidden');
        if (leaveHours == -1) {
            $('.js-leaveSpecifiedHours').removeClass('hidden');
        }
    };

    function init() {
        $('.js-leaveHours').on('change', handleLeaveHoursChange);
    };

    init();
});