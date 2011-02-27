// Name: RoguelikeMain
// Description: This is the main program that initiates the roguelike game
// Author: Collin Reeser
// Contributors: Alex Williams
//               Evan Gifford
// Log: 
// - Created a ton of simple interacting things so that we can ease into using
//   the graphical library, and documented them heavily -- Collin Reeser
//
// TODO:
// - See if the credits animation screwing with the entire background thing can
//   be fixed
// - Actually implement the main loop and all that necessary nonsense for an
//   actual game
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;

namespace NRH5Roguelike
{
    class RoguelikeMain
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine( "Seriously even!" );
            Console.WriteLine("Commit by Alex Williams.");
            Console.WriteLine("I want you inside me.");
            Console.Beep();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            // Define constants for the size of the window so that window-
            //dependant things can be easily updated
            const int WINDOW_WIDTH = 100;
            const int WINDOW_HEIGHT = 80;
            // Display an initializing message to the console
            Console.WriteLine( "Initilizing game window..." );
            // Create the new libtcod output window. This needs to be done only
            // once, but before anything else. The first value is the width in
            // cells the window is, the second is the height in cells, and the
            // third is the name of the window
            TCODConsole.initRoot( WINDOW_WIDTH , WINDOW_HEIGHT , 
                "Hello World!" );
            TCODRendererType renderer = TCODSystem.getRenderer();
            // Print to the console what renderer your machine is using. GLSL
            // is "best", but supported by fewer machines. Then comes OpenGL,
            // then straight SDL
            Console.WriteLine("Renderer is: " + renderer.ToString());
            // Load the "skull.png" image file from the exectuable directory
            TCODImage skullImage = new TCODImage( "skull.png" );
            // Set the maximum graphical update speed. This is important to
            // massively reduce CPU use with unnecessary updates
            TCODSystem.setFps( 30 );
            // Set the default background color of the root console with black
            TCODConsole.root.setBackgroundColor( TCODColor.black );
            // Clear is a function that overwrites every cell in the window with
            // a plain, empty cell of the color dictated by setBackgroundColor()
            TCODConsole.root.clear();
            // Flush writes the graphical data that is pending to the screen.
            // Any graphics updates are not shown to the screen until flush is
            // called. Try to minimize this call
            TCODConsole.flush();
            // Create a key variable to capture keyboard input
            TCODKey Key = TCODConsole.checkForKeypress();
            // These two variables will track the location of the player sprite
            int x = 0; 
            int y = 0;
            // Boolean used to determine if the credits have been played yet,
            // and ensures it is only played once
            bool creditsDone = false;
            // While the user has not closed the window and while the user has
            // not pressed escape, do stuff
            while ( !TCODConsole.isWindowClosed() && Key.KeyCode != 
                TCODKeyCode.Escape )
            {
                // Make sure the screen is blank before writing on it again with
                // graphical updates
                TCODConsole.root.clear();
                // Display the credits graphic for the libtcod library
                if ( !creditsDone && 
                    TCODConsole.renderCredits( 50 , 50 , false ) )
                {
                    creditsDone = true;
                }
                // Block until the user presses a key, then capture that key
                Key = TCODConsole.checkForKeypress(
                    (int)TCODKeyStatus.KeyPressed);
                // If the key pressed is an arrow key, update the location that
                // the character sprite will be rendered at. However, ensure
                // that the location of the character never goes out of bounds
                // of the screen
                if ( Key.KeyCode == TCODKeyCode.Down )
                {
                    y = (y + 1 < WINDOW_HEIGHT ) ? y + 1 : y;
                }
                else if ( Key.KeyCode == TCODKeyCode.Up )
                {
                    y = ( y - 1 > -1 ) ? y - 1 : y;
                }
                else if ( Key.KeyCode == TCODKeyCode.Right )
                {
                    x = ( x + 1 < WINDOW_WIDTH ) ? x + 1 : x;
                }
                else if ( Key.KeyCode == TCODKeyCode.Left )
                {
                    x = ( x - 1 > -1 ) ? x - 1 : x;
                }
                // If the player is 30 cells in the x and y direction away from
                // the origin, display text in a window frame around the moving
                // player
                if ( x >= 30 && y >= 30 )
                {
                    // Print the frame, where the top left origin of the frame
                    // is the length of the first string to the left of the
                    // player, and up one cell from the player, and the box
                    // extends to twice + 1 the length of the strings
                    // (accomodating the players sprite), and three cells down
                    TCODConsole.root.printFrame( x - 16 , y - 1 , 33 , 3 );
                    // Set the color to be used for the strings
                    TCODConsole.root.setForegroundColor( TCODColor.magenta );
                    // Print the strings before and after the player, with the
                    // proper alignement
                    TCODConsole.root.printEx( x - 1 , y , 
                        TCODBackgroundFlag.Set , TCODAlignment.RightAlignment , 
                        "TROLOLOLOLOL XD" );
                    TCODConsole.root.printEx( x + 1 , y , 
                        TCODBackgroundFlag.Set , TCODAlignment.LeftAlignment ,
                        " DX TROLOLOLOLOL" );
                    // Revert the foreground color back to white for printing
                    // everything else
                    TCODConsole.root.setForegroundColor( TCODColor.white );
                }
                // If the player sits inside cell (45,45), draw the skull to
                // the screen, where the center of the skull is at (20,20)
                if ( x == 45 && y == 45 )
                {
                    skullImage.blit( TCODConsole.root , 20f , 20f );
                }
                // Print a helpful hint at the bottom left corner, with a left
                // alignment
                TCODConsole.root.printEx( 0 , WINDOW_HEIGHT - 1 , 
                    TCODBackgroundFlag.Set , TCODAlignment.LeftAlignment , 
                    "Go to (45,45)!" );
                // Display the x and y coordinates in the bottom right
                // corner, each with a right alignment
                TCODConsole.root.printEx( 96 , 79 , TCODBackgroundFlag.Set ,
                    TCODAlignment.RightAlignment , "" + x );
                TCODConsole.root.printEx( 99 , 79 , TCODBackgroundFlag.Set ,
                    TCODAlignment.RightAlignment , "" + y );
                // Print the character sprite with a center alignment
                TCODConsole.root.printEx( x , y , TCODBackgroundFlag.Set , 
                    TCODAlignment.CenterAlignment , "@" );
                // Flush the changes to the screen
                TCODConsole.flush();
            }
        }
    }
}
