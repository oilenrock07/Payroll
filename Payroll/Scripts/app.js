$(function() {

    function handleChangeDate() {
        $(this).datepicker('hide');
    }

    function init() {
        $('.datepicker').each(function (index, value) {

            var startDate = $(value).attr('start');
            var endDate = $(value).attr('end');

            $(value).datepicker({ format: 'mm/dd/yyyy', startDate: startDate, endDate: endDate });
        });

        $('.timepicker').datetimepicker({ format: 'LT' });
        $('.datepicker').on('changeDate', handleChangeDate);
    }

    init();
});