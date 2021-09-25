extends "res://scripts/power_ups/AbstractPowerUp.gd"

export (float) var max_wind_speed = 300
export (Curve) var wind_speed_curve
var wind_speed = 0

func on_activated():
	$AudioStreamPlayer2D.play()

func on_deactivated():
	pass

func _process(delta):
	if activated:
		var rocks = get_tree().get_nodes_in_group('rocks')
		var screen_size = get_viewport_rect().size
		wind_speed = wind_speed_curve.interpolate(1 - ($Timer.time_left / duration)) * max_wind_speed
		for rock in rocks:
			if rock.frozen:
				continue
			if rock.position.x < screen_size.x / 2:
				rock.position.x -= wind_speed * delta
			else:
				rock.position.x += wind_speed * delta
