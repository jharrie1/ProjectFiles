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

function recieved() {
    tasktitle = document.getElementById("tasktitle").value;
    description = document.getElementById("description").value;
    date = document.getElementById("date").value;
    time = document.getElementById("time").value;
    hours = document.getElementById("hours").value;
    areaLocation = document.getElementById("areaLocation").value;

    var currentDate = new Date(Date.now());
    var newDate = date.concat(' ' + time);
    var inputDate = new Date(newDate);
    //Creating Date objects: https://www.w3schools.com/js/js_dates.asp
    //Getting current date: https://www.w3schools.com/js/js_date_methods.asp
    //Reminder of how to concatenate strings in javascript: https://www.w3schools.com/jsref/jsref_concat_string.asp

    //Consider coming back and adding return to the first two if conditions.
    if (inputDate < currentDate) {
        alert("Time has already passed. Choose another time or date.")
    }

    else if (tasktitle, description, date, time, hours, areaLocation == "")
    {
        alert("Please fill out all values before submitting.")
    }

    else if (tasktitle.length > 50) {
        alert('Title too long. Please enter a shorter description.')
        return;
    }

    else if (description.length > 500) {
        alert('Description too long. Please enter a shorter description.')
        return;
    }

    else if (hours.length > 2) {
        alert('Hours too long. Please keep under 24 hours.')
        return;
    }

    else if (areaLocation.length > 200) {
        alert('Location description too long. Please enter a shorter location description.')
        return;
    }

    else {
        var webMethod = "ProjectServices.asmx/CreateTask";
        var parameters = "{\"title\":\"" + encodeURI(tasktitle) + "\", \"description\":\"" + encodeURI(description) + "\", \"date\":\"" + encodeURI(date) +
            "\", \"time\":\"" + encodeURI(time) + "\", \"hours\":\"" + encodeURI(hours) + "\", \"location\":\"" + encodeURI(areaLocation) +"\"}";


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

}
