using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable()]

public class Block
{
    public BlockType type;

    public Block(BlockType type1)
    {
        type = type1;
    }



    [System.SerializableAttribute()]
    public struct Tile { public int x; public int y;}

    [System.SerializableAttribute()]
    public struct SolidSides { public bool up; public bool down; public bool east; public bool west; public bool north; public bool south;}

    public SolidSides solidsides;


    public MeshData Blockdata
     (PlanetChunk planetchunk, int x, int y, int z, MeshData meshData)
    {

        

        if (this.type != BlockTypes.typeEmpty)
        {

            GetSolid(planetchunk, x, y, z, meshData);

            meshData.useRenderDataForCol = true;

            if (solidsides.up == false)
            {
                meshData.BuildSideUp(x, y, z, this.type);
            }
            if (solidsides.down == false)
            {
                meshData.BuildSideDown(x, y, z, this.type);
            }
            if (solidsides.east == false)
            {
                meshData.BuildSideEast(x, y, z, this.type);
            }
            if (solidsides.west == false)
            {
                meshData.BuildSideWest(x, y, z, this.type);
            }
            if (solidsides.north == false)
            {
                meshData.BuildSideNorth(x, y, z, this.type);
            }
            if (solidsides.south == false)
            {
                meshData.BuildSideSouth(x, y, z, this.type);
            }




            if (type.light == true)
            {

                meshData.Light(type.light);

                if (!solidsides.up && !solidsides.down && !solidsides.east && !solidsides.west && !solidsides.north && !solidsides.south)
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


        if (solidsides.up == false)
        {
            meshData.AddFaceCount();
        }
        if (solidsides.down == false)
        {
            meshData.AddFaceCount();
        }
        if (solidsides.east == false)
        {
            meshData.AddFaceCount();
        }
        if (solidsides.west == false)
        {
            meshData.AddFaceCount();
        }
        if (solidsides.north == false)
        {
            meshData.AddFaceCount();
        }
        if (solidsides.south == false)
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
            solidsides.up = true;
        }
        else
        {
            solidsides.up = false;
        }

        if (planetchunk.GetBlock(x, y - 1, z).IsSolid)
        {
            solidsides.down = true;
        }
        else
        {
            solidsides.down = false;
        }

        if (planetchunk.GetBlock(x, y, z + 1).IsSolid)
        {
            solidsides.north = true;
        }
        else
        {
            solidsides.north = false;
        }

        if (planetchunk.GetBlock(x, y, z - 1).IsSolid)
        {
            solidsides.south = true;
        }
        else
        {
            solidsides.south = false;
        }

        if (planetchunk.GetBlock(x + 1, y, z).IsSolid)
        {
            solidsides.east = true;
        }
        else
        {
            solidsides.east = false;
        }

        if (planetchunk.GetBlock(x - 1, y, z).IsSolid)
        {
            solidsides.west = true;
        }
        else
        {
            solidsides.west = false;
        }

    }





































}