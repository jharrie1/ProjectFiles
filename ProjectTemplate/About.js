/*Summary: 
 * The home function involves only the header element.
 * First, the AdminCheck ajax function is used to tell if a user is signed in, and if so, if they are a normal user or an admin.
 * 0 represents a non-signed in user, 1 represents a normal user, and 2 represents an admin.
 * Then, depending on the response, the home button href is changed and additional anchor elements are added to the page specific to the user's access.
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
            else if (status == 2) {
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
            else {
                home.href = "index.html";

                var About = document.createElement("a");
                var accountCreation = document.createElement("a");
                var Login = document.createElement("a");

                About.className = "right";
                accountCreation.className = "right";
                Login.className = "right";

                About.href = "About.html";
                accountCreation.href = "accountCreation.html";
                Login.href = "login.html";

                About.innerHTML = "About";
                accountCreation.innerHTML = "Create an Account";
                Login.innerHTML = "Log In";

                headerBar.appendChild(Login);
                headerBar.appendChild(About);
                headerBar.appendChild(accountCreation);
                
            }
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
}