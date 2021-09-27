extends KinematicBody2D

const Utils = preload('res://scripts/Utils.gd')

signal dead

export (int) var max_move_speed = 300
export (int) var move_acceleration = 700
export (int) var stop_move_acceleration = 1000
var gravity = ProjectSettings.get_setting('physics/2d/default_gravity')
export (int) var jump_speed = 500

var velocity := Vector2.ZERO

func _physics_process(delta: float) -> void:
	var x_acceleration = 0
	# set horizontal velocity
	if Input.is_action_pressed('move_right'):
		x_acceleration += move_acceleration
	if Input.is_action_pressed('move_left'):
		x_acceleration -= move_acceleration
	 # jump will happen on the next frame
	if Input.is_action_just_pressed('jump'):
		if is_on_floor():
			velocity.y = -jump_speed # negative Y is up in Godot
	
	velocity.x += x_acceleration * delta
	velocity.x = clamp(velocity.x, -max_move_speed, max_move_speed)
	
	if x_acceleration == 0:
		velocity.x = Utils.converge_value(velocity.x, 0, stop_move_acceleration * delta)

	# apply gravity
	# player always has downward velocity
	velocity.y += gravity * delta

	# actually move the player
	velocity = move_and_slide(velocity, Vector2.UP)
	handle_collisions()

func handle_collisions():
	for i in get_slide_count():
		var collision = get_slide_collision(i)
		if collision.collider is TileMap:
			var true_pos = collision.collider.to_local(collision.position - collision.normal - Vector2.ONE)
			var tile_pos = collision.collider.world_to_map(true_pos)
			var tile_id = collision.collider.get_cellv(tile_pos)
			if tile_id == -1:
				continue
			var tile_name = collision.collider.tile_set.tile_get_name(tile_id)
			if tile_name == 'Spike':
				emit_signal('dead')

func _process(delta: float) -> void:
	change_animation()

func change_animation():
	# face left or right
	if velocity.x > 0:
		$AnimatedSprite.flip_h = false
	elif velocity.x < 0:
		$AnimatedSprite.flip_h = true
	
	if velocity.y < 0:
		$AnimatedSprite.play('jump')
	if velocity.y > 0 and not is_on_floor():
		$AnimatedSprite.play('fall')
	else:
		if velocity.x != 0:
			$AnimatedSprite.play('walk')
		else:
			$AnimatedSprite.play('idle')
		
