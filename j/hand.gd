extends Node2D

@export var card_scene: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func update_cards(cards):
	# Remove old
	for child in get_children():
		if child is Card:
			child.queue_free()
		
	var count = cards.size()
	if count == 0:
		return
		
	# Dedicate at least 250 for the main center card
	var width = 250
	
	# Add 50 for other cards
	if (count > 1):
		width += (count - 1) * 50
		
	var screen_size = get_viewport().get_visible_rect().size
	
	# Half screen size - half total width + account for anchor
	var left = screen_size.x / 2 - width / 2 + 125
	
	for i in count:
		var card = card_scene.instantiate()
		card.suit = cards[i]["suit"]
		card.number = cards[i]["number"]
		card.position = Vector2(
			left + i * 50,
			screen_size.y
		)
		add_child(card)
		

func reset_cards():
	update_cards([])

func randomize_cards():
	var count = randi_range(2, 5)
	
	var cards = []
	cards.resize(count)
	
	for i in count:
		cards[i] = {
			"suit": [
				'spade',
				'heart',
				'clover',
				'diamond',
			].pick_random(),
			"number": randi_range(2, 14)
		}
		
	update_cards(cards)
