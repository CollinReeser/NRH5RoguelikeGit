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
//              all of the parameters, it will save time in !constantly creating
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
//
// TODO:
// - Implement algorithm
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRH5Roguelike.Utility
{
    class Pathfinder
    {
        // This refers to the number of attributes a given node has within the
        // context of the pathfinder searchspace
        private static const short NUM_OF_ELEMENTS = 5;
        // These are constants for indexing in the searchSpace array
        private static const short G_SCORE_INDEX = 0;
        private static const short H_SCORE_INDEX = 1;
        private static const short PARENT_INDEX = 2;
        private static const short NODE_TYPE_INDEX = 3;
        private static const short LIST_INDEX = 4;
        // These are used to differentiate node types
        private static const short END_NODE = -1;
        private static const short NORMAL_NODE = 0;
        private static const short START_NODE = 1;
        // Used to differentiate lists
        private static const short CLOSED_LIST = -1;
        private static const short NOT_LISTED = 0;
        private static const short OPEN_LIST = 1;
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
        private static short[][][] searchSpace;

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
        public static PathfindData pathfind( Entity.Monster monster,
            Dungeon.DungeonLevel level , short startXCoord , short startYCoord , 
            short endXCoord , short endYCoord , short searchDistance = 0 , 
            bool guesspath = false , byte algorithmToUse = 0 )
        {
            // Determine if the map we're searching on is larger in some way
            // than the searchspace previously defined
            if ( Pathfinder.searchSpace.Length < level.getHeight() ||
                Pathfinder.searchSpace[0].Length < level.getWidth() )
            {
                Pathfinder.searchSpace = 
                    new short[level.getHeight()][level.getWidth()]
                        [NUM_OF_ELEMENTS];
            }
            // Temp code for compilation
            return new PathfindData( (byte) Direction.NORTH, 0 );
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
            for ( short y = 0; y < Pathfinder.searchSpace.Length; y++ )
            {
                for (short x = 0; x < Pathfinder.searchSpace[y].Length; x++)
                {
                    Pathfinder.searchSpace[y][x][LIST_INDEX] = NOT_LISTED;
                }
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
        public PathfindData( byte directionOfPath , short lengthOfPath )
        {
            this.directionOfPath = directionOfPath;
            this.lengthOfPath = lengthOfPath;
        }
        // The data members of the PathfindData struct. They are readonly so
        // that while they can be initialized with any data value that is
        // desired or needed at runtime, they cannot thereafter be changed
        public readonly byte directionOfPath;
        public readonly short lengthOfPath;
    }
    // Name: Direction
    // Description: Describes a tile direction, where NORTH is 0 and the values
    //              thereafter are the value previous plus 1. NORTHEAST is 1.
    public enum Direction
    {
        NORTH, NORTHEAST, EAST, SOUTHEAST, SOUTH, SOUTHWEST, WEST,
        NORTHWEST
    }
}
