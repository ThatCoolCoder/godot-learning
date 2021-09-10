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

func _on_Rock_body_entered(body):
	print('body entered')
	if body.is_in_group('rock_killer'):
		queue_free()
