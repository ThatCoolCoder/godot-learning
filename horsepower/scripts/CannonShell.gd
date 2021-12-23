extends RigidBody2D

func _process(delta):
	rotation = linear_velocity.angle()


func _on_DeathTimer_timeout():
	queue_free()
