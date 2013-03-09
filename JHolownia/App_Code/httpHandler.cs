using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

/// <summary>
/// Summary description for HttpHandler
/// </summary>
public class HttpHandler : IHttpHandler
{
	public HttpHandler()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public bool IsReusable
    {
        get { return true; }
    }

    // FIXME: look at URI template matching
    public void ProcessRequest(HttpContext context)
    {
        HttpRequest Request = context.Request;

        string path = Request.Path.Replace("/soft338/jholownia/", "");

        // So apparently we can use strings in switch statement
        switch (path.ToLower())
        {
            case "sex":
                processSex(context);
                break;
            case "drugs":
                processDrugs(context);
                break;
            case "rockandroll":
                processRockAndRoll(context);
                break;
            default:
                break;
        }
    }

    // Sex methods
    //--------------------------------------------------------------------
    
    // does this need to be public?
    public void processSex(HttpContext context)
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
        Response.Write("<html>");
        Response.Write("<body>");
        Response.Write("<p>This is the new response from the httphandler written for ");
        Response.Write("Sex path and the GET verb");
        Response.Write("</p>");
        Response.Write("<p>");
        Response.Write(sexes.ToString());
        Response.Write("</p>");
        Response.Write("<p><a href=\"http://fostvm.fost.plymouth.ac.uk/soft338/jholownia/\">Home</a></p>");
        Response.Write("</body>");
        Response.Write("</html>");
    }

    // add try catch around serializers
    private void postNewSex(HttpContext context)
    {
        // Deserialise what is comming in

        // Create th new serializer object
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Sex));
        // then use the ReadObject method for the serializer to input into the module object
        
        Sex s = (Sex)jsonData.ReadObject(context.Request.InputStream);
        // Now we can use our module object to write to our database
        int modID = DatabaseHandler.insertNewSex(s);

        // For the moment we will just output to the response object that we have a new ID
        // In reality we would do more then this to conform to good practice and to also catch any errors
        HttpResponse Response = context.Response;
        Response.Write("<html>");
        Response.Write("<body>");
        Response.Write("<p>This is the new response from the httphandler written for ");
        Response.Write("Sex path and the POST verb");
        Response.Write("</p>");
        Response.Write("<p>");
        Response.Write(s.ToString());
        Response.Write("</p>");
        Response.Write("<p><a href=\"http://fostvm.fost.plymouth.ac.uk/soft338/jholownia/\">Home</a></p>");
        Response.Write("</body>");
        Response.Write("</html>");
    }

    private void updateSex(HttpContext context)
    {
    
    }

    private void deleteSex(HttpContext context)
    {
    
    }

    // Drugs methods
    //--------------------------------------------------------------------

    public void processDrugs(HttpContext context)
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

    // RockAndRoll methods
    //--------------------------------------------------------------------
    
    public void processRockAndRoll(HttpContext context)
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
}