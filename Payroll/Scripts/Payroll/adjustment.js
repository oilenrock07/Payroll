$(function() {
    function init() {
        $('.js-payrollDates').on('change', handlePayrollDateChange);
        $('.js-adjustment').on('change', handleAdjustmentTypeChange);
        $('body').on('click', '.js-viewAdjustment', handleViewAdjustmentClick);

        handleAdjustmentTypeChange();
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

    function handleAdjustmentTypeChange() {
        var adjustmentType = $('.js-adjustment option:selected').data('type');

        var adjustmentLabel = adjustmentType == 'Add' ? '+' : '-';
        $('.js-adjustmentTypeLabel').html(adjustmentLabel);
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