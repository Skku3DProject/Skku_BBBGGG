using System.Collections.Generic;
using UnityEngine;

public class TilePos
{
    private const float ATLAS_SIZE = 16f;
    private const float UV_OFFSET = 0.001f;

    private readonly Vector2[] _uvs;

    public TilePos(int xPos, int yPos)
    {
        _uvs = new Vector2[]
        {
            new Vector2(xPos/ATLAS_SIZE + UV_OFFSET, yPos/ATLAS_SIZE + UV_OFFSET),
            new Vector2(xPos/ATLAS_SIZE + UV_OFFSET, (yPos+1)/ATLAS_SIZE - UV_OFFSET),
            new Vector2((xPos+1)/ATLAS_SIZE - UV_OFFSET, (yPos+1)/ATLAS_SIZE - UV_OFFSET),
            new Vector2((xPos+1)/ATLAS_SIZE - UV_OFFSET, yPos/ATLAS_SIZE + UV_OFFSET),
        };
    }

    public Vector2[] GetUVs() => _uvs;

    public static readonly Dictionary<Tile, TilePos> tiles = new Dictionary<Tile, TilePos>()
    {
        {Tile.Dirt, new TilePos(5,0)},
        {Tile.Grass, new TilePos(6,0)},
        {Tile.GrassSide, new TilePos(7,0)},
        {Tile.Stone, new TilePos(4,0)},
        {Tile.Snow, new TilePos(2,1)},
        {Tile.SnowSide, new TilePos(1,1)},
    };
}