using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

/// <summary>
/// Summary description for httpHandler
/// </summary>
public class httpHandler : IHttpHandler
{
	public httpHandler()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        // the handler should be called whenever a path is called /Module/
        // This handler is called whenever a file ending 
        // in .sample is requested. A file with that extension
        // does not need to exist.

        String verb = "";
        String data = "<p>Data:</p>\n";
        Object o = null;
        
        if (Request.Path.Contains("module"))
        {
            verb = "module";
            o = new Module("API Programming", "SOFT338", "Spring");
                       
            List<Module> modules = DbConnection.getAllModules();

            data += "<p>\n";

            foreach (Module m in modules)
            {
                data += m.ToString();
                data += "<br />";
            }

            data += "</p>";

        }
        else if (Request.Path.Contains("practical"))
        {
            verb = "practical";
            o = new Session("Practical session", DateTime.Now);
        }
        else if (Request.Path.Contains("lecture"))
        {
            verb = "lecture";
            DateTime dt = new DateTime(2013, 02, 15);
            o = new Session("SOFT338 lecture", dt);
        }
        else
        {
            verb = "unrecognized request";
            o = new Object();
        }


        Response.Write("<html>");
        Response.Write("<body>");
        Response.Write("<p>This is the new response from the httphandler 3 written for ");
        Response.Write(verb);
        Response.Write("</p>");
        Response.Write("<p>");
        Response.Write(o.ToString());
        Response.Write(data);
        Response.Write("</p>");
        Response.Write("<p><a href=\"http://fostvm.fost.plymouth.ac.uk/soft338/jholownia/\">Home</a></p>");
        Response.Write("</body>");
        Response.Write("</html>");
    }

    private void postNewModule(HttpContext context)
    {
        // Deserialise what is comming in

        // Create th new serializer object
        DataContractJsonSerializer jsonData = new DataContractJsonSerializer(typeof(Module));
        // then use the ReadObject method for the serializer to input into the module object
        Module m = (Module)jsonData.ReadObject(context.Request.InputStream);
        // Now we can use our module object to write to our database
        Int32 modID = DbConnection.insertNewModule(m);

        // For the moment we will just output to the response object that we have a new ID
        // In reality we would do more then this to conform to good practice and to also catch any errors
        HttpResponse Response = context.Response;
        Response.Write("<html>");
        Response.Write("<body>");
        Response.Write("<p>This is the new response from the httphandler written for ");
        Response.Write("Module path and the POST verb");
        Response.Write("</p>");
        Response.Write("<p>");
        Response.Write(m.ToString());        
        Response.Write("</p>");
        Response.Write("<p><a href=\"http://fostvm.fost.plymouth.ac.uk/soft338/jholownia/\">Home</a></p>");
        Response.Write("</body>");
        Response.Write("</html>");
    }
}