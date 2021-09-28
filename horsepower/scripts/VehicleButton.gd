extends Button

export var vehicle_name := 'Unnamed vehicle'
export (PackedScene) var vehicle

func _ready():
	text = vehicle_name

func _on_VehicleButton_pressed():
	VehicleSettings.crnt_vehicle = vehicle
