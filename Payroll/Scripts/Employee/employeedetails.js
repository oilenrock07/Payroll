$(function() {

    function init() {
        $('.datepicker').each(function() {
            if ($(this).val() == '01/01/0001' || $(this).val() == '1/1/0001 12:00:00 AM') {
                $(this).val('');
            }
        });
    };

    init();
});