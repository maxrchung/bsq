using System.Text.Json.Serialization;

namespace BalaloGame;

public enum CardSuit
{
    None, // Unspecified
    Spade,
    Heart,
    Diamond,
    Club,
    Star, // Wild Card
}

public enum CardValue
{
    None, // Unspecified
    Question, // Wild Card
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace,
}

public enum CardModifier
{
    None,
    Sparkling,
    Shiny,
    Rainbow,
}

public static class CardTypeExtensions
{
    public static bool CanBeUsedAs(this CardValue v, CardValue other) =>
        other == CardValue.None || v == CardValue.Question || v == other;

    public static bool CanBeUsedAs(this CardSuit s, CardSuit other) =>
        other == CardSuit.None || s == CardSuit.Star || s == other;

    public static bool IsConcrete(this CardValue v) => v is not CardValue.None and not CardValue.Question;
    public static bool IsConcrete(this CardSuit s) => s is not CardSuit.None and not CardSuit.Star;

    public static int IntegerValue(this CardValue v) => v switch
    {
        CardValue.None => 0,
        CardValue.Question or CardValue.Two => 2,
        CardValue.Three => 3,
        CardValue.Four => 4,
        CardValue.Five => 5,
        CardValue.Six => 6,
        CardValue.Seven => 7,
        CardValue.Eight => 8,
        CardValue.Nine => 9,
        CardValue.Ten => 10,
        CardValue.Jack or CardValue.Queen or CardValue.King => 11,
        CardValue.Ace => 12,
        _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
    };
}

public class Card
{
    private CardValue _value;
    private CardSuit _suit;
    private CardModifier _modifier = CardModifier.None;

    public CardSuit Suit => _suit;
    public CardValue Value => _value;

    public Card(CardValue value, CardSuit suit, CardModifier modifier = CardModifier.None)
    {
        _value = value;
        _suit = suit;
        _modifier = modifier;
    }

    public bool Is(CardValue value) => _value.CanBeUsedAs(value);
    public bool Is(CardSuit suit) => _suit.CanBeUsedAs(suit);

    public bool IsSameSuit(Card other) => Is(other.Suit);
    public bool IsSameValue(Card other) => Is(other.Value);

    public bool HasConcreteSuit => _suit is not CardSuit.None and not CardSuit.Star;

    public override string ToString()
    {
        return $"{_value} of {_suit}s";
    }
}