using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uv;
    public Color32[,,] lightcolors;
    public float[,,] lightintensity;
    public float[,,] lightrange;
    public ResizeableArray<Vector3> lightpositions;
    public Vector2[] UVs = new Vector2[4];

    public enum Direction { north, east, south, west, up, down };

    public float tileSize = localVars.tileSize;

    public int vertcount = 0;
    public int tricount = 0;
    public int uvcount = 0;

    public int verts = 0;
    public int tris = 0;
    public int uvs = 0;

    Vector3 vert0;
    Vector3 vert1;
    Vector3 vert2;
    Vector3 vert3;

    int v0;
    int v1;
    int v2;
    int v3;

    public bool light;

    public bool useRenderDataForCol;

    public void Light(bool val)
    {
        if (val == true)
        {
            lightpositions = new ResizeableArray<Vector3>(localVars.chunklength * localVars.chunklength * localVars.chunklength); //Fix this later
            lightcolors = new Color32[localVars.chunklength, localVars.chunklength, localVars.chunklength];
            lightintensity = new float[localVars.chunklength, localVars.chunklength, localVars.chunklength];
            lightrange = new float[localVars.chunklength, localVars.chunklength, localVars.chunklength];
            light = true;
        }
    }

    public void AddFaceCount()
    {
        vertcount = vertcount + 4;
        uvcount = uvcount + 4;
        tricount = tricount + 6;
    }
    public void Rebuild()
    {
        vertices = new List<Vector3>(vertcount);
        triangles = new List<int>(tricount);
        uv = new List<Vector2>(uvcount);
    }

    public void AddLight(int x, int y, int z, byte R, byte G, byte B, byte A, float range, float intensity)
    {
        lightcolors[x, y, z] = new Color32(R, G, B, A);
        lightintensity[x, y, z] = intensity;
        lightrange[x, y, z] = range;
        lightpositions.Add(new Vector3(x, y, z));
    }

    public void Clear()
    {

        vertcount = 0;
        uvcount = 0;
        tricount = 0;
        tris = 0;
        verts = 0;
        uvs = 0;

        if (light == true)
        {
            lightpositions.Clear();
            light = false;
        }

    }

    public void AddUVRange(Vector2[] vect2s)
    {
        uv.Add(vect2s[0]);
        uv.Add(vect2s[1]);
        uv.Add(vect2s[2]);
        uv.Add(vect2s[3]);

    }

    public void BuildSideUp(int x, int y, int z, BlockType type)
    {

        vert1 = new Vector3(x + -0.5f, y + 0.5f, z + 0.5f);
        vertices.Add(vert1);
        verts++;
        v1 = verts - 1;

        vert2 = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        vertices.Add(vert2);
        verts++;
        v2 = verts - 1;

        vert3 = new Vector3(x + 0.5f, y + 0.5f, z + -0.5f);
        vertices.Add(vert3);
        verts++;
        v3 = verts - 1;

        vert0 = new Vector3(x + -0.5f, y + 0.5f, z + -0.5f);
        vertices.Add(vert0);
        verts++;
        v0 = verts - 1;

        triangles.Add(v3);
        triangles.Add(v0);
        triangles.Add(v1);

        triangles.Add(v1);
        triangles.Add(v2);
        triangles.Add(v3);

        AddUVRange(FaceUVs(Direction.up, type));

    }

    public void BuildSideDown(int x, int y, int z, BlockType type)
    {

        vert0 = new Vector3(x + -0.5f, y + -0.5f, z + -0.5f);
        vertices.Add(vert0);
        verts++;
        v0 = verts - 1;

        vert1 = new Vector3(x + -0.5f, y + -0.5f, z + 0.5f);
        vertices.Add(vert1);
        verts++;
        v1 = verts - 1;

        vert2 = new Vector3(x + 0.5f, y + -0.5f, z + 0.5f);
        vertices.Add(vert2);
        verts++;
        v2 = verts - 1;

        vert3 = new Vector3(x + 0.5f, y + -0.5f, z + -0.5f);
        vertices.Add(vert3);
        verts++;
        v3 = verts - 1;

        triangles.Add(v1);
        triangles.Add(v0);
        triangles.Add(v3);

        triangles.Add(v1);
        triangles.Add(v3);
        triangles.Add(v2);

        AddUVRange(FaceUVs(Direction.down, type));

    }


    public void BuildSideEast(int x, int y, int z, BlockType type)
    {

        vert0 = new Vector3(x + 0.5f, y + 0.5f, z + -0.5f);
        vertices.Add(vert0);
        verts++;
        v0 = verts - 1;

        vert1 = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        vertices.Add(vert1);
        verts++;
        v1 = verts - 1;

        vert2 = new Vector3(x + 0.5f, y + -0.5f, z + 0.5f);
        vertices.Add(vert2);
        verts++;
        v2 = verts - 1;

        vert3 = new Vector3(x + 0.5f, y + -0.5f, z + -0.5f);
        vertices.Add(vert3);
        verts++;
        v3 = verts - 1;

        triangles.Add(v0);
        triangles.Add(v1);
        triangles.Add(v3);

        triangles.Add(v3);
        triangles.Add(v1);
        triangles.Add(v2);

        AddUVRange(FaceUVs(Direction.west, type));

    }

    public void BuildSideWest(int x, int y, int z, BlockType type)
    {

        vert0 = new Vector3(x + -0.5f, y + 0.5f, z + 0.5f);
        vertices.Add(vert0);
        verts++;
        v0 = verts - 1;

        vert1 = new Vector3(x + -0.5f, y + 0.5f, z + -0.5f);
        vertices.Add(vert1);
        verts++;
        v1 = verts - 1;

        vert2 = new Vector3(x + -0.5f, y + -0.5f, z + -0.5f);
        vertices.Add(vert2);
        verts++;
        v2 = verts - 1;

        vert3 = new Vector3(x + -0.5f, y + -0.5f, z + 0.5f);
        vertices.Add(vert3);
        verts++;
        v3 = verts - 1;

        triangles.Add(v0);
        triangles.Add(v1);
        triangles.Add(v2);

        triangles.Add(v0);
        triangles.Add(v2);
        triangles.Add(v3);

        AddUVRange(FaceUVs(Direction.east, type));

    }

    public void BuildSideNorth(int x, int y, int z, BlockType type)
    {

        vert0 = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        vertices.Add(vert0);
        verts++;
        v0 = verts - 1;

        vert1 = new Vector3(x + -0.5f, y + 0.5f, z + 0.5f);
        vertices.Add(vert1);
        verts++;
        v1 = verts - 1;

        vert2 = new Vector3(x + -0.5f, y + -0.5f, z + 0.5f);
        vertices.Add(vert2);
        verts++;
        v2 = verts - 1;

        vert3 = new Vector3(x + 0.5f, y + -0.5f, z + 0.5f);
        vertices.Add(vert3);
        verts++;
        v3 = verts - 1;

        triangles.Add(v0);
        triangles.Add(v1);
        triangles.Add(v3);

        triangles.Add(v3);
        triangles.Add(v1);
        triangles.Add(v2);

        AddUVRange(FaceUVs(Direction.north, type));

    }

    public void BuildSideSouth(int x, int y, int z, BlockType type)
    {

        vert0 = new Vector3(x + -0.5f, y + 0.5f, z + -0.5f);
        vertices.Add(vert0);
        verts++;
        v0 = verts - 1;

        vert1 = new Vector3(x + 0.5f, y + 0.5f, z + -0.5f);
        vertices.Add(vert1);
        verts++;
        v1 = verts - 1;

        vert2 = new Vector3(x + 0.5f, y + -0.5f, z + -0.5f);
        vertices.Add(vert2);
        verts++;
        v2 = verts - 1;

        vert3 = new Vector3(x + -0.5f, y + -0.5f, z + -0.5f);
        vertices.Add(vert3);
        verts++;
        v3 = verts - 1;

        triangles.Add(v3);
        triangles.Add(v0);
        triangles.Add(v1);

        triangles.Add(v3);
        triangles.Add(v1);
        triangles.Add(v2);

        AddUVRange(FaceUVs(Direction.south, type));

    }

    public Vector2[] FaceUVs(Direction direction, BlockType type)
    {
        Block.Tile tilePos = type.tile;

        UVs[0] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y);
        UVs[1] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y + tileSize);
        UVs[2] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y + tileSize);
        UVs[3] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y);

        return UVs;
    }

    /* This is before I made it optimized and confusing.

    public void BuildSideUp(int x, int y, int z, BlockType type)
    {
        int v0;
        int v1;
        int v2;
        int v3;


        Vector3 vert1 = new Vector3(x + -0.5f, y + 0.5f, z + 0.5f);
        vertices[verts++] = vert1;
        v1 = verts - 1;

        Vector3 vert2 = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        vertices[verts++] = vert2;
        v2 = verts - 1;

        Vector3 vert3 = new Vector3(x + 0.5f, y + 0.5f, z + -0.5f);
        vertices[verts++] = vert3;
        v3 = verts - 1;

        Vector3 vert0 = new Vector3(x + -0.5f, y + 0.5f, z + -0.5f);
        vertices[verts++] = vert0;
        v0 = verts - 1;

        triangles[tris++] = v3;
        triangles[tris++] = v0;
        triangles[tris++] = v1;

        triangles[tris++] = v1;
        triangles[tris++] = v2;
        triangles[tris++] = v3;

        AddUVRange(FaceUVs(Direction.up, type));

    }

    public void BuildSideDown(int x, int y, int z, BlockType type)
    {
        int v4;
        int v5;
        int v6;
        int v7;

        Vector3 vert4 = new Vector3(x + -0.5f, y + -0.5f, z + -0.5f);
        vertices[verts++] = vert4;
        v4 = verts - 1;

        Vector3 vert5 = new Vector3(x + -0.5f, y + -0.5f, z + 0.5f);
        vertices[verts++] = vert5;
        v5 = verts - 1;

        Vector3 vert6 = new Vector3(x + 0.5f, y + -0.5f, z + 0.5f);
        vertices[verts++] = vert6;
        v6 = verts - 1;

        Vector3 vert7 = new Vector3(x + 0.5f, y + -0.5f, z + -0.5f);
        vertices[verts++] = vert7;
        v7 = verts - 1;

        triangles[tris++] = v5;
        triangles[tris++] = v4;
        triangles[tris++] = v7;

        triangles[tris++] = v5;
        triangles[tris++] = v7;
        triangles[tris++] = v6;

        AddUVRange(FaceUVs(Direction.down, type));

    }


    public void BuildSideEast(int x, int y, int z, BlockType type)
    {
        int v2;
        int v3;
        int v6;
        int v7;

        Vector3 vert3 = new Vector3(x + 0.5f, y + 0.5f, z + -0.5f);
        vertices[verts++] = vert3;
        v3 = verts - 1;

        Vector3 vert2 = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        vertices[verts++] = vert2;
        v2 = verts - 1;

        Vector3 vert6 = new Vector3(x + 0.5f, y + -0.5f, z + 0.5f);
        vertices[verts++] = vert6;
        v6 = verts - 1;

        Vector3 vert7 = new Vector3(x + 0.5f, y + -0.5f, z + -0.5f);
        vertices[verts++] = vert7;
        v7 = verts - 1;

        triangles[tris++] = v3;
        triangles[tris++] = v2;
        triangles[tris++] = v7;

        triangles[tris++] = v7;
        triangles[tris++] = v2;
        triangles[tris++] = v6;

        AddUVRange(FaceUVs(Direction.west, type));

    }

    public void BuildSideWest(int x, int y, int z, BlockType type)
    {
        int v0;
        int v1;
        int v4;
        int v5;

        Vector3 vert1 = new Vector3(x + -0.5f, y + 0.5f, z + 0.5f);
        vertices[verts++] = vert1;
        v1 = verts - 1;

        Vector3 vert0 = new Vector3(x + -0.5f, y + 0.5f, z + -0.5f);
        vertices[verts++] = vert0;
        v0 = verts - 1;

        Vector3 vert4 = new Vector3(x + -0.5f, y + -0.5f, z + -0.5f);
        vertices[verts++] = vert4;
        v4 = verts - 1;

        Vector3 vert5 = new Vector3(x + -0.5f, y + -0.5f, z + 0.5f);
        vertices[verts++] = vert5;
        v5 = verts - 1;

        triangles[tris++] = v1;
        triangles[tris++] = v0;
        triangles[tris++] = v4;

        triangles[tris++] = v1;
        triangles[tris++] = v4;
        triangles[tris++] = v5;

        AddUVRange(FaceUVs(Direction.east, type));

    }

    public void BuildSideNorth(int x, int y, int z, BlockType type)
    {
        int v1;
        int v2;
        int v5;
        int v6;

        Vector3 vert2 = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        vertices[verts++] = vert2;
        v2 = verts - 1;

        Vector3 vert1 = new Vector3(x + -0.5f, y + 0.5f, z + 0.5f);
        vertices[verts++] = vert1;
        v1 = verts - 1;

        Vector3 vert5 = new Vector3(x + -0.5f, y + -0.5f, z + 0.5f);
        vertices[verts++] = vert5;
        v5 = verts - 1;

        Vector3 vert6 = new Vector3(x + 0.5f, y + -0.5f, z + 0.5f);
        vertices[verts++] = vert6;
        v6 = verts - 1;

        triangles[tris++] = v2;
        triangles[tris++] = v1;
        triangles[tris++] = v6;

        triangles[tris++] = v6;
        triangles[tris++] = v1;
        triangles[tris++] = v5;

        AddUVRange(FaceUVs(Direction.north, type));

    }

    public void BuildSideSouth(int x, int y, int z, BlockType type)
    {
        int v0;
        int v3;
        int v4;
        int v7;

        Vector3 vert0 = new Vector3(x + -0.5f, y + 0.5f, z + -0.5f);
        vertices[verts++] = vert0;
        v0 = verts - 1;

        Vector3 vert3 = new Vector3(x + 0.5f, y + 0.5f, z + -0.5f);
        vertices[verts++] = vert3;
        v3 = verts - 1;

        Vector3 vert7 = new Vector3(x + 0.5f, y + -0.5f, z + -0.5f);
        vertices[verts++] = vert7;
        v7 = verts - 1;

        Vector3 vert4 = new Vector3(x + -0.5f, y + -0.5f, z + -0.5f);
        vertices[verts++] = vert4;
        v4 = verts - 1;

        triangles[tris++] = v4;
        triangles[tris++] = v0;
        triangles[tris++] = v3;

        triangles[tris++] = v4;
        triangles[tris++] = v3;
        triangles[tris++] = v7;

        AddUVRange(FaceUVs(Direction.south, type));

    }
    */


}