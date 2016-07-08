using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class localVars {

    public static int chunklength = 16; //As of now, this CANNOT BE CHANGED. This is legacy only. You have been warned.
    public static int poolSize = 5000;
    public static GameObject chunkprefab;
    public static Chunkpool ChunkPool;
    public const float tileSize = 0.0625f;

}
