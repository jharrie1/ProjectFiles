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
		////////////////////////////////////////////////////////////////////////
		///replace the values of these variables with your database credentials
		////////////////////////////////////////////////////////////////////////
		private string dbID = "solo";
		private string dbPass = "!!Solo440";
		private string dbName = "solo";
		////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////
		///call this method anywhere that you need the connection string!
		////////////////////////////////////////////////////////////////////////
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}
		////////////////////////////////////////////////////////////////////////



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
    }
}
