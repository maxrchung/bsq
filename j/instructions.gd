extends Node2D
@onready var generic_click: AudioStreamPlayer = $"../generic_click"


var is_open = false

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_trigger_button_pressed() -> void:
	is_open = not is_open
	generic_click.play()
	if is_open:
		$Background.visible = true
		$BodyText.visible = true
	else:
		$Background.visible = false
		$BodyText.visible = false
		
