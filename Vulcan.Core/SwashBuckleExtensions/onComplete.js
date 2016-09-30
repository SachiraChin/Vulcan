$('#input_apiKey').change(function () {
    debugger;
    console.log("test");
    var key = $('#input_apiKey')[0].value;
    if (key && key.trim() != '') {
        key = 'Bearer ' + key;
        // console.log(window.authorizations);
        if (window.authorizations.remove)
            window.authorizations.remove("api_key");
        window.authorizations.add('key', new ApiKeyAuthorization('Authorization', key, 'header'));
    }
});