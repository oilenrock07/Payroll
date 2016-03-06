$(function() {

    function handleChangeDate() {
        $(this).datepicker('hide');
    }

    function init() {
        $('.datepicker').datepicker({ format: 'mm/dd/yyyy' });
        $('.datepicker').on('changeDate', handleChangeDate);
    }

    init();
});