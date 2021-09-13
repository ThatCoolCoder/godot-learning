extends Node2D

export (float) var duration = 5

func _ready():
	$Sprite.show()

func on_activated():
	print('Error: on_activated method of power up was not overridden')
	
func on_deactivated():
	print('Error: on_deactivated method of power up was not overridden')

func _on_area_entered(area):
	if area.is_in_group('player'):
		$Sprite.hide()
		on_activated()
		$Timer.start()

func _on_Timer_timeout():
	on_deactivated()
