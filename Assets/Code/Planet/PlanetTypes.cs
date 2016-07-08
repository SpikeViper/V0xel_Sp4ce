using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlanetTypes {

    public static PlanetType typeRock;

    static PlanetTypes()
    {
        typeRock = new PlanetType
        {
            typename = "Rock",

            life = false,

            genblocks = new List<BlockType>()
            {
                BlockTypes.typeCore,
                BlockTypes.typeStone,
                BlockTypes.typeEmpty,
                BlockTypes.typeBedrock
            },

            genores = new List<BlockType>()
            {
                BlockTypes.typeOreIron,
                BlockTypes.typeOreUranium
            }

        };


    }
	
}
