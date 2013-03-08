using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Session
/// </summary>
public class Session
{
    private string EventTitle { get; set; }
    private DateTime Occurence { get; set; }
    private string Staff { get; set; }
    private string Room { get; set; }

	public Session(string eventTitle, DateTime occurence)
	{
        this.EventTitle = eventTitle;
        this.Occurence = occurence;
	}
        
    public override string ToString()
    {
        return EventTitle + ", " + Occurence.ToString();
    }
}