/*Summary
 * 
 * Similar logic to userManagement, with difference being that the promote and ban functions either promote a user to an admin position or
 *  ban a user from the system and blacklist their email.
 * */

function home() {

    var home = document.getElementById("home");

    var webMethod = "ProjectServices.asmx/AdminCheck";

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
}

function recieved() {

    var value = document.getElementById('searchBar').value; 
    var table = document.getElementById("table");

    if (value == "") {
        alert("Please type a value to search.")
        return;
    }

    table.style.visibility = "hidden";

    var webMethod = "ProjectServices.asmx/GetUsers";
    var parameters = "{\"search\":\"" + encodeURI(value) + "\"}";

     $.ajax({

         type: "POST",
         url: webMethod,
         data: parameters,
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

             for (i = 0; i < userArray.length; i++) {
                 var fieldset = document.createElement("fieldset");
                 var button1 = document.createElement("input");
                 var button2 = document.createElement("input");
                 var label = document.createElement('label');
                 var div1 = document.createElement('div');
                 var div2 = document.createElement('div');

                 fieldset.name = String(userArray[i]["id"]);

                 button1.type = "button";
                 button1.value = "Promote";
                 button1.setAttribute("onclick", "promote(this);"); //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                 button1.onclick = function () { promote(this); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859

                 button2.type = "button";
                 button2.value = "Ban";
                 button2.setAttribute("onclick", "ban(this);"); //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                 button2.onclick = function () { ban(this); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859

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
};

function promote(element) {

    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;
    var table = document.getElementById("table");


    var webMethod = "ProjectServices.asmx/Promote";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var result = msg.d;
            alert(result);
            recieved();
            table.style.visibility = "hidden";
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });

}

function ban(element) {

    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;
    var table = document.getElementById("table");


    var webMethod = "ProjectServices.asmx/Ban";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var result = msg.d;
            alert(result);
            recieved();
            table.style.visibility = "hidden";
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });

}

function display(element) {
    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;

    var webMethod = "ProjectServices.asmx/GetUserInfo"; 
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            
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