using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Serialization;
using System.IO;
using UnityThreading;
using System.Linq;
using MovementEffects;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]


public class PlanetChunk : MonoBehaviour {

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
    public int chunklength;
    public bool firstrender = true;
    Thread genthread;


    // Use this for initialization
    public IEnumerator<float> Generate()
    {

        chunklength = localVars.chunklength;
        meshData = new MeshData();
        colmesh = new Mesh();
        mesh = new Mesh();
        this.coll = this.gameObject.GetComponent<MeshCollider>();
        this.filter = this.gameObject.GetComponent<MeshFilter>();
        genthread = new Thread(() => { blocks = planet.Generator.Generate(blocks, "rock", planet.planetSize, planet.planetSeed, (int)Position.x, (int)Position.y, (int)Position.z);});
        genthread.Start();
        while (genthread.IsAlive == true) { yield return 0f; }

        Generated = true;

    }


    public BlockType GetBlock(int x, int y, int z)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < chunklength && y < chunklength && z < chunklength)
        {
            return blocks[x, y, z].type;
        }
        else
        {

            if (x > chunklength - 1) {
                return planet.GetBlock(new Vector3(Position.x + 1, Position.y, Position.z), 0, y, z);
            }

            if (x == -1) {
                return planet.GetBlock(new Vector3(Position.x - 1, Position.y, Position.z), 15, y, z);
            }


            if (y > chunklength - 1) {
                return planet.GetBlock(new Vector3(Position.x, Position.y + 1, Position.z), x, 0, z);
            }

            if (y == -1) {
                return planet.GetBlock(new Vector3(Position.x, Position.y - 1, Position.z), x, 15, z);
            }

            if (z > chunklength - 1) {
                return planet.GetBlock(new Vector3(Position.x, Position.y, Position.z + 1), x, y, 0);
            }

            if (z == -1) {
                return planet.GetBlock(new Vector3(Position.x, Position.y, Position.z - 1), x, y, 15);
            }

            return BlockTypes.typeStone;

        }



    }


    public void SetBlock(int x, int y, int z, BlockType type)
    {

        if (x >= 0 && y >= 0 && z >= 0 && x < chunklength && y < chunklength && z < chunklength)
        {
            blocks[(int)x, (int)y, (int)z].type = type;
            modified = true;

            if (lights[(int)x, (int)y, (int)z] != null)
            {
                Destroy(lights[(int)x, (int)y, (int)z]);
                lights[(int)x, (int)y, (int)z] = null;
            }


            if (x == chunklength - 1)
            {
                planet.UpdateChunk((int)Position.x + 1, (int)Position.y, (int)Position.z);
            }
            else if (y == chunklength - 1)
            {
                planet.UpdateChunk((int)Position.x, (int)Position.y + 1, (int)Position.z);
            }
            else if (z == chunklength - 1)
            {
                planet.UpdateChunk((int)Position.x, (int)Position.y, (int)Position.z + 1);
            }
            else if (x < 0)
            {
                planet.UpdateChunk((int)Position.x - 1, (int)Position.y, (int)Position.z);
            }
            else if (y < 0)
            {
                planet.UpdateChunk((int)Position.x, (int)Position.y - 1, (int)Position.z);
            }
            else if (z < 0)
            {
                planet.UpdateChunk((int)Position.x, (int)Position.y, (int)Position.z - 1);
            }


        }
        else
        {


            if (x > chunklength - 1)
            {
                planet.SetBlock(new Vector3(Position.x + 1, Position.y, Position.z), 0, (int)y, (int)z, type);
            }
            else if (y > chunklength - 1)
            {
                planet.SetBlock(new Vector3(Position.x, Position.y + 1, Position.z), (int)x, 0, (int)z, type);
            }
            else if (z > chunklength - 1)
            {
                planet.SetBlock(new Vector3(Position.x, Position.y, Position.z + 1), (int)x, (int)y, 0, type);
            }
            else if (x < 0)
            {
                planet.SetBlock(new Vector3(Position.x - 1, Position.y, Position.z), chunklength - 1, (int)y, (int)z, type);
            }
            else if (y < 0)
            {
                planet.SetBlock(new Vector3(Position.x, Position.y - 1, Position.z), (int)x, chunklength - 1, (int)z, type);
            }
            else if (z < 0)
            {
                planet.SetBlock(new Vector3(Position.x, Position.y, Position.z - 1), (int)x, (int)y, chunklength - 1, type);
            }

            

        }




        UpdatePlanetChunk();

    }

    // Update is called once per frame
    public void UpdatePlanetChunk () {


        if (Generated == true)
        {
     
            meshData.Clear();

            for (int x = 0; x < chunklength; x++)
            {
                for (int y = 0; y < chunklength; y++)
                {
                    for (int z = 0; z < chunklength; z++)
                    {

                        if (blocks[x, y, z].type != BlockTypes.typeEmpty)
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

                        if (blocks[x, y, z].type != BlockTypes.typeEmpty)
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
        coll.sharedMesh = colmesh;
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

}
