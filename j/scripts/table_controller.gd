extends Node

const OTHER_PLAYER_SCENE = preload("res://scenes/OtherPlayer.tscn")

@onready var spawn_path: Path2D = $SpawnPath

var game_state: GameState

func init_table(num_players: int) -> void:
	print("init table called")
	game_state = GameState.new()
	game_state.player_count = num_players
	_create_other_players()
	
func _create_other_players() -> void:
	#var other_player_ids = get_lobby()
		
	var curve: Curve2D = spawn_path.curve
	var total_length: float = curve.get_baked_length()

	var n_other_players = game_state.player_count - 1
	for i in n_other_players:
		
		var progress_ratio: float = _calculate_progress_ratio(i, n_other_players)

		# Sample the curve length using the ratio
		var offset: float = progress_ratio * total_length

		# Get the exact global position on the path
		var spawn_pos: Vector2 = spawn_path.global_transform * curve.sample_baked(offset)

		var instance: OtherPlayer = OTHER_PLAYER_SCENE.instantiate()
		instance.global_position = spawn_pos
		#instance.player_id = other_player_ids[i]
		instance.hand_size = randi_range(1, 6)
		
		game_state.other_players.append(instance)
		add_child(instance)

# Calculate the proportional distance along the path (0.0 to 1.0)
func _calculate_progress_ratio(player_idx: int, n_other: int) -> float:
	# Hard code distances for low player ct
	if n_other == 1: return 0.5 
	if n_other == 2: return [0.2, 0.8][player_idx]
	if n_other == 3: return [0.1, 0.5, 0.9][player_idx]
	return float(player_idx) / float(n_other - 1)
