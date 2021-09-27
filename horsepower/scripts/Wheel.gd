extends RigidBody2D

export (int) var drive_torque = 100
export (int) var brake_torque = 500

func _ready():
	add_to_group('wheels')

func _physics_process(delta):
	position.x = 0

func drive(direction: int = 1):
	apply_torque_impulse(drive_torque * direction)

func brake():
	apply_torque_impulse(brake_torque * -sign(angular_velocity))
