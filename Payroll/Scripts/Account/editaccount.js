$(function () {

    function handleFormSubmit() {
        var roleIds = [];
        $('input.js-role:checkbox:checked').each(function () {
            roleIds.push($(this).data('roleid'));
        });

        $('#CheckedRoles').val(roleIds.toString());
    };

    function init() {

        $('form').on('submit', handleFormSubmit);
    };

    init();
});