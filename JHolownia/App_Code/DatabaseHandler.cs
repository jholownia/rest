using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Reflection;

/// <summary>
/// Handles database connection and sql queries.
/// </summary>
public class DatabaseHandler
{
    private static string _ConnectionString = WebConfigurationManager.ConnectionStrings["SOFT338_ConnectionString"].ConnectionString;
    public static string LastError = "";
    
    // Sex methods
    //--------------------------------------------------------------------
    #region Sex methods
    
    /// <summary>
    /// Gets all the sex from the database.
    /// </summary>
    /// <returns>List of all sex items</returns>
    public static List<Sex> getAllSex()
    {
        List<Sex> sexList = new List<Sex>();
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Sex", con);

        using (con)
        {
            try
            {
	            con.Open();	
	            SqlDataReader reader = cmd.ExecuteReader();
	
	            while (reader.Read())
	            {
	                Sex s = new Sex((string)reader["Name"], (string)reader["Description"]);
	                sexList.Add(s);
	            }
            }
            catch (System.Exception ex)
            {
                LastError = ex.Message;
            }
        }

        return sexList;
    }

    /// <summary>
    /// Gets a single Sex items with specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Sex object or null</returns>
    public static Sex getSex(int id)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Sex WHERE SexID='" + id.ToString() + "'", con);

        Sex sex = null;

