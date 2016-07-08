using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using CoherentNoise.Generation.Modification;
using System.Threading;

public class GeneratePlanet
{

    public BillowNoise OreGen = new BillowNoise();
    public BillowNoise CaveGen = new BillowNoise();
    public int airx;
    public int airy;
    public int airz;
    public float CoreRadius;
    public float BedrockRadius;
    public float SubstoneRadius;
    public float StoneRadius;
    public int r;
    public float txp;
    public float typ;
    public float tzp;
    public float xyz;
    public int chunklength = localVars.chunklength;
    public float value;
    public float value2;
    public BlockType core = BlockTypes.typeCore;
    public BlockType bedrock = BlockTypes.typeBedrock;
    public BlockType substone = BlockTypes.typeSubstone;
    public BlockType stone = BlockTypes.typeStone;
    public BlockType empty = BlockTypes.typeEmpty;
    public List<BlockOreType> OreTypes = new List<BlockOreType>();
    public BlockOreType ironore = BlockTypes.typeOreIron;
    public BlockOreType uraniumore = BlockTypes.typeOreUranium;
    public BlockOreType ore;
    bool Airchunk;


    public GeneratePlanet()
    {

    }


    public Block[,,] Generate(Block[,,] blocks, string PlanetType, int PlanetSize, int seed, int posx, int posy, int posz)
    {

        //if (profiled == false)
        //{
        //    Profiler.BeginSample("Gen");
        //    profiled = true;
        //}

        Airchunk = true;

        r = PlanetSize / 2;

        CoreRadius = (r - PlanetSize / 2.5f) * (r - PlanetSize / 2.5f);
        BedrockRadius = (r - PlanetSize / 3.5f) * (r - PlanetSize / 3.5f);
        SubstoneRadius = (r - PlanetSize / 5) * (r - PlanetSize / 5);
        StoneRadius = (r - PlanetSize / 6) * (r - PlanetSize / 6);

        for (int tx = 0; tx < chunklength; tx++)
        {
            txp = (tx + (posx * chunklength) - r) * (tx + (posx * chunklength) - r);

            for (int ty = 0; ty < chunklength; ty++)
            {
                typ = (ty + (posy * chunklength) - r) * (ty + (posy * chunklength) - r);

                for (int tz = 0; tz < chunklength; tz++)
                {


                    tzp = (tz + (posz * chunklength) - r) * (tz + (posz * chunklength) - r);

                    xyz = txp + typ + tzp;

                    if (xyz <= CoreRadius)
                    {
                        Airchunk = false;
                        blocks[tx, ty, tz] = new Block(core);
                    }

                    else if (xyz <= BedrockRadius)
                    {
                        Airchunk = false;
                        blocks[tx, ty, tz] = new Block(bedrock);
                    }

                    else if (xyz <= SubstoneRadius)
                    {
                        Airchunk = false;
                        blocks[tx, ty, tz] = new Block(substone);
                    }

                    else if (xyz <= StoneRadius)
                    {
                        Airchunk = false;
                        blocks[tx, ty, tz] = new Block(stone);
                    }
                    else
                    {
                        blocks[tx, ty, tz] = new Block(empty);
                    }

                }

            }

            //if (profiledone == false && profiled == true)
            //{
            //   Profiler.EndSample();
            //    profiledone = true;
            //}

        }



        if (Airchunk == false)
        {

            CaveGen.seed = seed;
            CaveGen.Frequency = 0.08f;
            CaveGen.Persistence = 0.4f;
            CaveGen.OctaveCount = 1;

            int nx = posx * chunklength;
            int ny = posy * chunklength;
            int nz = posz * chunklength;

            for (int x = 0; x < chunklength; x++)
            {
                for (int y = 0; y < chunklength; y++)
                {
                    for (int z = 0; z < chunklength; z++)
                    {

                        if (blocks[x, y, z].type != empty)
                        {

                            value = CaveGen.GetValue(new Vector3(x + nx, y + ny, z + nz));

                            if (value > 0)
                            {
                                blocks[x, y, z].type = empty;
                            }

                        }
                    }
                }
            }

            OreGen.seed = seed;
            OreGen.Frequency = 0.1f;
            OreGen.Persistence = 0.4f;
            OreGen.OctaveCount = 1;

            if (OreTypes.Contains(uraniumore) == false)
            {
                OreTypes.Add(uraniumore);
            }
            if (OreTypes.Contains(ironore) == false)
            {
                OreTypes.Add(ironore);
            }

            for (int x = 0; x < chunklength / 2; x++)
            {
                int x5 = x * 2;
                for (int y = 0; y < chunklength / 2; y++)
                {
                    int y5 = y * 2;
                    for (int z = 0; z < chunklength / 2; z++)
                    {
                        int z5 = z * 2;

                        if (blocks[x5, y5, z5].type != empty)
                        {

                            for (int i = 0; i < OreTypes.Count; i++)
                            {

                                ore = OreTypes[i];

                                if (blocks[x5, y5, z5].type == ore.BaseBlock)
                                {

                                    value2 = OreGen.GetValue(new Vector3(x5 + nx, y5 + ny, z5 + nz));

                                    if (value2 > ore.rarity)
                                    {
                                        blocks[x5, y5, z5].type = ore;

                                        if (x5 != chunklength)
                                        {
                                            blocks[x5 + 1, y5, z5].type = ore;
                                        }
                                        if (y5 != chunklength)
                                        {
                                            blocks[x5, y5 + 1, z5].type = ore;
                                        }
                                        if (z5 != chunklength)
                                        {
                                            blocks[x5, y5, z5 + 1].type = ore;
                                        }
                                        if (x5 != 0)
                                        {
                                            blocks[x5 - 1, y5, z5].type = ore;
                                        }
                                        if (y5 != 0)
                                        {
                                            blocks[x5, y5 - 1, z5].type = ore;
                                        }
                                        if (z5 != 0)
                                        {
                                            blocks[x5, y5, z5 - 1].type = ore;
                                        }
                                        ////////////////////////////////////
                                        if (x5 < chunklength - 2)
                                        {
                                            blocks[x5 + 2, y5, z5].type = ore;
                                        }
                                        if (y5 < chunklength - 2)
                                        {
                                            blocks[x5, y5 + 2, z5].type = ore;
                                        }
                                        if (z5 < chunklength - 2)
                                        {
                                            blocks[x5, y5, z5 + 2].type = ore;
                                        }
                                        if (x5 > 1)
                                        {
                                            blocks[x5 - 2, y5, z5].type = ore;
                                        }
                                        if (y5 > 1)
                                        {
                                            blocks[x5, y5 - 2, z5].type = ore;
                                        }
                                        if (z5 > 1)
                                        {
                                            blocks[x5, y5, z5 - 2].type = ore;
                                        }
                                        ////////////////////////////////////
                                        if (x5 != 0 && z5 != 0)
                                        {
                                            blocks[x5 - 1, y5, z5 - 1].type = ore;
                                        }
                                        if (x5 != 0 && y5 != 0)
                                        {
                                            blocks[x5 - 1, y5 - 1, z5].type = ore;
                                        }
                                        if (z5 != 0 && y5 != 0)
                                        {
                                            blocks[x5, y5 - 1, z5 - 1].type = ore;
                                        }
                                        if (x5 != chunklength && z5 != chunklength)
                                        {
                                            blocks[x5 + 1, y5, z5 + 1].type = ore;
                                        }
                                        if (x5 != chunklength && y5 != chunklength)
                                        {
                                            blocks[x5 + 1, y5 + 1, z5].type = ore;
                                        }
                                        if (z5 != chunklength && y5 != chunklength)
                                        {
                                            blocks[x5, y5 + 1, z5 + 1].type = ore;
                                        }
                                        ////////////////////////////////////
                                        if (x5 != 0 && y5 != 0 && z5 != 0)
                                        {
                                            blocks[x5 - 1, y5 - 1, z5 - 1].type = ore;
                                        }
                                        if (x5 != chunklength && y5 != chunklength && z5 != chunklength)
                                        {
                                            blocks[x5 + 1, y5 + 1, z5 + 1].type = ore;
                                        }
                                        if (x5 != 0 && y5 != chunklength && z5 != chunklength)
                                        {
                                            blocks[x5 - 1, y5 + 1, z5 + 1].type = ore;
                                        }
                                        if (x5 != 0 && y5 != 0 && z5 != chunklength)
                                        {
                                            blocks[x5 - 1, y5 - 1, z5 + 1].type = ore;
                                        }
                                        if (x5 != chunklength && y5 != 0 && z5 != chunklength)
                                        {
                                            blocks[x5 + 1, y5 - 1, z5 + 1].type = ore;
                                        }
                                        if (x5 != chunklength && y5 != chunklength && z5 != 0)
                                        {
                                            blocks[x5 + 1, y5 + 1, z5 - 1].type = ore;
                                        }

                                    }

                                }
                            }

                        }
                    }
                }
            }

        }
        
        return blocks;


    }


}
