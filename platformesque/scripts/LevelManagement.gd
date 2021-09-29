extends Node

var crnt_level_num = 1
		
func restart_crnt_level():
	start_level(crnt_level_num)

func start_level(level_num: int):
	var level_name = str(level_num).pad_zeros(2)
	get_tree().change_scene('res://scenes/levels/%s.tscn' % level_name)
