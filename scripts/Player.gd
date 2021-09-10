extends Node2D

signal hit

export (int) var speed = 40

var target_x = 0
var screen_size
var frozen = false

func _ready():
	unfreeze()
	screen_size = get_viewport_rect().size

func reset(_position: Vector2):
	print('Player reset')
	target_x = _position.x
	position = Vector2(_position.x, _position.y)
	$CollisionShape2D.set_deferred('disabled', false)
	$AnimatedSprite.animation = 'left'

func _process(delta):
	if frozen:
		return
		
	if Input.is_mouse_button_pressed(1):
		target_x = get_viewport().get_mouse_position().x
	
	var position_difference = target_x - position.x
	if abs(position_difference) < delta * speed:
		position.x = target_x
	else:
		 position.x += delta * speed * sign(position_difference)
	position.x = constrain_value(position.x, 0, screen_size.x)
	
	if position_difference < 0:
		$AnimatedSprite.animation = 'left'
	elif position_difference > 0:
		$AnimatedSprite.animation = 'right'
	else:
		pass
		# Don't have still animation yet, so just leave it on previous animation
		#$AnimatedSprite.animation = 'still'
	
func _on_Player_body_entered(body):
	print('rip')
	emit_signal('hit')
	$CollisionShape2D.set_deferred('disabled', true)
	# $AnimatedSprite.animation = 'die'

func freeze():
	frozen = true

func unfreeze():
	frozen = false

func constrain_value(value, min_value, max_value):
	return min(max_value, max(min_value, value))

func converge_value(value, target, increment):
	var difference = target - value
	if abs(difference) < increment:
		return target
	else:
		 return value + increment * sign(difference)
