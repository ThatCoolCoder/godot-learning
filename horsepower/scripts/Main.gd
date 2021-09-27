extends Node2D

export (float) var max_slope_deg = 30
var max_slope = null
export (float) var point_distance = 15
export (float) var first_point_height = 400
export (float) var max_slope_change_deg = 10
var max_slope_change = null
var lower_edge_height = 1000
var upper_edge_points = []
var max_upper_edge_point_dist = 100000
var screen_size = Vector2()

func _ready():
	screen_size = get_viewport_rect().size
	max_slope = max_slope_deg * (PI / 180)
	max_slope_change = max_slope_change_deg * (PI / 180)
	for i in range(10):
		upper_edge_points.append(Vector2(-screen_size.x / 2 + (screen_size.x / 10) * i,
			first_point_height))
	generate_polygons()

func _process(delta):
	screen_size = get_viewport_rect().size
	spawn_new_points()
	delete_old_points()
	generate_polygons()

func spawn_new_points():
	while upper_edge_points[-1].x - ($Car.position.x + screen_size.x / 2) < point_distance:
		var new_point = Vector2()
		new_point.x = upper_edge_points[-1].x + point_distance
		var crnt_slope = (upper_edge_points[-1] - upper_edge_points[-2]).angle()
		crnt_slope += rand_range(-max_slope_change, max_slope_change)
		crnt_slope = clamp(crnt_slope, -max_slope, max_slope)
		var height_offset = point_distance * atan(crnt_slope)
		new_point.y = upper_edge_points[-1].y + height_offset
		upper_edge_points.append(new_point)
		
	while ($Car.position.x - screen_size.x / 2) - upper_edge_points[0].x < point_distance:
		var new_point = Vector2()
		new_point.x = upper_edge_points[0].x - point_distance
		var crnt_slope = -(upper_edge_points[1] - upper_edge_points[0]).angle()
		crnt_slope += rand_range(-max_slope_change, max_slope_change)
		crnt_slope = clamp(crnt_slope, -max_slope, max_slope)
		var height_offset = point_distance * atan(crnt_slope)
		new_point.y = upper_edge_points[0].y + height_offset
		upper_edge_points.push_front(new_point)
		
func delete_old_points():
	for point in upper_edge_points:
		if point.x < $Car.position.x - max_upper_edge_point_dist or \
			$Car.position.x + max_upper_edge_point_dist < point.x:
			upper_edge_points.erase(point)

func generate_polygons():
	var points = PoolVector2Array()
	for upper_edge_point in upper_edge_points:
		if $Car.position.x - screen_size.x < upper_edge_point.x and \
			upper_edge_point.x < $Car.position.x + screen_size.x:
			points.append(upper_edge_point)
		
	points.append(Vector2(upper_edge_points[-1].x,
		lower_edge_height + upper_edge_points[-1].y))
	points.append(Vector2(upper_edge_points[0].x,
		lower_edge_height + upper_edge_points[0].y))
	points.append(upper_edge_points[0])
	$Ground/Polygon2D.polygon = points
	$Ground/CollisionPolygon2D.polygon = points
	
