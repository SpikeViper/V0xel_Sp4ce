using UnityEngine;
using System.Collections;

[System.Serializable()]

public static class BlockTypes
{
    public static BlockType typeStone;
    public static BlockType typeSubstone;
    public static BlockType typeBedrock;
    public static BlockType typeCore;
    public static BlockType typeGlass;
    public static BlockType typeEmpty;
    public static BlockOreType typeOreUranium;
    public static BlockOreType typeOreIron;

    static BlockTypes()
    {

        // creating block types:
        typeStone = new BlockType
        {
            temperature = 0,
            light = false,
            density = 400,
            BlockName = "Stone",
            tile = new Block.Tile { x = 3, y = 1 },
            IsSolid = true
        };
        typeSubstone = new BlockType
        {
            temperature = 0,
            light = false,
            density = 500,
            BlockName = "Substone",
            tile = new Block.Tile { x = 5, y = 1 },
            IsSolid = true
        };
        typeBedrock = new BlockType
        {
            temperature = 0,
            light = false,
            density = 800,
            BlockName = "Bedrock",
            tile = new Block.Tile { x = 7, y = 1 },
            IsSolid = true
        };
        typeCore = new BlockType
        {
            temperature = 1000,
            light = false,
            density = 1000,
            BlockName = "Core",
            tile = new Block.Tile { x = 1, y = 1 },
            IsSolid = true
        };
        typeGlass = new BlockType
        {
            temperature = 0,
            light = false,
            density = 20,
            BlockName = "Glass",
            tile = new Block.Tile { x = 1, y = 3 },
            IsSolid = false
        };
        typeEmpty = new BlockType
        {
            temperature = 0,
            light = false,
            density = 0,
            BlockName = "Empty",
            tile = new Block.Tile { x = 0, y = 0 },
            IsSolid = false
        };
        typeOreUranium = new BlockOreType
        {
            temperature = 0,
            light = false,
            density = 500,
            BlockName = "Uranium Ore",
            tile = new Block.Tile { x = 9, y = 1 },
            IsSolid = true,
            BaseBlock = typeSubstone,
            rarity = 0.5f
        };
        typeOreIron = new BlockOreType
        {
            temperature = 0,
            light = false,
            density = 800,
            BlockName = "Iron Ore",
            tile = new Block.Tile { x = 3, y = 3 },
            IsSolid = true,
            BaseBlock = typeBedrock,
            rarity = 0.3f
        };


    }

}
