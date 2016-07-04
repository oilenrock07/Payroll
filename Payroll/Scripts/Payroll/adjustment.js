$(function() {
    function init() {
        $('.js-payrollDates').on('change', handlePayrollDateChange);
        $('body').on('click', '.js-viewAdjustment', handleViewAdjustmentClick);
    }

    function handlePayrollDateChange() {
        $.ajax({
            url: '/Payroll/GetAdjustments',
            data: { date: $(this).val() },
            type: 'POST',
            success: handlePayrollDateChangeSuccess
        });
    }

    function handlePayrollDateChangeSuccess(responseData) {
        $('#adjustment-container').html(responseData);
    }

    function handleViewAdjustmentClick() {

        var data = {
            id: $(this).data('employeeid'),
            date: $('.js-payrollDates').val()
        }

        $.ajax({
            url: '/Payroll/ViewEmployeeAdjustmentDetails',
            data: data,
            type: 'POST',
            success: function (responseData) {
                $('.modal-body').html(responseData);
                $('#ModalAdjustment').modal('toggle');
            }
        });
    }

    init();
});