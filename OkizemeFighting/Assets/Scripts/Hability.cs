using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hability {

    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value != _name)
                _name = value;
        }
    }

    private string _description;
    public string Description
    {
        get { return _description; }
        set
        {
            if (value != _description)
                _description = value;
        }
    }

    private int _comboCost;
    public int ComboCost
    {
        get { return _comboCost; }
        set
        {
            if (value != _comboCost)
                _comboCost = value;
        }
    }

    private int _damages;
    public int Damages
    {
        get { return _damages; }
        set
        {
            if (value != _damages)
                _damages = value;
        }
    }

    public Hability(string name, string description, int comboCost, int damages)
    {
        Name = name;
        Description = description;
        ComboCost = comboCost;
        Damages = damages;
    }
}
