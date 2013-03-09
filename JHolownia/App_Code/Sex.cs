using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

/// <summary>
/// Summary description for Sex
/// </summary>
[DataContract]
public class Sex
{
    // Name
    private string name;
    [DataMember]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    // Description
    private string description;
    [DataMember]
    public string Description
    {
        get { return description; }
        set { description = value; }
    }
    
    // Sex constructor    
    public Sex(string name, string description)
	{
        this.name = name;
        this.description = description;
	}
    
}