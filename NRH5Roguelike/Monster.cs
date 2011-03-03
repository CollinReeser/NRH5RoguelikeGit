// Name: Monster
// Description: Although implementation details are as of yet undecided, this
//              class will act either as a superclass for tens or hundreds of
//              individually implemented subclass Monsters, or it will act as a
//              base for some clever, modularly implemented system for monsters
// Author: Collin Reeser
// Contributors:
// Log:
// - Did some documentation and laid a skeleton for some methods needed by the
//   pathfinder implementation
//
// TODO:
// - Determine what system of Monster implementation we're going to go with, and
//   begin implementing it

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRH5Roguelike.Entity
{
	class Monster
	{
        // Data members

        // The "name" of the monster, such that status messages print along the
        // lines of "You see here a <name>.", where <name> could be something
        // like "Goblin War Chief"
        string name;
        // This is the base speed of the monster, where potions, spells, and
        // attribute gains add to or subtract from this value. LOWER IS BETTER
        int baseSpeed;
        // This boolean, when true, signifies that the player playing the game
        // gets to choose the next move for this monster. Typically only one
        // monster has this attribute as true, but in the case of certain spells
        // this can change
        bool isPlayer;

        // Queries

        // Name: getTileWeight
        // Description: This method takes an integer that represents what kind 
        //              of dungeon tile is being referred to, and it returns a 
        //              value that represents the "difficulty", or weight, of
        //              traversing a tile of that terrain. 10 is default, lower
        //              is better
        // Parameters: short tileType , the terrain type that is being queried
        // Returns: a short that represents the tile weight
        public short getTileWeight(short tileType)
        {
            const short DEFAULT_WEIGHT = 10;
            // This is temp code for getting the pathfinder working
            return DEFAULT_WEIGHT;
        }
	}
}
