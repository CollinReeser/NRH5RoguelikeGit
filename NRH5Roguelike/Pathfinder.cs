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
//              wieghts of travelling through different pices of terrain
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
//              returned, can be called with a 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRH5Roguelike.Utility
{
    class Pathfinder
    {

    }
}
