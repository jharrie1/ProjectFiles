function recieved() {
    fname = document.getElementById("firstname").value;
    lname = document.getElementById("lastname").value;
    email = document.getElementById("email").value;
    username = document.getElementById("username").value;
    password = document.getElementById("password").value;
    cpassword = document.getElementById("confirmpassword").value;

    if (password != cpassword) {
       alert("Password is inconsistent. Please verify password and password confirmation.")
       return;
    }
}

//Questions:

    //1. Is there a way to prevent the form from automatically refershing the page?
    //2. Is email validation automatic, or do I have to program something in javascript?