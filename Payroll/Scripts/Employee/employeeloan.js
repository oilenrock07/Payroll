$(function() {

    function handleFrequencyChange() {
        var frequency = parseInt($(this).val());
        $('.js-loanPaymentOption').addClass('hidden');
        switch (frequency) {
        case 1:
            $('.js-weekly').removeClass('hidden');
            break;
        case 2:
            $('.js-bimonthly').removeClass('hidden');
            break;
        case 3:
            $('.js-monthly').removeClass('hidden');
            break;
        }
    };

    function init() {
        $('.js-loanFrequency').on('change', handleFrequencyChange);
    };

    init();
});