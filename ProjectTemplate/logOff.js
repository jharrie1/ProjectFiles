/*Summary
 * Goes to the logoff function, gets rid of the current session variables, then takes the user back to the non-signed in home page.
 * */
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
/*Summary
 * Gives a user the choice on whether they still want to delete their account.
 * If approved, then call the DeleteAccount method to remove them from the database.
 * Finally, call the logoff function to send them back to the non-user home page.
 * */
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