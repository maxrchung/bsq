extends VBoxContainer

func _handle_lobby_info_updated(info: SrvCxn.LobbyInfo) -> void:
	for child in get_children():
		remove_child(child)
		child.queue_free()
	for player in info.players:
		var playerLabel = Label.new()
		playerLabel.text = player.name
		add_child(playerLabel)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ServerConnection.lobby_info_updated.connect(_handle_lobby_info_updated)
