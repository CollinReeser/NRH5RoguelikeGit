// Name: Pathfinder
// Description: This is an implementation of the A* graph-searching algorithm,
//              to be used as the pathfinder for all creatures that have the
//              ability to choose where they move turn-by-turn. This particular
//              implementation shall use a heap to keep the open nodes sorted.
//              The initial and most likely final implementation strategy will
//              be to use native arrays for nearly everything, data management-
//              wise. Although memory heavy, it will prove to be a much faster
//              implementation strategy, and speed in this case is critical.
//              The pathfinder will exist with a pre-constructed object, such
//              that all data needed to provide a pathfinding solution will be
//              passed to a static method. While this will require a bit of
//              up-front work for those who use the pathfinder, in filling in
//              all of the parameters, it will save time in constantly creating
//              and destroying pathfinder objects. If this strategy proves too
//              inconvenient, it shall be possible to change. The information
//              the pathfinder must have access to is a reference to the Monster
//              that is doing the pathfinding, the desired start and end
//              coordinates of the path to be generated, and a reference to the
//              level the path is being searched on (which must include at least
//              dungeon layer, monster layer, and pathfinder-exception layer
//              data). The method will then return (subject to heavy change):
//              The direction that the first step of the path is in (since
//              between turns the path will need to be generated again. This is
//              a potential area for improvement where cached data about the
//              path can be a massive optimization), and the length of the path
//              itself. More information may be needed and can be provided if
//              the situation arises. This pathfinder takes into account the
//              wieghts of travelling through different pieces of terrain
//              (dependant on the monster that is pathfinding), assumes going
//              in any direction costs the same (with maybe a slightly higher
//              cost for diagonals just so the results are cleaner, and
//              assuming each of the eight directions hosts an identical
//              type of landscape tile), will ensure through checks with the
//              time-dependant data of the monster that a monster will only
//              pathfind through terrain that it can when it actually gets to it
//              (take a temporarily levitating monster: If the monster were to
//              lose levitation while currently floating above a lava pit, the
//              pathfinder will not allow passing over the lava pit as an
//              acceptable path, even though at the time of invocation the
//              monster can indeed pass over the lava), can take (and should,
//              for everyone except the actual player) a variable that dictates
//              how far the pathfinder is allowed to search before exiting with
//              no path, can be called as Djikstra's algorithm along with
//              multiple desired goals, so that the closest goal's path is
//              returned, can be called with a greedy depth first search so
//              very rapid close-range paths can be found, and should be able to
//              find "kinda" paths that find paths to areas near the actual
//              desired goal
// Author: Collin Reeser
// Contributors:
// Log:
// - Heavy documentation. Begin skeleton of methods needed
// - Complete skeleton except for the open list heap. Began a unit test to be
//   used for pathfind testing. It also demonstrates beautifully well behaved
//   and cooperative code between different parts of the emerging framework
// - Implemented heap
// - Implemented most of algorithm
//
// TODO:
// - Finish algorithm
// - Polish
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
using NRH5Roguelike.Entity;
using NRH5Roguelike.Dungeon;
using System.Diagnostics;

