extends Node2D

var player_name = ""
var card_count = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Name.text = "Name: " + player_name
	
	if int(card_count) == 0:
		$CardCount.text = "Cards: -"
	else:
		$CardCount.text = "Cards: " + str(int(card_count))

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
