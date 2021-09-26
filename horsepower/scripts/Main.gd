extends Node2D

export (float) var max_slope_deg = 30
var max_slope = null
export (float) var min_point_distance = 150
export (float) var max_point_distance = 600
export (float) var first_point_height = 400
var lower_edge_height = 1000
var upper_edge_points = []
var screen_size = Vector2()

func _ready():
	screen_size = get_viewport_rect().size
	max_slope = max_slope_deg * (PI / 180)
	upper_edge_points.append(Vector2(-screen_size.x, first_point_height))
	upper_edge_points.append(Vector2(screen_size.x, first_point_height))
	generate_polygons()

func _process(delta):
	screen_size = get_viewport_rect().size
	spawn_new_points()
	delete_old_points()
	generate_polygons()

func spawn_new_points():
	var prev_point = upper_edge_points[-1]
	if prev_point.x - $Car.position.x < screen_size.x:
		var new_point = Vector2()
		var point_distance = rand_range(min_point_distance, max_point_distance)
		new_point.x = prev_point.x + point_distance
		var max_height_offset = point_distance * tan(max_slope)
		new_point.y = prev_point.y + rand_range(-max_height_offset, max_height_offset)
		upper_edge_points.append(new_point)
		
func delete_old_points():
	# First find all of the points that are off screen
	var points_off_screen = []
	for point in upper_edge_points:
		if $Car.position.x - point.x > screen_size.x:
			points_off_screen.append(point)
	
	# Then delete all of them except the last,
	# because that one is still needed for a good polylgon
	if len(points_off_screen) > 1:
		for point_idx in range(len(points_off_screen) - 1):
			upper_edge_points.erase(points_off_screen[point_idx])
	print(len(upper_edge_points))

func generate_polygons():
	var points = PoolVector2Array()
	for upper_edge_point in upper_edge_points:
		points.append(upper_edge_point)
	points.append(Vector2(upper_edge_points[-1].x,
		lower_edge_height + upper_edge_points[-1].y))
	points.append(Vector2(upper_edge_points[0].x,
		lower_edge_height + upper_edge_points[0].y))
	points.append(upper_edge_points[0])
	$Ground/Polygon2D.polygon = points
	$Ground/CollisionPolygon2D.polygon = points
	
