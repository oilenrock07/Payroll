$(function() {

    function handleFormSubmit(e) {

        $('.js-errorsummary').addClass('hidden');        
        if (!$('#frmEmployeeDetails').valid()) {

            if ($('.field-validation-error').length > 0) {
                $('.js-errorsummary').html('');

                $.each($('.field-validation-error'), function (index, value) {
                    $('.js-errorsummary').append('<div>' + $(value).find('span').html() + '</div>');
                });

                $('.js-errorsummary').removeClass('hidden');
            }
        }

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

    function handleWorkScheduleOptionClick() {
        $('#WorkScheduleId').val($(this).val());
        $('.js-workScheduleName').html($(this).closest('tr').find('td:nth-child(2)').html());
        $('.js-workScheduleDay').html($(this).closest('tr').find('td:nth-child(3)').html());
        $('.js-workScheduleTime').html($(this).closest('tr').find('td:nth-child(4)').html());
        $('.js-workScheduleLink').text('Change');
        $('#ModalWorkSchedules').modal('hide');
    }

    function init() {
        if ($('#WorkScheduleId').val() == '0')
            $('#WorkScheduleId').val('');

        $('form').on('submit', handleFormSubmit);
        $('.js-workScheduleOption').on('click', handleWorkScheduleOptionClick);
        $('.datepicker').each(function() {
            if ($(this).val() == '01/01/0001' || $(this).val() == '1/1/0001 12:00:00 AM') {
                $(this).val('');
            }
        });
    };

    init();
});