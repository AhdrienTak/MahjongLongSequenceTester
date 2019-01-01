using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongHandTenpaiTester
{
    class Program
    {
        private static Wall wall;
        private static Hand hand;

        static void Main(string[] args)
        {

            //Instantiate the drawing wall
            wall = new Wall();

            //Instantiate the Mahjong tiles.
            for (int i = 0; i < 4; i++)
            {
                //Create Dragons
                wall.AddTile(new Tile("dr")); //Create Red   Dragons
                wall.AddTile(new Tile("dw")); //Create White Dragons
                wall.AddTile(new Tile("dg")); //Create Green Dragons
                //Create Winds
                wall.AddTile(new Tile("we")); //Create East  Winds
                wall.AddTile(new Tile("ws")); //Create South Winds
                wall.AddTile(new Tile("ww")); //Create West  Winds
                wall.AddTile(new Tile("wn")); //Create North Winds
                //Create suits
                for (int j = 1; j < 10; j++)
                {
                    //TODO Assign akadora
                    wall.AddTile(new Tile("m" + j)); //Create Man
                    wall.AddTile(new Tile("p" + j)); //Create Pin
                    wall.AddTile(new Tile("s" + j)); //Create Sou
                }
            }

            //Shuffle the Mahjong deck.
            wall.Shuffle();

            //Deal a 13 tile hand with a 14th "draw"
            hand = new Hand(wall);
            hand.Draw(wall.DrawTile());
            hand.Print();

            //Begin Testing loop
            Console.Write("Command: ");
            string command = Console.ReadLine();
            while (!command.ToLower().Equals("exit"))
            {
                //List Tenpai combinations and their point values
                if (command.ToLower().Equals("check"))
                {
                    LegalHandChecker(hand.GetCopyOfTiles());
                }

                // Discard a held tile
                else if (command.ToLower().Contains("discard"))
                {
                    if (hand.DiscardTile())
                    {
                        hand.Draw(wall.DrawTile());
                    }
                }

                // Reset the wall and draw a new hand
                else if (command.ToLower().Contains("reset"))
                {
                    //Add all of the hand tiles back to the wall
                    for (int i = 1; i < 15; i++)
                    {
                        wall.AddTile(hand.GetTile(i));
                    }
                    //Shuffle the wall
                    wall.Shuffle();
                    //Draw a new hand
                    hand = new Hand(wall);
                    hand.Draw(wall.DrawTile());
                }

                //Get the user's next command
                hand.Print();
                Console.Write("Command: ");
                command = Console.ReadLine();
            }

        }

        private static void LegalHandChecker(Tile[] curHand)
        {
            //TODO Create a lookup table of hands already examined (for pruning)

            //TODO Detect any sequences that are length 4 in the hand
            int sequenceIndex = 0;
            while (sequenceIndex < curHand.Length - 2)
            {
                if (curHand[sequenceIndex].GetTileType() == 2) //If the tile is not an honor
                {
                    //Check the tile's suit
                    char suit = curHand[sequenceIndex].GetName().ElementAt(0);
                    int value = curHand[sequenceIndex].GetId();

                    //Check if next 3 tiles are a same-suit sequence
                    if (curHand[sequenceIndex + 1].GetName().ElementAt(0) == suit && curHand[sequenceIndex + 1].GetId() == value + 1 &&
                        curHand[sequenceIndex + 2].GetName().ElementAt(0) == suit && curHand[sequenceIndex + 2].GetId() == value + 2 &&
                        curHand[sequenceIndex + 3].GetName().ElementAt(0) == suit && curHand[sequenceIndex + 3].GetId() == value + 3 )
                    {
                        //TODO perform a long-sequence temp meld
                    }
                    sequenceIndex++;
                }
            }
        }

        //This tool checks for legal Tenpais, given a set of tiles
        private void TenpaiChecker()
        {
            //TODO Perform a forward pass with sequence prioritization
            //TODO Perform a forward pass with triplet prioritization
            //TODO Perform a backward pass with sequence prioritization
            //TODO Perform a backward pass with triplet prioritization
        }

    }

    public class Tile
    {
        private string name;
        private int id;
        private int type; // 2 = honor, 1 = terminal, 0 = simple
        private Boolean red;

        //Tile Constructor
        public Tile(string tileName)
        {
            //Initialize attributes
            name = tileName;
            id = -1;
            type = -1;
            red = false;

            //Assign the tile's name and ID, unless it is invalid
            //----
            //Check for dragons
            if (tileName.ElementAt(0) == 'd')
            {
                switch (tileName.ElementAt(1))
                {
                    case 'r': id = 27; break;
                    case 'w': id = 28; break;
                    case 'g': id = 29; break;
                }
                type = 2;
            }
            //Check for winds
            else if (tileName.ElementAt(0) == 'w')
            {
                switch (tileName.ElementAt(1))
                {
                    case 'e': id = 30; break;
                    case 's': id = 31; break;
                    case 'w': id = 32; break;
                    case 'n': id = 33; break;
                }
                type = 2;
            }
            //Check for suits
            else
            {
                //Get the numerical value of the tile
                int value;
                try
                {
                    //Assign the 1-9 value of the tile
                    value = Int32.Parse(tileName.Substring(1, 1));

                    //Multiply for the suit (1:man ; 2:pin ; 3:sou)
                    switch (tileName.ElementAt(0))
                    {
                        case 'm': value +=  0; break;
                        case 'p': value +=  9; break;
                        case 's': value += 18; break;
                    }

                    //Assign the final calculated ID number
                    id = value;

                    //Determine if the tile is a terminal
                    if (tileName.ElementAt(1) == '1' || tileName.ElementAt(1) == '9')
                    {
                        type = 1;
                    } else
                    {
                        type = 0;
                    }
                }
                //Throw an error if the tile's name is invalid
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        //Cloning Constructor
        public Tile(Tile sourceTile)
        {
            //Copy Attributes
            name = sourceTile.GetName();
            id = sourceTile.GetId();
            type = sourceTile.GetTileType();
            red = sourceTile.IsRed();
        }

        //Mutator that marks a tile as an Aka-Dora
        public void SetRed(Boolean akaDora) { red = akaDora; }

        //Accessor Methods
        public string GetName() { return name; }
        public int GetId() { return id; }
        public int GetTileType() { return type; }
        public Boolean IsRed() { return red; }
    }

    public class Hand
    {
        private Tile[] hand;
        private Tile tile14;

        //Constructor
        public Hand()
        {
            hand = new Tile[13];
            tile14 = null;
        }

        //Constructor Actual
        public Hand(Wall inWall)
        {
            hand = new Tile[13];
            DrawInitialHand(inWall);
        }

        //Draw 13 Tiles from wall
        public void DrawInitialHand(Wall inWall)
        {
            for (int i = 0; i < 13; i++) { hand[i] = inWall.DrawTile(); }
            SortHand();
        }

        //Sorts Hand
        public void SortHand()
        {
            Array.Sort(hand, delegate (Tile t1, Tile t2) { return t1.GetId().CompareTo(t2.GetId()); });
        }

        //Draw 1 Tile from wall
        public void Draw(Tile inTile)
        {
            tile14 = inTile;
        }

        //Peek at held tile
        public Tile GetTile(int index)
        {
            index--;
            if (index == 13) { return tile14; }
            return hand[index];
        }

        //Returns a copy to avoid mutating the source hand
        public Tile[] GetCopyOfTiles()
        {
            Tile[] outSet = new Tile[13];
            for (int i = 0; i < hand.Length; i++)
            {
                outSet[i] = hand[i];
            }
            return outSet;
        }

        //TODO discard a tile currently just destroys it; it normally goes to a discard pool
        public Boolean DiscardTile()
        {
            Boolean success = true;
            Console.Write("Select tile to discard: ");
            string command = Console.ReadLine();
            for (int i = 0; i < hand.Length + 1; i++)
            {
                if (i == 13)
                {
                    if (tile14.GetName().ToLower().Equals(command.ToLower()))
                    {
                        tile14 = null;
                    }
                    else
                    {
                        success = false;
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine("Tile not found, canceling operation");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else if (i < 13 && hand[i].GetName().ToLower().Equals(command.ToLower()))
                {
                    hand[i] = tile14;
                    tile14 = null;
                    break;
                }
            }
            SortHand();
            return success;
        }

        //Print the contents of the hand to the console
        public void Print()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Hand:  ");
            
            //Print the 13 hand tiles
            for (int i = 0; i < hand.Length; i++)
            {
                Console.BackgroundColor = ConsoleColor.White;
                string nextName = hand[i].GetName();
                if     (nextName.ElementAt(0) == 'm' || nextName.Equals("dr")) { Console.ForegroundColor = ConsoleColor.DarkRed;   }
                else if(nextName.ElementAt(0) == 'p' || nextName.Equals("dw")) { Console.ForegroundColor = ConsoleColor.DarkBlue;  }
                else if(nextName.ElementAt(0) == 's' || nextName.Equals("dg")) { Console.ForegroundColor = ConsoleColor.DarkGreen; }
                else { Console.ForegroundColor = ConsoleColor.Black; }
                Console.Write(" " + nextName + " ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ");
            }

            //Print the 14th Tile
            Console.Write(" :[ ");
            Console.BackgroundColor = ConsoleColor.White;
            string name14 = tile14.GetName();
            if      (name14.ElementAt(0) == 'm' || name14.Equals("dr")) { Console.ForegroundColor = ConsoleColor.DarkRed; } 
            else if (name14.ElementAt(0) == 'p' || name14.Equals("dw")) { Console.ForegroundColor = ConsoleColor.DarkBlue; } 
            else if (name14.ElementAt(0) == 's' || name14.Equals("dg")) { Console.ForegroundColor = ConsoleColor.DarkGreen; } 
            else { Console.ForegroundColor = ConsoleColor.Black; }
            Console.Write(" " + name14 + " ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ]:");
        }
    }

    public class Wall
    {
        private Random rand = new Random();
        private LinkedList<Tile> wall;

        public Wall()
        {
            wall = new LinkedList<Tile>();
        }

        public void AddTile(Tile tile)
        {
            wall.AddLast(tile);
        }

        public void Shuffle()
        {
            for (int i = 0; i < 3; i++)
            {
                LinkedList<Tile> newDeck = new LinkedList<Tile>();
                while (wall.Count > 0)
                {
                    int targetIndex = rand.Next(wall.Count());
                    Tile nextTile = wall.ElementAt(targetIndex);
                    wall.Remove(nextTile);
                    newDeck.AddLast(nextTile);
                }
                wall = newDeck;
            }
        }

        public Tile DrawTile()
        {
            if (wall.Count > 0)
            {
                Tile outputTile = wall.ElementAt(0);
                wall.RemoveFirst();
                return outputTile;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            string outputString = "";
            for (int i = 0; i < wall.Count(); i++)
            {
                outputString += wall.ElementAt(i).GetName();
                outputString += "\n";
            }
            return outputString;
        }
    }

}
