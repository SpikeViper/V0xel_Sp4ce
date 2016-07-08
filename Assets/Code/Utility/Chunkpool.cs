using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunkpool : MonoBehaviour {

    public ResizeableArray<GameObject> Pool = new ResizeableArray<GameObject>(localVars.poolSize);
    public GameObject chunk;
    public PlanetChunk script;

    // Use this for initialization
    public void Start () { 

        localVars.chunkprefab = (GameObject)Resources.Load("Prefabs/PlanetChunkObject");
        localVars.ChunkPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<Chunkpool>();

        for (int i = 0; i < localVars.poolSize; i++)
        {
            chunk = Instantiate(localVars.chunkprefab);
            script = chunk.GetComponent<PlanetChunk>();
            script.meshData = new MeshData();
            chunk.SetActive(false);
            Pool.Add(chunk);
        }

        Debug.Log(Pool.Count);

    }
	
}
