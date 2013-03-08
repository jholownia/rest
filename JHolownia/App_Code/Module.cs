using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

[DataContract]
/// <summary>
/// Summary description for Module
/// </summary>
public class Module
{
    public string Title { get; set; }
    public string Code { get; set; }
    public string Term { get; set; }
    public Session[] Lectures { get; set; }
    public Session[] Practicals { get; set; }    
    
    public Module(string title, string code, string term)
	{
        this.Title = title;
        this.Code = code;
        this.Term = term;
	}

    public override string ToString()
    {
        return this.Title + ", " + this.Code + ", " + this.Term;
    }
}