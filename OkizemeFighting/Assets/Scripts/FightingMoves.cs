using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingMoves {

    private string _type;
    public string Type
    {
        get { return _type; }
        set
        {
            if (value != _type)
                _type = value;
        }
    }

    private double _typeValue;
    public double TypeValue
    {
        get { return _typeValue; }
        set
        {
            if (value != _typeValue)
                _typeValue = value;
        }
    }

    private Hability _hability;
    public Hability Hability
    {
        get { return _hability; }
        set
        {
            if (value != _hability)
                _hability = value;
        }
    }

    public FightingMoves(string type, double typeValue)
    {
        Type = type;
        TypeValue = typeValue;
        Hability = null;
    }

    public FightingMoves(string type = "", string name = "", string description = "", int combocost = 0, int damages = 0)
    {
        Type = type;
        Hability = new Hability(name, description, combocost, damages);
    }
}
