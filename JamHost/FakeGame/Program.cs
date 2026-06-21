using BalaloGame;
using BalaloGame.Scoring;

List<Card> hand1 =
[
    new(CardValue.Ten, CardSuit.Star),
    new(CardValue.Question, CardSuit.Star),
];
PrintHand(hand1);

var game = new GameBoard();
var p1 = game.AddPlayer(Guid.NewGuid());
p1.HandSize = 5;

for (var i = 0; i < 10; i++)
{
    game.NextRound();
    PrintHand(p1.HandCards);
}

void PrintHand(List<Card> hand)
{
    var score = MatchFinder.GetScoringType(hand);
    Console.WriteLine("=== HAND ===");
    Console.WriteLine($"{string.Join(", ", hand)}");
    Console.WriteLine($"IS A {score}");
    Console.WriteLine("=============\n");
}