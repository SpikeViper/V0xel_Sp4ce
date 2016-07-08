using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable()]

public class BlockType
{
    public int temperature;
    public bool light;
    public float LightRange;
    public float LightIntensity;
    public int density;
    public string BlockName;
    public byte LR;
    public byte LG;
    public byte LB;
    public byte LA;
    public Block.Tile tile;
    public bool IsSolid;
}
