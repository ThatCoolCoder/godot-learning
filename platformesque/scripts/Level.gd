extends Node

func _on_player_dead():
	LevelManagement.restart_crnt_level()


func _on_Player_finished_level():
	LevelManagement.start_level(LevelManagement.crnt_level_num + 1)
