extends Button

export (PackedScene) var vehicle

func _ready():
	text = vehicle.instance().vehicle_name

func _on_VehicleButton_pressed():
	VehicleSettings.set_crnt_vehicle(vehicle)
