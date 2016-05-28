$(function() {

    function handleFrequencyChange() {
        var frequency = parseInt($(this).val());
        $('.js-loanPaymentOption').addClass('hidden');
        switch (frequency) {
        case 3:
            $('.js-weekly').removeClass('hidden');
            break;
        case 5:
            $('.js-bimonthly').removeClass('hidden');
            break;
        case 6:
            $('.js-monthly').removeClass('hidden');
            break;
        }
    };


    function init() {
        $('.js-loanFrequency').on('change', handleFrequencyChange);
    };

    init();
});