﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable()]

public class Block
{
    private BlockType type;

    public Block(BlockType type1)
    {
        type = type1;
    }



    /// <summary>
    /// The position on the Texture Atlas of this block's texture
    /// </summary>
    [System.SerializableAttribute()]
    public struct Tile { public int x; public int y;}

    /// <summary>
    /// Holds the state of each side of the block (Solid = true, Non-Solid = false)
    /// </summary>
    public bool up; public bool down; public bool east; public bool west; public bool north; public bool south;


    /// <summary>
    /// Sends data about this block to MeshData so it can be rendered
    /// </summary>
    /// <param name="planetchunk">The PlanetChunk that contains this block</param>
    /// <param name="x">The X coordinate of the Block</param>
    /// <param name="y">The Y coordinate of the Block</param>
    /// <param name="z">The Z coordinate of the Block</param>
    /// <param name="meshData">The MeshData object the mesh is written to</param>
    /// <returns></returns>
    public MeshData Blockdata
     (PlanetChunk planetchunk, int x, int y, int z, MeshData meshData)
    {

        

        if (this.type != BlockTypes.typeEmpty)
        {

            GetSolid(planetchunk, x, y, z, meshData);

            meshData.useRenderDataForCol = true;

            if (up == false)
            {
                meshData.BuildSideUp(x, y, z, this.type);
            }
            if (down == false)
            {
                meshData.BuildSideDown(x, y, z, this.type);
            }
            if (east == false)
            {
                meshData.BuildSideEast(x, y, z, this.type);
            }
            if (west == false)
            {
                meshData.BuildSideWest(x, y, z, this.type);
            }
            if (north == false)
            {
                meshData.BuildSideNorth(x, y, z, this.type);
            }
            if (south == false)
            {
                meshData.BuildSideSouth(x, y, z, this.type);
            }




            if (type.light == true)
            {

                meshData.Light(type.light);

                if (!up && !down && !east && !west && !north && !south)
                {
                    meshData.AddLight(x, y, z, type.LR, type.LG, type.LB, type.LA, type.LightRange, type.LightIntensity);
                }

            }

            return meshData;

        }
   
        return meshData;
    }

    /// <summary>
    /// Counts the amount of vertices, triangles, and uvs will be needed to correctly set the size of the lists.
    /// </summary>
    /// <param name="meshData">MeshData this data is being sent to</param>
    public void PreCount(MeshData meshData)
    {


        if (up == false)
        {
            meshData.AddFaceCount();
        }
        if (down == false)
        {
            meshData.AddFaceCount();
        }
        if (east == false)
        {
            meshData.AddFaceCount();
        }
        if (west == false)
        {
            meshData.AddFaceCount();
        }
        if (north == false)
        {
            meshData.AddFaceCount();
        }
        if (south == false)
        {
            meshData.AddFaceCount();
        }


    }

    //Deferred mesh method

    //protected MeshData FaceDataUp
    //    (PlanetChunk planetchunk, int x, int y, int z, MeshData meshData)
    //{
    //    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
    //    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
    //    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
    //    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
    //
    //    meshData.AddQuadTriangles();
    //    meshData.AddUVRange(FaceUVs(Direction.up));
    //    return meshData;
    //}

    /// <summary>
    /// Gets whether or not the blocks next to this one are solid. Used to determine which sides to render.
    /// </summary>
    /// <param name="planetchunk">The planetchunk this block is contained by.</param>
    /// <param name="x">The X coordinate of the Block</param>
    /// <param name="y">The Y coordinate of the Block</param>
    /// <param name="z">The Z coordinate of the Block</param>
    /// <param name="meshData">The MeshData the data is being sent to (to render the block).</param>
    public void GetSolid(PlanetChunk planetchunk, int x, int y, int z, MeshData meshData)
    {

        if (planetchunk.GetBlock(x, y + 1, z).IsSolid)
        {
            up = true;
        }
        else
        {
            up = false;
        }

        if (planetchunk.GetBlock(x, y - 1, z).IsSolid)
        {
            down = true;
        }
        else
        {
            down = false;
        }

        if (planetchunk.GetBlock(x, y, z + 1).IsSolid)
        {
            north = true;
        }
        else
        {
            north = false;
        }

        if (planetchunk.GetBlock(x, y, z - 1).IsSolid)
        {
            south = true;
        }
        else
        {
            south = false;
        }

        if (planetchunk.GetBlock(x + 1, y, z).IsSolid)
        {
            east = true;
        }
        else
        {
            east = false;
        }

        if (planetchunk.GetBlock(x - 1, y, z).IsSolid)
        {
            west = true;
        }
        else
        {
            west = false;
        }

    }

    public void SetType(BlockType newtype)
    {
        this.type = newtype;
    }

    public BlockType GetType()
    {
        return this.type;
    }

}
