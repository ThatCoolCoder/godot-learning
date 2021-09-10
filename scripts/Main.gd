extends Node

export (int) var min_rock_time = 1
export (int) var max_rock_time = 2
export (float) var starting_rock_speed = 200
export (float) var rock_speed_increment = 5
var rock_speed
var score = 0
const score_increment = 1

export (PackedScene) var Rock

func _ready():
	new_game()

func new_game():
	score = 0
	rock_speed = starting_rock_speed
	$Player.reset($StartPosition.position)
	$RockTimer.wait_time = rand_range(min_rock_time, max_rock_time)
	$RockTimer.start()
	$ScoreTimer.start()
	$HUD/ScoreLabel.show()

func _on_RockTimer_timeout():
	# Create a Mob instance and add it to the scene.
	var rock = Rock.instance()
	add_child(rock)

	# Set the mob's position to a random location.
	rock.position.x = randi() % 400
	
	rock.rotation = rand_range(0, TAU)

	# Set the velocity (speed & direction).
	rock.speed = rock_speed
	rock_speed += rock_speed_increment
	$RockTimer.wait_time = rand_range(min_rock_time, max_rock_time)
	$RockTimer.wait_time *= starting_rock_speed / rock_speed

func game_over():
	$Player.freeze()
	$RockTimer.stop()
	$ScoreTimer.stop()
	$HUD/ScoreLabel.hide()
	get_tree().call_group('rocks', 'freeze')
	var timeout = $HUD.display_message(
		'You lose\n' + 
		'Your score: {score}'.format({'score' : int(score)}))
	yield(get_tree().create_timer(timeout), 'timeout')
	
	get_tree().call_group('rocks', 'queue_free')
	get_tree().change_scene("res://scenes/TitleScreen.tscn")


func _on_ScoreTimer_timeout():
	score += score_increment * (rock_speed / starting_rock_speed)
	$HUD.display_score(score)
