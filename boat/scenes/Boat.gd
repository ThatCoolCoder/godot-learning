extends RigidBody2D

export var forward_motor_power := 40.0
export var backward_motor_power := 20.0

func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	if Input.is_action_pressed("drive_forward"):
		apply_central_impulse(Vector2(forward_motor_power, 0).rotated(rotation))
	if Input.is_action_pressed("drive_backward"):
		pass
		apply_central_impulse(Vector2(-backward_motor_power, 0).rotated(rotation))

	
	var steering_force := Vector2.ZERO
	if Input.is_action_pressed("turn_left"):
		steering_force += 1 * Vector2(0, 1)
	if Input.is_action_pressed("turn_right"):
		steering_force += 1 * Vector2(0, -1)
	apply_impulse(position, steering_force)
