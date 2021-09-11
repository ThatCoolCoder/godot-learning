extends Node

export (float) var rock_chance = 40
export (float) var starting_rock_speed = 300
export (float) var rock_speed_increment = 10
var rock_speed
export (float) var min_rock_scale = 1
export (float) var max_rock_scale = 5
export (float) var min_rock_rotation_speed = -PI / 2
export (float) var max_rock_rotation_speed = PI / 2
var score = 0
const score_increment = 1

export (PackedScene) var Rock

func _ready():
	new_game()
	
func _process(delta):
	var chance_multiplier =  starting_rock_speed / rock_speed
	if $Player.alive and randf() < 1 / (rock_chance * chance_multiplier):
		create_rock()
	
	rock_speed += rock_speed_increment * delta
	get_tree().set_group('rocks', 'speed', rock_speed)

func new_game():
	score = 0
	rock_speed = starting_rock_speed
	$Player.reset($StartPosition.position)
	$ScoreTimer.start()
	$HUD/ScoreLabel.show()

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

func game_over():
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

func _on_Floor_body_entered(body):
	body.queue_free()
