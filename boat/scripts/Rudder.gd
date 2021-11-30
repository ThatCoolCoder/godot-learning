extends Node2D

export var area := 10.0

func _physics_process(delta):
	var steering_force := Vector2.ZERO
	if Input.is_action_pressed("turn_left"):
		steering_force += area * Vector2(0, 1) * get_parent().linear_velocity.length()
	if Input.is_action_pressed("turn_right"):
		steering_force += area * Vector2(0, -1) * get_parent().linear_velocity.length()
	get_parent().apply_impulse(position, steering_force)