namespace NRH5Roguelike.Utility
{
    static class Pathfinder
    {
        // Name: Main (unit test)
        // Description: This is the unit test of the pathfinder. It will be
        //              used to exhaustively test the features of the pathfinder
        //              to ensure that not only does it work correctly with
        //              optimal input data, but handles gracefully bad input
        //              data, and does it all in an extremely efficient, 
        //              optimized, sexy-speedy manner
        static void Main(string[] args)
        {
            // Define constants for the size of the window so that window-
            //dependant things can be easily updated
            const int WINDOW_WIDTH = 80;
            const int WINDOW_HEIGHT = 55;
            // Display an initializing message to the console
            Console.WriteLine("Initilizing game window...");
            // Create the new libtcod output window. This needs to be done only
            // once, but before anything else. The first value is the width in
            // cells the window is, the second is the height in cells, and the
            // third is the name of the window
            TCODConsole.initRoot(WINDOW_WIDTH, WINDOW_HEIGHT,
                "Hello World!");
            // Set the maximum graphical update speed. This is important to
            // massively reduce CPU use with unnecessary updates
            TCODSystem.setFps(30);
            // Set the default background color of the root console with black
            TCODConsole.root.setBackgroundColor(TCODColor.black);
            // Clear is a function that overwrites every cell in the window with
            // a plain, empty cell of the color dictated by setBackgroundColor()
            TCODConsole.root.clear();
            // Flush writes the graphical data that is pending to the screen.
            // Any graphics updates are not shown to the screen until flush is
            // called. Try to minimize this call
            TCODConsole.flush();
            TCODKey Key = TCODConsole.checkForKeypress();
            // Create dungeon objects eventually to be used for pathfinding test
            Monster monster = new Monster();
            // Set the bit for isPlayer in the bitfield
            monster.setFlag(Monster.AttributeFlags.isPlayer);
            DungeonLevel dungeon = new DungeonLevel(TCODConsole.root);
            dungeon.addMonsterToDungeon(monster);


            // PATHFINDING TESTS
            Stopwatch st = new Stopwatch();
            for (int i = 0; i < 500000; i++)
            {
                st.Start();
                //Console.WriteLine(
                pathfind(monster, dungeon, monster.XCoord, monster.YCoord,
                (short)30,
                (short)30);//.directionOfPath);
                st.Stop();
            }
            Console.WriteLine(st.ElapsedMilliseconds/500000.0);
            // PATHFINDING TESTS


            // While the user has not closed the window and while the user has
            // not pressed escape, do stuff
            while (!TCODConsole.isWindowClosed())
            {
                dungeon.printToScreen();
                TCODConsole.flush();
                dungeon.doAction();
            }
        }
        public static readonly short ARB_HIGH = 30000;
        // This refers to the number of attributes a given node has within the
        // context of the pathfinder searchspace
        private static readonly short NUM_OF_ELEMENTS = 6;
        // These are constants for indexing in the searchSpace array
        private static readonly short G_SCORE_INDEX = 0;
        private static readonly short H_SCORE_INDEX = 1;
        private static readonly short PARENT_INDEX_X = 2;
        private static readonly short PARENT_INDEX_Y = 5;
        private static readonly short NODE_TYPE_INDEX = 3;
        private static readonly short LIST_INDEX = 4;
        // These are used to differentiate node types
        private static readonly short END_NODE = -1;
        private static readonly short NORMAL_NODE = 0;
        private static readonly short START_NODE = 1;
        // Used to differentiate lists
        private static readonly short CLOSED_LIST = -1;
        private static readonly short NOT_LISTED = 0;
        private static readonly short OPEN_LIST = 1;
        // Used as a testibly modifiable value for the H-Score incrementation
        // amount, variable based on the prettiness of the resultant path
        static readonly short H_SCORE_CONSTANT = 10;
        // Used to hold the end location so that it isn't passed between method
        // calls unnecessarily
        private static short currentEndXCoord;
        private static short currentEndYCoord;
        // Static reference to the dungeon level to be used
        private static DungeonLevel level;
        // Static reference to the pathfinding monster
        private static Monster monster;
        // Static temp vars for holding calulated heuristics and G-scores, so
        // that millions of cycles aren't wasted just creating these variables
        // constantly
        private static short tempH;
        private static short tempG;
        // Static variable to hold how long the path is
        private static short pathLength;
        // A final temp value
        private static short temp;
        // Used to calculate direction of path
        private static PathfindTile lastTile;

        // This is the search space used to undergo the pathfinding algorithm
        // within. The first and second indexes are the Y and X coordinates
        // coorosponding to the dungeon map, respectively, and the last is the
        // index that accesses several pieces of information about the node at
        // that point.
        // First index space: 0 == Y-coordinate of the node
        // Second index space: 0 == X-coordinate of the node
        // Third index space: 0 == G-Score of node (the distance from the start,
        //                        which accounts for tile weight)
        //                    1 == H-score of node (heuristic to the end)
        //                    2 == Location of parent (represented by Direction
        //                        enum value)
        //                    3 == Node type ( NORMAL_NODE == normal , 
        //                         START_NODE == start node , 
        //                         END_NODE == endNode )
        //                    4 == List the node is is ( 
        //                         CLOSED_LIST == closed list ,
        //                         OPEN_LIST == open list , 
        //                         NOT_LISTED == not listed)
        private static short[, ,] searchSpace;

