using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net;

// add namespace

/// <summary>
/// Implements HttpHandler interface to handle incoming HTTP requests.
/// </summary>
public class HttpHandler : IHttpHandler
{        
    // Delegates are used to call correct method based on the uri template
    delegate void HandleRequest(HttpContext context, UriTemplateMatch template);
    UriTemplateTable _templateTable = null;
    object _tableLock = new object();

    /// <summary>
    /// Creates template table for uri template pattern matching.
    /// </summary>
    UriTemplateTable TemplateTable
    {
        get 
        {
            if (_templateTable == null)
            {
                lock (_tableLock)
                {
                    if (_templateTable == null)
                    {
                        HttpRequest request = HttpContext.Current.Request;
                        string fullUrl = request.Url.ToString();                        
                        string baseUrl = fullUrl.Substring(0, fullUrl.IndexOf(request.ApplicationPath.ToLower()));

                        _templateTable = new UriTemplateTable();
                        _templateTable.BaseAddress = new Uri(baseUrl);
                                                
                        // Uri templates v1

                        // Sex
                        UriTemplate sexTemplate = new UriTemplate(request.ApplicationPath + "/v1/Sex");
                        UriTemplate sexItemTemplate = new UriTemplate(request.ApplicationPath + "/v1/Sex/{SexID}");
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(sexTemplate, new HandleRequest(processSex)));
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(sexItemTemplate, new HandleRequest(processSexItem)));

                        // Drugs
                        UriTemplate drugsTemplate = new UriTemplate(request.ApplicationPath + "/v1/Drugs");
                        UriTemplate drugsItemTemplate = new UriTemplate(request.ApplicationPath + "/v1/Drugs/{DrugID}");
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(drugsTemplate, new HandleRequest(processDrugs)));
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(drugsItemTemplate, new HandleRequest(processDrugsItem)));

                        // RockAndRoll
                        UriTemplate rockandrollTemplate = new UriTemplate(request.ApplicationPath + "/v1/RockAndRoll/{rockAndRollID}");
                        UriTemplate rockandrollItemTemplate = new UriTemplate(request.ApplicationPath + "/v1/RockAndRoll/{rockAndRollID}");
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(rockandrollTemplate, new HandleRequest(processRockAndRoll)));
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(rockandrollItemTemplate, new HandleRequest(processRockAndRollItem)));
                    }
                }
            }
            return _templateTable;
        }
    }
    
    /// <summary>
    /// Allows object instance caching.
    /// </summary>
    public bool IsReusable
    {
        get { return true; }
    }
        
    /// <summary>
    /// Main method for processing incoming http requests. 
    /// Uses uri templates to call correct method for particular request.
    /// </summary>
    /// <param name="context">HTTP context</param>
    public void ProcessRequest(HttpContext context)
    {   
        UriTemplateMatch match = TemplateTable.MatchSingle(context.Request.Url);

        if (match == null)
        {
            dispatchNotFound(context);
        }
        else
        {
            HandleRequest handleRequest = (HandleRequest)match.Data;
            handleRequest(context, match);
        }  
    }

    // Http response codes
    //--------------------------------------------------------------------
    #region Http responses
    
    /// <summary>
    /// Returns HTTP status 200 - OK
    /// </summary>
    /// <param name="context">HTTP context</param>
    private void dispatchOk(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.OK;
    }

    /// <summary>
    /// Returns HTTP status 404 - Not Found
    /// </summary>
    /// <param name="context">HTTP context</param>
    private void dispatchNotFound(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.NotFound;
    }

    /// <summary>
    /// Returns HTTP status 405 - Not Allowed
    /// </summary>
    /// <param name="context">HTTP context</param>
    private void dispatchNotAllowed(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.MethodNotAllowed;
    }

    /// <summary>
    /// Returns HTTP status 400 - Bad Request
    /// </summary>
    /// <param name="context">HTTP context</param>
    private void dispatchBadRequest(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.BadRequest;
    }

    /// <summary>
    /// Returns HTTP status 201 - Created
    /// </summary>
    /// <param name="context">HTTP context</param>
    private void dispatchCreated(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.Created;
    }
    #endregion

    // Sex methods
    //--------------------------------------------------------------------
    #region Sex methods

    /// <summary>
    /// Handles /Sex uri. Allowed http verbs include GET and POST.
    /// </summary>    
    private void processSex(HttpContext context, UriTemplateMatch template)
    {       
        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getAllSex(context);
                break;
            case "post":
                postNewSex(context);
                break;          
            default:
                dispatchNotAllowed(context);
                break;
        }
    }

    /// <summary>
    /// Handles /Sex/{SexID} uri. Allowed http verbs include GET, PUT and DELETE.
    /// </summary>    
    private void processSexItem(HttpContext context, UriTemplateMatch template)
    {
        Int32 sexID = 0;
                
        // Check if we have a correct ID value
        if (!Int32.TryParse(template.BoundVariables["SexID"], out sexID))
        {
            dispatchBadRequest(context);
            return;
        }

        // Store SexID value in the Items collection in the context
        context.Items.Add("SexID", sexID);

        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getSexItem(context);
                break;           
            case "put":
                updateSex(context);
                break;
            case "delete":
                deleteSex(context);
                break;
            default:
                dispatchNotAllowed(context);
                break;
        }
    }

    /// <summary>
    /// Returns the list of all the sex in JSON format.
    /// </summary>
    private void getAllSex(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;      
        context.Response.ContentType = "application/json";       
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(IEnumerable<Sex>));
        IEnumerable<Sex> sexes = DatabaseHandler.getAllSex();
        jsonData.WriteObject(outputStream, sexes);
    }

    /// <summary>
    /// Returns a single sex item in JSON format.
    /// </summary> 
    private void getSexItem(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;
        context.Response.ContentType = "application/json";
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
        Sex sex = DatabaseHandler.getSex((int)context.Items["SexID"]);
        jsonData.WriteObject(outputStream, sex);
    }

    /// <summary>
    /// Creates a new sex item and adds it to the database.
    /// Returns HTTP status 201 on success and an ID of the newly created entry.
    /// </summary>    
    private void postNewSex(HttpContext context)
    {
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
        Sex sex = null;

        if (context.Request.Headers["Content-Type"].ToLower() == "application/json")
        {
            sex = (Sex)jsonData.ReadObject(context.Request.InputStream);
            int result = DatabaseHandler.insertNewSex(sex);
                       
            // return HTTP status 201 and an ID of newly created item
            context.Response.StatusCode = (Int32)HttpStatusCode.Created;
            context.Response.AddHeader("id", result.ToString());
        }
        else
        {
            dispatchBadRequest(context);
        }
    }

    /// <summary>
    /// Updates an existing sex item.
    /// Returns OK on success, Not Found if invalid ID was provided and Bad Request if other issues appear.
    /// </summary>
    private void updateSex(HttpContext context)
    {
        try
        {
            Sex sex = null;
            if (context.Request.Headers["Content-Type"].ToLower() == "application/json")
            {
                DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
                sex = (Sex)jsonData.ReadObject(context.Request.InputStream);
                int result = DatabaseHandler.updateSex((int)context.Items["SexID"], sex);
                 
                dispatchOk(context);
                             
            }
            else
            {
                dispatchBadRequest(context);
            }
        }
        catch (System.Exception)
        {
            dispatchBadRequest(context);
        }    
    }

    /// <summary>
    /// Removes sex record with given ID.
    /// Returns OK or Not Found.
    /// </summary>
    private void deleteSex(HttpContext context)
    {
        int result = DatabaseHandler.deleteSex((int)context.Items["SexID"]);            
        dispatchOk(context);
    }
    #endregion

    // Drugs methods
    //--------------------------------------------------------------------
    #region Drugs methods

    /// <summary>
    /// Handles /Drugs uri. Allowed http verbs include GET and POST.
    /// </summary>    
    private void processDrugs(HttpContext context, UriTemplateMatch template)
    {
        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getAllDrugs(context);
                break;
            case "post":
                postNewDrug(context);
                break;
            default:
                dispatchNotAllowed(context);
                break;
        }
    }

    /// <summary>
    /// Handles /Drugs/{DrugID} uri. Allowed http verbs include GET, PUT and DELETE.
    /// </summary>    
    private void processDrugsItem(HttpContext context, UriTemplateMatch template)
    {
        Int32 drugID = 0;

        // Check if we have a correct ID value
        if (!Int32.TryParse(template.BoundVariables["DrugID"], out drugID))
        {
            dispatchBadRequest(context);
            return;
        }

        // Store DrugID value in the Items collection in the context
        context.Items.Add("DrugID", drugID);

        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getDrugsItem(context);
                break;
            case "put":
                updateDrug(context);
                break;
            case "delete":
                deleteDrug(context);
                break;
            default:
                dispatchNotAllowed(context);
                break;
        }
    }

    /// <summary>
    /// Returns the list of all the drugs in JSON format.
    /// </summary>
    private void getAllDrugs(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;
        context.Response.ContentType = "application/json";
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(IEnumerable<Drug>));
        IEnumerable<Drug> drugs = DatabaseHandler.getAllDrugs();
        jsonData.WriteObject(outputStream, drugs);
    }

    /// <summary>
    /// Returns a single drug item in JSON format.
    /// </summary> 
    private void getDrugsItem(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;
        context.Response.ContentType = "application/json";
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Drug));
        Drug drug = DatabaseHandler.getDrug((int)context.Items["DrugID"]);
        jsonData.WriteObject(outputStream, drug);
    }

    /// <summary>
    /// Creates a new drug item and adds it to the database.
    /// Returns HTTP status 201 on success and an ID of the newly created entry.
    /// </summary>    
    private void postNewDrug(HttpContext context)
    {
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Drug));
        Drug drug = null;

        if (context.Request.Headers["Content-Type"].ToLower() == "application/json")
        {
            drug = (Drug)jsonData.ReadObject(context.Request.InputStream);
            int result = DatabaseHandler.insertNewDrug(drug);

            // return HTTP status 201 and an ID of newly created item
            context.Response.StatusCode = (Int32)HttpStatusCode.Created;
            context.Response.AddHeader("id", result.ToString());
        }
        else
        {
            dispatchBadRequest(context);
        }
    }

    /// <summary>
    /// Updates an existing drugs item.
    /// Returns OK on success, Not Found if invalid ID was provided and Bad Request if other issues appear.
    /// </summary>
    private void updateDrug(HttpContext context)
    {
        try
        {
            Drug drug = null;
            if (context.Request.Headers["Content-Type"].ToLower() == "application/json")
            {
                DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
                drug = (Drug)jsonData.ReadObject(context.Request.InputStream);
                int result = DatabaseHandler.updateDrug((int)context.Items["DrugID"], drug);

                dispatchOk(context);

            }
            else
            {
                dispatchBadRequest(context);
            }
        }
        catch (System.Exception)
        {
            dispatchBadRequest(context);
        }
    }

    /// <summary>
    /// Removes drug record with given ID.  
    /// </summary>
    private void deleteDrug(HttpContext context)
    {
        int result = DatabaseHandler.deleteDrug((int)context.Items["DrugID"]);
        dispatchOk(context);
    }
    #endregion

    // RockAndRoll methods
    //--------------------------------------------------------------------
    #region RockAndRoll methods

    /// <summary>
    /// Handles /RockAndRoll uri. Allowed http verbs include GET and POST.
    /// </summary>    
    private void processRockAndRoll(HttpContext context, UriTemplateMatch template)
    {
        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getAllRockAndRoll(context);
                break;
            case "post":
                postNewRockAndRoll(context);
                break;
            default:
                dispatchNotAllowed(context);
                break;
        }
    }

    /// <summary>
    /// Handles /RockAndRoll/{rockAndRollID} uri. Allowed http verbs include GET, PUT and DELETE.
    /// </summary>    
    private void processRockAndRollItem(HttpContext context, UriTemplateMatch template)
    {
        Int32 rockAndRollID = 0;

        // Check if we have a correct ID value
        if (!Int32.TryParse(template.BoundVariables["rockAndRollID"], out rockAndRollID))
        {
            dispatchBadRequest(context);
            return;
        }

        // Store rockAndRollID value in the Items collection in the context
        context.Items.Add("rockAndRollID", rockAndRollID);

        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getRockAndRollItem(context);
                break;
            case "put":
                updateRockAndRoll(context);
                break;
            case "delete":
                deleteRockAndRoll(context);
                break;
            default:
                dispatchNotAllowed(context);
                break;
        }
    }

    /// <summary>
    /// Returns the list of all the RockAndRoll in JSON format.
    /// </summary>
    private void getAllRockAndRoll(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;
        context.Response.ContentType = "application/json";
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(IEnumerable<RockAndRoll>));
        IEnumerable<RockAndRoll> rockAndRolls = DatabaseHandler.getAllRockAndRoll();
        jsonData.WriteObject(outputStream, rockAndRolls);
    }

    /// <summary>
    /// Returns a single RockAndRoll item in JSON format.
    /// </summary> 
    private void getRockAndRollItem(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;
        context.Response.ContentType = "application/json";
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(RockAndRoll));
        RockAndRoll rockAndRoll = DatabaseHandler.getRockAndRoll((int)context.Items["rockAndRollID"]);
        jsonData.WriteObject(outputStream, rockAndRoll);
    }

    /// <summary>
    /// Creates a new RockAndRoll item and adds it to the database.
    /// Returns HTTP status 201 on success and an ID of the newly created entry.
    /// </summary>    
    private void postNewRockAndRoll(HttpContext context)
    {
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(RockAndRoll));
        RockAndRoll rockAndRoll = null;

        if (context.Request.Headers["Content-Type"].ToLower() == "application/json")
        {
            rockAndRoll = (RockAndRoll)jsonData.ReadObject(context.Request.InputStream);
            int result = DatabaseHandler.insertNewRockAndRoll(rockAndRoll);

            // return HTTP status 201 and an ID of newly created item
            context.Response.StatusCode = (Int32)HttpStatusCode.Created;
            context.Response.AddHeader("id", result.ToString());
        }
        else
        {
            dispatchBadRequest(context);
        }
    }

    /// <summary>
    /// Updates an existing RockAndRoll item.
    /// Returns OK on success, Not Found if invalid ID was provided and Bad Request if other issues appear.
    /// </summary>
    private void updateRockAndRoll(HttpContext context)
    {
        try
        {
            RockAndRoll rockAndRoll = null;
            if (context.Request.Headers["Content-Type"].ToLower() == "application/json")
            {
                DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(RockAndRoll));
                rockAndRoll = (RockAndRoll)jsonData.ReadObject(context.Request.InputStream);
                int result = DatabaseHandler.updateRockAndRoll((int)context.Items["rockAndRollID"], rockAndRoll);

                dispatchOk(context);

            }
            else
            {
                dispatchBadRequest(context);
            }
        }
        catch (System.Exception)
        {
            dispatchBadRequest(context);
        }
    }

    /// <summary>
    /// Removes RockAndRoll record with given ID.
    /// Returns OK or Not Found.
    /// </summary>
    private void deleteRockAndRoll(HttpContext context)
    {
        int result = DatabaseHandler.deleteRockAndRoll((int)context.Items["rockAndRollID"]);
        dispatchOk(context);
    }
    #endregion
}