// Name: DungeonLevel
// Description: Dungeon levels are the individual floors of the dungeon. A
//              dungeonLevel keeps track of all of its layers, including the
//              MonsterLayer, ItemLayer, EventLayer, EffectsLayer, DungeonLayer,
//              PathfindExceptionLayer, and TotemLayer, and has the ability to
//              respond to queries about this information. A dungeon level is
//              is also responsible for the initial populating of the level with
//              monsters and items, and is responsible for the occasional 
//              repopulating with monsters
// Author: Collin Reeser
// Contributors:
// Log:
// - Created skeleton for use as an existent entity with the pathfinder.
// - Added several helper methods, implemented the screen printer, and added
//   some temp code for testing
//
// TODO:
// - Fill out all the layers involved in the dungeon

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRH5Roguelike.Entity;
using libtcod;

namespace NRH5Roguelike.Dungeon
{
	class DungeonLevel
	{
        // This is the dungeon layer of this dungeon level
        internal DungeonLayer dungeonLayer;
        // This is the monster layer of this dungeon level
        internal MonsterLayer monsterLayer;
        // This is the reference to the root console for drawing
        private readonly TCODConsole console;
        // Name: DungeonLevel (constructor)
        // Description: Instantiates an instance of the DungeonLevel class
        public DungeonLevel( TCODConsole console )
        {
            dungeonLayer = new DungeonLayer( console );
            monsterLayer = new MonsterLayer( console );
            // Temp Code
            monsterLayer.DungeonLevel = this;
            // End temp code
            this.console = console;
        }

        // Name: getDungeonHeight
        // Description: This is a convenience method and simply calls the
        //              getHeight method of the underlying DungeonLayer of this
        //              DungeonLevel
        // Returns: int , the value returned by a call to this DungeonLevel's
        //          DungeonLayer
        public int getDungeonHeight()
        {
            return dungeonLayer.getHeight();
        }

        // Name: getDungeonWidth
        // Description: This is a convenience method and simply calls the
        //              getWidth method of the underlying DungeonLayer of this
        //              DungeonLevel
        // Returns: int , the value returned by a call to this DungeonLevel's
        //          DungeonLayer
        public int getDungeonWidth()
        {
            return dungeonLayer.getWidth();
        }

        // Name: addMonsterToDungeon
        // Description: Takes a Monster reference and adds it to the Monster
        //              Layer of this dungeon level. For now the location is
        //              random, but later the randomness is limited to being
        //              outside the scope of the LoS of the player, to maintain
        //              belief. Also, later an overloaded version of this method
        //              will need to exist such that spawn coordinates can be
        //              given
        // Parameters: Monster monster , a reference to the monster to be added
        public void addMonsterToDungeon(Monster monster)
        {
            this.monsterLayer.addMonster( monster );
        }

        // Name: doAction
        // Description: Calls the doAction method for every actor on the entire
        //              dungeon floor. Monsters, totems, events, effects, events
        //              layers
        public void doAction()
        {
            monsterLayer.doAction();
        }

        // Name: printToScreen
        // Description: This method examines all of the layers of the dungeon
        //              level, locates the thing at each cell with the highest
        //              display priority (and with things with transparency,
        //              continues to find the next highest things with display
        //              priority until one without transparency is found, and
        //              displays all of them at that cell), determines the color
        //              and prints them to the cell.
        //              Strategy: The order of priority is such: DungeonLayer
        //              is always lowest, then the item layer, then the Monster
        //              and Totem layers equally, then the effects layer. All of
        //              these layers can have transparency for what's on them.
        //              So, a brute force method to doing this would be to print
        //              everything with transparency and the first opaque high
        //              priority thing at each layer, then move up to the next
        //              layer and repeat, with the effects layer going last and
        //              the dungeon layer going first. An emergent property
        //              should be that everything is displayed as it should be
        //              Optimization: Make an overloaded version of this method
        //              that takes an area and only updates that area, ie, the
        //              LoS area of the player.
        //              For the future: Once the layout of the actual game
        //              screen is decided upon, and since scrolling will
        //              probably be chosen as the best route, this method is
        //              going to need to only print to a certain area on the
        //              screen, which is going to be limited in scope anyway,
        //              while the overloaded version would be obsolete. Also,
        //              this method will need to differentiate between what the
        //              player currently sees with LoS (which will probably
        //              require some sort of a parameter, but God help us if it
        //              needs an LoS object), and what the player has seen
        //              before but does not currently see (the OnceSeen Layer)
        public void printToScreen()
        {
            dungeonLayer.printToScreen();
            monsterLayer.printToScreen();
        }

        // Name: printDungeonToConsole
        // Description: Print the dungeon level to the console. This consists of
        //              finding what has the most display priority for any
        //              particular cell out of all the dungeon level layers, and
        //              printing that, for each cell.
        //              FOR NOW, only prints the dungeon layer
        public void printDungeonToConsole()
        {
            dungeonLayer.printMapToConsole();
        }
	}
}
