using System.Reflection.Metadata.Ecma335;

namespace BalaloGame;

public class Deck
{
    private readonly List<Card> _baseCards = [];
    private Queue<int> _remaining = [];

    public IReadOnlyList<Card> Cards => _baseCards;

    public int Size => _baseCards.Count;

    public static Deck GenerateDefault()
    {
        var deck = new Deck();
        foreach (var suit in Enum.GetValues<CardSuit>())
        {
            if (suit is CardSuit.None or CardSuit.Star) continue;
            foreach (var value in Enum.GetValues<CardValue>())
            {
                if (value is CardValue.None or CardValue.Question) continue;
                deck._baseCards.Add(new Card(value, suit));
            }
        }

        deck.Shuffle();
        return deck;
    }

    public void Add(Card card)
    {
        _baseCards.Add(card);
    }

    public void Shuffle()
    {
        _remaining = new Queue<int>(Enumerable.Range(0, _baseCards.Count).Shuffle());
    }

    public void Straighten()
    {
        _remaining = new Queue<int>(Enumerable.Range(0, _baseCards.Count));
    }

    public IEnumerable<Card> DrawMany(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return Draw();
        }
    }

    public Card Draw()
    {
        var index = _remaining.Dequeue();
        return _baseCards[index];
    }
}