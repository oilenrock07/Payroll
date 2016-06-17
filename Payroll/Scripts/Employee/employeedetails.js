$(function() {

    function handleFormSubmit(e) {
        var checkedDepartments = [];
        $('input.js-department:checkbox:checked').each(function () {
            checkedDepartments.push($(this).data('departmentid'));
        });

        var checkedDeductions = [];
        $('input.js-deduction:checkbox:checked').each(function () {
            var deduction = {
                Amount: $(this).closest('tr').find(':text').val(),
                DeductionId: $(this).val()
        };
            checkedDeductions.push(deduction);
        });

        $('#CheckedDepartments').val(checkedDepartments.toString());
        $('#CheckedEmployeeDeductions').val(JSON.stringify(checkedDeductions));
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