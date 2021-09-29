extends RigidBody2D

export (int) var drive_torque = 100
export (int) var brake_torque = 500
export (float) var max_rotation_speed_deg = 360 * 30
var max_rotation_speed

func _ready():
	max_rotation_speed = Utils.deg_to_rad(max_rotation_speed_deg)
	add_to_group('wheels')

func _physics_process(delta):
	if Input.is_action_pressed('drive_forward') and angular_velocity < max_rotation_speed:
		drive(1)
	if Input.is_action_pressed('drive_backward') and angular_velocity > -max_rotation_speed:
		drive(-1)
	if Input.is_action_pressed('brake'):
		brake()
	position.x = 0
	print(clamp(angular_velocity, -max_rotation_speed, max_rotation_speed))
	#angular_velocity += 0.1

func drive(direction: int = 1):
	apply_torque_impulse(drive_torque * direction)

func brake():
	apply_torque_impulse(brake_torque * -sign(angular_velocity))
