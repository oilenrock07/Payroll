$(function() {

    function handleEmployeeDeleteClick(e) {
        if (!confirm('Are you sure you want to delete this employee?')) {
            e.preventDefault();
            return;
        }
            
    }

    function init() {
        $('.js-employeeDelete').on('click', handleEmployeeDeleteClick);
    }

    init();
})