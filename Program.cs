using System;
using System.Collections.Generic;


namespace BlackJack
{
    class Program
    {
        private static Dictionary<string, int> cardDeck = new Dictionary<string, int>()
            {
                {"ace_of_clubs", 1}, {"ace_of_hearts", 1}, {"ace_of_spades", 1}, {"ace_of_diamonds", 1},
                {"2_of_clubs", 2}, {"2_of_hearts", 2}, {"2_of_spades", 2}, {"2_of_diamonds", 2},
                {"3_of_clubs", 3}, {"3_of_hearts", 3}, {"3_of_spades", 3}, {"3_of_diamonds", 3},
                {"4_of_clubs", 4}, {"4_of_hearts", 4}, {"4_of_spades", 4}, {"4_of_diamonds", 4},
                {"5_of_clubs", 5}, {"5_of_hearts", 5}, {"5_of_spades", 5}, {"5_of_diamonds", 5},
                {"6_of_clubs", 6}, {"6_of_hearts", 6}, {"6_of_spades", 6}, {"6_of_diamonds", 6},
                {"7_of_clubs", 7}, {"7_of_hearts", 7}, {"7_of_spades", 7}, {"7_of_diamonds", 7},
                {"8_of_clubs", 8}, {"8_of_hearts", 8}, {"8_of_spades", 8}, {"8_of_diamonds", 8},
                {"9_of_clubs", 9}, {"9_of_hearts", 9}, {"9_of_spades", 9}, {"9_of_diamonds", 9},
                {"10_of_clubs", 10}, {"10_of_hearts", 10}, {"10_of_spades", 10}, {"10_of_diamonds", 10},
                {"king_of_clubs", 10}, {"king_of_hearts", 10}, {"king_of_spades", 10}, {"king_of_diamonds", 10},
                {"queen_of_clubs", 10}, {"queen_of_hearts", 10}, {"queen_of_spades", 10}, {"queen_of_diamonds", 10},
                {"jack_of_clubs", 10}, {"jack_of_hearts", 10}, {"jack_of_spades", 10}, {"jack_of_diamonds", 10},

            };
        private static Dictionary<string, int> playerScores = new Dictionary<string, int>()
        {
            {"dealer", 0 }
        };

        static void Main(string[] args)
        {
            //currently hardcoded for one player but some simple changes could accept args from main to specify how many
            playerScores.Add("player0", 0);
            while (gameStart(1, 1)) { }
            finalScreen();           
        }

        private static void finalScreen()
        {
            Console.WriteLine("The final scores are");
            foreach(KeyValuePair<string, int> player in playerScores)
            {
                Console.WriteLine($"{player.Key} scored {player.Value}");
            }
            Console.WriteLine("press enter key to close");
            Console.ReadLine();
        }

        private static bool gameStart(int numOfDecks, int numOfPlayers)
        {
            List<KeyValuePair<string, int>> score = new List<KeyValuePair<string, int>>();
            Dictionary<string, List<string>> decksInPlay = new Dictionary<string, List<string>>();
            decksInPlay.Add("dealer", new List<string>());
            for (int playerNum = 0; playerNum < numOfPlayers; playerNum++)
            {
                decksInPlay.Add($"player{playerNum}", new List<string>());
                score.Add(new KeyValuePair<string, int>($"player{playerNum}", 0));
            }
            score.Add(new KeyValuePair<string, int>("dealer", 0));
            var keys = cardDeck.Keys;
            List<string> currentDeck = new List<string>();
            for (int deckNum = 0; deckNum < numOfDecks; deckNum++)
            {
                currentDeck.AddRange(keys);
            }
            shuffle(currentDeck);
            foreach (List<string> deck in decksInPlay.Values)
            {
                draw(deck, currentDeck);//draw simply takes a card from currentDeck and adds it to the named deck in play
                draw(deck, currentDeck);
            }

            displayBoard(decksInPlay);
            addUpScore(decksInPlay, score); //orders score so first position has most points
            if (score[0].Value == 21)
            {
                if (score[1].Value == 21)
                {
                    playerScores[score[0].Key]++;
                    playerScores[score[1].Key]++;
                    //dealer and player score one point
                }
                playerScores[score[0].Key] += 2;
                winningScreen(score, decksInPlay);
                Console.WriteLine("Would you like to play another round?\nType yes or no");
                string playAgain = Console.ReadLine();
                Console.WriteLine();
                if (playAgain == "yes")
                {
                    return true;
                }
                return false;
            }

            foreach (string player in decksInPlay.Keys)
            {
                if (player == "dealer")
                {
                    continue;
                }
                while (score[findPlayerIndex(player, score)].Value < 21 && score[findPlayerIndex(player, score)].Value != 0)
                {
                    Console.WriteLine($"{player} Type hit to draw, Type hold to stop");
                    string playerChoice = Console.ReadLine();
                    if (playerChoice == "hit")
                    {
                        draw(decksInPlay[player], currentDeck);
                        addUpScore(decksInPlay, score);
                        displayBoard(decksInPlay);
                        if (score[findPlayerIndex(player, score)].Value == 0)
                        {
                            Console.WriteLine("Over 21 you are busted");
                            break;
                        }
                        
                    }
                    else if (playerChoice == "hold")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please type hit or hold");
                    }
                }            
            }
            int dealerIndex = findPlayerIndex("dealer", score);
            while (score[dealerIndex].Value < 17 || (score[0].Key != "dealer" && score[1].Value != score[0].Value))
            {
                draw(decksInPlay["dealer"], currentDeck);
                addUpScore(decksInPlay, score);
                dealerIndex = findPlayerIndex("dealer", score);
                displayBoard(decksInPlay);
                if(score[findPlayerIndex("dealer", score)].Value > 21)
                {
                    break;
                }
            }
            if (score[dealerIndex].Value == 18 &&
                (
                decksInPlay["dealer"].Contains("ace_of_clubs") ||
                decksInPlay["dealer"].Contains("ace_of_spades") ||
                decksInPlay["dealer"].Contains("ace_of_hearts") ||
                decksInPlay["dealer"].Contains("ace_of_diamonds")
                ))
            {
                int totalScore = 0;
                foreach (string card in decksInPlay["dealer"])
                {
                    totalScore += cardDeck[card];
                }
                if (totalScore < score[dealerIndex].Value)
                {
                    draw(decksInPlay["dealer"], currentDeck);
                    addUpScore(decksInPlay, score);
                    displayBoard(decksInPlay);
                }
            }
            winningScreen(score, decksInPlay);
            if(score[0].Key == "dealer")
            {
                playerScores["dealer"]++;
            }
            else if(score[0].Value == score[1].Value && score[1].Key == "dealer")
            {
                playerScores["dealer"]++;
            }
            else //I don't check for a tie that doesn't involve the dealer here since two non-dealer players tieing is currently undefined in the requirements
            {
                playerScores[score[0].Key]++;
            }
            Console.WriteLine("Would you like to play another round?\nType yes or no");
            string playAnotherRound = Console.ReadLine();
            Console.WriteLine();
            if (playAnotherRound == "yes")
            {
                return true;
            }
            return false;
        }

