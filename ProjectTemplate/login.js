/*Summary
 * Get the username and password and pass it to the LogOn web service.
 * Based on the response, the user is taken to a new index page, told that their credentials are incorrect, or tells them that their account is not 
 *  admin approved.
 * */
function recieved() {
    username = document.getElementById("username").value;
    password = document.getElementById("password").value;

    var webMethod = "ProjectServices.asmx/LogOn";
    var parameters = "{\"username\":\"" + encodeURI(username) + "\", \"password\":\"" + encodeURI(password) + "\"}";


    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var responseFromServer = msg.d;

            if (responseFromServer == "admin") {
                document.getElementById("username").value = "";
                document.getElementById("password").value = "";
                window.open("adminIndex.html", "_self");
            }
            else if (responseFromServer == "user")
            {
                document.getElementById("username").value = "";
                document.getElementById("password").value = "";
                window.open("userIndex.html", "_self");
            }
            else if (responseFromServer == "notapproved") {
                document.getElementById("username").value = "";
                document.getElementById("password").value = "";
                alert("Cannot sign in until account is approved.");
            }
            else {
                alert("Incorrect username or password. Please try again.");
            }
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
};