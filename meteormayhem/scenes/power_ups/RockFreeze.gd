extends "res://scenes/power_ups/AbstractPowerUp.gd"

func on_activated():
	var rocks = get_tree().get_nodes_in_group('rocks')
	for rock in rocks:
		rock.frozen = true
	
func on_deactivated():
	var rocks = get_tree().get_nodes_in_group('rocks')
	for rock in rocks:
		rock.frozen = false
