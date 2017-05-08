$(function() {

    function handleLeaveHoursChange() {
        var leaveHours = parseInt($(this).val());

        $('.js-leaveSpecifiedHours').addClass('hidden');
        if (leaveHours == -1) {
            $('.js-leaveSpecifiedHours').removeClass('hidden');
        }
    };

    function handleLeaveSubmitClick() {
        var startDate = $('.js-startDate').val();
        var endDate = $('.js-endDate').val();

        var employeeId = $('.js-employeeIdTypeAhead').val();
        if (employeeId == '' || employeeId == undefined) {
            employeeId = 0;
        }

        $.ajax({
            url: '/Employee/EmployeeLeavesContent',
            data: { startDate: startDate, endDate: endDate, employeeId: parseInt(employeeId) },
            method: 'POST',
            success: function (responseData) {
                $('#content').html(responseData);
            }
        });
    }

    function init() {
        $('.js-leaveHours').on('change', handleLeaveHoursChange);
        $('.js-submitLeaveSearch').on('click', handleLeaveSubmitClick);

        //preload the date today
        handleLeaveSubmitClick();
    };

    init();
});