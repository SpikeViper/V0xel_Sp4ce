using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Serialization;
using System.IO;
using UnityThreading;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]


public class PlanetChunk : MonoBehaviour
{

    public MeshData meshData;
    public Block[,,] blocks = new Block[localVars.chunklength, localVars.chunklength, localVars.chunklength];
    public MeshFilter filter;
    public MeshCollider coll;
    public Mesh colmesh;
    public Mesh mesh;
    public string PlanetType;
    public int Temperature;
    public List<Block> types;
    public int StarDistance;
    public Planet planet;
    public Vector3 Position;
    public bool Generated = false;
    public bool FirstUpdate = false;
    public GameObject[,,] lights;
    public bool modified;
    int timer;
    public int updates;
    public int chunklength;
    public bool firstrender = true;
    Thread genthread;


    // Generates using a coroutine. This speedss things up ALOT. We also use a thread here to get some heavy calculations out of the main thread. 
    public IEnumerator Generate()
    {

        chunklength = localVars.chunklength;
        meshData = new MeshData();
        colmesh = new Mesh();
        mesh = new Mesh();
        this.coll = this.gameObject.GetComponent<MeshCollider>();
        this.filter = this.gameObject.GetComponent<MeshFilter>();
        genthread = new Thread(() => { blocks = planet.Generator.Generate(blocks, "rock", planet.planetSize, planet.planetSeed, (int)Position.x, (int)Position.y, (int)Position.z); });
        genthread.Start();
        while (genthread.IsAlive == true) { yield return null; }

        Generated = true;

    }

    //Gets a block (type) using coordinates. This will change to also support getting the block object itself.
    public BlockType GetBlock(int x, int y, int z)
    {
        int x2 = (int)Position.x;
        int y2 = (int)Position.y;
        int z2 = (int)Position.z;

        if (x >= 0 && y >= 0 && z >= 0 && x < chunklength && y < chunklength && z < chunklength)
        {
            return blocks[x, y, z].GetType();
        }
        else
        {

            if (x > chunklength - 1)
            {
                return planet.GetBlock(new Vector3(x2 + 1, y2, z2), 0, y, z);
            }

            if (x == -1)
            {
                return planet.GetBlock(new Vector3(x2 - 1, y2, z2), 15, y, z);
            }


            if (y > chunklength - 1)
            {
                return planet.GetBlock(new Vector3(x2, y2 + 1, z2), x, 0, z);
            }

            if (y == -1)
            {
                return planet.GetBlock(new Vector3(x2, y2 - 1, z2), x, 15, z);
            }

            if (z > chunklength - 1)
            {
                return planet.GetBlock(new Vector3(x2, y2, z2 + 1), x, y, 0);
            }

            if (z == -1)
            {
                return planet.GetBlock(new Vector3(x2, y2, z2 - 1), x, y, 15);
            }

            return BlockTypes.typeStone;

        }



    }



    //Sets a block using coordinates and Type to set it to
    public void SetBlock(int x, int y, int z, BlockType typenew)
    {

        int x2 = (int)Position.x;
        int y2 = (int)Position.y;
        int z2 = (int)Position.z;

        Debug.Log(x + " " + y + " " + z);

        if (x > -1 && y > -1 && z > -1 && x < chunklength && y < chunklength && z < chunklength)
        {
            blocks[x, y, z].SetType(typenew);
            modified = true;

            if (lights[x, y, z] != null)
            {
                Destroy(lights[x, y, z]);
                lights[x, y, z] = null;
            }

        }
        else
        {


            if (x > chunklength - 1)
            {
                planet.SetBlock(new Vector3(x2 + 1, y2, z2), 0, y, z, typenew);
            }
            else if (y > chunklength - 1)
            {
                planet.SetBlock(new Vector3(x2, y2 + 1, z2), x, 0, z, typenew);
            }
            else if (z > chunklength - 1)
            {
                planet.SetBlock(new Vector3(x2, y2, z2 + 1), x, y, 0, typenew);
            }
            else if (x < 0)
            {
                planet.SetBlock(new Vector3(x2 - 1, y2, z2), chunklength - 1, y, z, typenew);
            }
            else if (y < 0)
            {
                planet.SetBlock(new Vector3(x2, y2 - 1, z2), x, chunklength - 1, z, typenew);
            }
            else if (z < 0)
            {
                planet.SetBlock(new Vector3(x2, y2, z2 - 1), x, y, chunklength - 1, typenew);
            }



        }




        UpdatePlanetChunk();

    }

