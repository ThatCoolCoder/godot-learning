extends "res://scenes/power_ups/AbstractPowerUp.gd"

export (float) var speed_boost_amount = 1000

# Called when the node enters the scene tree for the first time.
func _ready():
	._ready()

func on_activated():
	var players = get_tree().get_nodes_in_group('player')
	print(len(players))
	for player in players:
		player.speed += speed_boost_amount
	
func on_deactivated():
	var players = get_tree().get_nodes_in_group('player')
	for player in players:
		player.speed -= speed_boost_amount
