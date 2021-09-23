extends Node2D

signal hit

export (int) var base_speed = 300
var speed = base_speed

var target_x = 0
var screen_size
var alive = true

func _ready():
	screen_size = get_viewport_rect().size

func reset(_position: Vector2):
	target_x = _position.x
	position = Vector2(_position.x, _position.y)
	$CollisionShape2D.set_deferred('disabled', false)
	$AnimatedSprite.animation = 'left'
	alive = true
	speed = base_speed

func _process(delta):
	if not alive:
		return
		
	if Input.is_mouse_button_pressed(1):
		target_x = get_viewport().get_mouse_position().x
	
	var position_difference = target_x - position.x
	if abs(position_difference) < delta * speed:
		position.x = target_x
	else:
		 position.x += delta * speed * sign(position_difference)
	position.x = constrain_value(position.x, 0, screen_size.x)
	
	var previous_animation = $AnimatedSprite.animation
	if position_difference < 0:
		$AnimatedSprite.animation = 'left'
	elif position_difference > 0:
		$AnimatedSprite.animation = 'right'
	else:
		$AnimatedSprite.animation = 'still'
		
	if $AnimatedSprite.animation != previous_animation:
		$AnimatedSprite.play()
	
func _on_Player_body_entered(body):
	emit_signal('hit')
	alive = false
	$CollisionShape2D.set_deferred('disabled', true)
	$AnimatedSprite.animation = 'die'
	$AnimatedSprite.play()

func constrain_value(value, min_value, max_value):
	return min(max_value, max(min_value, value))

func converge_value(value, target, increment):
	var difference = target - value
	if abs(difference) < increment:
		return target
	else:
		 return value + increment * sign(difference)
