using BalaloGame;
using JamServer.RPC;

namespace JamServer.Lobby;

public class LobbyPlayer
{
    public required string Name { get; init; }
    public required PlayerChannel Channel { get; init; }
}

public class GameLobby
{
    private readonly Deck _deck = Deck.GenerateDefault();

    private readonly List<LobbyPlayer> _players = new();

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
                Players = _players.Select(x => x.Name).ToList()
            }
        };
        await InvokeAll(msg);
    }

    private DeckInfo DeckToRpc() => new DeckInfo
    {
        Cards = _deck.Cards.Select(x => new CardInfo
        {
            Suit = x.Suit,
            Value = x.Value
        }).ToList()
    };

    public async ValueTask<LobbyPlayer> BindPlayer(string name, PlayerChannel channel)
    {
        var player = new LobbyPlayer
        {
            Name = name,
            Channel = channel,
        };
        _players.Add(player);
        await BroadcastLobbyChange();
        await channel.Send(new RpcResponse
        {
            Id = 0,
            Deck = DeckToRpc()
        });
        return player;
    }
}