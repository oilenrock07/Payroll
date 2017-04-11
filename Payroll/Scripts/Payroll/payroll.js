$(function () {

    function printPayroll() {
        //alert($(this).data('payroll'));
        window.print();
    }

    function handleFormSubmit(e) {
        var cancel = false;
        $.ajax({
            url: '/Payroll/IsPayrollComputed',
            data: { date: $('#Date').val() },
            async: false,
            method: 'POST',
            success: function (response) {
                if (response == true) {

                    if (!confirm("Payroll is already generated for this cutoff.\nWould you like to generate again? This will remove the last entries.")) {
                        e.preventDefault();
                        cancel = true;
                    }
                }
            }
        });

        if (!cancel) $('.js-message').toggleClass('hidden');
    };

    function init() {
        $('form').on('submit', handleFormSubmit);
        $('.js-printPayroll').on('click', printPayroll);
    };

    init();
});