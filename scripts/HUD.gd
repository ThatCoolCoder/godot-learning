extends CanvasLayer

export (float) var message_duration = 2
var _callback = null

func _ready():
	display_score(0)
	display_level(1)
	$MessageLabel.hide()

func display_message(message, callback = null):
	$MessageLabel.show()
	$MessageLabel.text = message
	$MessageTimer.wait_time = message_duration
	$MessageTimer.start()
	_callback = callback
	return message_duration

func _on_MessageTimer_timeout():
	$MessageLabel.hide()
	if _callback != null:
		call(_callback)

func display_score(score):
	$ScoreLabel.text = 'Score: ' + str(int(score))

func display_level(level):
	$LevelLabel.text = 'Level: ' + str(level)
