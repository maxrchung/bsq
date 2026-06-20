using System.Diagnostics.CodeAnalysis;

namespace JamServer.Lobby;

public record LobbyListEntry(Guid Id, string Name);

public class LobbyCoordinator
{
    private readonly ILogger<LobbyCoordinator> _logger;
    private readonly Dictionary<Guid, GameLobby> _lobbies = new();

    public LobbyCoordinator(ILogger<LobbyCoordinator> logger)
    {
        _logger = logger;
        _logger.LogInformation("Lobby coordinator started");

        // DEBUG
        CreateLobby("hi :)");
    }

    public bool TryFindLobby(Guid id, [MaybeNullWhen(false)] out GameLobby lobby) =>
        _lobbies.TryGetValue(id, out lobby);

    public GameLobby CreateLobby(string name)
    {
        var id = Guid.NewGuid();
        var lobby = new GameLobby(name, id);
        _lobbies.Add(id, lobby);
        return lobby;
    }

    public IEnumerable<LobbyListEntry> GetLobbies() => _lobbies.Select(x => new LobbyListEntry(x.Key, x.Value.Name));
}