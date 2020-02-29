function logoff() {

    var webMethod = "ProjectServices.asmx/LogOff";

    $.ajax({
        type: "POST",
        url: webMethod,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            window.open("index.html", "_self");
        },
        error: function (e) {
            alert(e);
        }
    });
}

function deleteAccount() {

    var input = confirm("Are you sure you want to delete your account?");
    //Refreshed confirm method here: https://www.w3schools.com/jsref/met_win_confirm.asp

    if (input == true) {

        var webMethod = "ProjectServices.asmx/DeleteAccount";

        $.ajax({
            type: "POST",
            url: webMethod,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                logoff();
            },
            error: function (e) {
                alert(e);
            }
        });

    }


}