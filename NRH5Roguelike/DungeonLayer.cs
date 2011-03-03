// Name: DungeonLayer
// Description: This is the actual 2D array that represents the semi-permanent
//              features of the dungeon, including walls, doors, floors, statues
//              and altars, etcetera. A few helper methods determine information
//              about the dungeon or can modify it in some way
// Author: Collin Reeser
// Contributors:
// Log:
// - Added the getHeight and getWidth methods for use with the pathfinder, and
//   added the 2D array itself in its most basic form
// 
// TODO:
// - Add the helper methods and whatnot to query data about the dungeon or
//   modify it
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRH5Roguelike.Dungeon
{
	class DungeonLayer
	{
        // This holds the map information for semi-permanent features of the
        // dungeon, and is what the player actually explores
        private short[][] dungeon;
        // Name: getWidth
        // Description: Returns the length of the array of the dungeon that
        //              represents the width of the dungeon, and is the part of
        //              the array indexed by the second set of brackets
        // Return: Returns the length of the arrays held by the first array of
        //         dungeon
	}
    // This is going to be the worlds most goddamn massive enum ever, Jesus. So
    // this enum describes every single tile that can exist in a dungeon, from
    // the most basic floor and wall tiles to sand floors and sand walls to
    // muddy tiles and muddy floors and whatever the hell else. Just make sure
    // that something that should be represented in the Effects layer, like
    // water, does not appear here. Another example is while we can have sand
    // tiles, if the floor is not sandy sandstone, but is instead three feet of
    // quicksand, the floor itself can be whatever but the effects layer is
    // where the quicksand behavior itself should lay. Depending on where our
    // implementation takes us, colors may be left out in favor of a different
    // system, but for now this is fine
    public static enum DungeonTiles
    {
        GREY_STONE_FLOOR , 
        GREY_STONE_WALL
    }
    // This is an equally massive string array that holds the string
    // representations for each of these possible dungeon tile features. The
    // separation is necessary so that the DungeonTiles enum refers to numbers,
    // and the DungeonTilesDesc enum refers to descriptions
    public static enum DungeonTilesDesc
    {
        GREY_STONE_FLOOR = "grey stone floor" , 
        GREY_STONE_WALL = "grey stone wall"
    }
}
