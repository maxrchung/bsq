namespace BalaloGame.Scoring;

public class EffectiveCard
{
    public Card BaseCard { get; init; }

    private CardValue? _forcedValue;
    private CardSuit? _forcedSuit;
    private bool _used = false;

    public CardValue Value => _forcedValue ?? BaseCard.Value;
    public CardSuit Suit => _forcedSuit ?? BaseCard.Suit;

    public bool IsCompatible(CardValue value) => Value.CanBeUsedAs(value);
    public bool IsCompatible(CardSuit suit) => Suit.CanBeUsedAs(suit);

    public EffectiveCard(Card baseCard, CardValue? value = null, CardSuit? suit = null)
    {
        BaseCard = baseCard;
        _forcedValue = value;
        _forcedSuit = suit;
    }

    private bool IsCompatibleMaybe(CardValue value, bool allowChange)
    {
        if (allowChange) return IsCompatible(value);
        return Value == value;
    }

    private bool IsCompatibleMaybe(CardSuit suit, bool allowChange)
    {
        if (allowChange) return IsCompatible(suit);
        return Suit == suit;
    }

    public bool TryUse(CardValue value, bool allowChange = false)
    {
        if (_used) return false;
        if (!IsCompatibleMaybe(value, allowChange)) return false;
        _used = true;
        _forcedValue = value;
        return true;
    }

    public bool TryUse(CardSuit suit, bool allowChange = false)
    {
        if (_used) return false;
        if (!IsCompatibleMaybe(suit, allowChange)) return false;
        _used = true;
        _forcedSuit = suit;
        return true;
    }

    public EffectiveCard ForcedConcrete()
    {
        var newSuit = Suit.IsConcrete() ? Suit : CardSuit.Diamond;
        var newValue = Value.IsConcrete() ? Value : CardValue.Two;
        return new EffectiveCard(BaseCard, newValue, newSuit);
    }

    public EffectiveCard MergedWith(EffectiveCard other)
    {
        return new EffectiveCard(BaseCard, _forcedValue ?? other._forcedValue, suit: _forcedSuit ?? other._forcedSuit);
    }

    public override string ToString()
    {
        return $"{Value} of {Suit}";
    }
}

public class EffectiveHand
{
    public const int HandSize = 5;

    private readonly List<EffectiveCard> _cards;

    public IReadOnlyList<EffectiveCard> Cards => _cards;
    public int Size => _cards.Count;
    public bool IsFullHand => Size == HandSize;

    public EffectiveHand(IEnumerable<Card> cards)
    {
        _cards = cards.OrderByDescending(x => x.Value).Select(x => new EffectiveCard(x)).ToList();
    }

    private EffectiveHand(IEnumerable<EffectiveCard> cards)
    {
        _cards = cards.OrderByDescending(x => x.Value).ToList();
    }

    public bool TryTakeOf(CardSuit suit)
    {
        var withReal = _cards.FirstOrDefault(x => x.TryUse(suit));
        if (withReal is not null) return true;
        var withWild = _cards.FirstOrDefault(x => x.TryUse(suit, true));
        return withWild is not null;
    }

    public bool TryTakeOf(CardValue value)
    {
        var withReal = _cards.FirstOrDefault(x => x.TryUse(value));
        if (withReal is not null) return true;
        var withWild = _cards.FirstOrDefault(x => x.TryUse(value, true));
        return withWild is not null;
    }

    public CardValue HighestConcreteValue => _cards.FirstOrDefault(x => x.Value.IsConcrete())?.Value ?? CardValue.None;

    public EffectiveHand Clone() => new(_cards.Select(x => x.BaseCard).ToList());

    public EffectiveHand? TryForceStraight()
    {
        if (!IsFullHand) return null;
        var highest = HighestConcreteValue;
        if (highest < CardValue.Seven) return null;
        var highBound = (int)highest;
        var lowBound = highBound - 4;
        var straight = Clone();
        for (var i = lowBound; i < highBound; i++)
        {
            if (!straight.TryTakeOf((CardValue)i)) return null;
        }

        return straight;
    }

    public EffectiveHand? TryForceFlush()
    {
        if (!IsFullHand) return null;
        var uniqueSuits = new HashSet<CardSuit>(_cards.Select(x => x.Suit).Where(x => x.IsConcrete()));
        if (uniqueSuits.Count != 1) return null;
        var flush = Clone();
        var target = uniqueSuits.First();
        for (var i = 0; i < Size; i++)
        {
            if (!flush.TryTakeOf(target)) return null;
        }

        return flush;
    }

    public EffectiveHand MergeWith(EffectiveHand other)
    {
        return new EffectiveHand(_cards.Select((x, i) => x.MergedWith(other._cards[i])));
    }

    public List<EffectiveHand> GetNOfAKind()
    {
        var result = new List<EffectiveHand>();
        for (var i = 0; i < Size; i++)
        {
            var current = _cards[i];
            if (!current.Value.IsConcrete()) continue;
            var hypothetical = Clone();
            var newHand = new List<EffectiveCard>([current]);
            for (var j = i + 1; j < Size; j++)
            {
                var other = hypothetical._cards[j];
                if (other.TryUse(current.Value, true))
                {
                    newHand.Add(other);
                }
            }

            if (newHand.Count > 1)
            {
                result.Add(new EffectiveHand(newHand));
            }
        }

        return result.OrderByDescending(x => x.Size).ToList();
    }

    public ScoredMatch BuildCardResult(ScoringType type) => new()
    {
        Type = type,
        Cards = _cards.Select(x => x.ForcedConcrete()).ToList(),
    };
}

public static class MatchFinder
{
    private static EffectiveHand? TryStraightFlush(EffectiveHand? straight, EffectiveHand? flush)
    {
        if (straight == null || flush == null) return null;
        return straight.MergeWith(flush);
    }
    
    public static ScoredMatch GetScoringType(IEnumerable<Card> cards) => GetScoringType(new EffectiveHand(cards));

    public static ScoredMatch GetScoringType(EffectiveHand hand)
    {
        // straights and flushes
        var maybeStraight = hand.TryForceStraight();
        var maybeFlush = hand.TryForceFlush();
        var maybeStraightFlush = TryStraightFlush(maybeStraight, maybeFlush);

        var ofAKindList = hand.GetNOfAKind();
        var highestOfAKind = ofAKindList.FirstOrDefault()?.Size ?? 0;

        if (highestOfAKind == 5)
        {
            return ofAKindList.First().BuildCardResult(ScoringType.FiveOfAKind);
        }

        if (maybeStraightFlush != null)
        {
            return maybeStraightFlush.BuildCardResult(
                maybeStraightFlush.HighestConcreteValue == CardValue.Ace
                    ? ScoringType.RoyalFlush
                    : ScoringType.StraightFlush);
        }

        if (highestOfAKind == 4)
        {
            return ofAKindList.First().BuildCardResult(ScoringType.FourOfAKind);
        }

        // TODO: full house
        if (maybeFlush != null)
        {
            return maybeFlush.BuildCardResult(ScoringType.Flush);
        }

        if (maybeStraight != null)
        {
            return maybeStraight.BuildCardResult(ScoringType.Straight);
        }

        if (highestOfAKind == 3)
        {
            return ofAKindList.First().BuildCardResult(ScoringType.ThreeOfAKind);
        }

        if (highestOfAKind == 2)
        {
            return ofAKindList.First().BuildCardResult(ScoringType.Pair);
        }

        return new ScoredMatch
        {
            Type = ScoringType.HighCard,
            Cards = hand.Cards.Take(1).Select(x => x.ForcedConcrete()).ToList(),
        };
    }
}