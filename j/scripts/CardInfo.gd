# CardInfo.gd
class_name CardInfo
extends Resource

# Properties with type hints and default values
@export var desc: String = "Blank Card"

func _init(d: String):
	desc = d
