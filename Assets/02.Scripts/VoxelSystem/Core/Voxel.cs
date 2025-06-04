using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Voxel
{
    private readonly Tile _top;
    private readonly Tile _side;
    private readonly Tile _bottom;

    public TilePos TopPos;
    public TilePos SidePos;
    public TilePos BottomPos;

    public Voxel(Tile tile)
    {
        _top = _side = _bottom = tile;
        UpdateTilePositions();
    }

    public Voxel(Tile top, Tile side, Tile bottom)
    {
        _top = top;
        _side = side;
        _bottom = bottom;
        UpdateTilePositions();
    }

    void UpdateTilePositions()
    {
        TopPos = TilePos.tiles[_top];
        SidePos = TilePos.tiles[_side];
        BottomPos = TilePos.tiles[_bottom];
    }


    public static Dictionary<VoxelType, Voxel> blocks = new Dictionary<VoxelType, Voxel>()
    {
        {VoxelType.Grass, new Voxel(Tile.Grass, Tile.GrassSide, Tile.Dirt)},
        {VoxelType.Dirt, new Voxel(Tile.Dirt)},
        {VoxelType.Stone, new Voxel(Tile.Stone)},
        {VoxelType.Snow, new Voxel(Tile.Snow, Tile.SnowSide, Tile.Dirt) }
    };
}
