using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;


/// <summary>
/// Summary description for DbConnection
/// </summary>
public class DbConnection
{
    private static string ConnectionString_ = WebConfigurationManager.ConnectionStrings["SOFT338_ConnectionString"].ConnectionString;

	public DbConnection()
	{
        
	}
                
    public static List<Module> getAllModules()
    {
        List<Module> modules = new List<Module>();

        SqlConnection con = new SqlConnection(ConnectionString_);

        SqlCommand cmd = new SqlCommand("SELECT * FROM modules", con);

        using (con)
        {
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Module m = new Module((string)reader["Title"], (string)reader["Code"], (string)reader["Term"]);
                modules.Add(m);
            }
        }

        return modules;

        // To use JSON (proper way) - BTW this goes into HttpHandler
        // private void getAllModules(HttpContext context){
        // Stream outputStream = ContextBoundObject.Response.OutputStream;
        // ContextBoundObject.Response.ContentType = "application/json";
        // Create the new serializer object - NOTE the type entered into constructor
        // DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(IEnumerable<Module>));
        // Get a list of modules as IEnumerable
        // IEnumerable<Module> modules = DbConnection.getAllModules();
        // jsonData.WriteObject(outputStream, modules);
        // } // test in postman - done!
    }

    public static Int32 insertNewModule(Module newModule)
    {
        // create new connection using the connection string in web.config
        SqlConnection con = new SqlConnection(ConnectionString_);
        
        SqlCommand cmd = new SqlCommand("INSERT into Module (Title, Code, Term) VALUES('"+newModule.Title+"', '" +newModule.Code+"','"+newModule.Term+"'); " + "SELECT CAST(Scope_Identity() as int)", con);
        Int32 returnID = 0;
        using (con)
        {
            try
            {
                con.Open();
                returnID = (Int32)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                // LastError = ex.Message;
            }
        }
        return returnID;
    }
}