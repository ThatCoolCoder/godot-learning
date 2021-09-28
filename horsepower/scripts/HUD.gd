extends Control

export (int) var car_flip_speed_threshold = 150
export (int) var car_flip_angle_threshold_deg = 130
var car_flip_angle_threshold
export (float) var speed_conversion_factor = 0.045

func _ready():
	$StuckText.hide()
	car_flip_angle_threshold = car_flip_angle_threshold_deg / (180 / PI)

func _process(delta):
	var cars = get_tree().get_nodes_in_group('cars')
	if len(cars) == 0:
		return
	var car = cars[0]
	
	var angle_valid = car.rotation < -car_flip_angle_threshold or \
		car_flip_angle_threshold < car.rotation
	var speed_valid = car.linear_velocity.length() < car_flip_speed_threshold
	if angle_valid and speed_valid:
		$StuckText.show()
	else:
		$StuckText.hide()
		
	$SpeedLabel.text = '%s km/h' % int(car.linear_velocity.length() * speed_conversion_factor)


func _on_PauseButton_pressed():
	get_tree().paused = true
	$PauseMenu.popup()
