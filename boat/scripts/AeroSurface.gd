extends Node2D

export var area := 1
export (NodePath) var body_path

onready var body: RigidBody2D = get_node(body_path)

func _physics_process(delta):
	var heading_delta = body.linear_velocity.angle() - global_rotation
	#print(body.linear_velocity.angle(), global_rotation, heading_delta)
