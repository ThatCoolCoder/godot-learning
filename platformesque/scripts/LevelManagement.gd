extends Node2D

const crnt_level_file_name = 'user://crnt_level.dat'

func _ready():
	set_crnt_level_num(1)
	var file = File.new()
	if not file.file_exists(crnt_level_file_name):
		set_crnt_level_num(1)

func get_crnt_level_num():
	var file = File.new()
	file.open(crnt_level_file_name, File.READ)
	var level_num = file.get_8()
	file.close()
	return level_num

func set_crnt_level_num(level_num: int):
	var file = File.new()
	file.open(crnt_level_file_name, File.WRITE)
	file.store_8(level_num)
	file.close()

func restart_crnt_level():
	start_level(get_crnt_level_num())

func start_level(level_num: int):
	var level_name = str(level_num).pad_zeros(2)
	get_tree().change_scene('res://scenes/levels/%s.tscn' % level_name)
