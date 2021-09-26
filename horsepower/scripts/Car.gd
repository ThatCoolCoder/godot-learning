extends RigidBody2D

export (float) var driving_torque = 400
export (float) var braking_torque = 1000

func _physics_process(delta):
	if Input.is_action_pressed('ui_up'):
		$FrontWheelHolder/Wheel.apply_torque_impulse(driving_torque)
		$RearWheelHolder/Wheel.apply_torque_impulse(driving_torque)
	elif Input.is_action_pressed('ui_down'):
		$FrontWheelHolder/Wheel.apply_torque_impulse(-driving_torque)
		$RearWheelHolder/Wheel.apply_torque_impulse(-driving_torque)
	else:
		pass
	if Input.is_action_just_pressed('ui_accept'):
		position.y -= 100
		rotation = 0
		linear_velocity = Vector2()
		angular_velocity = 0
	$FrontWheelHolder/Wheel.position.x = 0
	$RearWheelHolder/Wheel.position.x = 0
