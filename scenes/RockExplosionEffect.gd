extends Node2D

const Utils = preload('res://scripts/Utils.gd')

export (float) var small_rock_scale = 1
export (float) var large_rock_scale = 5
var rock_scale = 1
export (float) var min_volume = 10
export (float) var max_volume = 30
var audio_finished = false

func _ready():
	$CPUParticles2D.emitting = true
	$AudioStreamPlayer2D.volume_db = Utils.map_value(rock_scale,
		small_rock_scale, large_rock_scale, min_volume, max_volume)

func _process(delta):
	if not $CPUParticles2D.emitting and audio_finished:
		queue_free()

func _on_AudioStreamPlayer2D_finished():
	audio_finished = true
