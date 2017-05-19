$(function() {

    function handleAttendanceClick() {
        var startDate = $('.js-startDate').val();
        var endDate = $('.js-endDate').val();

        var employeeId = $('.js-employeeIdTypeAhead').val();
        if (employeeId == '' || employeeId == undefined) {
            employeeId = 0;
        }

        $.ajax({
            url: '/Attendance/AttendanceContent',
            data: { startDate: startDate, endDate: endDate, employeeId: parseInt(employeeId) },
            method: 'POST',
            success: function(responseData) {
                $('#content').html(responseData);
            }
        });
    };

    function handleHoursPerCompanyClick() {
        var startDate = $('.js-startDate').val();
        var endDate = $('.js-endDate').val();

        var employeeId = $('.js-employeeIdTypeAhead').val();
        if (employeeId == '' || employeeId == undefined) {
            employeeId = 0;
        }

        $.ajax({
            url: '/Attendance/HoursPerCompanyContent',
            data: { startDate: startDate, endDate: endDate, employeeId: parseInt(employeeId) },
            method: 'POST',
            success: function (responseData) {
                $('#content').html(responseData);
            }
        });
    }

    function handleCreateAttendanceClick() {
        //this should be executed first before submitting the form to the controller
        //append the time to the date

        var clockIn = $('.js-clockIn').val();
        var clockOut = $('.js-clockOut').val();
        var clockInTime = $('.js-clockInTime').val();
        var clockOutTime = $('.js-clockOutTime').val();
    }

    function init() {
        $('.js-submitAttendance').on('click', handleAttendanceClick);
        $('.js-submitHoursPerCompany').on('click', handleHoursPerCompanyClick);
        $('.js-createAttendance').on('click', handleCreateAttendanceClick);
    }

    init();
});