extends Node

class_name StateMgr

enum GameStateT {
	Invalid,
	Menu,
	Lobby,
	Round,
	EmergencyMeeting,
	GameOver,
}

signal state_changed(new_state: GameStateT)

var CurrentState: GameStateT = GameStateT.Invalid

func change_state(new_state: GameStateT) -> void:
	if CurrentState == new_state:
		return
	CurrentState = new_state
	state_changed.emit(new_state)
