using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
namespace Bunifu.Data.Helper

{
    public class Mysql
    {

        #region VARIABLES
        public string lastError;                                        // Holds the last error
       public  string lastQuery;                                        // Holds the last query
       public  int RecordCount;                                                // Holds the total number of records returned
       public int AffectedRecords;
       public   string LastInsertedId = "";
     


       static string before_edit = "";
       static string after_edit = "";

        /// <summary>
        /// Runs the transaction.
        /// </summary>
        /// <param name="QueryArray">The query array.</param>
        /// <returns></returns>
        public MySqlTransaction RunTransaction(List<string> QueryArray)
        { 
            MySqlTransaction myTrans=null;
            //coming soon
            return myTrans;
        }

        string ConectionString = "";
        public  MySqlConnection mysqlConnection = null;
        public  MySqlDataAdapter mysqlDataAdapter = null;
        public  string server = "127.0.0.1";



        private string User = "root";
        private string DatabaseName = "";
        private string password = "";
         private string port="";
         DataGridView tempdata;
         DataSet ds;
           public  string ConnectionType = "";
         string RecordSource = "";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Mysql"/> class.
        /// </summary>
        /// <param name="database_name">Name of the database.</param>
        /// <param name="host">The host.</param>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <param name="port">The port.</param>
        public Mysql(string database_name, string host = "localhost", string user = "root", string password = "", string port = "3306")
        {
            try
            {
                 DatabaseName = database_name;
                this.server = host;
                this.User = user;
                this.password = password;
                this.port = port;
                  ConectionString = "SERVER=" + this.server + ";" + "DATABASE=" + this.DatabaseName + ";" + "UID=" + this.User + ";" + "PASSWORD=" + this.password + ";";
                  mysqlConnection = new MySqlConnection(ConectionString);
                  OpenConnection();
            }
            catch (Exception err)
            {
                lastError = (err.Message);
                Console.WriteLine("ERROR: "+err.Message);
            }
        }

