extends PopupPanel

func _on_VehicleSelectMenu_about_to_show():
	# Focus the currently selected vehicle
	var buttons = $MarginContainer/VBoxContainer/HBoxContainer.get_children()
	for button in buttons:
		if button.text == VehicleSettings.crnt_vehicle_name:
			button.grab_focus()
			break

func _on_SaveButton_pressed():
	hide()
