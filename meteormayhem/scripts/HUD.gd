extends CanvasLayer

export (float) var message_duration = 2

func _ready():
	display_score(0)
	$MessageLabel.hide()

func display_message(message):
	$MessageLabel.show()
	$MessageLabel.text = message
	$MessageTimer.wait_time = message_duration
	$MessageTimer.start()
	return message_duration

func _on_MessageTimer_timeout():
	$MessageLabel.hide()

func display_score(score):
	$ScoreLabel.text = 'Score: ' + str(int(score))
