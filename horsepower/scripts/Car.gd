extends RigidBody2D

var wheels = []

func _ready():
	for node in get_tree().get_nodes_in_group('wheels'):
		if is_a_parent_of(node):
			wheels.append(node)

func _physics_process(delta):
	if Input.is_action_pressed('drive_forward'):
		for wheel in wheels:
			wheel.drive(1)
	if Input.is_action_pressed('drive_backward'):
		for wheel in wheels:
			wheel.drive(-1)
	if Input.is_action_pressed('brake'):
		for wheel in wheels:
			wheel.brake()
		
	if Input.is_action_just_pressed('reset_vehicle'):
		position.y -= 100
		rotation = 0
		linear_velocity = Vector2()
		angular_velocity = 0
