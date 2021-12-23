extends Node

static func deg_to_rad(degrees: float):
	return degrees * (PI / 180)

static func rad_to_deg(radians: float):
	return radians * (180 / PI)

static func constrain(val, min_val, max_val):
	return min(max(min_val, val), max_val)
