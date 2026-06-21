extends Node2D

@export var index = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func get_card():
	var suit = ""
	if $SuitDropdown.selected > 0:
		suit = $SuitDropdown.get_item_text($SuitDropdown.selected)
	
	var value = ""
	if $NumberDropdown.selected > 0:
		value = $NumberDropdown.get_item_text($NumberDropdown.selected)
		
	if not suit and not value:
		return
		
	if not suit and value:
		return {
			"None": value
		}
	
	if not value and suit:
		return {
			suit: "None"
		}
		
	return {
		suit: value
	}

func set_card(suit, value):
	for i in $SuitDropdown.item_count:
		if $SuitDropdown.get_item_text(i) == suit:
			$SuitDropdown.selected = i
			break
	
	for i in $NumberDropdown.item_count:
		if $NumberDropdown.get_item_text(i) == str(value):
			$NumberDropdown.selected = i
			break

func decide_to_show_clear():
	var suit = ""
	if $SuitDropdown.selected > 0:
		suit = $SuitDropdown.get_item_text($SuitDropdown.selected)
		
	var value = ""
	if $NumberDropdown.selected > 0:
		value = $NumberDropdown.get_item_text($NumberDropdown.selected)
		
	if not suit and not value:
		$ClearButton.visible = false
	else:
		$ClearButton.visible = true

func enable(should_enable):
	if should_enable:
		$SuitDropdown.disabled = false
		$NumberDropdown.disabled = false
		decide_to_show_clear()
	else:
		$SuitDropdown.disabled = true
		$NumberDropdown.disabled = true
		$ClearButton.visible = false

func _on_clear_button_pressed() -> void:
	$SuitDropdown.selected = -1
	$NumberDropdown.selected = -1
	$ClearButton.visible = false
	
func _on_dropdown_item_selected(index: int) -> void:
	var suit = ""
	if $SuitDropdown.selected > 0:
		suit = $SuitDropdown.get_item_text($SuitDropdown.selected)
		
	var value = ""
	if $NumberDropdown.selected > 0:
		value = $NumberDropdown.get_item_text($NumberDropdown.selected)
		
	if not suit and not value:
		$ClearButton.visible = false
	else:
		$ClearButton.visible = true
		
	
