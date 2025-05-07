using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

// Command Line Blackjack
// by Duncan Clark
//------------------------------------------------------------------------------------------------------------
// Just a simple C# Blackjack game for the command line. Made it as an introduction project to C# programming.
// -----------------------------------------------------------------------------------------------------------
// TODO: Add betting system
// TODO: Handle input errors
// TODO: Make sure the deck can't run out of cards mid-round

class CommandLineBlackjack
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Blackjack!\n");

        int player_count = Game.ChoosePlayerCount();

        Console.WriteLine((player_count > 1) ? "\nYou have choosen " + player_count + " players.\n" : "\nWelcome!\n");

        Player[] players = new Player[player_count];

        for (int i = 0; i < player_count; i++)
        {
            Console.Write((player_count > 1) ? "Enter name for player " + (i + 1) + ": " : "Enter your name: ");
            string name = Console.ReadLine();
            players[i] = new Player();
            players[i].name = name;
        }

        if (players.Length > 1)
        {
            Console.Write("\nThe turn order is:");

            for (int i = 0; i < players.Length; i++)
            {
                Console.Write((i != players.Length - 1) ? " " + players[i].name + "," : " then " + players[i].name + ".\n");
            }
        } 

        Console.WriteLine("\nLet's begin!");

        Thread.Sleep(2300);

        bool playing = true;

        Deck deck = new Deck();
        deck.FillDeck();
        deck.Shuffle();

        Dealer dealer = new Dealer();

        Game game = new Game(deck, players, dealer);

        while (playing)
        {
            game.Round();
        }
    }
}

public class Game(Deck deck, Player[] players, Dealer dealer)
{

    public int roundCount = 0;
    public static int ChoosePlayerCount()
    {
        int count = 0;

        while (count > 4 || count < 1)
        {
            Console.Write("Choose the number of players (1-4): ");
            count = Convert.ToInt32(Console.ReadLine());

            if (count == 0)
            {
                Console.WriteLine("Not enough players. Try again.\n");
            }
            else if (count > 4)
            {
                Console.WriteLine("Too many players. Try again.\n");
            }
        }

        return count;
    }

