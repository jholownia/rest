using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

/// <summary>
/// Summary description for Drug
/// </summary>
/// [DataContract]
public class Drug
{
    // Name
    private string name;
    [DataMember]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    // Chemical formula
    private string formula;
    [DataMember]
    public string Formula
    {
        get { return formula; }
        set { formula = value; }
    }

    // Administration method
    private string administration;
    [DataMember]
    public string Administration
    {
        get { return administration; }
        set { administration = value; }
    }
    
    // Effects
    private string effects;
    [DataMember]
    public string Effects
    {
        get { return effects; }
        set { effects = value; }
    }

    // Drug constructor
	public Drug(string name, string formula, string administration, string effects)
	{
        this.name = name;
        this.formula = formula;
        this.administration = administration;
        this.effects = effects;
	}
}