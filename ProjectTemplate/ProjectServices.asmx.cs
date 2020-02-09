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
        [WebMethod]
        public bool LogOn(string username, string password)
        {
            //Parameter names were changed.

            bool success = false;

            //string changed to match the current database tables.
            string sqlSelect = "SELECT id FROM users WHERE username=@idValue and password=@passValue";

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
                success = true;
            }
           
            return success;
        }

        //RequestAccount Function (based on RequestAccount function and the GetAccounts() function for data validation)
        [WebMethod]
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
            
            //If data transfer works, success message is sent back to the user. 
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
    }
}
