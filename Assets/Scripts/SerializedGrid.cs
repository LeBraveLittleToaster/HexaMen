using System;

[Serializable]
public class SerializedGrid
{
    public SerializedHex[,] grid;
}
[Serializable]
public class SerializedHex
{
    public int layerHeight;
    public SerializedHexType hexType;
    public int column;
    public int row;
}

[Serializable]
public enum SerializedHexType
{
    NONE,
    PLAYER,
    ENEMY
}