tool

extends PinJoint2D

export (int) var drive_torque = 100
export (int) var brake_torque = 500
export (float) var max_rotation_speed_deg = 360 * 30
export (float) var radius = 15
export (float) var _softness = 2
export (StreamTexture) var texture
var max_rotation_speed

func _ready():
	if Engine.editor_hint:
		return
	max_rotation_speed = Utils.deg_to_rad(max_rotation_speed_deg)
	add_to_group('wheels')
	update_sprite()
	$Wheel/CollisionShape2D.shape.radius = radius
	softness = _softness
	node_a = get_parent().get_path()

func _draw():
	if not Engine.editor_hint:
		return
	update_sprite()

func _physics_process(delta):
	if Engine.editor_hint:
		return
	if Input.is_action_pressed('drive_forward') and $Wheel.angular_velocity < max_rotation_speed:
		drive(1)
	if Input.is_action_pressed('drive_backward') and $Wheel.angular_velocity > -max_rotation_speed:
		drive(-1)
	if Input.is_action_pressed('brake'):
		brake()
	position.x = 0

func drive(direction: int = 1):
	$Wheel.apply_torque_impulse(drive_torque * direction)

func brake():
	$Wheel.apply_torque_impulse(brake_torque * -sign($Wheel.angular_velocity))

func update_sprite():
	$Wheel/Sprite.texture = texture
	var scale = radius * 2 / $Wheel/Sprite.texture.get_width()
	$Wheel/Sprite.scale.x = scale
	$Wheel/Sprite.scale.y = scale
