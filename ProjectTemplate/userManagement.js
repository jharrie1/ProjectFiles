function accept(element) {

    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;
    var table = document.getElementById("table");


    var webMethod = "ProjectServices.asmx/ActivateAccount";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            recieved();
            table.style.visibility = "hidden";
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });

}

function deny(element) {

    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;
    var table = document.getElementById("table");


    var webMethod = "ProjectServices.asmx/RejectAccount";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            recieved();
            table.style.visibility = "hidden";
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });

}

function recieved() {

    var webMethod = "ProjectServices.asmx/GetRequests";

    $.ajax({
        type: "POST",
        url: webMethod,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            userArray = msg.d;
            var form = document.getElementById("form1")

            var removeArray = []
            
            //Where I found the nodeName method instructions: https://stackoverflow.com/questions/254302/how-can-i-determine-the-type-of-an-html-element-in-javascript
            for (i = 0; i < form.length; i++) {
                if (form[i].nodeName == "FIELDSET") {
                    removeArray.push(i);
                }
            }
            console.log(removeArray.length)
            for (i = 0; i < removeArray.length; i++) {
                var value = removeArray[0];
                form[value].remove();
            } 
            //Changed line 77, the square brackets held i, now it is just 0. 
            //Wrote this way as doing it in single for loop breaks loop when an element is removed. 
            //Call based on if name property is null, not by indicy.

            for (i = 0; i < userArray.length; i++) {
                var fieldset = document.createElement("fieldset");
                var button1 = document.createElement("input");
                var button2 = document.createElement("input");
                var label = document.createElement('label');
                var div1 = document.createElement('div');
                var div2 = document.createElement('div');

                fieldset.name = String(userArray[i]["id"]);

                button1.type = "button";
                button1.value = "Approve";
                button1.setAttribute("onclick", "accept(this);"); //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                button1.onclick = function () { accept(this); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859

                button2.type = "button";
                button2.value = "Deny";
                button2.setAttribute("onclick", "deny(this);"); //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                button2.onclick = function () { deny(this); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859

                label.innerHTML = userArray[i]["username"];
                label.setAttribute("onclick", "display(this);"); //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                label.onclick = function () { display(this); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859

                form.appendChild(fieldset);
                fieldset.appendChild(div1);
                fieldset.appendChild(div2);
                div1.appendChild(button1);
                div1.appendChild(button2);
                div2.appendChild(label);
            }
            
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });

    var home = document.getElementById("home");

    webMethod = "ProjectServices.asmx/AdminCheck";



    $.ajax({
        type: "POST",
        url: webMethod,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var status = msg.d;
            if (status == 1) {
                home.href = "userIndex.html";
            }
            else if (status == 2) {
                home.href = "adminIndex.html";
            }

            else {
                home.href = "index.html";
            }

        },

        error: function (e) {
                alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
};

//Want a label element clicked, whereby the label will be passed to the function
//This will allow us to find the parent element and get the id from it's name
//We can send this informatoin to the server, which will return to us a user object with our necessary information.
//Take this information, create a new row in our table, and store the values in the table. 
function display(element) {
    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;

    var webMethod = "ProjectServices.asmx/GetUserInfo"; //Change function name later. 
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //May need to update recieve function to set table visibility to hidden.
            var userArray = msg.d;
            var table = document.getElementById("table");
            var idrow = document.getElementById("idrow");
            var firstnamerow = document.getElementById("firstnamerow");
            var lastnamerow = document.getElementById("lastnamerow");
            var emailrow = document.getElementById("emailrow");
            var usernamerow = document.getElementById("usernamerow");

            table.style.visibility = "visible";

            idrow.innerHTML = userArray[0]['id'];
            firstnamerow.innerHTML = userArray[0]['firstName'];
            lastnamerow.innerHTML = userArray[0]['lastName'];
            emailrow.innerHTML = userArray[0]['email'];
            usernamerow.innerHTML = userArray[0]['username'];
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
}