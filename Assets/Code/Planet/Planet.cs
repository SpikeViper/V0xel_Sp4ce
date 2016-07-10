using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityThreading;





public class Planet : MonoBehaviour
{


    public GeneratePlanet Generator;

    public bool update = true;
    public string PlanetType = "Rock";
    public int Temperature;
    public int planetSize; 
    public GameObject[] players;

    public bool ChunkListReady;
    public bool fromlist;
    
    public int timer;

    public int StarDistance;

    public PlanetChunk[,,] planetchunks;
    public bool[,,] ischunkloaded;
    public int planetSeed;

    PlanetChunk script;
    PlanetChunk chunkscript;
    GameObject chunk;

    int currentLoad;

    public float loadRange = 10;

    GameObject gamenewchunk;

    float loadRangeSq;

    public List<GameObject> LoadedChunks = new List<GameObject>();
    public List<GameObject> playersnear = new List<GameObject>();

    public List<Vector4> toLoad = new List<Vector4>();

    int length;

    public int loaded;

    public string path;


    public GameObject[,,] lightsobject = new GameObject[16, 16, 16];

    public int thisx;
    public int thisy;
    public int thisz;

    float dX;
    float dY;
    float dZ;

    float distanceSq;

    public bool Generated;

    int chunklength;

    WaitForSeconds shortWait = new WaitForSeconds(0.01f);

    GameObject newchunk;




    // Use this for initialization
    void Start()
    {

        chunklength = localVars.chunklength;

        GetPlayers();

        ChunkListReady = false;

        thisx = (int)this.transform.position.x;
        thisy = (int)this.transform.position.y;
        thisz = (int)this.transform.position.z;

        loadRangeSq = loadRange * loadRange;

        planetSize = GetPlanetSize(PlanetType);
        planetchunks = new PlanetChunk[(planetSize / chunklength) + 1, (planetSize / chunklength) + 1, (planetSize / chunklength) + 1];
        length = (planetSize / chunklength) + 1;
        ischunkloaded = new bool[(planetSize / chunklength) + 1, (planetSize / chunklength) + 1, (planetSize / chunklength) + 1];

        planetSeed = GetPlanetSeed();
        path = Application.persistentDataPath + "/Planet " + planetSeed + "/Chunks/";
        StarDistance = GetDistanceFromStar();
        GetPlanetType();

        Temperature = GetPlanetTemperature(Temperature);

        Generator = new GeneratePlanet();

        for (int x = 1; x <= planetSize / chunklength; x++)
        {
            for (int y = 1; y <= planetSize / chunklength; y++)
            {
                for (int z = 1; z <= planetSize / chunklength; z++)
                {
                    ischunkloaded[x, y, z] = false;
                }
            }
        }


     

    }



    int GetPlanetSeed()
    {
        int seed = UnityEngine.Random.Range(-1000000000, 1000000000);
        return seed;
    }

    void UpdatePlanetChunks()
    {
        for (int x = 1; x <= planetSize / chunklength; x++)
        {
            for (int y = 1; y <= planetSize / chunklength; y++)
            {
                for (int z = 1; z <= planetSize / chunklength; z++)
                {
                    if (ischunkloaded[x, y, z] == true)
                    {
                        planetchunks[x, y, z].UpdatePlanetChunk();
                    }

                }
            }
        }
    }

    //TODO Figure out whats wrong here

    public void UpdateChunk(int x, int y, int z)
    {

            if (x > length || y > length || z > length || x < 1 || y < 1 || z < 1)
            {
                //Just do nothing at all.
            }
            else if (ischunkloaded[x, y, z] == true && planetchunks[x, y, z].Generated == true)
            {
                planetchunks[x, y, z].UpdatePlanetChunk();
            }
            else
            {
                LoadChunk(new Vector3(x, y, z));
            }



    }

