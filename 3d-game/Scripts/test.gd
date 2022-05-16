extends CollisionShape


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

onready var x = $AnimationPlayer

# Called when the node enters the scene tree for the first time.
func _ready():
	x.playback_active = true
