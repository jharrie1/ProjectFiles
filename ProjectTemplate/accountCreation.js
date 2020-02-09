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
        "\", \"firstname\":\"" + encodeURI(fname) + "\", \"lastname\":\"" + encodeURI(lname) + "\", \"email\":\"" + encodeURI(email) +"\"}";

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

//Questions:

    //1. Is there a way to prevent the form from automatically refershing the page?
    //2. Why does the submit button for the login page not work, but the method does?
    //3. Why cant the server find my accountCreation.js file?
    //4. Why doesn't this code connect to the backend code?
    //