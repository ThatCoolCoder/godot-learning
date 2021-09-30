extends Camera2D

export (float) var zoom_rate = 1.1

func _process(delta: float):
	return
	if Input.is_action_pressed('zoom_out'):
		zoom /= zoom_rate
	if Input.is_action_pressed('zoom_in'):
		zoom *= zoom_rate
