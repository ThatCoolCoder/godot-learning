extends PopupPanel

func _ready():
	$MarginContainer/VBoxContainer/MaxSlopeSlider.value = WorldSettings.ground_max_slope_deg
	$MarginContainer/VBoxContainer/SlopeRandomnessSlider.value = WorldSettings.ground_slope_randomness_deg
	$MarginContainer/VBoxContainer/PointDistanceSlider.value = WorldSettings.ground_point_distance

func _on_UnpauseButton_pressed():
	hide()
	get_tree().paused = false

func _on_MaxSlopeSlider_value_changed(value):
	WorldSettings.ground_max_slope_deg = value

func _on_SlopeRandomnessSlider_value_changed(value):
	WorldSettings.ground_slope_randomness_deg = value

func _on_PointDistanceSlider_value_changed(value):
	WorldSettings.ground_point_distance = value
	
func _on_SlopeBiasSlider_value_changed(value):
	WorldSettings.ground_slope_bias_deg = value

func _on_RestartButton_pressed():
	get_tree().paused = false
	get_tree().reload_current_scene()

func _on_ChangeVehicleButton_pressed():
	$MarginContainer/VBoxContainer/CenterContainer/VehicleSelectMenu.popup()

