$(function() {

    function handleTypeAheadGetData(term, process) {
        $.ajax({
            url: '/Lookup/LookUpCompany',
            method: 'POST',
            data: { criteria: $('.company-typeahead').val() },
            success: function (responseData) {
                process(responseData);
            }
        });
    }

    function handleUpdater(item) {
        $('.js-companyIdTypeAhead').val(item.id);
        return item.name;
    }

    function handleTypeAheadKeyDown() {
        $('.js-companyIdTypeAhead').val('');
    }

    function init() {
        $("input.typeahead.company-typeahead").keydown(handleTypeAheadKeyDown);
        $("input.typeahead.company-typeahead").typeahead({
            source: handleTypeAheadGetData,
            minLength: 2,
            items: 9999,
            updater: handleUpdater
        });
    }

    init();
});