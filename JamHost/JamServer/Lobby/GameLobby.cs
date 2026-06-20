using BalaloGame;
using JamServer.RPC;

namespace JamServer.Lobby;

public class GameLobbyPlayer
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required PlayerChannel Channel { get; init; }

    public required GamePlayer GamePlayer { get; init; }

    public required GameLobby Lobby { get; init; }
}

public record EmergencyMeetingState
{
    public bool IsActive { get; set; } = false;

    public void Start()
    {
        IsActive = true;
        VotesFor.Clear();
        VotesAgainst.Clear();
    }

    public List<GameLobbyPlayer> VotesFor { get; init; } = [];
    public List<GameLobbyPlayer> VotesAgainst { get; init; } = [];

    public EmergencyMeetingInfo ToRpc() => new()
    {
        IsActive = IsActive,
        VotesFor = VotesFor.Select(x => x.Id).ToList(),
        VotesAgainst = VotesAgainst.Select(x => x.Id).ToList(),
    };
}

public class GameLobby
{
    private readonly GameBoard _board = new();

    private readonly List<GameLobbyPlayer> _players = new();

    public readonly string Name;
    public readonly Guid Id;

    private readonly EmergencyMeetingState _emergencyMeeting = new();

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

    private DeckInfo DeckToRpc() => new()
    {
        Cards = _board.Cards.Select(CardInfo.From).ToList()
    };

    private bool ShouldRevealHandTo(GameLobbyPlayer observer, GameLobbyPlayer target) => observer.Id == target.Id;

    public IReadOnlyList<PlayerHandInfo> HandInfoFor(GameLobbyPlayer player)
    {
        return _players.Select(x => new PlayerHandInfo
        {
            Id = x.Id,
            CardCount = x.GamePlayer.HandSize,
            Cards = ShouldRevealHandTo(player, x)
                ? x.GamePlayer.HandCards.Select(CardInfo.From).ToList()
                : null
        }).ToList();
    }

    public List<GameLobbyPlayer> GetPlayers()
    {
        return _players;
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

    public async Task<bool> ValidateBid(List<Dictionary<string, string>> raw_bid) {
        var bid = new List<Card>();
        foreach (Dictionary<string, string> raw_card in raw_bid) {
            foreach (var (suit, value) in raw_card)
            {
                Enum.TryParse<CardSuit>(suit, true, out CardSuit card_suit);
                Enum.TryParse<CardValue>(value, true, out CardValue card_value);
                var new_card = new Card(card_value, card_suit);
                bid.Add(new_card);
            }
        }
        if (_board.ValidateBid(bid)) {
            _board.SetBid(bid);
            return true;
        }
        return false;
    }

    public async Task UpdateDeck()
    {
        await InvokeAll(new RpcResponse { Id = 0, Deck = DeckToRpc() });
    }

    private async Task BroadcastMeeting()
    {
        await InvokeAll(new RpcResponse { Id = 0, EmergencyMeeting = _emergencyMeeting.ToRpc() });
    }

    public async Task InvokeCtl(GameLobbyPlayer player, InvokeCtlType type)
    {
        if (type == InvokeCtlType.StartGame)
        {
            await UpdateDeck();
            await NextRound();
        }
        else if (type == InvokeCtlType.EmergencyMeetingVoteFor)
        {
            if (!_emergencyMeeting.IsActive) return;
            _emergencyMeeting.VotesFor.Add(player);
            await BroadcastMeeting();
        }
        else if (type == InvokeCtlType.EmergencyMeetingVoteAgainst)
        {
            if (!_emergencyMeeting.IsActive)
            {
                _emergencyMeeting.Start();
            }

            _emergencyMeeting.VotesAgainst.Add(player);
            await BroadcastMeeting();
        }
    }

    public async ValueTask<GameLobbyPlayer> BindPlayer(string name, PlayerChannel channel)
    {
        var player = new GameLobbyPlayer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Channel = channel,
            GamePlayer = _board.AddPlayer(),
            Lobby = this,
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