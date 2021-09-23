extends Node2D

const Utils = preload('res://scripts/Utils.gd')

export (PackedScene) var ExplosionEffects
export (float) var speed = 0
export (float) var rotation_speed = 0
export (bool) var frozen = false

func _ready():
	var animations = ['fall_1', 'fall_2', 'fall_3']
	var animation = animations[randi() % len(animations)]
	$AnimatedSprite.animation = animation
	$AnimatedSprite.play()

func _process(delta):
	if not frozen:
		position.y += speed * delta
		rotation += rotation_speed * delta
		
func _exit_tree():
	var effect = ExplosionEffects.instance()
	effect.rock_scale = (scale.x + scale.y) / 2
	effect.position = position
	effect.position.y += $CollisionShape2D.shape.radius * effect.rock_scale / 2
	get_parent().add_child(effect)

func _on_VisibilityNotifier2D_screen_exited():
	queue_free()

func _on_Rock_body_entered(body):
	print('body entered')
	if body.is_in_group('rock_killer'):
		queue_free()
