extends Node2D

export (PackedScene) var starting_vehicle
var crnt_vehicle = null

func _ready():
	VehicleSettings.set_crnt_vehicle(starting_vehicle)
	set_crnt_vehicle(starting_vehicle)
	$Ground.car_position = crnt_vehicle.position

func _process(delta: float):
	$Ground.car_position = crnt_vehicle.position
	if crnt_vehicle.vehicle_name != VehicleSettings.crnt_vehicle_name:
		set_crnt_vehicle(VehicleSettings.crnt_vehicle)

func set_crnt_vehicle(vehicle: PackedScene):
	var old_vehicle = crnt_vehicle
	crnt_vehicle = vehicle.instance()
	add_child(crnt_vehicle)
	if old_vehicle != null:
		crnt_vehicle.transform = old_vehicle.transform
		crnt_vehicle.linear_velocity = old_vehicle.linear_velocity
		old_vehicle.queue_free()