        // Name: pathfind
        // Description: Takes several parameters and returns a struct
        //              representing the results of processing that data with
        //              the A* search algorithm. This method attempts to find
        //              a best path to the end node from the start node,
        //              utilizing data available to it from the Monster and
        //              DungeonLevel references
        // Parameters: Monster monster , a reference to a monster where
        //             pathfinding data specific to that monster is mined
        //             DungeonLevel level , a reference to a DungeonLevel so
        //             that critical information about its layers, in regards to
        //             pathfinding, can be mined
        //             short startXCoord , 
        //             short startYCoord , the x and y-coordinate on the 
        //             DungeonLayer of the DungeonLevel that the entity is 
        //             beginning to pathfind from
        //             short endXCoord ,
        //             short endYCoord , the x and y-coordinate on the
        //             DungeonLayer of the DungeonLevel that the entity is
        //             attempting to find a path to
        //             short searchDistance , how far the pathfinder will search
        //             before breaking with either a noPath or a "kinda" path.
        //             The default is 0, or search the entire searchspace
        //             bool guessPath , if true, the pathfinder will try to
        //             return a path to a location near the goal, or if the
        //             pathfinder exits early because of a searchDistance limit,
        //             will return with a path "going toward" the goal. The
        //             default is off (false)
        //             byte algToUse , -1 is Djikstra's algorithm. This is
        //             just a bad choice. 1 is greedy best-first, which is
        //             optimal in the case of known very short paths, as it is
        //             fastest and its inaccuracy is ruled out in short paths.
        //             0 is A*, which is the fastest longer-distance algorithm
        //             to use, and is also the default
        // Returns: An instance of a PathfindData struct that carries both the
        //          length of the calculated path, and the direction in which
        //          the first step of the path begins
        public static PathfindData pathfind(Monster monster,
            DungeonLevel level, short startXCoord, short startYCoord,
            short endXCoord, short endYCoord, short searchDistance = 0,
            bool guesspath = false, byte algorithmToUse = 0)
        {
            // Determine if the map we're searching on is larger in some way
            // than the searchspace previously defined
            if (Pathfinder.searchSpace == null || 
                Pathfinder.searchSpace.GetLength(0) < level.getDungeonHeight()
                || Pathfinder.searchSpace.GetLength(1) <
                level.getDungeonWidth())
            {
                Pathfinder.searchSpace =
                    new short[level.getDungeonHeight(),
                    level.getDungeonWidth(), NUM_OF_ELEMENTS];
            }
            else
            {
                clearSearchSpace();
            }
            // Initialize the static end coordinates
            currentEndXCoord = endXCoord;
            currentEndYCoord = endYCoord;
            // Set the start and end nodes in the searchSpace
            searchSpace[startYCoord, startXCoord, NODE_TYPE_INDEX] = START_NODE;
            searchSpace[endYCoord, endXCoord, NODE_TYPE_INDEX] = END_NODE;
            Pathfinder.level = level;
            Pathfinder.monster = monster;
            // Initialize the pathfinding heap array
            NodeHeap.initializePathfindList(level);
            // Create the first tile, to represent the start tile. The F-Score
            // is a G-Score of 0 (start node), and the full heuristic
            PathfindTile currentTile =
                new PathfindTile(startXCoord, startYCoord,
                /*0 +*/ calcHeuristic(startXCoord, startYCoord));
            // Do while we are not currently exploring the end node and there is
            // more in the list
            do
            {
                // Add the tile to the closed list
                searchSpace[currentTile.yCoord, currentTile.xCoord,
                    LIST_INDEX] = CLOSED_LIST;
                // Expand the node to explore its neighbors
                expandNode(currentTile);
                // Pull the root tile, which should be the most optimal tile
                // to explore next
                currentTile = NodeHeap.pullRoot();
            } while (searchSpace[currentTile.yCoord, currentTile.xCoord,
                NODE_TYPE_INDEX] != END_NODE && !NodeHeap.isHeapEmpty());
            // If we found the end node, then calculate a path back
            if (searchSpace[currentTile.yCoord, currentTile.xCoord,
                NODE_TYPE_INDEX] == END_NODE)
            {
                pathLength = 0;
                while (searchSpace[currentTile.yCoord, currentTile.xCoord,
                    NODE_TYPE_INDEX] != START_NODE)
                {
                    lastTile = currentTile;
                    temp = searchSpace[currentTile.yCoord,
                        currentTile.xCoord, PARENT_INDEX_X];
                    currentTile.yCoord = searchSpace[currentTile.yCoord,
                        currentTile.xCoord, PARENT_INDEX_Y];
                    currentTile.xCoord = temp;
                    pathLength++;
                }
                return new PathfindData(
                    Pathfinder.calcDirection(currentTile.xCoord,
                    currentTile.yCoord, lastTile.xCoord, lastTile.yCoord),
                    pathLength);
            }
            // Temp code for compilation
            return new PathfindData((byte)Direction.NORTH, 0);
        }