    public void DealOut()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < players.Count(); j++)
            {
                players[j].hand.Add(deck.Deal());
            }

            dealer.hand.Add(deck.Deal());
        }

        dealer.hand[0].faceUp = true;
    }

    public void Round()
    {
        screenClear();

        string roundStartMarquee = " Round " + ++roundCount + " begins!";
        drawBox(roundStartMarquee);

        if (deck.Count() <= 24 || roundCount == 1)
        {
            deck.Shuffle();
            Console.WriteLine("\n  - Shuffling cards...");
            Thread.Sleep(1000);
        }

        Console.WriteLine("\n  - Dealing cards...\n\n");
        Thread.Sleep(1000);
        DealOut();

        Thread.Sleep(800);

        for (int i = 0; i < players.Count(); i++)
        {
            Turn(players[i]);
        }

        Turn(dealer);
        results(dealer, players);

        for (int j = 0; j < players.Count(); j++)
        {
            players[j].hand.Clear();
        }

        dealer.hand.Clear();
    }

    public void Turn(Player player)
    {
        Console.WriteLine("Round " + roundCount + "\n-------\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        Console.Write("(" + player.name + ") It is your turn. Press any key to begin: ");
        Console.ReadLine();

        for (int n = 0; n < player.name.Length + 42; n++)
        {
            Console.Write("-");
        }

        // Goofy grammer check that should probably be a function instead

        Card grammer = new Card();
        char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
        bool vowelCheck = false;

        Console.Write("\n\n  - The dealer has ");
        
        for (int i = 0; i < vowels.Length; i++)
        {
            if (grammer.FaceValue[dealer.hand[0].value - 2][0] == vowels[i])
            {
                Console.Write("an ");
                vowelCheck = true;
                break;
            }
        }

        if (!vowelCheck)
        {
            Console.Write("a ");
        }

        Console.Write(dealer.hand[0].PrintCard() + ".");


        bool hitting = true;
        int hitCount = 0;

        while (hitting)
        {
            hitCount++;
            Console.Write("\n\n(" + player.name + ") Your cards are: ");
            player.printHand();
            Console.WriteLine("\n(" + player.name + ") Your hand value is: " + player.evaluateScore() + "\n");
      

            if (player.evaluateScore() == 21)
            {   
                Console.WriteLine((hitCount != 1) ? "(" + player.name + ") You got a BlackJack!" : "(" + player.name + ") You started with a Blackjack!");
                Thread.Sleep(1200);
                hitting = false;
            }
            else if (player.evaluateScore() > 21)
            {
                Console.WriteLine("(" + player.name + ") You bust.");
                Thread.Sleep(1200);
                hitting = false;
            }
            else
            {
                Console.Write("\n(" + player.name + ") Would you like to hit? (Y/N): ");

                string? confirm;
                confirm = Console.ReadLine();

                if (confirm != null && (confirm == "y" || confirm == "Y" || confirm == "Yes" || confirm == "yes"))
                {
                    player.hand.Add(deck.Deal());
                }
                else if (confirm != null && (confirm == "n" || confirm == "N" || confirm == "No" || confirm == "no"))
                {
                    hitting = false;
                    Console.Write("\n(" + player.name + ") You stand.");
                }
                else
                {
                    Console.WriteLine("\nCommand not recognized.");
                }
            }
        }

        Console.Write("\n\n(" + player.name + ") Press any key to end your turn: ");
        Console.ReadLine();
        Console.WriteLine("\n");
    }

    public void Turn(Dealer dealer)
    {
        Thread.Sleep(400);
        screenClear();
        Console.WriteLine("Round " + roundCount + "\n-------\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        Console.WriteLine("\n\n(Dealer): It is now the dealer's turn.");

        Thread.Sleep(1200);

        bool hitting = true;

        while (hitting)
        {
            Console.Write("\n(Dealer): The dealer has: ");
            dealer.printHand();
            Console.WriteLine("\n(Dealer): The dealer's score is: " + dealer.evaluateScore());
            Console.WriteLine("\n");
            Thread.Sleep(1600);

            if (dealer.evaluateScore() <= 16)
            {
                Console.WriteLine("(Dealer): The dealer hits.\n");
                dealer.hand.Add(deck.Deal());
                Thread.Sleep(1600);
            }
            else if (dealer.softSeventeen())
            {
                Console.WriteLine("(Dealer): The dealer has a soft seventeen and must hit.\n");
                dealer.hand.Add(deck.Deal());
                Thread.Sleep(1600);
            }
            else if (dealer.evaluateScore() >= 17 && dealer.evaluateScore() < 21)
            {
                Console.WriteLine("(Dealer): The dealer stays.\n");
                hitting = false;
                Thread.Sleep(1600);
            }
            else if (dealer.evaluateScore() == 21)
            {
                Console.WriteLine("(Dealer): Blackjack!\n");
                hitting = false;
                Thread.Sleep(2000);
            }
            else if (dealer.evaluateScore() > 21)
            {
                Console.WriteLine("(Dealer): Dealer busts!\n");
                hitting = false;
                Thread.Sleep(2000);
            }
        }
    }

    public void results(Dealer dealer, Player[] players)
    {
        screenClear();

        string roundMarquee = "Round " + roundCount + " Results";

        drawBox(roundMarquee);
        Thread.Sleep(1000);

        Console.WriteLine((dealer.evaluateScore() < 22) ? "  - The dealer's score is: " + dealer.evaluateScore() : "  - The dealer busted.");
        Console.WriteLine("\n\n");
        Thread.Sleep(1000);



        string longestName = "";

        for (int l = 0; l < players.Length; l++)
        {
            if (players[l].name.Length > longestName.Length)
            {
                longestName = players[l].name;
            }
        }

        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];

            int difference = longestName.Length - player.name.Length;
            double convert = (difference / 2) + 2;

            for (int j = 0; j < Math.Ceiling(convert); j++)
            {
                Console.Write(" ");
            }

            Console.Write("[" + player.name + "]");

            for (int m = 0; m < Math.Ceiling(convert); m++)
            {
                Console.Write(" ");
            }

            Console.Write(":");

            // Evaluating winning and losing condintions:

            if (player.evaluateScore() > dealer.evaluateScore() && player.evaluateScore() < 21)
            {
                Console.Write(" Your score of " + player.evaluateScore() + " beat the dealer's " + dealer.evaluateScore() + ".");
                Thread.Sleep(1000);
            }
            else if (player.evaluateScore() < 21 && dealer.evaluateScore() > 21)
            {
                Console.Write(" Your score of " + player.evaluateScore() + " beat the dealer's bust.");
                Thread.Sleep(1000);
            }
            else if (player.evaluateScore() == 21 && dealer.evaluateScore() != 21)
            {
                Console.Write(" You beat the dealer's " + dealer.evaluateScore() + " with a Blackjack!");
                Thread.Sleep(1000);
            }
            else if (player.evaluateScore() == 21 && dealer.evaluateScore() == 21)
            {
                Console.Write(" There is a push. You and the dealer both have a Blackjack.");
                Thread.Sleep(1000);
            }
            else if (player.evaluateScore() == dealer.evaluateScore() && player.evaluateScore() < 21)
            {
                Console.Write(" There is a push. You and the dealer both have a score of " + player.evaluateScore() + ".");
                Thread.Sleep(1000);
            }
            else if (player.evaluateScore() > 21)
            {
                Console.Write(" You bust with a score of " + player.evaluateScore() + ".");
                Thread.Sleep(1000);
            }
            else if (player.evaluateScore() < dealer.evaluateScore() && dealer.evaluateScore() < 22)
            {
                Console.Write(" The dealer's score of " + dealer.evaluateScore() + " beats your " + player.evaluateScore() + ".");
                Thread.Sleep(1000);
            }
            else
            {
                Console.WriteLine("  ERROR: EDGE CASE");
            }

            Console.Write("\n  ");

            for (int n = 0; n < longestName.Length + 2; n++)
            {
                Console.Write("-");
            }

            Console.WriteLine("\n");
        }

        Console.Write("\n\n  - Press any key to move to the next round: ");
        Console.ReadLine();
        Thread.Sleep(800);
    }

    public void screenClear()
    {
        Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
    }

    public void drawBox(string word)
    {
        double boxLength = word.Length * 1.5;

        Console.Write("  ");

        for (double i = 0; i < Math.Ceiling(boxLength); i++)
        {
            Console.Write("-");
        }

        Console.WriteLine("");

        for (double j = 0; j < Math.Ceiling(boxLength) / 4; j++)
        {
            Console.Write(" ");
        }

        Console.Write(word);

        for (double j = 0; j < Math.Ceiling(boxLength) / 4; j++)
        {
            Console.Write(" ");
        }
        Console.Write("\n  ");

        for (double i = 0; i < Math.Ceiling(boxLength); i++)
        {
            Console.Write("-");
        }

        Console.WriteLine("\n");
    }
}

