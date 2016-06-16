using MySql.Data.MySqlClient;
using System;

namespace DataAccess {
    public class MySqlClient {

        private static string ConntionStr = "Data Source=xxx;Initial Catalog=xxx;User Id=xxx;Password=xxx;";
        private static MySqlConnection connection = new MySqlConnection(ConntionStr);

        //open connection to database
        private bool OpenConnection() {
            try {
                connection.Open();
                return true;
            } catch(MySqlException ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Close connection
        private bool CloseConnection() {
            try {
                connection.Close();
                return true;
            } catch(MySqlException ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Insert statement
        public void ExecSQL(string sql) {
            //open connection
            if(this.OpenConnection() == true) {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }
    }
}