        public AutoCompleteStringCollection GetAutoCompleteStringCollection(string sqlQuerry)
        {
                this.GetRows(sqlQuerry);

            AutoCompleteStringCollection Collection= new AutoCompleteStringCollection();

            for (int i = 0; i < this.RecordCount; i++)
            {
                   Collection.Add(this.GetResults(i,0).ToString());
            }

            return Collection;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Mysql"/> class.
        /// </summary>
        ~Mysql()
        {
            this.CloseConnection();
        }


        /// <summary>
        /// Gets the connection instance.
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConnection()
        {
            return this.mysqlConnection;
        }


        /// <summary>
        /// Gets all cells.
        /// </summary>
        /// <param name="SqlCommand">The SQL command.</param>
        /// <returns></returns>
        public DataView GetRows(string SqlCommand)
        {
            RecordCount = 0;
            RecordSource = SqlCommand;
            
             
            try
            {
                string command = RecordSource.ToUpper();
                MySqlDataAdapter da = new MySqlDataAdapter(RecordSource, mysqlConnection);
                ds = new DataSet();
                 
                da.Fill(ds);
              
                tempdata = new DataGridView();
                RecordCount = ds.Tables[0].Rows.Count;
                 
                lastQuery = SqlCommand;
                 
                return ds.Tables[0].DefaultView;
              
            }
            catch (Exception err)
            {
               
                lastError = (err.Message);
                Console.WriteLine("ERROR: " + err.Message);
                return null;
            }


        }

        

        /// <summary>
        /// Gets all cells.
        /// </summary>
        /// <param name="Target">The target.</param>
        /// <param name="Where">The where.</param>
        public void GetAll(DataGridView Target,string Where)
        {
            RecordCount = 0;
            string Sql_command = "SELECT ";
            Target.Rows.Clear();


            if (Target.Tag.ToString().Trim().Length==0)
            {
                MessageBox.Show("Set Table name as tag", "Bunfu.MySQL");
                return;   
            }

               if (Target.Columns.Count>0)
                {

                    for (int i = 0; i < Target.Columns.Count; i++)
                    {

                        string tg="";
                        try
                        {
                            tg = Target.Columns[i].Tag.ToString();

                        }
                        catch (Exception)
                        {
                            
                            
                        }
                        if (tg.Length>0)
                        {

                            Sql_command += Target.Columns[i].Tag.ToString() + " AS '" + Target.Columns[i].HeaderText.Trim() + "'";

                        }
                        else
                        {
                            Sql_command += "`" + Target.Columns[i].HeaderText.Trim() + "`";

                        }  


                        if (i< Target.Columns.Count-1)
                        {
                            Sql_command += ",";
                        }
                    }

                    Sql_command += " FROM `" + Target.Tag.ToString().Trim() + "` "+Where;
                }
                else
                {
                    MessageBox.Show("No columns Defined, Rem Set Table name as tag", "Bunfu.MySQL");
                    return;
                }
                  RecordSource = Sql_command;
                  lastQuery = Sql_command;
                
             //FIX HEADERS

                  for (int i = 0; i < Target.Columns.Count; i++)
                  {
                      if(Target.Columns[i].CellType==typeof(DataGridViewComboBoxCell))
                      {

                          DataGridViewComboBoxCell tmplate = (DataGridViewComboBoxCell)Target.Columns[i].CellTemplate;
                          if (tmplate.Items.Count==0)
                          {
                              tmplate.Items.Clear();
                              Target.Columns[i].CellTemplate = tmplate;

                              this.GetRows("SELECT DISTINCT `" + Target.Columns[i].HeaderText.Trim() + "` FROM `" + Target.Tag + "`;");

                              for (int j = 0; j < this.RecordCount; j++)
                              {
                                  tmplate.Items.Add(this.GetResults(j,Target.Columns[i].HeaderText.Trim()));
                              }
                          }

                          


                      }
                  }


            ///dump dada
              
             try
            {
                this.GetRows(Sql_command);
                for (int i = 0; i < this.RecordCount; i++)
                {
                    string[] rec = new string[Target.Columns.Count];
                    for (int j = 0; j < Target.Columns.Count; j++)
                    {
                        rec[j] = this.GetResults(i,Target.Columns[j].HeaderText.Trim() );
                    }

                    Target.Rows.Add(rec);
                }

            }
            catch (Exception err)
            {         
                lastError = (err.Message);
                Console.WriteLine("ERROR: " + err.Message);
            }

        }


        /// <summary>
        /// Gets the cell Value.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="isNumber">if set to <c>true</c> [is number].</param>
        /// <returns></returns>
        public object GetCell(string query, bool isNumber = false)
        {
            string ans = "";

            this.GetRows(query);
            if (this.RecordCount > 0)
            {
                ans = this.GetResults(0, 0).ToString();
                if (isNumber)
                {
                    try
                    {
                        return double.Parse(ans);

                    }
                    catch (Exception)
                    {

                        return 0;
                    }
                }
                else
                {
                    return ans;
                }
            }


            if (isNumber)
            {
                return 0;
            }
            else
            {
                try
                {
                    return double.Parse( ans.Trim());
                }
                catch (Exception)
                {
                    return 0;
                }
            }


        }



        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <param name="ROW">The row.</param>
        /// <param name="COLUMN_NAME">Name of the column.</param>
        /// <returns></returns>
        public string GetResults(int ROW, string COLUMN_NAME)
        {
            try
            {
                return ds.Tables[0].Rows[ROW][COLUMN_NAME].ToString();
            }
            catch (Exception err)
            {
                return "";
                Console.WriteLine("ERROR: " + err.Message);
                lastError = (err.Message);
            }
        }



        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <param name="ROW">The row.</param>
        /// <param name="COLUMN_NAME">Name of the column.</param>
        /// <returns></returns>
        public string GetResults(int ROW, int COLUMN_NAME)
        {
            try
            {
                return ds.Tables[0].Rows[ROW][COLUMN_NAME].ToString();
            }
            catch (Exception err)
            {
                try
                {
                    return ds.Tables[0].Rows[ROW][0].ToString();
                }
                catch (Exception)
                {

                    return "";
                }
                Console.WriteLine("ERROR: " + err.Message);

                lastError = (err.Message);
            }
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void OpenConnection()
        {
            try
            {

                mysqlConnection.Open();
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR: " + err.Message);

                lastError = (err.Message);
            }
        }
        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void CloseConnection()
        {
            try
            {

                mysqlConnection.Close();
            }
            catch (Exception err)
            {
                lastError = (err.Message);
                Console.WriteLine("ERROR: " + err.Message);

            }
        }




        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql_comm">The SQL comm.</param>
        public void ExecuteNonQuery(string sql_comm)
       {
           try
           {
               RecordCount = 0;
              
               MySqlCommand myc = new MySqlCommand(sql_comm, mysqlConnection);
               AffectedRecords=myc.ExecuteNonQuery();
               LastInsertedId = myc.LastInsertedId.ToString();
               lastQuery = sql_comm;
               
           }
           catch (Exception err)
           {
                Console.WriteLine("ERROR: " + err.Message);
                LastInsertedId = null;
               lastError = (err.Message);
           }
       }

        //-----------------------------update code------------------------

        /// <summary>
        /// Handles the Event event of the CellEnter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        public void CellEnter_Event(object sender, DataGridViewCellEventArgs e)
        {
       
            try
            {
                before_edit = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            }
            catch (Exception err)
            {
                before_edit = "";
                Console.WriteLine("ERROR: " + err.Message);

            }
            Console.WriteLine("Cell enter {0}",before_edit);
        }


        /// <summary>
        /// Cells the end edit event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        /// <param name="Tablename">The tablename.</param>
        /// <param name="PKcolumn">The p kcolumn.</param>
        /// <returns></returns>
        public bool CellEndEdit_Event(object sender, DataGridViewCellEventArgs e, string Tablename, int PKcolumn)
        {
            lastError = null;
           try
            {
                after_edit = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            }
            catch (Exception)
            {
                after_edit = "";

            }
            try
            {


                if (before_edit!=after_edit)
                {
                    string id = ((DataGridView)sender).Columns[PKcolumn].HeaderText;
                    string IDVAL = "";
                    try { IDVAL = ((DataGridView)sender).Rows[e.RowIndex].Cells[PKcolumn].Value.ToString(); }
                    catch (Exception) { }
                    string col = ((DataGridView)sender).Columns[e.ColumnIndex].HeaderText;
                    string ans = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Replace("'", "\'").Replace("\\", "\\\\'");
                    string querry = "";
                    if (IDVAL.Trim().Length > 0)
                    {
                        this.ExecuteNonQuery("UPDATE `" + Tablename + "` SET `" + col + "` = '" + ans + "' WHERE `" + id + "` = " + IDVAL + " LIMIT 1;");

                    }
                    else
                    {
                        ////---if idval== balnk then its al new record
                        ////---GET THE NEXT AUTONUMBER
                        this.GetRows("SELECT (MAX(" + id + ")+1) FROM  `" + Tablename + "`;");
                        string thenewid = this.GetResults(0, 0);


                        if (thenewid.Trim().Length == 0)
                        {
                            thenewid = "0";
                        }

                        querry = "INSERT INTO `" + Tablename + "` (`" + col + "`,`" + id + "`) VALUES ('" + ans + "'," + thenewid + ");";

                        this.ExecuteNonQuery(querry);
                        ((DataGridView)sender).Rows[e.RowIndex].Cells[PKcolumn].Value = thenewid;
                    }


                    return (true);
                    
                }
                return false;
            }
            catch (Exception ERR)
            {

                lastError = ERR.Message;
                if (!lastError.ToLower().Contains("instance"))
                {
                    Console.WriteLine("ERROR: " + ERR.Message);

                }
                return false;

            }

        }




        //----updates the current row


        /// <summary>
        /// Users the deleting row event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataGridViewRowCancelEventArgs"/> instance containing the event data.</param>
        /// <param name="Table_name">Name of the table.</param>
        /// <param name="PKcolumn">The p kcolumn.</param>
        /// <returns></returns>
        public bool UserDeletingRow_Event(object sender, DataGridViewRowCancelEventArgs e, string Table_name, int PKcolumn)
        {
            lastError = null;
            try
            {
                string id = ((DataGridView)sender).Columns[PKcolumn].HeaderText;

                string IDVAL = ((DataGridView)sender).Rows[e.Row.Index].Cells[PKcolumn].Value.ToString();

                string querry = "DELETE FROM `" + Table_name + "` WHERE `" + id + "` = " + IDVAL + " LIMIT 1;";

                this.ExecuteNonQuery(querry);
                return true;

            }
            catch (Exception err)
            {
                lastError = err.Message;
                return false;
            }



        }
   



       
       


    }
}

    
   



 
    
    