    public BlockType GetBlock(Vector3 Chunkpos, int x, int y, int z) {

        int cx = (int)Chunkpos.x;
        int cy = (int)Chunkpos.y;
        int cz = (int)Chunkpos.z;

        if (ischunkloaded[cx, cy, cz] == true && planetchunks[cx, cy, cz].Generated == true)
        {
            return planetchunks[cx, cy, cz].blocks[x, y, z].type;
        }
        else
        {
            return BlockTypes.typeEmpty;
        }
    }


    public void SetBlock(Vector3 Chunkpos, int x, int y, int z, BlockType type)
    {

        int cx = (int)Chunkpos.x;
        int cy = (int)Chunkpos.y;
        int cz = (int)Chunkpos.z;

        if (ischunkloaded[cx, cy, cz] == false)
        {
            LoadChunk(Chunkpos);
        }
        planetchunks[cx, cy, cz].SetBlock(x, y, z, type);

    }

    public virtual string GetPlanetType(){

        int distance = StarDistance;

        // Get type from distance to star.


        return "Rock";

    }

    public int GetPlanetSize(string planettype)
    {
        int Size;


            Size = UnityEngine.Random.Range(1, 3);
            Size = Size * chunklength;

        return 512;

    }

    public int GetPlanetTemperature(int temp)
    {

        int distance = GetDistanceFromStar();

        Debug.Log("Planet Temperature not yet ready!");

        return 0;

    }

    // Updates the planet based on its contents
    public void UpdatePlanet()
    {

        GetPlayers();

    }

    public int GetDistanceFromStar()
    {


        //Get the distance from the star
        Debug.Log("Star Distance not yet ready!");

        return 0;

    }

