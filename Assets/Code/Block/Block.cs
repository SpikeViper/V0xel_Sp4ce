using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable()]

public class Block
{
    public BlockType type;
    public bool blockupsolid;
    public bool blockdownsolid;
    public bool blockeastsolid;
    public bool blockwestsolid;
    public bool blocknorthsolid;
    public bool blocksouthsolid;

    public Block(BlockType type1)
    {
        type = type1;
    }

    [System.SerializableAttribute()]
    public struct Tile { public int x; public int y;}


    public MeshData Blockdata
     (PlanetChunk planetchunk, int x, int y, int z, MeshData meshData)
    {

        

        if (this.type != BlockTypes.typeEmpty)
        {

            GetSolid(planetchunk, x, y, z, meshData);

            meshData.useRenderDataForCol = true;

            if (blockupsolid == false)
            {
                meshData.BuildSideUp(x, y, z, this.type);
            }
            if (blockdownsolid == false)
            {
                meshData.BuildSideDown(x, y, z, this.type);
            }
            if (blockeastsolid == false)
            {
                meshData.BuildSideEast(x, y, z, this.type);
            }
            if (blockwestsolid == false)
            {
                meshData.BuildSideWest(x, y, z, this.type);
            }
            if (blocknorthsolid == false)
            {
                meshData.BuildSideNorth(x, y, z, this.type);
            }
            if (blocksouthsolid == false)
            {
                meshData.BuildSideSouth(x, y, z, this.type);
            }




            if (type.light == true)
            {

                meshData.Light(type.light);

                if (!blockupsolid && !blockdownsolid && !blockeastsolid && !blockwestsolid && !blocknorthsolid && !blocksouthsolid)
                {
                    meshData.AddLight(x, y, z, type.LR, type.LG, type.LB, type.LA, type.LightRange, type.LightIntensity);
                }

            }

            return meshData;

        }
   
        return meshData;
    }

    public void PreCount(MeshData meshData)
    {


        if (blockupsolid == false)
        {
            meshData.AddFaceCount();
        }
        if (blockdownsolid == false)
        {
            meshData.AddFaceCount();
        }
        if (blockeastsolid == false)
        {
            meshData.AddFaceCount();
        }
        if (blockwestsolid == false)
        {
            meshData.AddFaceCount();
        }
        if (blocknorthsolid == false)
        {
            meshData.AddFaceCount();
        }
        if (blocksouthsolid == false)
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


    public void GetSolid(PlanetChunk planetchunk, int x, int y, int z, MeshData meshData)
    {

        if (planetchunk.GetBlock(x, y + 1, z).IsSolid)
        {
            blockupsolid = true;
        }
        else
        {
            blockupsolid = false;
        }

        if (planetchunk.GetBlock(x, y - 1, z).IsSolid)
        {
            blockdownsolid = true;
        }
        else
        {
            blockdownsolid = false;
        }

        if (planetchunk.GetBlock(x, y, z + 1).IsSolid)
        {
            blocknorthsolid = true;
        }
        else
        {
            blocknorthsolid = false;
        }

        if (planetchunk.GetBlock(x, y, z - 1).IsSolid)
        {
            blocksouthsolid = true;
        }
        else
        {
            blocksouthsolid = false;
        }

        if (planetchunk.GetBlock(x + 1, y, z).IsSolid)
        {
            blockeastsolid = true;
        }
        else
        {
            blockeastsolid = false;
        }

        if (planetchunk.GetBlock(x - 1, y, z).IsSolid)
        {
            blockwestsolid = true;
        }
        else
        {
            blockwestsolid = false;
        }

    }





































}