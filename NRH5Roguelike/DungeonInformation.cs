// Name: DungeonInformation
// Description: Contains an enum that refers to every tile type that exists,
//              and a string array that maps to the enum, where each string in
//              the array is a descriptor for that tile type
// Author: Collin Reeser
// Contributors:
// Log:
// - Started the enum and string array
// - Changed the enum to shorts to save 50% of the memory used (default is int).
//   In addition, modified the way the enum can be used in order to improve
//   efficiency and readability and usability
//
// TODO:
// - Completely fill the enum and array, and ensure they match up
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRH5Roguelike.Dungeon
{
	static class DungeonInformation
	{
        // This is going to be the worlds most goddamn massive enum ever, Jesus.
        // So this enum describes every single tile that can exist in a dungeon,
        // from the most basic floor and wall tiles to sand floors and sand 
        // walls to muddy tiles and muddy floors and whatever the hell else. 
        // Just make sure that something that should be represented in the 
        // Effects layer, like water, does not appear here. Another example is 
        // while we can have sand tiles, if the floor is not sandy sandstone, 
        // but is instead three feet of quicksand, the floor itself can be 
        // whatever but the effects layer is where the quicksand behavior itself
        // should lay. Depending on where our implementation takes us, colors 
        // may be left out in favor of a different system, but for now this is 
        // fine. Try to keep tiles that will have the same ASCII code next to
        // eachother so that implementing getASCIICode will be easier. Walls all
        // next to eachother, similar floor tiles next to eachother, altars next
        // to eachother, etcetera. Keep all similar tiles within the confines of
        // the start and end descriptors for their type. That way, referring to
        // ALL walls or ALL floors, etcetera, is a matter of confining a value
        // to x > start && x < end
        public enum DungeonTiles :short
        {
            START_WALLS ,
            SUPER_STONE ,
            GREY_STONE_WALL ,
            ACID_WALL,
            END_WALLS ,
            START_FLOORS ,
            GREY_STONE_FLOOR ,
            SAND_FLOOR,
            CLOUD_FLOOR,
            END_FLOORS
        }
        // This is a massive string[] that holds all of the the string
        // representations for each of these possible dungeon tile features. The
        // separation is necessary so that the DungeonTiles enum refers to 
        // numbers, and the DungeonTilesDesc array refers to descriptions
        public static readonly string[] DungeonTilesDesc = new string[]
        {
            "START_WALLS" ,
            "a very strong stone wall" ,
            "a grey stone wall" ,
            "a wall covered in acid, dangerous to touch",
            "END_WALLS" ,
            "START_FLOORS" ,
            "a grey stone floor" ,
            "a sandy floor",
            "a floor that looks so soft you could sleep forever",
            "END_FLOORS"
            
        };

        // Name: getASCIICode
        // Description: This method takes a value valid in the enum DungeonTiles
        //              and returns the ASCII code that represents that tile
        // Parameters: A short valid in the DungeonTiles enum
        // Returns: A char that is the ASCII code representation of the enum val
        public static char getASCIICode(short dungeonTilesValue)
        {
            if (dungeonTilesValue > (short) DungeonTiles.START_WALLS &&
                dungeonTilesValue < (short) DungeonTiles.END_WALLS)
            {
                // Return the wall character, octothorp
                return '#';
            }
            else if (dungeonTilesValue > (short)DungeonTiles.START_FLOORS &&
                dungeonTilesValue < (short)DungeonTiles.END_FLOORS)
            {
                // Up for argument what the default floor tile will be, but for
                // now it is period
                return '.';
            }
            // The "default" is '?', which should only be displayed when this
            // method messes up, in that we didn't account for some enum values
            // in here. That or code somewhere else messed up. In any case, '?'
            // shouldn't be used as a normal tile symbol
            else
            {
                return '?';
            }
        }
	}
}
