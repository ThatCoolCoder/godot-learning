extends Area2D

func _on_Floor_body_entered(body):
	if body.is_in_group('rocks'):
		body.explode()
		body.queue_free()
