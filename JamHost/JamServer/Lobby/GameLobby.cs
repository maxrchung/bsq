using BalaloGame;
using JamServer.RPC;

namespace JamServer.Lobby;

public class GameLobbyPlayer
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required PlayerChannel Channel { get; init; }

    public required GamePlayer GamePlayer { get; init; }
}

public class GameLobby
{
    private readonly GameBoard _board = new();

    private readonly List<GameLobbyPlayer> _players = new();

    public readonly string Name;
    public readonly Guid Id;

    public GameLobby(string name, Guid id)
    {
        Name = name;
        Id = id;
    }

    private async Task InvokeAll(RpcResponse msg)
    {
        await Task.WhenAll(_players.Select(x => x.Channel.Send(msg)));
    }

    private async Task BroadcastLobbyChange()
    {
        var msg = new RpcResponse
        {
            Id = 0,
            LobbyChange = new LobbyChangeMessage
            {
                Id = Id,
                Name = Name,
                Players = _players.Select(LobbyPlayer.From).ToList()
            }
        };
        await InvokeAll(msg);
    }

    private DeckInfo DeckToRpc() => new DeckInfo
    {
        Cards = _board.Cards.Select(CardInfo.From).ToList()
    };

    private bool RevealHandTo(GameLobbyPlayer observer, GameLobbyPlayer target) => observer.Id == target.Id;

    private IReadOnlyList<PlayerHandInfo> HandInfoFor(GameLobbyPlayer player)
    {
        return _players.Select(x => new PlayerHandInfo
        {
            Id = x.Id,
            CardCount = x.GamePlayer.HandSize,
            Cards = RevealHandTo(player, x)
                ? x.GamePlayer.HandCards.Select(CardInfo.From).ToList()
                : null
        }).ToList();
    }

    private async Task UpdateHands()
    {
        await Task.WhenAll(_players.Select(player =>
            player.Channel.Send(new RpcResponse { Id = 0, PlayerHands = HandInfoFor(player) })).ToList());
    }

    public async Task NextRound()
    {
        _board.NextRound();
        await UpdateHands();
    }

    public async Task UpdateDeck()
    {
        await InvokeAll(new RpcResponse { Id = 0, Deck = DeckToRpc() });
    }

    public async ValueTask<GameLobbyPlayer> BindPlayer(string name, PlayerChannel channel)
    {
        var player = new GameLobbyPlayer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Channel = channel,
            GamePlayer = _board.AddPlayer()
        };
        _players.Add(player);
        await channel.Send(new RpcResponse
        {
            Id = 0,
            LocalIdChange = player.Id,
            Deck = DeckToRpc(),
        });
        await BroadcastLobbyChange();
        return player;
    }
}