namespace BalaloGame;

public class GameBoard
{
    private readonly Deck _deck = Deck.GenerateDefault();
    private readonly List<GamePlayer> _players = [];

    public Card this[int index] => _deck.Cards[index];
    
    public IEnumerable<Card> Cards => _deck.Cards;

    public void NextRound()
    {
        var cards = Enumerable.Range(0, _deck.Size).Shuffle().ToList();
        var offset = 0;
        foreach (var player in _players)
        {
            player.SetHand(cards.Skip(offset).Take(player.HandSize).ToList());
            offset += player.HandSize;
        }
    }

    public GamePlayer AddPlayer()
    {
        var player = new GamePlayer(this);
        _players.Add(player);
        return player;
    }

    public void RemovePlayer(GamePlayer player)
    {
        _players.Remove(player);
    }
}