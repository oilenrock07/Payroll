$(function () {
    function handleFormSubmit(e) {
        
        $.ajax({
            url: '/Payroll/IsPayrollComputed',
            data: { date: $('#Date').val() },
            async: false,
            method: 'POST',
            success: function (response) {
                if (response == true) {

                    if (!confirm("Payroll is already generated for this cutoff.\nWould you like to generate again? This will remove the last entries."))
                        e.preventDefault();
                }
            }
        });

        $('.js-message').toggleClass('hidden');
    };

    function init() {
        $('form').on('submit', handleFormSubmit);
    };

    init();
});