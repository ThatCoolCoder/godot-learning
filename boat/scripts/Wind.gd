extends Node

var vector = Vector2.ZERO
var direction setget set_direction, get_direction
var strength setget set_strength, get_strength

func set_direction(new_direction):
	vector = Vector2(vector.length(), 0).rotated(new_direction)

func get_direction():
	return vector.angle()

func set_strength(new_strength):
	vector = vector.normalized() * new_strength

func get_strength():
	return vector.length()
