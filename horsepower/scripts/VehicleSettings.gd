extends Node

var crnt_vehicle := PackedScene.new()
var crnt_vehicle_name = "SuzukiApv"

func set_crnt_vehicle(vehicle: PackedScene):
	crnt_vehicle = vehicle
	var instance = crnt_vehicle.instance()
	crnt_vehicle_name = instance.vehicle_name
