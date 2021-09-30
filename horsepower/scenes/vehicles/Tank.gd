extends "res://scripts/Vehicle.gd"

export (float) var cannon_exit_velocity = 2000
export (PackedScene) var projectile

func shoot():
	var target_pos = get_global_mouse_position() - position
	var projectile_velocity = target_pos.normalized()
	projectile_velocity *= cannon_exit_velocity
	
	var projectile_instance = projectile.instance()
	projectile_instance.position = position
	projectile_instance.linear_velocity = projectile_velocity
	get_parent().add_child(projectile_instance)

func _process(delta):
	if Input.is_action_just_pressed('shoot'):
		shoot()
