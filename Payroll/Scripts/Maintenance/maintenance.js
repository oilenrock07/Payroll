$(function() {

    function handleMaintenanceDelete() {
        if (!confirm('Are you sure you want to delete this?')) {
            e.preventDefault();
            return;
        }
    };

    function init() {
        $('.js-maintenanceDelete').on('click', handleMaintenanceDelete);
    };

    init();
});