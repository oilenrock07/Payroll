$(function() {

    function handleFormSubmit() {
        var checkedDepartments = [];
        $('input.js-department:checkbox:checked').each(function () {
            checkedDepartments.push($(this).data('departmentid'));
        });

        $('#CheckedDepartments').val(checkedDepartments.toString());
    };

    function init() {

        $('form').on('submit', handleFormSubmit);

        $('.datepicker').each(function() {
            if ($(this).val() == '01/01/0001' || $(this).val() == '1/1/0001 12:00:00 AM') {
                $(this).val('');
            }
        });
    };

    init();
});