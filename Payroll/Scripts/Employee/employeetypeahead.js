$(function() {

    function handleTypeAheadGetData(term, process) {
        $.ajax({
            url: '/Lookup/LookUpEmployee',
            method: 'POST',
            data: { criteria: $('.employee-typeahead').val() },
            success: function (responseData) {
                process(responseData);
            }
        });
    }

    function handleUpdater(item) {
        $('.js-employeeIdTypeAhead').val(item.id);
        return item.name;
    }

    function handleTypeAheadKeyDown() {
        $('.js-employeeIdTypeAhead').val('');
    }

    function init() {
        $("input.typeahead.employee-typeahead").keydown(handleTypeAheadKeyDown);
        $("input.typeahead.employee-typeahead").typeahead({
            source: handleTypeAheadGetData,
            minLength: 2,
            items:9999,
            updater: handleUpdater
        });
    }

    init();
});