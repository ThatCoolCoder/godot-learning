extends Area2D

export (float) var duration = 5
var activated = false

func _ready():
	$Timer.wait_time = duration

func on_activated():
	print('Error: on_activated method of power up was not overridden')
	
func on_deactivated():
	print('Error: on_deactivated method of power up was not overridden')

func _on_area_entered(area):
	if activated:
		return
		
	if area.is_in_group('players'):
		$AnimatedSprite.hide()
		activated = true
		on_activated()
		$Timer.start()

func _on_Timer_timeout():
	activated = false
	on_deactivated()
	queue_free()