    // Updates the planetchunk.
    public void UpdatePlanetChunk()
    {

        updates = updates + 1;

        if (Generated == true)
        {

            meshData.Clear();

            for (int x = 0; x < chunklength; x++)
            {
                for (int y = 0; y < chunklength; y++)
                {
                    for (int z = 0; z < chunklength; z++)
                    {

                        if (blocks[x, y, z].GetType() != BlockTypes.typeEmpty)
                        {
                            blocks[x, y, z].PreCount(meshData);
                        }

                    }
                }
            }


            meshData.Rebuild();


            for (int x = 0; x < chunklength; x++)
            {
                for (int y = 0; y < chunklength; y++)
                {
                    for (int z = 0; z < chunklength; z++)
                    {

                        if (blocks[x, y, z].GetType() != BlockTypes.typeEmpty)
                        {
                            meshData = blocks[x, y, z].Blockdata(this, x, y, z, meshData);
                        }

                    }
                }
            }


            RenderMesh();

        }

    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    void RenderMesh()
    {
        if (firstrender == false)
        {
            mesh.Clear();
        }

        mesh.SetVertices(meshData.vertices);
        mesh.SetTriangles(meshData.triangles, 0);
        mesh.SetUVs(0, meshData.uv);

        if (firstrender == false)
        {
            colmesh.Clear();
        }

        colmesh.SetVertices(meshData.vertices);
        colmesh.SetTriangles(meshData.triangles, 0);

        if (meshData.light == true)
        {

            for (int i = 0; i < meshData.lightpositions.Count; i++)
            {

                Vector3 position = meshData.lightpositions[i];

                int x = (int)position.x;
                int y = (int)position.y;
                int z = (int)position.z;

                if (lights[x, y, z] == null)
                {


                    Color32 color = meshData.lightcolors[x, y, z];
                    float intensity = meshData.lightintensity[x, y, z];
                    float range = meshData.lightrange[x, y, z];

                    GameObject light = new GameObject("Light " + x + " " + y + " " + z);
                    Light lightComp = light.AddComponent<Light>();
                    lightComp.color = color;
                    lightComp.intensity = intensity;
                    lightComp.range = range;
                    light.transform.parent = this.transform;
                    light.transform.localPosition = new Vector3(x, y, z);

                    lights[x, y, z] = light;

                }

            }

        }

        filter.sharedMesh = mesh;
        coll.sharedMesh = mesh;
        filter.sharedMesh.RecalculateNormals();

        firstrender = false;

    }


    public void Update()
    {


        if (Generated == true && FirstUpdate == false)
        {
            UpdatePlanetChunk();
            FirstUpdate = true;
        }



    }

    public void UpdateNeighbors()
    {

        int x = (int)Position.x;
        int y = (int)Position.y;
        int z = (int)Position.z;

        planet.UpdateChunk(x + 1, y, z);
        planet.UpdateChunk(x, y + 1, z);
        planet.UpdateChunk(x, y, z + 1);
        planet.UpdateChunk(x - 1, y, z);
        planet.UpdateChunk(x, y - 1, z);
        planet.UpdateChunk(x, y, z - 1);

    }

    public void loadneighbors(float Range)
    {
        if (Range > 0)
        {
            int x = (int)Position.x;
            int y = (int)Position.y;
            int z = (int)Position.z;

            planet.LoadChunk(new Vector3(x + 1, y, z));
            planet.LoadChunk(new Vector3(x, y + 1, z));
            planet.LoadChunk(new Vector3(x, y, z + 1));
            planet.LoadChunk(new Vector3(x - 1, y, z));
            planet.LoadChunk(new Vector3(x, y - 1, z));
            planet.LoadChunk(new Vector3(x, y, z - 1));

            planet.planetchunks[x + 1, y, z].loadneighbors(Range - 1);
            planet.planetchunks[x, y + 1, z].loadneighbors(Range - 1);
            planet.planetchunks[x, y, z + 1].loadneighbors(Range - 1);
            planet.planetchunks[x - 1, y, z].loadneighbors(Range - 1);
            planet.planetchunks[x, y - 1, z].loadneighbors(Range - 1);
            planet.planetchunks[x, y, z - 1].loadneighbors(Range - 1);
        }
    }

    static float MoveWithinBlock(float pos, float norm, bool adjacent = false)
    {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
        {
            if (adjacent)
            {
                pos += (norm / 2);
            }
            else
            {
                pos -= (norm / 2);
            }
        }

        return (float)pos;
    }

    public void SetBlock(RaycastHit hit, BlockType type, bool adjacent = false)
    {
        Vector3 pos = GetBlockPos(hit, adjacent);

        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        x = Mathf.RoundToInt(x);
        y = Mathf.RoundToInt(y);
        z = Mathf.RoundToInt(z);

        SetBlock(x, y, z, type);
    }

    public Vector3 GetBlockPos(RaycastHit hit, bool adjacent = false)
    {


        Vector3 WorldSpace = new Vector3(
        MoveWithinBlock(hit.point.x, hit.normal.x, adjacent),
        MoveWithinBlock(hit.point.y, hit.normal.y, adjacent),
        MoveWithinBlock(hit.point.z, hit.normal.z, adjacent)
        );

        Vector3 NewWorldSpace = new Vector3(WorldSpace.x - this.transform.position.x, WorldSpace.y - this.transform.position.y, WorldSpace.z - this.transform.position.z);

        return NewWorldSpace;



    }
}
