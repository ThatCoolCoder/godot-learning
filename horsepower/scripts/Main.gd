extends Node2D

func _ready():
	$Ground.car_position = $Car.position

func _process(delta: float):
	$Ground.car_position = $Car.position
