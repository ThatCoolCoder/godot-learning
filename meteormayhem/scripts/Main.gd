extends Node

const Utils = preload('res://scripts/Utils.gd')

export (float) var rock_chance = 55
export (float) var starting_rock_speed = 300
export (float) var rock_speed_increment = 7
export (float) var max_rock_speed = 1200
var rock_speed
export (float) var min_rock_scale = 1
export (float) var max_rock_scale = 5
export (float) var min_rock_rotation_speed = -PI / 2
export (float) var max_rock_rotation_speed = PI / 2
export (float) var score_speed = 10 # How fast the score goes up
export (float) var score_speed_increment = 10 / 30 # How fast the score going up goes up
var score = 0
export (Array, PackedScene) var power_ups
export (Array, int) var power_up_spawn_position_xs = [50, 350]
export (float) var power_up_chance = 900

export (PackedScene) var Rock

func _ready():
	if OS.get_name()=="HTML5":
		OS.set_window_maximized(true)
	new_game()
	
func _process(delta):
	if $Player.alive and randf() < 1 / power_up_chance:
		create_power_up()
	
	var chance_multiplier =  starting_rock_speed / rock_speed
	if $Player.alive and randf() < 1 / (rock_chance * chance_multiplier):
		create_rock()
	
	rock_speed += rock_speed_increment * delta
	rock_speed = min(rock_speed, max_rock_speed)
	get_tree().set_group('rocks', 'speed', rock_speed)
	
	score_speed += score_speed_increment * delta

func new_game():
	score = 0
	rock_speed = starting_rock_speed
	$ScoreTimer.start()
	$HUD/ScoreLabel.show()
	$Player.reset($StartPosition.position)

func create_rock():
	# Create a Mob instance and add it to the scene.
	var rock = Rock.instance()
	add_child(rock)
	
	rock.position.x = randi() % 400
	rock.rotation = rand_range(0, TAU)
	rock.rotation_speed = rand_range(min_rock_rotation_speed, max_rock_rotation_speed)
	var scale = rand_range(min_rock_scale, max_rock_scale)
	rock.scale.x = scale
	rock.scale.y = scale
	rock.speed = rock_speed

func create_power_up():
	var power_up = power_ups[randi() % power_ups.size()].instance()
	
	power_up.position.x = power_up_spawn_position_xs[randi() % \
		power_up_spawn_position_xs.size()]
	power_up.position.y = $StartPosition.position.y
	
	add_child(power_up)

func game_over():
	$ScoreTimer.stop()
	$HUD/ScoreLabel.hide()
	
	var high_score = Utils.load_high_score()
	var message = ''
	if score > high_score:
		Utils.save_high_score(score)
		message = 'Game over\n' + \
			'You scored {score}\n'.format({'score' : int(score)}) + \
			'New high score!'
	else:
		message = 'Game over\n' + \
			'You scored {score}\n'.format({'score' : int(score)}) + \
			'Your high score is {high_score}\n'.format({'high_score' : int(high_score)})
	var timeout = $HUD.display_message(message)
	yield(get_tree().create_timer(timeout), 'timeout')
	
	get_tree().call_group('rocks', 'queue_free')
	get_tree().change_scene("res://scenes/TitleScreen.tscn")


func _on_ScoreTimer_timeout():
	score += score_speed * $ScoreTimer.wait_time
	$HUD.display_score(score)
