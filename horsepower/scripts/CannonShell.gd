extends RigidBody2D

func _process(delta):
	rotation = linear_velocity.angle()
