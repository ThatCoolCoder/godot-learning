extends "res://scripts/power_ups/AbstractPowerUp.gd"

func on_activated():
	var rocks = get_tree().get_nodes_in_group('rocks')
	for rock in rocks:
		rock.explode()
		rock.queue_free()
