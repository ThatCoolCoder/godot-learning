extends Node2D

export (float) var speed = 0
var frozen = false

func _process(delta):
	if not frozen:
		position.y += speed * delta

func _on_VisibilityNotifier2D_screen_exited():
	queue_free()

func freeze():
	frozen = true

func unfreeze():
	frozen = false
