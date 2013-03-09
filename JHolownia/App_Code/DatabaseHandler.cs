using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;


// add try catch and/or using

/// <summary>
/// Summary description for DatabaseHandler
/// </summary>
public class DatabaseHandler
{
    private static string ConnectionString_ = WebConfigurationManager.ConnectionStrings["SOFT338_ConnectionString"].ConnectionString;
    public static string LastError = "";

	public DatabaseHandler()
	{
        
	}

    // Sex methods
    //--------------------------------------------------------------------
                    
    public static List<Sex> getAllSex()
    {
        List<Sex> sexList = new List<Sex>();

        SqlConnection con = new SqlConnection(ConnectionString_);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Sex", con);

        using (con)
        {
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Sex s = new Sex((string)reader["Name"], (string)reader["Description"]);
                sexList.Add(s);
            }
        }

        return sexList;
    }

    public static int insertNewSex(Sex sex)
    {
        // create new connection using the connection string in web.config
        SqlConnection con = new SqlConnection(ConnectionString_);
        
        SqlCommand cmd = new SqlCommand("INSERT into Sex (Name, Description) VALUES('"+sex.Name+"', '" +sex.Description+"'); " + "SELECT CAST(Scope_Identity() as int)", con);
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
                LastError = ex.Message;
            }
        }
        return returnID;
    }

    public static int updateSex(Sex sex)
    {
        return 0;
    }

    public static int deleteSex(Sex sex)
    {
        return 0;
    }
    
    // Drugs methods
    //--------------------------------------------------------------------



    // RockAndRoll methods
    //--------------------------------------------------------------------
}