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
// - Added several accessors and modifiers for data, and added an upwards
//   reference to the list the monster is contained within. This may all be temp
//   code if a different implementation strategy is decided upon
//
// TODO:
// - Determine what system of Monster implementation we're going to go with, and
//   begin implementing it

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRH5Roguelike.Dungeon;
using libtcod;

namespace NRH5Roguelike.Entity
{
	class Monster
	{
        // Data members

        // The "name" of the monster, such that status messages print along the
        // lines of "You see here a <name>.", where <name> could be something
        // like "Goblin War Chief"
        private string name;
        // The coordinates of the monster on the dungeon layer they are on
        private short xCoord;
        private short yCoord;
        // The ASCII code that represents the monster
        private short aSCIICode;
        // The TCODColor that represents the color to print the monster's
        // ASCII code with
        private TCODColor color;
        // This is the base speed of the monster, where potions, spells, and
        // attribute gains add to or subtract from this value. LOWER IS BETTER
        private int baseSpeed;
        // This boolean, when true, signifies that the player playing the game
        // gets to choose the next move for this monster. Typically only one
        // monster has this attribute as true, but in the case of certain spells
        // this can change
        private bool isPlayer;
        // Just a constant to be used for keyboard input for monsters the plaer
        // controls
        private static TCODKey key = new TCODKey();
        // This is an upwards reference to the MonsterLayer the monster is
        // contained within
        private MonsterLayer monsterLayer;

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

        // Name: isWalkable
        // Description: Returns true if the monster, in its current state, can
        //              occupy the tile of the type passed in
        // Parameters: short tileValue , a valid value from the DungeonTiles
        //             enum
        // Returns: A bool that represents whether the monster can occupy that
        //          tile in its current state
        public bool isWalkable(short tileValue)
        {
            // For now, just make floor tiles walkable and wall tiles not
            if ( tileValue > (short)DungeonInformation.DungeonTiles.START_FLOORS
                && tileValue <
                (short)DungeonInformation.DungeonTiles.END_FLOORS )
            {
                return true;
            }
            return false;
        }

        // Name: doAction
        // Description: If this is an AI controlled monster, invoke the AI. If
        //              it is a player controlled monster, allow the player to
        //              make his action
        public void doAction()
        {
            if (!isPlayer)
            {
            }
            else
            {
                // This is temp code for keyboard input on the player's turn.
                // It most certainly does not belong here in its entirety, and
                // should probably be delegated to a dedicated class
                // ALSO THIS IMPLEMENTATION IS DISGUSTING JUST SAYING
                key = TCODConsole.waitForKeypress(true);
                if (key.KeyCode == TCODKeyCode.Down)
                {
                    yCoord = 
                        isWalkable( 
                        monsterLayer.dungeonLevel.dungeonLayer.getTile( 
                        xCoord, (short)(yCoord + 1) ) ) ? (short)(yCoord + 1) : 
                        yCoord;
                }
                else if (key.KeyCode == TCODKeyCode.Up)
                {
                    yCoord =
                        isWalkable(
                        monsterLayer.dungeonLevel.dungeonLayer.getTile(
                        xCoord, (short)(yCoord - 1))) ? (short)(yCoord - 1) :
                        yCoord;
                }
                else if (key.KeyCode == TCODKeyCode.Right)
                {
                    xCoord =
                        isWalkable(
                        monsterLayer.dungeonLevel.dungeonLayer.getTile(
                        (short)(xCoord + 1), yCoord)) ? (short)(xCoord + 1) :
                        xCoord;
                }
                else if (key.KeyCode == TCODKeyCode.Left)
                {
                    xCoord =
                        isWalkable(
                        monsterLayer.dungeonLevel.dungeonLayer.getTile(
                        (short)(xCoord - 1), yCoord )) ? (short)(xCoord - 1) :
                        xCoord;
                }
            }
        }

        // Accessors

        // Name: XCoord (accessor)
        // Description: Provides the accessor and mutator for the xCoord data
        //              member
        public short XCoord
        {
            get
            {
                return xCoord;
            }
            set
            {
                xCoord = value;
            }
        }

        // Name: YCoord (accessor)
        // Description: Provides the accessor and mutator for the yCoord data
        //              member
        public short YCoord
        {
            get
            {
                return yCoord;
            }
            set
            {
                yCoord = value;
            }
        }

        // Name: ASCIICode (accessor)
        // Description: Provides the accessor and mutator for the aSCIICode data
        //              member
        public short ASCIICode
        {
            get
            {
                return aSCIICode;
            }
            set
            {
                aSCIICode = value;
            }
        }

        // Name: Color (accessor)
        // Description: Provides the accessor and mutator for the color data
        //              member
        public TCODColor Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        // Name: IsPlayer (accessor)
        // Description: Provides the accessor and mutator for the isPlayer data
        //              member
        public bool IsPlayer
        {
            get
            {
                return isPlayer;
            }
            set
            {
                isPlayer = value;
            }
        }
        // Name: MonsterLayer (accessor)
        // Description: Provides the accessor and mutator for the MonsterLayer
        //              data member
        public MonsterLayer MonsterLayer
        {
            set
            {
                monsterLayer = value;
            }
        }
	}
}
