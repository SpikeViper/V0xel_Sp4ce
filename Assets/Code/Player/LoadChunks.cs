using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable()]

public class LoadChunks : MonoBehaviour {

    public Planet planet;
    public bool finished = true;
    public List<Vector4> LoadList = new List<Vector4>(4096);

    public float planetx;
    public float planety;
    public float planetz;

    public SortIntDescending sort = new SortIntDescending();


    public float x;
    public float y;
    public float z;

    float x2;
    float y2;
    float z2;

    float x3;
    float y3;
    float z3;

    float dis;
    public float lastx = 1;
    public float lasty = 1;
    public float lastz = 1;

    public bool Generate;

    public float length;

    public float Range = 1;
    public float maxRange = 8;

    public int loadamount = 0;

    public int it;

    // Update is called once per frame
    public void Update () {

        if (Generate == true)
        {

            if (planet == null)
            {
                GetPlanet();
                Vector3 pos = planet.transform.position;
                planetx = pos.x;
                planety = pos.y;
                planetz = pos.z;
                length = planet.planetSize / 16;
            }

            //Getting player position
            x2 = this.transform.position.x;
            y2 = this.transform.position.y;
            z2 = this.transform.position.z;

            //Check if the player has moved, skip a few calculations if they didn't
            if (x2 != lastx || y2 != lasty || z2 != lastz || LoadList.Count == 0)
            {


                if (LoadList.Count == 0)
                {
                    Range = Range + 1;
                }

                if (x2 != lastx || y2 != lasty || z2 != lastz)
                {

                    lastx = x;
                    lasty = y;
                    lastz = z;

                    //Normalizing position with planet position
                    x3 = x2 - planetx;
                    y3 = y2 - planety;
                    z3 = z2 - planetz;

                    //Converts into chunk position
                    x = Mathf.FloorToInt(x3 / (float)localVars.chunklength);
                    y = Mathf.FloorToInt(y3 / (float)localVars.chunklength);
                    z = Mathf.FloorToInt(z3 / (float)localVars.chunklength);

                }

                if (Range < maxRange)
                {
                    LoadList.Clear();


                    for (int x1 = (int)(x - Range); x1 <= x + Range; x1++)
                    {
                        if (x1 >= 0 && x1 < length)
                        {
                            for (int y1 = (int)(y - Range); y1 <= y + Range; y1++)
                            {
                                if (y1 >= 0 && y1 < length)
                                {
                                    for (int z1 = (int)(z - Range); z1 <= z + Range; z1++)
                                    {
                                        if (z1 >= 0 && z1 < length)
                                        {
                                            dis = (x - x1) * (x - x1) + (y - y1) * (y - y1) + (z - z1) * (z - z1);

                                            if (planet.ischunkloaded[x1, y1, z1] != true)
                                            {
                                                LoadList.Add(new Vector4(x1, y1, z1, dis));
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }

                    LoadList.Sort(sort);
                }

            }

            if (Range > maxRange)
            {
                Range = 1;
            }


            if (LoadList.Count > 0)
            {
                Vector3 newchunk = LoadList[0];
                LoadList.RemoveAt(0);
                planet.LoadChunk(new Vector3(newchunk.x, newchunk.y, newchunk.z));
            }



        }
        
	}


    void GetPlanet()
    {
        //TODO: Make this get closest planet once multi-planet functionality is added
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Planet>();
    }

    public class SortIntDescending : IComparer<Vector4>
    {
        int IComparer<Vector4>.Compare(Vector4 a, Vector4 b) //implement Compare
        {
            if (a.w > b.w)
                return 1; //normally greater than = 1
            if (a.w < b.w)
                return -1; // normally smaller than = -1
            else
                return 0; // equal
        }
    }

}
