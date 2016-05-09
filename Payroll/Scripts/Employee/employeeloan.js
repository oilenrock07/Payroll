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

    var employeeList = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/Employee/'
        }
    })

    function init() {
        $('.js-loanFrequency').on('change', handleFrequencyChange);
        $('#EmployeeId').typeahead(null, {
            name: 'test',
            display: 'value',
            source: employeeList
        });
    };

    init();
});