using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Voxel
{
    public Tile top, side, bottom;

    public TilePos topPos, sidePos, bottomPos;

    public Voxel(Tile tile)
    {
        top = side = bottom = tile;
        GetPositions();
    }

    public Voxel(Tile top, Tile side, Tile bottom)
    {
        this.top = top;
        this.side = side;
        this.bottom = bottom;
        GetPositions();
    }

    void GetPositions()
    {
        topPos = TilePos.tiles[top];
        sidePos = TilePos.tiles[side];
        bottomPos = TilePos.tiles[bottom];
    }


    public static Dictionary<VoxelType, Voxel> blocks = new Dictionary<VoxelType, Voxel>(){
        {VoxelType.Grass, new Voxel(Tile.Grass, Tile.GrassSide, Tile.Dirt)},
        {VoxelType.Dirt, new Voxel(Tile.Dirt)},
        {VoxelType.Stone, new Voxel(Tile.Stone)},
        {VoxelType.Trunk, new Voxel(Tile.TreeCX, Tile.TreeSide, Tile.TreeCX)},
        {VoxelType.Leaves, new Voxel(Tile.Leaves)},
    };
}
