extends KinematicBody2D

export (int) var walk_speed = 50
export (int) var gravity = ProjectSettings.get_setting('physics/2d/default_gravity')
var velocity = Vector2(0, 0)

func get_input():
	if Input.is_action_pressed('ui_left'):
		velocity.x = -walk_speed
	elif Input.is_action_pressed('ui_right'):
		velocity.x = walk_speed
	else:
		velocity.x = 0

func _physics_process(delta):
	velocity.y += gravity * delta
	get_input()
	var motion = velocity * delta
	move_and_collide(motion)