        // Name: expandNode
        // Description: Does most of the heavy lifting of the algorithm. Takes
        //              a node and pushes all the viable neighbors into the
        //              sorted open list complete with calculated F-Score, as
        //              well as updating the 3D array with those nodes and their
        //              information
        // Parameters: PathfindTile node , the node to be expanded
        private static void expandNode(PathfindTile node)
        {
            // For each direction, we must check that a node was not already
            // explored, then that it is walkable, and then finally add it to
            // the open list
            // NORTH
            if (searchSpace[node.yCoord - 1, node.xCoord, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile(node.xCoord,
                (short)(node.yCoord - 1))))
            {
                // Calculcate and preserve the H and G scores for the new node
                tempH = (short)(calcHeuristic((short)(node.yCoord - 1)
                    , (short)(node.xCoord)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                // Add node to heap
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord - 1),
                    node.xCoord, (short)(tempH + tempG)));
                // Add to open list
                searchSpace[node.yCoord - 1, node.xCoord, LIST_INDEX] =
                    OPEN_LIST;
                // Update scores
                searchSpace[node.yCoord - 1, node.xCoord, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord - 1, node.xCoord, G_SCORE_INDEX] =
                    tempG;
                // Update parent
                searchSpace[node.yCoord - 1, node.xCoord, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord - 1, node.xCoord, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            // If the tile is in the closed list, it means it is definitely
            // walkable. What we need to do now is determine if from this new
            // node, are we able to make the route shorter based on the new
            // G-Score? The heuristic would obviously already have been computed
            // for this node, so only the G-Score needs to be re-evaluated
            else if (searchSpace[node.yCoord - 1, node.xCoord, LIST_INDEX] ==
                CLOSED_LIST)
            {
                // Calculate what the new G-Score would be
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                // If the new G-Score is better than what it was before, then
                // update the node, add it back to the open list, and carry on
                if (tempG <
                    searchSpace[node.yCoord - 1, node.xCoord, G_SCORE_INDEX])
                {
                    // Update G-Score
                    searchSpace[node.yCoord - 1, node.xCoord, G_SCORE_INDEX] =
                        tempG;
                    // Update parent
                    searchSpace[node.yCoord - 1, node.xCoord, PARENT_INDEX_X] =
                        node.xCoord;
                    searchSpace[node.yCoord - 1, node.xCoord, PARENT_INDEX_Y] =
                        node.yCoord;
                    // Add to open list
                    searchSpace[node.yCoord - 1, node.xCoord, LIST_INDEX] =
                        OPEN_LIST;
                    // Add node to heap
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord - 1),
                        node.xCoord,
                        (short)(searchSpace[node.yCoord - 1, node.xCoord,
                        H_SCORE_INDEX] + tempG)));
                }
            }
            // NORTH EAST
            if (searchSpace[node.yCoord - 1, node.xCoord + 1, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile((short)(node.xCoord + 1), 
                (short)(node.yCoord - 1))))
            {
                tempH = (short)(calcHeuristic((short)(node.yCoord - 1)
                    , (short)(node.xCoord + 1)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord - 1),
                    (short)(node.xCoord + 1), (short)(tempH + tempG)));
                searchSpace[node.yCoord - 1, node.xCoord + 1, LIST_INDEX] =
                    OPEN_LIST;
                searchSpace[node.yCoord - 1, node.xCoord + 1, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord - 1, node.xCoord + 1, G_SCORE_INDEX] =
                    tempG;
                searchSpace[node.yCoord - 1, node.xCoord + 1, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord - 1, node.xCoord + 1, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            else if (searchSpace[node.yCoord - 1, node.xCoord + 1, LIST_INDEX]
                == CLOSED_LIST)
            {
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                if (tempG <
                    searchSpace[node.yCoord - 1, node.xCoord + 1,
                    G_SCORE_INDEX])
                {
                    searchSpace[node.yCoord - 1, node.xCoord + 1, G_SCORE_INDEX]
                        = tempG;
                    searchSpace[node.yCoord - 1, node.xCoord + 1,
                        PARENT_INDEX_X] = node.xCoord;
                    searchSpace[node.yCoord - 1, node.xCoord + 1,
                        PARENT_INDEX_Y] = node.yCoord;
                    searchSpace[node.yCoord - 1, node.xCoord + 1, LIST_INDEX] =
                        OPEN_LIST;
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord - 1),
                        (short)(node.xCoord + 1),
                        (short)(searchSpace[node.yCoord - 1, node.xCoord + 1,
                        H_SCORE_INDEX] + tempG)));
                }
            }
            // EAST
            if (searchSpace[node.yCoord, node.xCoord + 1, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile((short)(node.xCoord + 1), 
                node.yCoord)))
            {
                tempH = (short)(calcHeuristic((short)(node.yCoord)
                    , (short)(node.xCoord + 1)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord),
                    (short)(node.xCoord + 1), (short)(tempH + tempG)));
                searchSpace[node.yCoord, node.xCoord + 1, LIST_INDEX] =
                    OPEN_LIST;
                searchSpace[node.yCoord, node.xCoord + 1, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord, node.xCoord + 1, G_SCORE_INDEX] =
                    tempG;
                searchSpace[node.yCoord, node.xCoord + 1, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord, node.xCoord + 1, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            else if (searchSpace[node.yCoord, node.xCoord + 1, LIST_INDEX] ==
                CLOSED_LIST)
            {
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                if (tempG <
                    searchSpace[node.yCoord, node.xCoord + 1, G_SCORE_INDEX])
                {
                    searchSpace[node.yCoord, node.xCoord + 1, G_SCORE_INDEX] =
                        tempG;
                    searchSpace[node.yCoord, node.xCoord + 1, PARENT_INDEX_X] =
                        node.xCoord;
                    searchSpace[node.yCoord, node.xCoord + 1, PARENT_INDEX_Y] =
                        node.yCoord;
                    searchSpace[node.yCoord, node.xCoord + 1, LIST_INDEX] =
                        OPEN_LIST;
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord),
                        (short)(node.xCoord + 1),
                        (short)(searchSpace[node.yCoord, node.xCoord + 1,
                        H_SCORE_INDEX] + tempG)));
                }
            }
            // SOUTH EAST
            if (searchSpace[node.yCoord + 1, node.xCoord + 1, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile((short)(node.xCoord + 1), 
                (short)(node.yCoord + 1 ))))
            {
                tempH = (short)(calcHeuristic((short)(node.yCoord + 1)
                    , (short)(node.xCoord + 1)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord + 1),
                    (short)(node.xCoord + 1), (short)(tempH + tempG)));
                searchSpace[node.yCoord + 1, node.xCoord + 1, LIST_INDEX] =
                    OPEN_LIST;
                searchSpace[node.yCoord + 1, node.xCoord + 1, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord + 1, node.xCoord + 1, G_SCORE_INDEX] =
                    tempG;
                searchSpace[node.yCoord + 1, node.xCoord + 1, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord + 1, node.xCoord + 1, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            else if (searchSpace[node.yCoord + 1, node.xCoord + 1, LIST_INDEX]
                == CLOSED_LIST)
            {
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                if (tempG <
                    searchSpace[node.yCoord + 1, node.xCoord + 1,
                    G_SCORE_INDEX])
                {
                    searchSpace[node.yCoord + 1, node.xCoord + 1, G_SCORE_INDEX]
                        = tempG;
                    searchSpace[node.yCoord + 1, node.xCoord + 1,
                        PARENT_INDEX_X] = node.xCoord;
                    searchSpace[node.yCoord + 1, node.xCoord + 1,
                        PARENT_INDEX_Y] = node.yCoord;
                    searchSpace[node.yCoord + 1, node.xCoord + 1, LIST_INDEX] =
                        OPEN_LIST;
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord + 1),
                        (short)(node.xCoord + 1),
                        (short)(searchSpace[node.yCoord + 1, node.xCoord + 1,
                        H_SCORE_INDEX] + tempG)));
                }
            }
            // SOUTH
            if (searchSpace[node.yCoord + 1, node.xCoord, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile(node.xCoord, 
                (short)(node.yCoord + 1))))
            {
                tempH = (short)(calcHeuristic((short)(node.yCoord + 1)
                    , (short)(node.xCoord)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord + 1),
                    node.xCoord, (short)(tempH + tempG)));
                searchSpace[node.yCoord + 1, node.xCoord, LIST_INDEX] =
                    OPEN_LIST;
                searchSpace[node.yCoord + 1, node.xCoord, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord + 1, node.xCoord, G_SCORE_INDEX] =
                    tempG;
                searchSpace[node.yCoord + 1, node.xCoord, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord + 1, node.xCoord, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            else if (searchSpace[node.yCoord + 1, node.xCoord, LIST_INDEX] ==
                CLOSED_LIST)
            {
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                if (tempG <
                    searchSpace[node.yCoord + 1, node.xCoord, G_SCORE_INDEX])
                {
                    searchSpace[node.yCoord + 1, node.xCoord, G_SCORE_INDEX] =
                        tempG;
                    searchSpace[node.yCoord + 1, node.xCoord, PARENT_INDEX_X] =
                        node.xCoord;
                    searchSpace[node.yCoord + 1, node.xCoord, PARENT_INDEX_Y] =
                        node.yCoord;
                    searchSpace[node.yCoord + 1, node.xCoord, LIST_INDEX] =
                        OPEN_LIST;
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord + 1),
                        node.xCoord,
                        (short)(searchSpace[node.yCoord + 1, node.xCoord,
                        H_SCORE_INDEX] + tempG)));
                }
            }
            // SOUTH WEST
            if (searchSpace[node.yCoord + 1, node.xCoord - 1, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile((short)(node.xCoord - 1), 
                (short)(node.yCoord + 1))))
            {
                tempH = (short)(calcHeuristic((short)(node.yCoord + 1)
                    , (short)(node.xCoord - 1)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord + 1),
                    (short)(node.xCoord - 1), (short)(tempH + tempG)));
                searchSpace[node.yCoord + 1, node.xCoord - 1, LIST_INDEX] =
                    OPEN_LIST;
                searchSpace[node.yCoord + 1, node.xCoord - 1, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord + 1, node.xCoord - 1, G_SCORE_INDEX] =
                    tempG;
                searchSpace[node.yCoord + 1, node.xCoord - 1, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord + 1, node.xCoord - 1, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            else if (searchSpace[node.yCoord + 1, node.xCoord - 1, LIST_INDEX]
                == CLOSED_LIST)
            {
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                if (tempG <
                    searchSpace[node.yCoord + 1, node.xCoord - 1,
                    G_SCORE_INDEX])
                {
                    searchSpace[node.yCoord + 1, node.xCoord - 1, G_SCORE_INDEX]
                        = tempG;
                    searchSpace[node.yCoord + 1, node.xCoord - 1,
                        PARENT_INDEX_X] = node.xCoord;
                    searchSpace[node.yCoord + 1, node.xCoord - 1,
                        PARENT_INDEX_Y] = node.yCoord;
                    searchSpace[node.yCoord + 1, node.xCoord - 1, LIST_INDEX] =
                        OPEN_LIST;
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord + 1),
                        (short)(node.xCoord - 1),
                        (short)(searchSpace[node.yCoord + 1, node.xCoord - 1,
                        H_SCORE_INDEX] + tempG)));
                }
            }
            // WEST
            if (searchSpace[node.yCoord, node.xCoord - 1, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile((short)(node.xCoord - 1), 
                node.yCoord)))
            {
                tempH = (short)(calcHeuristic((short)(node.yCoord)
                    , (short)(node.xCoord - 1)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord),
                    (short)(node.xCoord - 1), (short)(tempH + tempG)));
                searchSpace[node.yCoord, node.xCoord - 1, LIST_INDEX] =
                    OPEN_LIST;
                searchSpace[node.yCoord, node.xCoord - 1, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord, node.xCoord - 1, G_SCORE_INDEX] =
                    tempG;
                searchSpace[node.yCoord, node.xCoord - 1, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord, node.xCoord - 1, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            else if (searchSpace[node.yCoord, node.xCoord - 1, LIST_INDEX] ==
                CLOSED_LIST)
            {
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                if (tempG <
                    searchSpace[node.yCoord, node.xCoord - 1, G_SCORE_INDEX])
                {
                    searchSpace[node.yCoord, node.xCoord - 1, G_SCORE_INDEX] =
                        tempG;
                    searchSpace[node.yCoord, node.xCoord - 1, PARENT_INDEX_X] =
                        node.xCoord;
                    searchSpace[node.yCoord, node.xCoord - 1, PARENT_INDEX_Y] =
                        node.yCoord;
                    searchSpace[node.yCoord, node.xCoord - 1, LIST_INDEX] =
                        OPEN_LIST;
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord),
                        (short)(node.xCoord - 1),
                        (short)(searchSpace[node.yCoord, node.xCoord - 1,
                        H_SCORE_INDEX] + tempG)));
                }
            }
            // NORTH WEST
            if (searchSpace[node.yCoord - 1, node.xCoord - 1, LIST_INDEX] ==
                NOT_LISTED &&
                monster.isWalkable(
                level.dungeonLayer.getTile((short)(node.xCoord - 1), 
                (short)(node.yCoord - 1))))
            {
                tempH = (short)(calcHeuristic((short)(node.yCoord - 1)
                    , (short)(node.xCoord - 1)));
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                NodeHeap.pushNode(new PathfindTile((short)(node.yCoord - 1),
                    (short)(node.xCoord - 1), (short)(tempH + tempG)));
                searchSpace[node.yCoord - 1, node.xCoord - 1, LIST_INDEX] =
                    OPEN_LIST;
                searchSpace[node.yCoord - 1, node.xCoord - 1, H_SCORE_INDEX] =
                    tempH;
                searchSpace[node.yCoord - 1, node.xCoord - 1, G_SCORE_INDEX] =
                    tempG;
                searchSpace[node.yCoord - 1, node.xCoord - 1, PARENT_INDEX_X] =
                    node.xCoord;
                searchSpace[node.yCoord - 1, node.xCoord - 1, PARENT_INDEX_Y] =
                    node.yCoord;
            }
            else if (searchSpace[node.yCoord - 1, node.xCoord - 1, LIST_INDEX]
                == CLOSED_LIST)
            {
                tempG = (short)
                    (searchSpace[node.yCoord, node.xCoord, G_SCORE_INDEX] + 10);
                if (tempG <
                    searchSpace[node.yCoord - 1, node.xCoord - 1,
                    G_SCORE_INDEX])
                {
                    searchSpace[node.yCoord - 1, node.xCoord - 1, G_SCORE_INDEX]
                        = tempG;
                    searchSpace[node.yCoord - 1, node.xCoord - 1,
                        PARENT_INDEX_X] = node.xCoord;
                    searchSpace[node.yCoord - 1, node.xCoord - 1,
                        PARENT_INDEX_Y] = node.yCoord;
                    searchSpace[node.yCoord - 1, node.xCoord - 1, LIST_INDEX] =
                        OPEN_LIST;
                    NodeHeap.pushNode(new PathfindTile((short)(node.yCoord - 1),
                        (short)(node.xCoord - 1),
                        (short)(searchSpace[node.yCoord - 1, node.xCoord - 1,
                        H_SCORE_INDEX] + tempG)));
                }
            }
        }

        // Name: calcHeuristic
        // Description: Return an as-the-crow-flies heuristic to the end point
        //              from the passed current location
        // Parameters: short xCoord ,
        //             short yCoord , the x and y coordinates of the current
        //             node location
        // Returns: An H-score of the heuristic
        private static short calcHeuristic(short xCoord, short yCoord)
        {
            short hScore = 0;
            bool isEnd = false;
            // Keep looping and incrementing the hScore by a value until the
            // end node is reached. The xCoord and yCoord values will be used
            // as the temp values because the starting value is only needed
            // initially
            for (; ; )
            {
                // Calculate movement of x portion
                if (xCoord < currentEndXCoord)
                {
                    xCoord++;
                }
                else if (xCoord > currentEndXCoord)
                {
                    xCoord--;
                }
                else
                {
                    // Here, we predict that we are at the end node. If the
                    // y-coordinate gets changed later on, then we weren't and
                    // this is switched back to false. If we arrive at the 
                    // "else" condition for y as well, then we know we are at
                    // the end node
                    isEnd = true;
                }
                // Calculate movement of y portion
                if (yCoord < currentEndYCoord)
                {
                    yCoord++;
                    isEnd = false;
                }
                else if (yCoord > currentEndYCoord)
                {
                    yCoord--;
                    isEnd = false;
                }
                else
                {
                    // If isEnd is true when we get here, then we are at the end
                    // node and must stop
                    if (isEnd)
                    {
                        break;
                    }
                }
                // Increment hScore after moving the node
                hScore += H_SCORE_CONSTANT;
            }
            return hScore;
        }

        // Name: clearSearchSpace
        // Description: Since the algorithm only cares whether a node is listed
        //              or not, and then what list that is, it is sufficient to
        //              simply zero out the LIST_INDEX of the searchSpace to
        //              "clear" it, instead of either zeroing everything out or
        //              creating a new object. However, testing must be done to
        //              this is this clearing method or simply creating a new
        //              array is the more efficient system
        private static void clearSearchSpace()
        {
            for (short y = 0; y < Pathfinder.searchSpace.GetLength(0); y++)
            {
                for (short x = 0; x < Pathfinder.searchSpace.GetLength(1); x++)
                {
                    Pathfinder.searchSpace[y, x, LIST_INDEX] = NOT_LISTED;
                }
            }
        }

        // Name: Direction
        // Description: Describes a tile direction, where NORTH is 0 and the 
        //              values thereafter are the value previous plus 1. 
        //              NORTHEAST is 1.
        public enum Direction
        {
            NORTH, NORTHEAST, EAST, SOUTHEAST, SOUTH, SOUTHWEST, WEST,
            NORTHWEST, INVALID
        }

        // Name: calcDirection
        // Descrption: Return the enum direction from one cell to the next
        // Parameters: short x1 ,
        //             short y1 ,
        //             short x2 ,
        //             short y2 , the coordinates of two cells.
        // Returns: An enum representing the direction that the second cell is 
        //          in in regards to the first cell
        public static Direction calcDirection(short x1, short y1, short x2,
            short y2)
        {
            if (x1 < x2)
            {
                if (y1 < y2)
                {
                    return Direction.SOUTHEAST;
                }
                if (y1 > y2)
                {
                    return Direction.NORTHEAST;
                }
                else
                {
                    return Direction.EAST;
                }
            }
            else if (x1 > x2)
            {
                if (y1 < y2)
                {
                    return Direction.SOUTHWEST;
                }
                if (y1 > y2)
                {
                    return Direction.NORTHWEST;
                }
                else
                {
                    return Direction.WEST;
                }
            }
            else
            {
                if (y1 < y2)
                {
                    return Direction.SOUTH;
                }
                if (y1 > y2)
                {
                    return Direction.NORTH;
                }
                else
                {
                    return Direction.INVALID;
                }
            }
        }

        // Name: PathfindData
        // Description: Is constructed with data pertaining to the result of a 
        //              pathfind operation
        public struct PathfindData
        {
            // Name: PathfindData (constructor)
            // Description: Constructs a PathfindData struct by initializing its
            //              readonly data members
            public PathfindData(Pathfinder.Direction directionOfPath,
                short lengthOfPath)
            {
                this.directionOfPath = directionOfPath;
                this.lengthOfPath = lengthOfPath;
            }
            // The data members of the PathfindData struct. They are readonly so
            // that while they can be initialized with any data value that is
            // desired or needed at runtime, they cannot thereafter be changed
            public readonly Pathfinder.Direction directionOfPath;
            public readonly short lengthOfPath;
        }
    }

    // Name: PathfindTile
    // Description: Used to hold pathfind data points
    struct PathfindTile
    {
        public short xCoord;
        public short yCoord;
        // Arbitrarily high value for initialization
        public short fScore;
        public PathfindTile(short xCoord, short yCoord, short fScore)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.fScore = fScore;
        }
    }

    // Name: NodeHeap
    // Description: This is a class that shall organize the nodes of the 
    //              pathfinder so that the next best node is always kept at the 
    //              top of the list
    static class NodeHeap
    {
        // An array used to hold the data points. Preliminarily holds enough
        // for the whole goddam map worth of tiles
        private static PathfindTile[] pathfindList;
        // End of list tracker. Holds value of first empty spot
        private static short endTracker = 0;
        // Keep track of node currently being perculated up
        private static short perculationNode = -1;

        // Name: initializePathfindList
        // Description: Initialize the pathfind list
        // Parameters: DungeonLevel level , the level size is needed to
        //             initialize the pathfind heap
        public static void initializePathfindList(DungeonLevel level)
        {
            // If it isn't null and is not the right size, resize and initialize
            if (pathfindList != null && pathfindList.Length !=
                (level.getDungeonWidth() - 1) * (level.getDungeonHeight() - 1))
            {
                pathfindList = new PathfindTile[(level.getDungeonWidth() - 1) *
                    (level.getDungeonHeight() - 1)];
                for (short i = 0; i < pathfindList.Length; i++)
                {
                    pathfindList[i] = new PathfindTile(-1, -1,
                        Pathfinder.ARB_HIGH);
                }
                endTracker = 0;
            }
            // If it was null, create and initialize
            else
            {
                pathfindList = new PathfindTile[(level.getDungeonWidth() - 1) *
                    (level.getDungeonHeight() - 1)];
                for (short i = 0; i < pathfindList.Length; i++)
                {
                    pathfindList[i] = new PathfindTile(-1, -1,
                        Pathfinder.ARB_HIGH);
                }
                endTracker = 0;
            }
        }

        // Name: pushNode
        // Description: Push a node to the heap. The strategy is pushing it to
        //              the end of the list, then perculating it up until its
        //              parent is rightfully smaller than it is
        // Parameters: PathfindTile newTile , the tile to be pushed to the heap
        public static void pushNode(PathfindTile newTile)
        {
            // Put the new tile at the end of the list. It will be perculated
            // up in a smidge
            pathfindList[endTracker] = newTile;
            // Update the official end of the list
            endTracker++;
            // Perculate the new node up
            perculationNode = (short)(endTracker - 1);
            // Continue to perculate up while this method returns that
            // perculation must continue
            while (perculateUp((short)(perculationNode)))
            {
            }
        }

        // Name: pullRoot
        // Description: Pull the top node; Remove it from the list and return it
        // Returns: A PathfindTile that was the root of the heap
        public static PathfindTile pullRoot()
        {
            // NOTE
            // This is a debugging check. Once the algorithm is correct it can
            // be removed
            if (endTracker != 0)
            {
                // Save the root for later
                PathfindTile root = pathfindList[0];
                // Set the root to be the very last entry in the list
                pathfindList[0] = pathfindList[--endTracker];
                // Clear what was the last entry to be "nothing"
                pathfindList[endTracker].fScore = Pathfinder.ARB_HIGH;
                // Perculate down the new "root"
                perculateDown(0);
                return root;
            }
            // Debugging return
            return new PathfindTile(-1, -1, Pathfinder.ARB_HIGH);
        }

        // Name: perculateDown
        // Description: A method that attempts to perculate a given node 
        //              downward if it needs to be
        // Parameters: short node , the index of the node that must be
        //             perculated down
        private static void perculateDown(short node)
        {
            // If child is valid, less than current node, and less than other
            // child, swap
            if (node * 2 + 1 < endTracker && pathfindList[node * 2 + 1].fScore <
                pathfindList[node].fScore && pathfindList[node * 2 + 1].fScore <
                pathfindList[node * 2 + 2].fScore)
            {
                swap((short)(node * 2 + 1), node);
                perculateDown((short)(node * 2 + 1));
            }
            // If child exists, and is less than current node, swap
            else if (node * 2 + 2 < endTracker &&
                pathfindList[node * 2 + 2].fScore < pathfindList[node].fScore)
            {
                swap((short)(node * 2 + 2), node);
                perculateDown((short)(node * 2 + 2));
            }
        }

        // Name: perculateUp
        // Description: Takes a node index and attempts to perculate it up, 
        //              returning true if perculation needs to continue, false 
        //              otherwise
        // Parameters: short node , the index of the node to be perculated up
        // Returns: A bool, true if perculation for that node needs to be
        //          continued, false otherwise
        private static bool perculateUp(short node)
        {
            // If we aren't the root node, then...
            if (perculationNode != 0)
            {
                // Calculate the parent of this node
                short parent = (perculationNode % 2 == 0) ?
                    ((short)((perculationNode >> 1) - 1)) :
                    (short)(perculationNode >> 1);
                // Determine if the parent is larger
                if (pathfindList[parent].fScore > pathfindList[node].fScore)
                {
                    // If it is, swap with the parent, and update the static
                    // perculationNode variable to refer to this new spot, so
                    // that when thios method is called again, the new location
                    // will be attempted to be perculated up
                    swap(parent, node);
                    perculationNode = parent;
                    // If the new spot is the root node, we are done, otherwise
                    // we must continue
                    if (perculationNode != 0)
                    {
                        return true;
                    }
                }
            }
            // If we reached here then the perculation process is done
            return false;
        }

        // Name: swap
        // Description: Basic swap method for swapping two array values if the 
        //              pathfindList
        // Parameters: short node1 ,
        //             short node2 , the nodes in the list to be swapped
        private static void swap(short node1, short node2)
        {
            PathfindTile temp = pathfindList[node1];
            pathfindList[node1] = pathfindList[node2];
            pathfindList[node2] = temp;
        }

        // Name: printNodeHeap
        // Description: Simple method that prints the first few values of the
        //              list. Magic numbers exist here because the method only
        //              exists for debugging purposes
        public static void printNodeHeap()
        {
            for (int i = 0; i < 15; i++)
            {
                Console.Write(pathfindList[i].fScore + " ");
            }
            Console.WriteLine();
        }

        // Name: isHeapEmpty
        // Description: Returns true if endTracker refers to an end that is the
        //              beginning of the list, ie, there are no entries
        // Returns: bool, true if the list is empty
        public static bool isHeapEmpty()
        {
            return endTracker == 0;
        }
    }
}