    public void GetPlayers()
    {

        //TODO Make a master players list

        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) < loadRange)
            {
                playersnear.Add(player);
            }
            else if (playersnear.Contains(player))
            {
                playersnear.Remove(player);
            }
        }

    }

    public void Update()
    {      
        //UnloadChunks();
    }

    public void SetBlock(RaycastHit hit, BlockType type, bool adjacent = false)
    {
        PlanetChunk planetchunk = hit.collider.GetComponent<PlanetChunk>();

        Vector3 pos = GetBlockPos(hit, planetchunk, adjacent);

        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        x = Mathf.RoundToInt(x);
        y = Mathf.RoundToInt(y);
        z = Mathf.RoundToInt(z);

        planetchunk.SetBlock(x, y, z, type);
    }

    public Vector3 GetBlockPos(RaycastHit hit, PlanetChunk chunk, bool adjacent = false)
    {


        Vector3 WorldSpace = new Vector3(
        MoveWithinBlock(hit.point.x, hit.normal.x, adjacent),
        MoveWithinBlock(hit.point.y, hit.normal.y, adjacent),
        MoveWithinBlock(hit.point.z, hit.normal.z, adjacent)
        );

        WorldSpace = new Vector3(WorldSpace.x - (chunk.Position.x * chunklength) - thisx, WorldSpace.y - (chunk.Position.y * chunklength) - thisy, WorldSpace.z - (chunk.Position.z * chunklength) - thisz);

        return WorldSpace;



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


    public void LoadChunk(Vector3 chunkpos)
    {
        using (gstring.Block())
        {

            if (ischunkloaded[(int)chunkpos.x, (int)chunkpos.y, (int)chunkpos.z] == false)
            {

                if (localVars.ChunkPool.Pool.Count == 0)
                {
                    newchunk = (GameObject)Instantiate(localVars.chunkprefab, new Vector3(thisx + (chunkpos.x * chunklength), thisy + (chunkpos.y * chunklength), thisz + (chunkpos.z * chunklength)), this.transform.rotation);
                    fromlist = false;
                }
                else
                {
                    newchunk = localVars.ChunkPool.Pool[0];
                    localVars.ChunkPool.Pool.Remove(newchunk);
                    fromlist = true;
                    newchunk.transform.position = new Vector3(thisx + (chunkpos.x * chunklength), thisy + (chunkpos.y * chunklength), thisz + (chunkpos.z * chunklength));
                }




                gstring chunkpath = gstring.Concat(chunkpos.x, " ", chunkpos.y, " ", chunkpos.z);
                newchunk.name = chunkpath;
                gamenewchunk = newchunk;
                gamenewchunk.transform.parent = this.gameObject.transform;
                script = gamenewchunk.GetComponent<PlanetChunk>();
                script.planet = this;
                script.Position = chunkpos;
                script.StarDistance = this.StarDistance;
                script.PlanetType = this.PlanetType;
                script.Temperature = this.Temperature;
                script.lights = lightsobject;

                if (System.IO.File.Exists(path + chunkpath) == true)
                {
                    loadfromfile(chunkpos, script);
                }
                else
                {
                    StartCoroutine(script.Generate());
                }

                planetchunks[(int)chunkpos.x, (int)chunkpos.y, (int)chunkpos.z] = script;
                ischunkloaded[(int)chunkpos.x, (int)chunkpos.y, (int)chunkpos.z] = true;
                LoadedChunks.Add(script.gameObject);

                if (fromlist == true)
                {
                    gamenewchunk.SetActive(true);
                }

            }



        }
        

    }

    public void UnloadChunks()
    {
        foreach (GameObject Chunk in LoadedChunks.ToArray())
        {
            foreach (GameObject player in playersnear.ToArray())
            {

                int playerx = (int)(player.transform.position.x);
                int playery = (int)(player.transform.position.y);
                int playerz = (int)(player.transform.position.z);
                float dX = Chunk.transform.position.x - playerx;
                float dY = Chunk.transform.position.y - playery;
                float dZ = Chunk.transform.position.z - playerz;
                float distanceSq = dX * dX + dY * dY + dZ * dZ;


                if (distanceSq > loadRange * loadRange)
                {
                    UnloadChunk(Chunk.GetComponent<PlanetChunk>());
                }


            }

        }
    }

    public void UnloadChunk(PlanetChunk chunk)
    {

        if (chunk != null)
        {

            if (chunk.modified == true)
            {
                BinaryFormatter bf = new BinaryFormatter();

                Block[,,] data = chunk.blocks;

                if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Planet " + planetSeed + "/Chunks/"))
                {
                    System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Planet " + planetSeed + "/Chunks/");
                }
                else
                {
                    Debug.Log("Directory found @ " + Application.persistentDataPath + "/Planet " + planetSeed + "/Chunks/");
                }
                FileStream file = File.Create(Application.persistentDataPath + "/Planet " + planetSeed + "/Chunks/" + chunk.Position.x + " " + chunk.Position.y + " " + chunk.Position.z);
                bf.Serialize(file, data);
                file.Close();

            }

            planetchunks[(int)chunk.Position.x, (int)chunk.Position.y, (int)chunk.Position.z] = null;
            ischunkloaded[(int)chunk.Position.x, (int)chunk.Position.y, (int)chunk.Position.z] = false;
            LoadedChunks.Remove(chunk.gameObject);
            localVars.ChunkPool.Pool.Add(chunk.gameObject);
            chunk.gameObject.SetActive(false);

        }

    }

    public void loadfromfile (Vector3 chunkpos, PlanetChunk script)
    {

        gstring chunkpath = chunkpos.x + " " + chunkpos.y + " " + chunkpos.z;

        script.meshData = new MeshData();
        script.coll = script.gameObject.GetComponent<MeshCollider>();
        script.filter = script.gameObject.GetComponent<MeshFilter>();

        Debug.Log("Loading block data for chunk: " + chunkpath);

        Block[,,] data = new Block[chunklength, chunklength, chunklength];

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path + chunkpath, FileMode.Open);
        data = (Block[,,])bf.Deserialize(file);
        file.Close();
        script.blocks = data;
        script.Generated = true;

    }
}