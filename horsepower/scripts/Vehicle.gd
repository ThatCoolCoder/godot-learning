extends RigidBody2D

export (String) var vehicle_name = 'Unnamed vehicle'
export (float) var rotation_torque = 400

func _physics_process(delta):
	if Input.is_action_pressed('rotate_left'):
		apply_torque_impulse(-rotation_torque)
	if Input.is_action_pressed('rotate_right'):
		apply_torque_impulse(rotation_torque)
		
	if Input.is_action_just_pressed('reset_vehicle'):
		position.y -= 100
		rotation = 0
		linear_velocity = Vector2()
		angular_velocity = 0
