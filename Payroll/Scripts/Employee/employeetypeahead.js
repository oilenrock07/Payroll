$(function() {

    function handleTypeAheadGetData(term, process) {
        $.ajax({
            url: '/Lookup/LookUpEmployee',
            method: 'POST',
            data: { criteria: $('.js-typeAhead').val() },
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
        $("input.typeahead").keydown(handleTypeAheadKeyDown);
        $("input.typeahead").typeahead({
            source: handleTypeAheadGetData,
            minLength: 2,
            updater: handleUpdater
        });
    }

    init();
});