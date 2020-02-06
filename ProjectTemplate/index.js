

    function TestButtonHandler() {
        var webMethod = "ProjectServices.asmx/TestConnection";
        var parameters = "{}";

        $.ajax({
            type: "POST",
            url: webMethod,
            data: parameters,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var responseFromServer = msg.d;
                alert(responseFromServer);
            },
            error: function (e) {
                alert("this code will only execute if javascript is unable to access the webservice");
            }
        });
    }