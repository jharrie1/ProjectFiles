function recieved() {
    fname = document.getElementById("firstname").value;
    lname = document.getElementById("lastname").value;
    email = document.getElementById("email").value;
    username = document.getElementById("username").value;
    password = document.getElementById("password").value;
    cpassword = document.getElementById("confirmpassword").value;

    if (password != cpassword) {
       alert("Password is inconsistent. Please try again.")
       return;
    }

    var webMethod = "ProjectServices.asmx/RequestAccount";
    var parameters = "{\"username\":\"" + encodeURI(username) + "\", \"password\":\"" + encodeURI(password) +
        "\", \"firstName\":\"" + encodeURI(fname) + "\", \"lastName\":\"" + encodeURI(lname) + "\", \"email\":\"" + encodeURI(email) +"\"}";

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