        private static int findPlayerIndex(string player, List<KeyValuePair<string, int>> score)
        {
            for (int playerNum = 0; playerNum < score.Count; playerNum++)
            {
                if (score[playerNum].Key == player)
                {
                    return playerNum;
                }
            }
            throw new SystemException("Couldn't find Player for score update");
        }

        private static void draw(List<string> deck, List<string> currentDeck)
        {
            string cardDrawn = currentDeck[currentDeck.Count - 1];
            currentDeck.RemoveAt(currentDeck.Count - 1);
            deck.Add(cardDrawn);
        }

        private static void shuffle(List<string> currentDeck)
        {
            var shuffleNumber = new Random();
            for(int numOfCardSwaps = 0; numOfCardSwaps < currentDeck.Count; numOfCardSwaps++)
            {
                swap(0, shuffleNumber.Next(1, currentDeck.Count - 1), currentDeck);
            }
            static void swap(int position1, int position2, List<string> currentDeck)
            {
                string tempCard = currentDeck[position1];
                currentDeck[position1] = currentDeck[position2];
                currentDeck[position2] = tempCard;
            }
        }

        private static void winningScreen(List<KeyValuePair<string, int>> score, Dictionary<string, List<string>> decksInPlay)
        {
            foreach(KeyValuePair<string, int> player in score)
            {
                Console.WriteLine($"{player.Key} cards are: ");
                foreach(string card in decksInPlay[player.Key])
                {
                    Console.WriteLine(card);
                }
                Console.WriteLine($"with a score of {player.Value}");
            }
        }

        private static void addUpScore(Dictionary<string, List<string>> decksInPlay, List<KeyValuePair<string, int>> score)
        {
            foreach (string player in decksInPlay.Keys) {
                int totalScore = 0;
                bool hasAce = false;
                foreach (string card in decksInPlay[player])
                {
                    totalScore += cardDeck[card];
                    if (card == "ace_of_spades" || card == "ace_of_clubs" || card == "ace_of_hearts" || card == "ace_of_diamonds")
                    {
                        hasAce = true;
                    }
                }
                if (totalScore <= 11 && hasAce)
                {
                    totalScore += 10;
                }
                if (totalScore > 21)
                {
                    totalScore = 0;
                }
                score[findPlayerIndex(player, score)] = new KeyValuePair<string, int>(player, totalScore);
            }
            score.Sort(comparePlayerScoresHighestFirst);
        }
        private static int comparePlayerScoresHighestFirst (KeyValuePair<string, int> player1, KeyValuePair<string, int> player2)
        {
            if(player1.Value < player2.Value)
            {
                return 1;
            }
            if(player1.Value > player2.Value)
            {
                return -1;
            }
            return 0;
        }
        private static void displayBoard(Dictionary<string, List<string>> decksInPlay)
        {
            bool isDealerFirstCard = false;
            foreach (string player in decksInPlay.Keys)
            {
                if(player == "dealer")
                {
                    isDealerFirstCard = true;
                }
                Console.WriteLine($"{player} cards are: ");
                foreach (string card in decksInPlay[player])
                {
                    if (isDealerFirstCard)
                    {
                        Console.WriteLine("facedown");
                        isDealerFirstCard = false;
                    }
                    else
                    {
                        Console.WriteLine(card);
                    }
                }
            }
        }
    }
}
