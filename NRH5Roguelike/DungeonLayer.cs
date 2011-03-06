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
// - Added a couple accessors and a couple query methods, in the process of
//   slowly implementing this class
// 
// TODO:
// - Continue to implement the class
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;

namespace NRH5Roguelike.Dungeon
{
	class DungeonLayer
	{
        // Default values for dungeon size
        private static readonly short DUNGEON_HEIGHT = 55;
        private static readonly short DUNGEON_WIDTH = 80;
        // This holds the map information for semi-permanent features of the
        // dungeon, and is what the player actually explores
        private short[,] dungeon;
        // This is used for printing to the game screen
        private readonly TCODConsole console;

        // Name: DungeonLayer (constructor)
        // Description: Creates an instance of the DungeonLayer class
        public DungeonLayer( TCODConsole console )
        {
            this.console = console;
            dungeon = new short[DUNGEON_HEIGHT , DUNGEON_WIDTH];
            // Create an empty dungeon
            this.fillDefaultFloor();
            this.injectPerimiter();
        }

        // Name: getHeight
        // Description: Returns the length of the array of the dungeon that
        //              represents the height of the dungeon, and is the part of
        //              the array indexed by the first set of brackets
        // Return: int , returns the length of the first array of the dungeon
        public int getHeight()
        {
            return dungeon.GetLength(0);
        }

        // Name: getWidth
        // Description: Returns the length of the array of the dungeon that
        //              represents the width of the dungeon, and is the part of
        //              the array indexed by the second set of brackets
        // Return: int , returns the length of the arrays held by the first 
        //         array of dungeon
        public int getWidth()
        {
            return dungeon.GetLength(1);
        }

        // Name: getTile
        // Descrption: Gets the tile at the provided x and y coordinates
        // Parameters: short xCoord , the x coordinate of the desired tile
        //             short yCoord , the y coordinate of the desired tile
        // Returns: The DungeonTiles enum vale for the tile at those coords
        public short getTile(short xCoord, short yCoord)
        {
            return dungeon[yCoord, xCoord];
        }

        // Name: fillDefaultFloor
        // Description: Fills the entire array minus the area that will be
        //              filled by injectPerimeter with the default flooring
        private void fillDefaultFloor()
        {
            for (int yCoord = 0; yCoord < dungeon.GetLength(0); yCoord++)
            {
                for (int xCoord = 0; xCoord < dungeon.GetLength(1); xCoord++)
                {
                    dungeon[yCoord, xCoord] = 
                        (short)DungeonInformation.DungeonTiles.GREY_STONE_FLOOR;
                }
            }
        }

        // Name: injectPerimeter
        // Description: Adds a perimeter of superstone (impassable under any
        //              and all circumstances) around the edges of the map
        private void injectPerimiter()
        {
            // Left hand perimeter
            for ( int yCoord = 0; yCoord < dungeon.GetLength(0); yCoord++ )
            {
                dungeon[yCoord , 0] = 
                    (short) DungeonInformation.DungeonTiles.SUPER_STONE;
            }
            // Right hand perimeter
            for (int yCoord = 0; yCoord < dungeon.GetLength(0); yCoord++)
            {
                dungeon[yCoord, dungeon.GetLength(1) - 1] =
                    (short)DungeonInformation.DungeonTiles.SUPER_STONE;
            }
            // Upper perimeter. These two start at 1 and end at just before the
            // last index because the previous two fors already filled those
            // spots with superstone, so this is an eensy optimization
            for (int xCoord = 1; xCoord < dungeon.GetLength(1) - 1; xCoord++)
            {
                dungeon[0, xCoord] =
                    (short)DungeonInformation.DungeonTiles.SUPER_STONE;
            }
            // Lower perimeter
            for (int xCoord = 1; xCoord < dungeon.GetLength(1) - 1; xCoord++)
            {
                dungeon[dungeon.GetLength(0) - 1, xCoord] =
                    (short)DungeonInformation.DungeonTiles.SUPER_STONE;
            }
        }

        // Name: printToScreen
        // Description: Prints the dungeon to the screen.
        //              For now: Displays the entire dungeon to the entire
        //              screen, assuming there is enough room
        //              In the future: Should account for the OnceSeen Layer,
        //              and only display to the game area of the game screen
        public void printToScreen()
        {
            for (int yCoord = 0; yCoord < dungeon.GetLength(0) &&
                yCoord < console.getHeight(); yCoord++)
            {
                for (int xCoord = 0; xCoord < dungeon.GetLength(1) &&
                xCoord < console.getWidth(); xCoord++)
                {
                    console.putCharEx(xCoord , yCoord ,
                        DungeonInformation.getASCIICode(
                        dungeon[yCoord, xCoord] ) ,
                        TCODColor.white, TCODColor.black );
                }
            }
        }

        // Name: printMapToConsole
        // Description: Prints the map to the console
        public void printMapToConsole()
        {
            for (int yCoord = 0; yCoord < dungeon.GetLength(0); yCoord++)
            {
                for (int xCoord = 0; xCoord < dungeon.GetLength(1); xCoord++)
                {
                    Console.Write( 
                        DungeonInformation.getASCIICode( 
                        dungeon[yCoord, xCoord] ) );
                }
                Console.WriteLine();
            }
        }
	}
}
