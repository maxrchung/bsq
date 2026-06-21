namespace BalaloGame.Scoring;

public enum ScoringType
{
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush,
    FiveOfAKind,
    FlushHouse,
    FlushFive,
}

public class ScoredMatch
{
    public static (int Base, int Multiplier) GetMultiplier(ScoringType type) => type switch
    {
        ScoringType.HighCard => (5, 1),
        ScoringType.Pair => (10, 2),
        ScoringType.TwoPair => (20, 2),
        ScoringType.ThreeOfAKind => (30, 3),
        ScoringType.Straight => (30, 4),
        ScoringType.Flush => (35, 4),
        ScoringType.FullHouse => (40, 4),
        ScoringType.FourOfAKind => (60, 7),
        ScoringType.StraightFlush or ScoringType.RoyalFlush => (100, 8),
        ScoringType.FiveOfAKind => (120, 12),
        ScoringType.FlushHouse => (140, 14),
        ScoringType.FlushFive => (160, 16),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public required ScoringType Type { get; init; }
    public IReadOnlyList<EffectiveCard> Cards { get; init; } = [];

    public int CalculateBaseScore()
    {
        var (score, multiplier) = GetMultiplier(Type);
        foreach (var card in Cards)
        {
            score += card.Value.IntegerValue();
        }

        return score * multiplier;
    }

    public override string ToString()
    {
        return $"[{CalculateBaseScore()}pts]: {Type} with ({string.Join(", ", Cards)})";
    }
}