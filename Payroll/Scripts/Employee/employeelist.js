$(function() {

    //configs
    var typeAheadConfig = {
        name: 'songs',
        valueKey: 'Name',
        remote: {
            url: '/Song/GetSongs?searchTterm=%QUERY'
        }
    };

    function handleEmployeeDeleteClick(e) {
        if (!confirm('Are you sure you want to delete this employee?')) {
            e.preventDefault();
            return;
        }
            
    }

    function handleSearchKeyDown(e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) { //Enter keycode
            handleSearchClick();
        }
    }

    function handleSearchClick() {
        var criteria = $('.js-searchEmployee').val();
        window.location = '/EmployeeController/SearchEmployee?query=' + criteria;
    }

    function init() {
        $('.js-employeeDelete').on('click', handleEmployeeDeleteClick);
        $('.js-searchEmployee').on('keydown', handleSearchKeyDown);
        $('.js-search').on('click', handleSearchClick);
        //$("#search").typeahead(typeAheadConfig);
    }

    init();
})