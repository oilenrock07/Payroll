$(function () {

    function handleCheckboxDeductionClick() {
        var isChecked = $(this).is(':checked');

        if (isChecked)
            $(this).closest('tr').find(':text').removeAttr('disabled');
        else
            $(this).closest('tr').find(':text').attr('disabled', 'disabled');
    };

    function handleDeductionTrClick(e) {
        $(this).find(':checkbox').click();
    }

    function handleTextboxClick(e) {
        e.stopImmediatePropagation();
    }

    function init() {
        $('.js-deductionTable :text').on('click', handleTextboxClick);
        //$('.js-deductionTable tr').on('click', handleDeductionTrClick);
        $('.js-deduction').on('click', handleCheckboxDeductionClick);
    }

    init();
});