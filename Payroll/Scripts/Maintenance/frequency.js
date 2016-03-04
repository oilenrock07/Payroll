$(function () {

    function handleFrequencyTypeChange() {
        var frequencyId = parseInt($(this).val());

        
        if (frequencyId == 1) {
            $('[data-frequency="enddate"]').addClass('hidden');
            $('[data-frequency="startdate"]').addClass('hidden');
            $('[data-frequency="weekly"]').removeClass('hidden');
        }
        else if (frequencyId == 2) {
            $('[data-frequency="weekly"]').addClass('hidden');
            $('[data-frequency="startdate"]').removeClass('hidden');
            $('[data-frequency="enddate"]').removeClass('hidden');
        }
        else {
            $('[data-frequency="weekly"]').addClass('hidden');
            $('[data-frequency="startdate"]').removeClass('hidden');
            $('[data-frequency="enddate"]').addClass('hidden');
        }

    }

    function init() {
        $('.js-frequencyType').on('change', handleFrequencyTypeChange);
    }

    init();
});