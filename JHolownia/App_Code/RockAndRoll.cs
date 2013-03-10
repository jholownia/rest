using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

/// <summary>
/// Summary description for RockAndRoll
/// </summary>
/// [DataContract]
public class RockAndRoll
{
    // Title
    private string title;
    [DataMember]
    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    // Artist
    private string artist;
    [DataMember]
    public string Artist
    {
        get { return artist; }
        set { artist = value; }
    }

    // Album
    private string album;
    [DataMember]
    public string Album
    {
        get { return album; }
        set { album = value; }
    }
    
    // Date
    private DateTime date;
    [DataMember]
    public System.DateTime Date
    {
        get { return date; }
        set { date = value; }
    }

    // YouTube link    
    private string link;
    [DataMember]
    public string Link
    {
        get { return link; }
        set { link = value; }
    }

	// RockAndRoll constructor    
    public RockAndRoll(string title, string artist, string album, DateTime date, string link)
	{
        this.title = title;
        this.artist = artist;
        this.album = album;
        this.date = date;
        this.link = link;
	}
}