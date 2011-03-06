// Name: MonsterLayer
// Description: This is a wrapper for a linked list of Monsters, or at least
//              is the implementation for now until a better system is
//              developed, and will have several methods that operate on the
//              information contained therein
// Author: Collin Reeser
// Contributors:
// Log:
// - Added a simple linked list implementation of the MonsterLayer, and
//   implemented the printToScreen() method
// - Added a couple helper methods to begin implementing the purpose of the
//   MonsterLayer
//
// TODO:
// - Heavily consider implemenation strategy. Implement it

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRH5Roguelike.Entity;
using libtcod;

namespace NRH5Roguelike.Dungeon
{
	class MonsterLayer
	{
        // This is the collection that holds the Monsters. This implementation
        // of the MonsterLayer may be temporary
        private List<Monster> monsterLayer;
        // This is the reference to the root console that will be used for
        // printing purposes
        private readonly TCODConsole console;
        // This is an upwards reference to the DungeonLevel the monster layer
        // is contained within
        internal DungeonLevel dungeonLevel;

        // Name: MonsterLayer (constructor)
        // Description: This is the constructor for the MonsterLayer. It
        //              instantiates the MonsterLayer list
        // Parameters: TCODConsole console , This should almost always be a
        //             reference to the root console
        public MonsterLayer( TCODConsole console )
        {
            this.console = console;
            monsterLayer = new List<Monster>();
        }

        // Name: addMonster
        // Description: Adds the monster reference to the monster layer list
        // Parameters: Monster monster , the monster to be added to the list
        public void addMonster(Monster monster)
        {
            // All temp code. As a reminder, quite a few things need to be
            // initialized when creating a monster
            monster.XCoord = 10;
            monster.YCoord = 10;
            monster.Color = TCODColor.white;
            monster.ASCIICode = (short)('@');
            monster.IsPlayer = true;
            monster.MonsterLayer = this;
            // End temp code
            monsterLayer.Add(monster);
        }

        // Name: printToScreen
        // Description: For now, attempts to print every single Monster in the
        //              list to the entire screen, with no care for efficiency.
        //              In the future: Will only update the monsters visually
        //              that actually need to be
        public void printToScreen()
        {
            foreach ( Monster monster in monsterLayer )
            {
                console.putCharEx(monster.XCoord, monster.YCoord,
                    monster.ASCIICode, monster.Color, 
                    console.getBackgroundColor());
            }
        }

        // Name: doAction
        // Description: Convenience method to call the doAction of every monster
        //              in the monster list
        public void doAction()
        {
            foreach (Monster monster in monsterLayer)
            {
                monster.doAction();
            }
        }

        // Accessors

        // Name: DungeonLevel (accessor)
        // Description: Provides an accessor for the DungeonLevel data member
        public DungeonLevel DungeonLevel
        {
            set
            {
                dungeonLevel = value;
            }
        }
	}
}
