extends "res://scenes/power_ups/AbstractPowerUp.gd"

export (float) var speed_boost_amount = 1000

func on_activated():
	var players = get_tree().get_nodes_in_group('player')
	for player in players:
		player.speed += speed_boost_amount
	
func on_deactivated():
	var players = get_tree().get_nodes_in_group('player')
	for player in players:
		player.speed -= speed_boost_amount
