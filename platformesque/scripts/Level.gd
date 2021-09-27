extends Node

func _on_player_dead():
	$LevelManagement.restart_crnt_level()
