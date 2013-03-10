using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Reflection;


// add try catch and/or using where necessary

/// <summary>
/// Summary description for DatabaseHandler
/// </summary>
public class DatabaseHandler
{
    private static string _ConnectionString = WebConfigurationManager.ConnectionStrings["SOFT338_ConnectionString"].ConnectionString;
    public static string LastError = "";

	public DatabaseHandler()
	{
        
	}

    // Sex methods
    //--------------------------------------------------------------------

    public static List<Sex> getAllSex()
    {
        List<Sex> sexList = new List<Sex>();

        SqlConnection con = new SqlConnection(_ConnectionString);

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

    public static Sex getSex(int id)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Sex WHERE SexID='" + id.ToString() + "'", con);

        Sex sex = null;

        using (con)
        {
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            sex = new Sex((string)reader["Name"], (string)reader["Description"]);
        }

        return sex;
    }

    public static int insertNewSex(Sex sex)
    {       
        SqlConnection con = new SqlConnection(_ConnectionString);
        
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

    public static int updateSex(int id, Sex sex)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("UPDATE Sex SET Name='" + sex.Name + "', Description='" + sex.Description + "'WHERE SexID='" + id.ToString() + "'", con);

        Sex oldSex = getSex(id);
        int returnID = 0;
        
        using (con)
        {
            try
            {
                con.Open();
                returnID = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }           
        }

        return returnID;
    }

    public static int deleteSex(int id)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("DELETE FROM Sex WHERE SexID='" + id.ToString() + "'", con);

        int returnID = 0;

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
    
    // Drugs methods
    //--------------------------------------------------------------------



    // RockAndRoll methods
    //--------------------------------------------------------------------
}