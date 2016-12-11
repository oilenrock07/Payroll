$(function() {

    function handleAttendanceLogClick() {
        var startDate = $('.js-startDate').val();
        var endDate = $('.js-endDate').val();

        var employeeId = $('.js-employeeIdTypeAhead').val();
        if (employeeId == '' || employeeId == undefined) {
            employeeId = 0;
        }

        $.ajax({
            url: '/Attendance/AttendanceLogContent',
            data: { startDate: startDate, endDate: endDate, employeeId: parseInt(employeeId) },
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