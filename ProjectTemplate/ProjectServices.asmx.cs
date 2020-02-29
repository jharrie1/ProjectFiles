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

        //LogIn Function (Based on the LogOn Function for accountmanager code file.
        //Comments will show changes made to get the function working
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

            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            
            DataTable sqlDt = new DataTable();
            
            sqlDa.Fill(sqlDt);
            
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

        //RequestAccount Function (based on RequestAccount function and the GetAccounts() function for data validation)
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

            con.Open();
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar()); //Check this out later to see if right method.
                success = "Account submitted for admin approval.";
            }
            catch (Exception e)
            {
            }
               
            con.Close();
            return success;
        }

        [WebMethod(EnableSession = true)]
        public User[] GetRequests()
        {
            //if (Session["id"] != null)
            //{
                DataTable sqlDt = new DataTable("users");

                string sqlSelect = "select id, username, password, firstname, lastname, email from users where approved=0 order by lastname";

                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);

                
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                
                sqlDa.Fill(sqlDt);

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
                return users.ToArray();
            //}
            /*else
            {
                return new User[0];
            }*/
        }//Uncomment the admin check later.
      
        [WebMethod(EnableSession = true)]
        public void ActivateAccount(string id) //Uncomment the admin check later.
        {
            //if (Convert.ToInt32(Session["admin"]) == 1)
           // {
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
            //}
        }

        [WebMethod(EnableSession = true)]
        public void RejectAccount(string id) //Uncomment the admin check later
        {
           // if (Convert.ToInt32(Session["admin"]) == 1)
            //{
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
           // }
        }

        [WebMethod(EnableSession = true)]
        public void LogOff()
        {
            Session.Abandon();
        }

        [WebMethod(EnableSession = true)]
        public bool CreateTask(string title, string description, string date, string time, string hours, string location)
        {

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

        [WebMethod(EnableSession = true)]
        public Task[] GetTasks(string search)
        {
            //if (Session["id"] != null)
            //{

            string test = '%' + search + '%';

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
                    //Can come back and add more returns later if need to display information.
                });
            }
            return tasks.ToArray();
            //}
            /*else
            {
                return new User[0];
            }*/
        }//Uncomment the admin check later.

        [WebMethod(EnableSession = true)]
        public string RegisterTask(string id) //Uncomment the admin check later.
        {
            //if (Convert.ToInt32(Session["admin"]) == 1)
            // {
            var success = "";
            string sqlSelect = "Select managerid from tasks where id=@idValue";
            MySqlConnection con = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, con);
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

            DataTable sqlDt = new DataTable("tasks");
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);

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
            //}
        }

        [WebMethod(EnableSession = true)]
        public User[] GetUsers(string search)
        {
            //if (Session["id"] != null)
            //{

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
                    //Can come back and add more returns later if need to display information.
                });
            }
            return users.ToArray();
            //}
            /*else
            {
                return new User[0];
            }*/
        }//Uncomment the admin check later.

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

        [WebMethod(EnableSession = true)]
        public User[] GetUserInfo(string id) //Uncomment the admin check later.
        {
            //if (Session["id"] != null)
            //{
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
            //}
            /*else
            {
                return new User[0];
            }*/
        }

        [WebMethod(EnableSession = true)]
        public Task[] GetTaskInfo(string id) //Uncomment the admin check later.
        {
            //if (Session["id"] != null)
            //{
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
            //}
            /*else
            {
                return new User[0];
            }*/
        }

        [WebMethod(EnableSession = true)]
        public Task[] GetCurrentTasks()
        {
            //if (Session["id"] != null)
            //{
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
            //}
            /*else
            {
                return new User[0];
            }*/
        }//Uncomment the admin check later.

        [WebMethod(EnableSession = true)]
        public void DeregisterTask(string id) //Uncomment the admin check later.
        {
            //if (Convert.ToInt32(Session["admin"]) == 1)
            // {
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
            //}
        }

        [WebMethod(EnableSession = true)]
        public string Promote(string id) //Uncomment the admin check later.
        {
            //if (Convert.ToInt32(Session["admin"]) == 1)
            // {
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
            //}
        }

        [WebMethod(EnableSession = true)]
        public string Ban(string id) //Uncomment the admin check later.
        {
            //if (Convert.ToInt32(Session["admin"]) == 1)
            // {
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
            //}
        }

        [WebMethod(EnableSession = true)]
        public void DeleteAccount() //Uncomment the admin check later.
        {
            //if (Convert.ToInt32(Session["admin"]) == 1)
            // {
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
            //}
        }
    }

  
}
