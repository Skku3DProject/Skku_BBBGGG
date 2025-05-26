using System.Collections.Generic;
using UnityEngine;

public class TilePos
{

    private int _xPos;
    private int _yPos;

    private Vector2[] _uvs;

    public TilePos(int xPos, int yPos)
    {
        _xPos = xPos;
        _yPos = yPos;
        _uvs = new Vector2[]
        {
            new Vector2(xPos/16f + .001f, yPos/16f + .001f),
            new Vector2(xPos/16f+ .001f, (yPos+1)/16f - .001f),
            new Vector2((xPos+1)/16f - .001f, (yPos+1)/16f - .001f),
            new Vector2((xPos+1)/16f - .001f, yPos/16f+ .001f),
        };
    }

    public Vector2[] GetUVs()
    {
        return _uvs;
    }


    public static Dictionary<Tile, TilePos> tiles = new Dictionary<Tile, TilePos>()
    {
        {Tile.Dirt, new TilePos(5,0)},
        {Tile.Grass, new TilePos(6,0)},
        {Tile.GrassSide, new TilePos(7,0)},
        {Tile.Stone, new TilePos(4,0)},

    };
}