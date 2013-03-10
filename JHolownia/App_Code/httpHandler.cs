using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net;

/// <summary>
/// Summary description for HttpHandler
/// </summary>
public class HttpHandler : IHttpHandler
{        
    delegate void HandleRequest(HttpContext context, UriTemplateMatch template);
    UriTemplateTable _templateTable = null;
    object _tableLock = new object();

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
                        UriTemplate drugsItemTemplate = new UriTemplate(request.ApplicationPath + "/v1/Drugs/{DrugsID}");
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(drugsTemplate, new HandleRequest(processDrugs)));
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(drugsItemTemplate, new HandleRequest(processDrugsItem)));

                        // RockAndRoll
                        UriTemplate rockandrollTemplate = new UriTemplate(request.ApplicationPath + "/v1/RockAndRoll/{RockAndRollID}");
                        UriTemplate rockandrollItemTemplate = new UriTemplate(request.ApplicationPath + "/v1/RockAndRoll/{RockAndRollID}");
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(rockandrollTemplate, new HandleRequest(processRockAndRoll)));
                        _templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(rockandrollItemTemplate, new HandleRequest(processRockAndRollItem)));
                    }
                }
            }
            return _templateTable;
        }
    }
   
    public bool IsReusable
    {
        get { return true; }
    }
        
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
    private void dispatchOk(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.OK;
    }

    private void dispatchNotFound(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.NotFound;
    }

    private void dispatchNotAllowed(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.MethodNotAllowed;
    }

    private void dispatchBadRequest(HttpContext context)
    {
        context.Response.StatusCode = (Int32)HttpStatusCode.BadRequest;
    }
    #endregion

    // Sex methods
    //--------------------------------------------------------------------
       
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

    private void processSexItem(HttpContext context, UriTemplateMatch template)
    {
        Int32 sexID = 0;
                
        if (!Int32.TryParse(template.BoundVariables["SexID"], out sexID))
        {
            dispatchBadRequest(context);
            return;
        }

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

       
    private void getAllSex(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;
        // Notify caller that the response resource is in JSON
        context.Response.ContentType = "application/json";

        // Create new serializer object
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(IEnumerable<Sex>));

        IEnumerable<Sex> sexes = DatabaseHandler.getAllSex();
        jsonData.WriteObject(outputStream, sexes);

        HttpResponse Response = context.Response;
    }

    private void getSexItem(HttpContext context)
    {
        Stream outputStream = context.Response.OutputStream;
        context.Response.ContentType = "application/json";
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
        Sex sex = DatabaseHandler.getSex((int)context.Items["SexID"]);
        jsonData.WriteObject(outputStream, sex);
        HttpResponse Response = context.Response;
    }

    // add try catch around serializers
    private void postNewSex(HttpContext context)
    {
        // Deserialise what is comming in

        // Create th new serializer object
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
        // then use the ReadObject method for the serializer to input into the module object
        
        Sex sex = (Sex)jsonData.ReadObject(context.Request.InputStream);
        // Now we can use our module object to write to our database
        int modID = DatabaseHandler.insertNewSex(sex);

        // For the moment we will just output to the response object that we have a new ID
        // In reality we would do more then this to conform to good practice and to also catch any errors
        HttpResponse Response = context.Response;
        Response.Write("<html>");
        Response.Write("<body>");
        Response.Write("<p>This is the new response from the httphandler written for ");
        Response.Write("Sex path and the POST verb");
        Response.Write("</p>");
        Response.Write("<p>");
        Response.Write(sex.ToString());
        Response.Write("</p>");
        Response.Write("<p><a href=\"http://fostvm.fost.plymouth.ac.uk/soft338/jholownia/\">Home</a></p>");
        Response.Write("</body>");
        Response.Write("</html>");
    }

    private void updateSex(HttpContext context)
    {
        try
        {
            Sex sex = null;
            if (context.Request.Headers["Content-Type"].ToLower() == "application/json")
            {
                DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
                sex = (Sex)jsonData.ReadObject(context.Request.InputStream);
                int modID = DatabaseHandler.updateSex((int)context.Items["SexID"], sex);
                
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

    private void deleteSex(HttpContext context)
    {
        int result = DatabaseHandler.deleteSex((int)context.Items["SexID"]);
        dispatchOk(context);
    }

    // Drugs methods
    //--------------------------------------------------------------------

    public void processDrugs(HttpContext context, UriTemplateMatch template)
    {
        HttpResponse Response = context.Response;

        //the handler should be called whenever a path is called 
        Response.Write("<html>");
        Response.Write("<body>");
        Response.Write("<h1>SOFT338 Response<h1>");
        Response.Write("<p>This is the response from the drugs path</p>");
        // Response.Write("<p>Event Title : " + s.EventTitle + "; Occurrence : " + s.Occurrence.ToShortDateString() + "</p>");
        Response.Write("</body>");
        Response.Write("</html>");

         
        /*
        // check the http verb
        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getAllSex(context);
                break;
            case "post":
                postNewSex(context);
                break;
            case "put":
                updateSex(context);
                break;
            case "delete":
                deleteSex(context);
                break;
            default:
                break;
        }
         */
    }

    public void processDrugsItem(HttpContext context, UriTemplateMatch template)
    {

    }

    // RockAndRoll methods
    //--------------------------------------------------------------------

    public void processRockAndRoll(HttpContext context, UriTemplateMatch template)
    {
         // check the http verb
        string verb = context.Request.HttpMethod.ToLower();

        switch (verb)
        {
            case "get":
                getAllSex(context);
                break;
            case "post":
                postNewSex(context);
                break;
            case "put":
                updateSex(context);
                break;
            case "delete":
                deleteSex(context);
                break;
            default:
                break;
        }
    }

    public void processRockAndRollItem(HttpContext context, UriTemplateMatch template)
    {

    }
}