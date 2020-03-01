
/*Summary
 * 
 * These function sare the same as the taskManagement.js file.
 * Only differences are the register which registers a user for a task (opposite of deregister) and clear, which clears away elements on the table. 
 * */

function home() {

    var home = document.getElementById("home");
    var headerBar = document.getElementById("headerBar");

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

                var About = document.createElement("a");
                var taskCreate = document.createElement("a");
                var taskSearch = document.createElement("a");
                var Logoff = document.createElement("a");
                var taskManagement = document.createElement("a");
                var DeleteAccount = document.createElement("a");

                About.className = "right";
                taskCreate.className = "right"
                taskSearch.className = "right";
                Logoff.className = "right";
                taskManagement.className = "right";
                DeleteAccount.className = "right";

                About.href = "About.html";
                taskCreate.href = "taskCreation.html"
                taskSearch.href = "taskSearch.html";
                Logoff.href = "#";
                taskManagement.href = "taskManagement.html";
                DeleteAccount.href = "#"

                About.innerHTML = "About";
                taskCreate.innerHTML = "Create Task"
                taskSearch.innerHTML = "Search Task";
                Logoff.innerHTML = "Log Off";
                taskManagement.innerHTML = "Current Tasks";
                DeleteAccount.innerHTML = "Delete this Account"

                Logoff.setAttribute("onclick", "logoff()") //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                DeleteAccount.setAttribute("onclick", "deleteAccount()") //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                Logoff.onclick = function () { logoff(); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859
                DeleteAccount.onclick = function () { deleteAccount(); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859


                headerBar.appendChild(Logoff);
                headerBar.appendChild(DeleteAccount);
                headerBar.appendChild(About);
                headerBar.appendChild(taskCreate)
                headerBar.appendChild(taskSearch);
                headerBar.appendChild(taskManagement);
            }
            else {
                home.href = "adminIndex.html";

                var About = document.createElement("a");
                var taskCreate = document.createElement("a");
                var taskSearch = document.createElement("a");
                var Logoff = document.createElement("a");
                var taskManagement = document.createElement("a");
                var userSearch = document.createElement("a");
                var userManagement = document.createElement("a");
                var DeleteAccount = document.createElement("a");

                About.className = "right";
                taskCreate.className = "right"
                taskSearch.className = "right";
                Logoff.className = "right";
                taskManagement.className = "right";
                userSearch.className = "right";
                userManagement.className = "right";
                DeleteAccount.className = "right";

                About.href = "About.html";
                taskCreate.href = "taskCreation.html"
                taskSearch.href = "taskSearch.html";
                Logoff.href = "#";
                taskManagement.href = "taskManagement.html";
                userSearch.href = "userSearch.html";
                userManagement.href = "userManagement.html";
                DeleteAccount.href = "#"

                About.innerHTML = "About";
                taskCreate.innerHTML = "Create Task"
                taskSearch.innerHTML = "Search Task";
                Logoff.innerHTML = "Log Off";
                taskManagement.innerHTML = "Current Tasks";
                userSearch.innerHTML = "Find User";
                userManagement.innerHTML = "New Users";
                DeleteAccount.innerHTML = "Delete this Account"

                Logoff.setAttribute("onclick", "logoff()") //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                DeleteAccount.setAttribute("onclick", "deleteAccount()") //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                Logoff.onclick = function () { logoff(); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859
                DeleteAccount.onclick = function () { deleteAccount(); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859


                headerBar.appendChild(Logoff);
                headerBar.appendChild(DeleteAccount);
                headerBar.appendChild(About);
                headerBar.appendChild(taskCreate)
                headerBar.appendChild(taskSearch);
                headerBar.appendChild(taskManagement);
                headerBar.appendChild(userSearch);
                headerBar.appendChild(userManagement);
            }
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
}

function register(element) {

    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;


    var webMethod = "ProjectServices.asmx/RegisterTask";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var message = msg.d;
            alert(msg.d);
            if (message == "Registered") {
                clear(id);
            }
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
}

function clear(id) {
    var form = document.getElementById("form1")
    var table = document.getElementById("table");

    table.style.visibility = "hidden";

    for (i = 0; i < form.length; i++) {
        if (form[i].name == id) {
            form[i].remove();
            return;
        }
    }
}

function recieved() {

    var value = document.getElementById('searchBar').value;
    var table = document.getElementById("table");

    if (value == "") {
        alert("Please type a value to search.")
        return;
    }

    table.style.visibility = "hidden";


     var webMethod = "ProjectServices.asmx/GetTasks";
     var parameters = "{\"search\":\"" + encodeURI(value) + "\"}";

     $.ajax({

         type: "POST",
         url: webMethod,
         data: parameters,
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: function (msg) {

             taskArray = msg.d;
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

             for (i = 0; i < taskArray.length; i++) {
                 var fieldset = document.createElement("fieldset");
                 var button1 = document.createElement("input");
                 var label = document.createElement('label');
                 var div1 = document.createElement('div');
                 var div2 = document.createElement('div');

                 fieldset.name = String(taskArray[i]["id"]);

                 button1.type = "button";
                 button1.value = "Register";
                 button1.setAttribute("onclick", "register(this);"); //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                 button1.onclick = function () { register(this); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859

                 label.setAttribute("onclick", "display(this);"); //Where: https://stackoverflow.com/questions/3316207/add-onclick-event-to-newly-added-element-in-javascript
                 label.onclick = function () { display(this); }; //Where: https://stackoverflow.com/questions/95731/why-does-an-onclick-property-set-with-setattribute-fail-to-work-in-ie/95859#95859
                 label.innerHTML = taskArray[i]["title"];

                 form.appendChild(fieldset);
                 fieldset.appendChild(div1);
                 fieldset.appendChild(div2);
                 div1.appendChild(button1);
                 div2.appendChild(label);
             }

         },
         error: function (e) {
             alert("this code will only execute if javascript is unable to access the webservice");
         }
     });
};

function display(element) {
    var div = element.parentElement;
    var fieldset = div.parentElement;
    var id = fieldset.name;

    var webMethod = "ProjectServices.asmx/GetTaskInfo";
    var parameters = "{\"id\":\"" + encodeURI(id) + "\"}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            
            var taskArray = msg.d;
            var table = document.getElementById("table");
            var idrow = document.getElementById("idrow");
            var titlerow = document.getElementById("titlerow");
            var descriptionrow = document.getElementById("descriptionrow");
            var daterow = document.getElementById("daterow");
            var timerow = document.getElementById("timerow");
            var hoursrow = document.getElementById("hoursrow");
            var locationrow = document.getElementById("locationrow");

            table.style.visibility = "visible";

            idrow.innerHTML = taskArray[0]['id'];
            titlerow.innerHTML = taskArray[0]['title'];
            descriptionrow.innerHTML = taskArray[0]['description'];
            timerow.innerHTML = taskArray[0]['starttime'];
            hoursrow.innerHTML = taskArray[0]['hours'];
            locationrow.innerHTML = taskArray[0]['location'];

            var newDate = new Date(taskArray[0]['date']);
            daterow.innerHTML = newDate.getMonth() + 1 + "-" + newDate.getDate() + "-" + newDate.getFullYear();
            //Reference to where I learned date objects in JS in the taskCreation file.
             

        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
}