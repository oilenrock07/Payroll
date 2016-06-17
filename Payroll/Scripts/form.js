var callback = null;

function handleFormSubmit(e) {

    e.preventDefault();

    var url = $(this).attr('action');
    var method = $(this).attr('method');
    var data = $(this).serializeArray();

    $.ajax({
        url: url,
        method: method,
        data: data,
        success: callback
    });
}

function initForm(formId, success) {
    callback = success;
    $('#'+ formId).on('submit', handleFormSubmit);
}