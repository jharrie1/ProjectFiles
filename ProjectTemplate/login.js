function recieved() {
    username = document.getElementById("username").value;
    password = document.getElementById("password").value;
    //Potentially have a ToString() method here to guarentee that these are strings. 
    //alert(username);

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
            alert(responseFromServer);
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
};