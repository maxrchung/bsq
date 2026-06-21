extends TextureRect

@export var suit: SuitName = SuitName.Spade:
	set(v):
		suit = v
		_update_material()
@export var value: int = 2:
	set(v):
		value = v
		_update_material()
@export var faceUp = true:
	set(v):
		faceUp = v
		_update_material()

enum SuitName {
	Spade = 0,
	Heart = 1,
	Club = 2,
	Diamond = 3,
}

func _calc_offsets(suit: SuitName, value: int) -> Vector2i:
	return Vector2i(value - 2, int(suit))

func _update_material():
	print("updating materials")
	print(get_instance_shader_parameter("Offset"))
	set_instance_shader_parameter("Offset", _calc_offsets(suit, value))
	set_instance_shader_parameter("FacingUp", faceUp)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_update_material()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
