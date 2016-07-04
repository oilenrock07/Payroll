$(function() {

    function handleMaintenanceDelete(e) {
        if (!confirm('Are you sure you want to delete this?')) {
            e.preventDefault();
            return false;
        }
    };

    function init() {
        $('body').on('click', '.js-maintenanceDelete', handleMaintenanceDelete);
    };

    init();
});