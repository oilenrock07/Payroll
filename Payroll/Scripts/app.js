$(function() {

    function handleChangeDate() {
        $(this).datepicker('hide');
    }

    function init() {
        $('.datepicker').datepicker({ format: 'mm/dd/yyyy' });
        $('.timepicker').datetimepicker({ format: 'LT' });
        $('.datepicker').on('changeDate', handleChangeDate);
    }

    init();
});