public class Player
{
    public string ? name;
    int chips;
    public List<Card> hand = new List<Card>();

    public void printHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            Console.Write(hand[i].PrintCard());

            if (i < hand.Count - 1) 
            { 
                Console.Write(" / "); 
            }
        }
    }

    public int evaluateScore()
    {
        int score = 0, aces = 0;

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].BJ_Value == 11)
            {
                aces++;
            }
            else
            {
                score += hand[i].BJ_Value;
            }
        }

        for (int j = 0; j < aces; j++)
        {
            if (score + (aces * 11) <= 21)
            {
                score += 11;
            }
            else
            {
                score += 1;
            }
        }

        return score;
    }
}

public class Dealer : Player
{
    public bool softSeventeen()
    {
        int aceCount = 0;

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].BJ_Value == 11)
            {
                aceCount++;
            }
        }

        if (aceCount > 0 && evaluateScore() == 17)
        {
            return true;
        }

        return false;
    }
}

public class Card
{
    public string ? suit;
    public int value, placement, BJ_Value;
    public bool faceUp = false;

    public string[] suits = {"Hearts", "Diamonds", "Clubs", "Spades"};

    public string[] FaceValue = {"Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", "Ace"};

    public string PrintCard()
    {
        return FaceValue[value - 2] + " of " + suit;
    }

    public void CreateCard(string pSuit, int pValue, int pBJ_Value)
    {
        suit = pSuit;
        value = pValue;
        BJ_Value = pBJ_Value;
    }
}

public class Deck
{
    List<Card> deck = new List<Card>();
    List<Card> exhaustPile = new List<Card>();
    List<Card> lastRoundsExhaustPiles = new List<Card>();

    public void FillDeck()
    {
        for (int j = 0; j < 4; j++) {
        
            for (int i = 0; i < 13; i++)
            {
                int tally = (j * 13) + i;
                Card newCard = new Card();
                
                if (i < 12 && i > 8)
                {
                    newCard.CreateCard(newCard.suits[j], i + 2, 10);
                }
                else if (i == 12)
                {
                    newCard.CreateCard(newCard.suits[j], i + 2, 11);

                }
                else
                {
                    newCard.CreateCard(newCard.suits[j], i + 2, i + 2);
                }

                deck.Add(newCard);
                deck[tally].placement = tally;
            }
        }
    }

    public Card Deal()
    {
        Card topCard = deck[0];
        exhaustPile.Add(topCard);
        deck.RemoveAt(0);

        return topCard;
    }

    public Card Deal(bool faced)
    {
        Card topCard = deck[0];
        topCard.faceUp = faced;
        exhaustPile.Add(topCard);
        deck.RemoveAt(0);

        return topCard;
    }

    public void Shuffle()
    {
        deck.Union(exhaustPile);
        Deck swap = new Deck();
        swap.FillDeck();

        List<int> taken = new List<int>();

        Random rnd = new Random();

        for (int i = 0; i < deck.Count; i++)
        {
            bool check = false;

            while (!check)
            {
                int spot = rnd.Next(0, 52);

                if (!taken.Contains(spot))
                {
                    taken.Add(spot);
                    deck[i] = swap.deck[spot];
                    check = true;
                }
            }
        }

        exhaustPile.Clear();
    }

    public void PrintDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Console.WriteLine(deck[i].PrintCard());
        }
    }
    public int Count()
    {
        return deck.Count;
    }
}


