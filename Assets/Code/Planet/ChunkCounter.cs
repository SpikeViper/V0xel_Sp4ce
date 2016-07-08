using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChunkCounter : MonoBehaviour {

    public Text number;
    public Planet planet;
    public GameObject planetObject;

    void Start()
    {
        planet = planetObject.GetComponent<Planet>();
    }
	
	// Update is called once per frame
	void Update () {

        number.text = planet.LoadedChunks.Count.ToString();

    }
}
