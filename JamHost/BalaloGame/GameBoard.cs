namespace BalaloGame;
using BalaloGame.Scoring;


public class GameBoard
{
    private readonly Deck _deck = Deck.GenerateDefault();
    private readonly List<GamePlayer> _players = [];

    public Card this[int index] => _deck.Cards[index];
    
    public IEnumerable<Card> Cards => _deck.Cards;

    public List<Card> _bid = new List<Card>();
    public int _bidValue = 0;
    public GamePlayer _bidPlayer;

    public int RoundNumber {get; private set;}
    public void NextRound()
    {   
        RoundNumber++;
        var cards = Enumerable.Range(0, _deck.Size).Shuffle().ToList();
        var offset = 0;
        foreach (var player in _players)
        {
            player.SetHand(cards.Skip(offset).Take(player.HandSize).ToList());
            offset += player.HandSize;
        }
        _bidValue = 0;
        _bid = new List<Card>();
        _bidPlayer = null;
    }

    public GamePlayer AddPlayer(Guid id)
    {
        var player = new GamePlayer(this, id);
        _players.Add(player);
        return player;
    }

    public void RemovePlayer(GamePlayer player)
    {
        _players.Remove(player);
    }

    public int GetBidValue() {
        return _bidValue;
    }

    public void SetBid(List<Card> bid) {
        _bid = bid;
        _bidValue = CalculateValue(bid);
    }

    public void SetBidPlayer(GamePlayer player) {
        _bidPlayer = player;
    }

    public GamePlayer GetBidPlayer() {
        return _bidPlayer;
    }

    public bool ValidateBid(List<Card> bid) {
        var bidValue = CalculateValue(bid);
        if (bidValue <= _bidValue) {
            return false;
        }
        return true;
    }

    public int CalculateValue(List<Card> bid) {
        var score = MatchFinder.GetScoringType(bid, true);
        return score.CalculateBaseScore();
    }

    public bool CheckBs() {
        var total_hands = new List<Card>();
        foreach (var player in _players) {
            total_hands.AddRange(player.HandCards);
        }
        
        Dictionary<Card, bool> hand_dict = new Dictionary<Card, bool>();
        foreach (var card in total_hands) {
            hand_dict.Add(card, false);
        }

        // need to replace bid values
        List<Card> cleaned_bid = new List<Card>();
        foreach (var bid_card in _bid) {
            CardSuit new_suit = bid_card.Suit == CardSuit.None ? CardSuit.Star : bid_card.Suit;
            CardValue new_value = bid_card.Value == CardValue.None ? CardValue.Question : bid_card.Value;
            Card new_card = new(new_value, new_suit);
            cleaned_bid.Add(new_card);
        }
        EffectiveHand bid_effective_hand = new EffectiveHand(cleaned_bid);
        var score = MatchFinder.GetScoringType(bid_effective_hand, false);
        var bid_effective_cards = score.Cards;

        var total_found = 0;
        foreach (var bid_card in bid_effective_cards) {
            var bid_card_suit = bid_card.Suit;
            var bid_card_value = bid_card.Value;

            foreach (var (card, is_used) in hand_dict) {
                if (bid_card_suit.CanBeUsedAs(card.Suit) && bid_card_value.CanBeUsedAs(card.Value) && !is_used) {
                    hand_dict[card] = true;
                    total_found += 1;
                    break;
                }
            }
        }

        if (total_found != bid_effective_cards.Count) {
            return true;
        }
        return false;
    }
}