extends CanvasLayer

const Utils = preload('res://scripts/Utils.gd')

func _ready():
	show_high_score(Utils.load_high_score())

func show_high_score(high_score):
	if high_score > 0:
		$HighScoreLabel.show()
		$ResetHighScoreButton.show()
		$HighScoreLabel.text = 'High score: ' + str(high_score)
	else:
		$HighScoreLabel.hide()
		$ResetHighScoreButton.hide()

func _on_PlayButton_pressed():
	get_tree().change_scene("res://scenes/Main.tscn")

func _on_ResetHighScoreButton_pressed():
	Utils.save_high_score(0)
	show_high_score(0)

func _on_MoreProjectsButton_pressed():
	OS.shell_open(Utils.more_projects_url)
