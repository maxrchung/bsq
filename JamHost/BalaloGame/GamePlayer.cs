namespace BalaloGame;

public class GamePlayer
{
    private readonly GameBoard _board;
    private List<int> _hand = [];
    public int HandSize = 3;

    public GamePlayer(GameBoard board)
    {
        _board = board;
    }

    public List<Card> HandCards => _hand.Select(i => _board[i]).ToList();

    public void SetHand(List<int> cards)
    {
        _hand = cards;
    }
}