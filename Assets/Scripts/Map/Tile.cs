using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public enum Type
    {
        Block = 0,
        Floor = 1,
        Hall = 2
    }

    public Type type;

    public Tile(Type type)
    {
        this.type = type;
    }
}
