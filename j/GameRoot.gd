extends Node2D

@onready var MenuSceneI = $BaseScenes/MenuScene
@onready var LobbySceneI = $BaseScenes/LobbyScene

func _on_state_change(new_state: StateMgr.GameStateT) -> void:
	match new_state:
		StateMgr.GameStateT.Menu:
			MenuSceneI.visible = true
			LobbySceneI.visible = false
		StateMgr.GameStateT.Lobby:
			MenuSceneI.visible = false
			LobbySceneI.visible = true

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	StateManager.state_changed.connect(_on_state_change)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
