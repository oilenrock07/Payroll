$(function() {

    function handleAttendanceLogClick() {
        var startDate = $('.js-startDate').val();
        var endDate = $('.js-endDate').val();

        $.ajax({
            url: '/Attendance/AttendanceLogContent',
            data: { startDate: startDate, endDate: endDate },
            method: 'POST',
            success: function(responseData) {
                $('#content').html(responseData);
            }
        });
    }

    function init() {
        $('.js-submitAttendancelLog').on('click', handleAttendanceLogClick);
    }

    init();
});