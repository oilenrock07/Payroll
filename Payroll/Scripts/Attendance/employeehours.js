$(function () {
    
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
    };

    function init() {
        $('.js-submitHoursPerCompany').on('click', handleHoursPerCompanyClick);
    }

    $('body').on('click', '.js-setCompany', function (e) {
        e.preventDefault();
        $(this).attr('data-target', '#modal-container');
        $(this).attr('data-toggle', 'modal');
    });

    $('#modal-container').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    }); 

    init();
});