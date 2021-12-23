extends Node2D

export var max_angle_up := 40
export var max_angle_down := 20

export (float) var projectile_speed = 1000
export (PackedScene) var projectile

onready var barrel_end = $"Position2D"

func _ready():
	pass

func _process(delta: float):
	rotation = 0
	var angle = get_angle_to(get_global_mouse_position()) - get_parent().global_rotation
	angle = Utils.constrain(angle, Utils.deg_to_rad(-max_angle_up), Utils.deg_to_rad(max_angle_down))
	rotation = angle

	if Input.is_action_just_pressed('shoot'):
		shoot()

func shoot():
	var projectile_velocity = Vector2(projectile_speed, 0)
	projectile_velocity = projectile_velocity.rotated(global_rotation)
	
	var projectile_instance = projectile.instance()
	projectile_instance.global_position = barrel_end.global_position
	projectile_instance.linear_velocity = projectile_velocity
	get_tree().root.add_child(projectile_instance)
