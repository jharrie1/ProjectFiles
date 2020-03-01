using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProjectTemplate
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		///Personal credentials are used
		private string dbID = "solo";
		private string dbPass = "!!Solo440";
		private string dbName = "solo";
		
		//connection string method used for every database call.
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}

        //Signs a user into their account and takes them to the proper home page based on the string return.
        [WebMethod(EnableSession = true)]
        public string LogOn(string username, string password)
        {
            //Parameter names were changed.

            string success = "";

            //string changed to match the current database tables.
            string sqlSelect = "SELECT id, status, approved FROM users WHERE username=@idValue and password=@passValue";

            //Using the getConString() method to get access to the database (had diff. method before, which was deleted.
            MySqlConnection con = new MySqlConnection(getConString());
            
            //Changed name of connection object argument to meet the new name (con)
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(username));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(password));

            //Fill a data table with the return (should be a single row).
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            DataTable sqlDt = new DataTable();
            sqlDa.Fill(sqlDt);
            
            //Choose what to return based on the approved/status variables returned. Finally return a string to signify this.
            if (sqlDt.Rows.Count > 0)
            {
                if (Convert.ToInt32(sqlDt.Rows[0]["approved"]) == 0 )
                {
                    return "notapproved";
                }

                Session["id"] = sqlDt.Rows[0]["id"];
                Session["status"] = sqlDt.Rows[0]["status"];

                if (Convert.ToInt32(Session["status"]) != 0)
                {
                    success = "admin";
                }
                else
                {
                    success = "user";
                }
            }

            else
            {
                success = "false";
            }
           
            return success;
        } 

        //Verify several conditions and then provide a string message on what the final result is.
        [WebMethod(EnableSession = true)]
        public string RequestAccount(string username, string password, string firstName, string lastName, string email)
        {
            //Have a string variable to be returned upon completion of this method.
            string success = " ";

            //Changed datatable name to match my own class/data (users). 
            DataTable sqlDt = new DataTable("users");

            //Changed column names and table name to match database.
            string sqlSelect = "select id, username, password, firstname, lastname, email from users order by lastname";

            //Use the getConString() function instead of prior code.
            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

            //Data table is filled.
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);

            //Changed to a User list to match current class.
            List<User> users = new List<User>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                users.Add(new User
                {
                    //Change variable names to match the class variables.
                    id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
                    username = sqlDt.Rows[i]["username"].ToString(),
                    password = sqlDt.Rows[i]["password"].ToString(),
                    firstName = sqlDt.Rows[i]["firstname"].ToString(),
                    lastName = sqlDt.Rows[i]["lastname"].ToString(),
                    email = sqlDt.Rows[i]["email"].ToString()
                });
            }
            //Create a user array object to loop through
            User[] userArray = users.ToArray();

            //Loop through user array to see if duplicate emails/usernames are being used. If so, I return an alert to the user and break
            //from the current method.
            for (int i = 0; i < userArray.Length; i++)
            {
                if (userArray[i].email == email)
                {
                    success = "Email already has a user account. Please use a different email.";
                    return success;
                }

                if (userArray[i].username == username)
                {
                    success = "Username is taken. Please try a different username.";
                    return success;
                }
            }


            //Once above checks are completed, check to see if the email being used is banned.
            //If so, return a message stating so and break.
            //However, if not, continue to insert the values into the useres table.
            sqlSelect = "Select email from banned where email=@emailValue";
            sqlCommand = new MySqlCommand(sqlSelect, con);
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
            sqlDt = new DataTable("banned"); //See if this needs changing.
            sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);
            if (sqlDt.Rows.Count > 0)
            {
                success = "Email is banned from creating new accounts.";
                return success;
            }


            //Create a new  sql statement if all information is non-repeating and pass values to the database to be stored.
            sqlSelect = "insert into users (username, password, firstname, lastname, email) " +
               "values(@idValue, @passValue, @fnameValue, @lnameValue, @emailValue); SELECT LAST_INSERT_ID();";
            sqlCommand = new MySqlCommand(sqlSelect, con);

            //names edited to match variable names listed prior.
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(username));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(password));
            sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
            sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));

            //Open the connection and then submit the new account to await approval.
            con.Open();
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar()); 
                success = "Account submitted for admin approval.";
            }
            catch (Exception e)
            {
            }
               
            con.Close();
            return success;
        }

        //Get user accounts that have not been approved
        //Only admins should have the power to approve new users, hence the admin check.
        [WebMethod(EnableSession = true)]
        public User[] GetRequests()
        {
            //Verify that the user is an admin.
            if (Convert.ToInt32(Session["status"]) == 1)
            {
                //create a new data table, string, and connection and then fill
                DataTable sqlDt = new DataTable("users");

                string sqlSelect = "select id, username, password, firstname, lastname, email from users where approved=0 order by lastname";
                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

                
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                sqlDa.Fill(sqlDt);

                //Create a user list and add users based on values returned.
                List<User> users = new List<User>();
                for (int i = 0; i < sqlDt.Rows.Count; i++)
                {
                        users.Add(new User
                        {
                            id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
                            username = sqlDt.Rows[i]["username"].ToString(),
                            password = sqlDt.Rows[i]["password"].ToString(),
                            firstName = sqlDt.Rows[i]["firstname"].ToString(),
                            lastName = sqlDt.Rows[i]["lastname"].ToString(),
                            email = sqlDt.Rows[i]["email"].ToString()
                        });
                }
                //Return the list as an array or an empty array if the user is not an admin.
                return users.ToArray();
            }
            else
            {
                return new User[0];
            }
        }
      
        //Change the approved column of a user to approve their account.
        //Since only an admin should do this, there is an admin check.
        //This function is straight forward, so I'm not going to add extra comments.
        [WebMethod(EnableSession = true)]
        public void ActivateAccount(string id)
        {
            if (Convert.ToInt32(Session["status"]) == 1)
            {
                string sqlSelect = "update users set approved=1 where id=@idValue";

                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);
                sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

                con.Open();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                }
                con.Close();
            }
        }

        //Same as ActivateAccount(), except we are removing the account instead of changing it.
        //Using an admin check as only admins should do this.
        //Also straightforward, so not going to add comments.
        [WebMethod(EnableSession = true)]
        public void RejectAccount(string id)
        {
            if (Convert.ToInt32(Session["status"]) == 1)
            {
                string sqlSelect = "delete from users where id=@idValue";

                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

                sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

                con.Open();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                }
                con.Close();
            }
        }

        //Clears the session, nothing else.
        [WebMethod(EnableSession = true)]
        public void LogOff()
        {
            Session.Abandon();
        }

        //Returns a true or false based on if adding a task to the task table was successful.
        [WebMethod(EnableSession = true)]
        public bool CreateTask(string title, string description, string date, string time, string hours, string location)
        {
            //Create our return variable, string, and connection parameters.
            bool success = false;

            string sqlSelect = "insert into tasks (managerid, title, description, date, starttime, hours, location) " +
               "values(@idValue, @titleValue, @descriptionValue, @dateValue, @starttimeValue, @hoursValue, @locationValue); SELECT LAST_INSERT_ID();";

            //Use the getConString() function instead of prior code.
            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);
            //names edited to match variable names listed prior.
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(Session["id"].ToString())); //Make sure this works later. 
            sqlCommand.Parameters.AddWithValue("@titleValue", HttpUtility.UrlDecode(title));
            sqlCommand.Parameters.AddWithValue("@descriptionValue", HttpUtility.UrlDecode(description));
            sqlCommand.Parameters.AddWithValue("@dateValue", HttpUtility.UrlDecode(date));
            sqlCommand.Parameters.AddWithValue("@starttimeValue", HttpUtility.UrlDecode(time));
            sqlCommand.Parameters.AddWithValue("@hoursValue", HttpUtility.UrlDecode(hours));
            sqlCommand.Parameters.AddWithValue("@locationValue", HttpUtility.UrlDecode(location));

            //Create the connection and try to add the new account to the tasks table.
            con.Open();
 
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
                success = true;
            }
            catch (Exception e)
            {
            }
            con.Close();
            return success;
        }

        //Returns related tasks to values entered in a search bar.
        [WebMethod(EnableSession = true)]
        public Task[] GetTasks(string search)
        {
            //Need to concatenate first % signs to the passed argument.
            string test = '%' + search + '%';

            //Then able to use the like keyword and create a list of tasks with their id and title to be passed back.
            DataTable sqlDt = new DataTable("tasks");

            string sqlSelect = "select id, title from tasks where title like @testValue or description like @testValue " +
                "or date like @testValue or starttime like @testValue or hours like @testValue or location like @testValue and volunteerid is null";

            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

            sqlCommand.Parameters.AddWithValue("@testValue", HttpUtility.UrlDecode(test));


            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            sqlDa.Fill(sqlDt);

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                tasks.Add(new Task
                {
                    id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
                    title = sqlDt.Rows[i]["title"].ToString()
                });
            }
            return tasks.ToArray();
        }

        //Allows a user to register for a task so long as they are not the ones that created it. 
        //String is returned to show whether the registration was successful.
        [WebMethod(EnableSession = true)]
        public string RegisterTask(string id)
        {
            //First, access database to make sure that the person registering is not the person that created the task.
            var success = "";
            string sqlSelect = "Select managerid from tasks where id=@idValue";
            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

            DataTable sqlDt = new DataTable("tasks");
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);

            //After confirmation, move on and update the task table to set the value of the volunteerid.
            if (Convert.ToInt32(sqlDt.Rows[0]["managerid"]) == Convert.ToInt32(Session["id"]))
            {
                success = "Cannot register for a task that you created.";
                return success;
            }

            sqlSelect = "update tasks set volunteerid=@sessionValue where id=@idValue";

            con = new MySqlConnection(getConString());
            sqlCommand = new MySqlCommand(sqlSelect, con);

            sqlCommand.Parameters.AddWithValue("@sessionValue", HttpUtility.UrlDecode(Session["id"].ToString()));
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

            con.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
                success = "Registered";
            }
            catch (Exception e)
            {
                success = e.ToString();
            }
            con.Close();
            return success;
        }

        //Returns a user array for a searched user. 
        //For now only admins can do this, due to the permissions associated with the user. 
        [WebMethod(EnableSession = true)]
        public User[] GetUsers(string search)
        {
            if (Convert.ToInt32(Session["status"]) == 1)
            {
                //Conceatentate the parameter, pass to use the like keyword, and create a user list if an admin or pass a blank list if not an admin.
                string test = '%' + search + '%';

                DataTable sqlDt = new DataTable("users");

                 string sqlSelect = "select id, username from users where approved=1 and (username like @testValue or firstname like @testValue " +
                    "or lastname like @testValue or email like @testValue)";

                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

                sqlCommand.Parameters.AddWithValue("@testValue", HttpUtility.UrlDecode(test));


                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

                sqlDa.Fill(sqlDt);

                List<User> users = new List<User>();
                for (int i = 0; i < sqlDt.Rows.Count; i++)
                {
                    users.Add(new User
                    {
                        id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
                        username = sqlDt.Rows[i]["username"].ToString()
                    });
                }
                return users.ToArray();
            }
            else
            {
                return new User[0];
            }
        }

        //Check to see if a current user is an admin to customize their header. 
        //0 is not signed in, 1 is a general user, 2 is an admin.
        [WebMethod(EnableSession = true)]
        public int AdminCheck()
        {
            try
            {
                if (Session["status"].ToString() == "0")
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            catch
            {
                return 0;
            }
        }

        //Get information to display when a user label is clicked.
        //Since only admins can currently access user information, this will have an admin check for now.
        [WebMethod(EnableSession = true)]
        public User[] GetUserInfo(string id)
        {
            //Just selecting the user information for the label we click on, nothing else. 
            if (Convert.ToInt32(Session["status"]) == 1)
            {
                DataTable sqlDt = new DataTable("users");

                string sqlSelect = "select id, username, firstname, lastname, email from users where id= @testValue";

                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

                sqlCommand.Parameters.AddWithValue("@testValue", HttpUtility.UrlDecode(id));


                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

                sqlDa.Fill(sqlDt);

                List<User> user = new List<User>();
                for (int i = 0; i < sqlDt.Rows.Count; i++)
                {
                    user.Add(new User
                    {
                        id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
                        username = sqlDt.Rows[i]["username"].ToString(),
                        firstName = sqlDt.Rows[i]["firstname"].ToString(),
                        lastName = sqlDt.Rows[i]["lastname"].ToString(),
                        email = sqlDt.Rows[i]["email"].ToString()
                    });
                }
                return user.ToArray();
            }
            else
            {
                return new User[0];
            }
        }

        //Same as the function above, as display task information when a label is clicked.
        //No other explanation is necessary.
        [WebMethod(EnableSession = true)]
        public Task[] GetTaskInfo(string id)
        {
            DataTable sqlDt = new DataTable("users");

            string sqlSelect = "select id, title, description, date, starttime, hours, location from tasks where id= @testValue";

            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

            sqlCommand.Parameters.AddWithValue("@testValue", HttpUtility.UrlDecode(id));


            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            sqlDa.Fill(sqlDt);

            List<Task> task = new List<Task>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                task.Add(new Task
                {
                    id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
                    title = sqlDt.Rows[i]["title"].ToString(),
                    description = sqlDt.Rows[i]["description"].ToString(),
                    date = sqlDt.Rows[i]["date"].ToString(),
                    starttime = sqlDt.Rows[i]["starttime"].ToString(),
                    hours = Convert.ToInt32(sqlDt.Rows[i]["hours"]),
                    location = sqlDt.Rows[i]["location"].ToString()
                });
            }
            return task.ToArray();
        }

        //Get tasks that a user is currently registered for. 
        [WebMethod(EnableSession = true)]
        public Task[] GetCurrentTasks()
        {
            //Similar to codes above,but this time we are checking in the volunteerid column.
            DataTable sqlDt = new DataTable("tasks");

            string sqlSelect = "select id, title from tasks where volunteerid=@idValue";

            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(Session["id"].ToString()));
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            sqlDa.Fill(sqlDt);

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                tasks.Add(new Task
                {
                    id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
                    title = sqlDt.Rows[i]["title"].ToString(),
                });
            }
            return tasks.ToArray();
        }

        //Unregister for a task that a user was priorly registered for.
        [WebMethod(EnableSession = true)]
        public void DeregisterTask(string id)
        {
            //Setting the volunteerid column to null to let the task be unclaimed.
            string sqlSelect = "update tasks set volunteerid=null where id=@idValue";

            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

            con.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            con.Close();
        }

        //Allows an admin to promote a user to an admin. 
        //Checks also to make sure that the user is not already promoted.
        //Since it is a web service involving users, it will have an admin check for now.
        [WebMethod(EnableSession = true)]
        public string Promote(string id) 
        {
            if (Convert.ToInt32(Session["status"]) == 1)
             {
                string sqlSelect = "Select status from users where id=@idValue";

                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);
                sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                DataTable sqlDt = new DataTable();
                sqlDa.Fill(sqlDt);

                if (Convert.ToInt32(sqlDt.Rows[0]["status"]) == 1)
                {
                    return "User is already an admin. No changes made.";
                }


                sqlSelect = "update users set status=1 where id=@idValue";
                sqlCommand = new MySqlCommand(sqlSelect, con);
                sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

                con.Open();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    con.Close();
                    return "User promoted to admin.";
                }
                catch (Exception e)
                {
                    con.Close();
                    return "Error, change could not be made.";
                }
            }
            else
            {
                return "Error, user is not an admin";
            }
        }

        //An admin can ban a user account, move their email to the banned table, and delete them from useres.
        //They cannot do this to other admins.
        //Since it deals with user accounts, it will have an admin check.
        [WebMethod(EnableSession = true)]
        public string Ban(string id)
        {
            //Do an initial check to see if the user is an admin, if not the ban can continue.
            if (Convert.ToInt32(Session["status"]) == 1)
             {
                string sqlSelect = "Select email, status from users where id=@idValue";

                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);
                sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                DataTable sqlDt = new DataTable();
                sqlDa.Fill(sqlDt);

                var email = sqlDt.Rows[0]["email"];

                if (Convert.ToInt32(sqlDt.Rows[0]["status"]) == 1)
                {
                    return "An admin cannot ban another admin.";
                }


                //Move email to one table and remove the user from our user table.
                sqlSelect = "insert into banned (adminid, email) values (@idValue, @emailValue); delete from users where email=@emailValue;";
                //Deletions automatically cascade to the tasks table, so don't need to explicitly call a delete for this table.
                sqlCommand = new MySqlCommand(sqlSelect, con);
                sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(Session["id"].ToString()));
                sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email.ToString()));

                con.Open();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    con.Close();
                    return "User has been banned.";
                }
                catch (Exception e)
                {
                    con.Close();
                    return "Error, change could not be made.";
                }
            }
            else
            {
                return ("Error, user is not an admin.");
            }
        }

        //Web service that allows a user to delete their own account.
        [WebMethod(EnableSession = true)]
        public void DeleteAccount() 
        {
            //Just remove their information from the user table in the database.
            string sqlSelect = "delete from users where id=@idValue";

            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(Session["id"].ToString()));

            con.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            con.Close();

        }
    }

  
}