        using (con)
        {
            try
            {
	            con.Open();	
	            SqlDataReader reader = cmd.ExecuteReader();	
	            reader.Read();
	            sex = new Sex((string)reader["Name"], (string)reader["Description"]);
            }
            catch (System.Exception ex)
            {
                LastError = ex.Message;
            }
        }
        return sex;
    }

    /// <summary>
    /// Inserts new Sex object into database.
    /// </summary>
    /// <param name="sex"></param>
    /// <returns>ID of the new object as int</returns>
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
                returnID = (Int32)cmd.ExecuteScalar();    // Why this always returns 0?
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }
        return returnID;
    }

    /// <summary>
    /// Updates an item with specified ID
    /// </summary>    
    /// <returns>ID of updated object as Int, 0 if an object with specified ID doesn't exist.</returns>
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

    /// <summary>
    /// Deletes an object with given ID.
    /// </summary>    
    /// <returns>ID of deleted object on success or 0 on failure.</returns>
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

    #endregion

    // Drugs methods
    //--------------------------------------------------------------------
    #region Drugs methods

    /// <summary>
    /// Gets all the drugs from the database.
    /// </summary>
    /// <returns>List of all drugs items</returns>
    public static List<Drug> getAllDrugs()
    {
        List<Drug> drugList = new List<Drug>();
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Drugs", con);

        using (con)
        {
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Drug d = new Drug((string)reader["Name"], (string)reader["Formula"], (string)reader["Administration"], (string)reader["Effects"]);
                    drugList.Add(d);
                }
            }
            catch (System.Exception ex)
            {
                LastError = ex.Message;
            }
        }

        return drugList;
    }

    /// <summary>
    /// Gets a single Drug item with specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Drug object or null</returns>
    public static Drug getDrug(int id)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Drugs WHERE DrugID='" + id.ToString() + "'", con);

        Drug drug = null;

        using (con)
        {
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                drug = new Drug((string)reader["Name"], (string)reader["Formula"], (string)reader["Administration"], (string)reader["Effects"]);
            }
            catch (System.Exception ex)
            {
                LastError = ex.Message;
            }
        }
        return drug;
    }

    /// <summary>
    /// Inserts new Drug object into database.
    /// </summary> 
    /// <returns>ID of the new object as int</returns>
    public static int insertNewDrug(Drug drug)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("INSERT into Drugs (Name, Formula, Administration, Effects) VALUES('" + drug.Name + "', '" + drug.Formula + 
            "', '" + drug.Administration + "', '" + drug.Effects + "'); " + "SELECT CAST(Scope_Identity() as int)", con);

        Int32 returnID = 0;

        using (con)
        {
            try
            {
                con.Open();
                returnID = (Int32)cmd.ExecuteScalar();    // Why this always returns 0?
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }
        return returnID;
    }

    /// <summary>
    /// Updates an item with specified ID
    /// </summary>    
    /// <returns>ID of updated object as Int, 0 if an object with specified ID doesn't exist.</returns>
    public static int updateDrug(int id, Drug drug)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("UPDATE Drugs SET Name='" + drug.Name + "', Formula='" + drug.Formula + "', Administration='" + drug.Administration + "', Effects='" + drug.Effects + "'WHERE SexID='" + id.ToString() + "'", con);

        Drug oldDrug = getDrug(id);
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

    /// <summary>
    /// Deletes an object with given ID.
    /// </summary>    
    /// <returns>ID of deleted object on success or 0 on failure.</returns>
    public static int deleteDrug(int id)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("DELETE FROM Drugs WHERE DrugID='" + id.ToString() + "'", con);

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

    #endregion
    
    // RockAndRoll methods
    //--------------------------------------------------------------------
    #region RockAndRoll methods

    /// <summary>
    /// Gets all the RockAndRoll from the database.
    /// </summary>
    /// <returns>List of all sex items</returns>
    public static List<RockAndRoll> getAllRockAndRoll()
    {
        List<RockAndRoll> rnrList = new List<RockAndRoll>();
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Rockandroll", con);

        using (con)
        {
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RockAndRoll r = new RockAndRoll((string)reader["Title"], (string)reader["Artist"], (string)reader["Album"], (DateTime)reader["Date"], (string)reader["Link"]);
                    rnrList.Add(r);
                }
            }
            catch (System.Exception ex)
            {
                LastError = ex.Message;
            }
        }

        return rnrList;
    }

    /// <summary>
    /// Gets a single RockAndRoll item with specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>RockAndRoll object or null</returns>
    public static RockAndRoll getRockAndRoll(int id)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("SELECT * FROM Rockandroll WHERE RockAndRollID='" + id.ToString() + "'", con);

        RockAndRoll rnr = null;

        using (con)
        {
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                rnr = new RockAndRoll((string)reader["Title"], (string)reader["Artist"], (string)reader["Album"], (DateTime)reader["Date"], (string)reader["Link"]);
            }
            catch (System.Exception ex)
            {
                LastError = ex.Message;
            }
        }
        return rnr;
    }

    /// <summary>
    /// Inserts new RockAndRoll object into database.
    /// </summary>  
    /// <returns>ID of the new object as int</returns>
    public static int insertNewRockAndRoll(RockAndRoll rnr)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("INSERT into Rockandroll (Title, Artist, Album, Date, Link) VALUES('" + rnr.Title + "', '" + rnr.Artist + "', '" + 
            rnr.Album + "', '" + rnr.Date + "', '" + rnr.Link + "'); " + "SELECT CAST(Scope_Identity() as int)", con);

        Int32 returnID = 0;

        using (con)
        {
            try
            {
                con.Open();
                returnID = (Int32)cmd.ExecuteScalar();    // Why this always returns 0?
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }
        return returnID;
    }

    /// <summary>
    /// Updates an item with specified ID
    /// </summary>    
    /// <returns>ID of updated object as Int, 0 if an object with specified ID doesn't exist.</returns>
    public static int updateRockAndRoll(int id, RockAndRoll rnr)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("UPDATE Rockandroll SET Title='" + rnr.Title + "', Artist='" + rnr.Artist + "', Album='" + rnr.Album + "', Date='" + rnr.Date + "', Link='" + rnr.Link + "'WHERE RockAndRollID='" + id.ToString() + "'", con);

        RockAndRoll oldRnr = getRockAndRoll(id);
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

    /// <summary>
    /// Deletes an object with given ID.
    /// </summary>    
    /// <returns>ID of deleted object on success or 0 on failure.</returns>
    public static int deleteRockAndRoll(int id)
    {
        SqlConnection con = new SqlConnection(_ConnectionString);

        SqlCommand cmd = new SqlCommand("DELETE FROM Rockandroll WHERE RockAndRollID='" + id.ToString() + "'", con);

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

    #endregion
}