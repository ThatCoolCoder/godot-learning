extends StaticBody2D

var first_point_height = 300
var lower_edge_height = 1000
var upper_edge_points = []
var max_upper_edge_point_dist = 20000

var screen_size = Vector2()
var car_position := Vector2()

func _ready():
	screen_size = get_viewport_rect().size
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
	var max_slope = Utils.deg_to_rad(WorldSettings.ground_max_slope_deg)
	var slope_randomness = Utils.deg_to_rad(WorldSettings.ground_slope_randomness_deg)
	var ground_slope_bias = Utils.deg_to_rad(WorldSettings.ground_slope_bias_deg)
	while upper_edge_points[-1].x - (car_position.x + screen_size.x / 2) < \
		WorldSettings.ground_point_distance:
		var new_point = Vector2()
		new_point.x = upper_edge_points[-1].x + WorldSettings.ground_point_distance
		var crnt_slope = (upper_edge_points[-1] - upper_edge_points[-2]).angle()
		crnt_slope += rand_range(-slope_randomness, slope_randomness)
		crnt_slope = clamp(crnt_slope, -max_slope + ground_slope_bias,
			max_slope + ground_slope_bias)
		var height_offset = WorldSettings.ground_point_distance * atan(crnt_slope)
		new_point.y = upper_edge_points[-1].y + height_offset
		upper_edge_points.append(new_point)
		
	while (car_position.x - screen_size.x / 2) - upper_edge_points[0].x < WorldSettings.ground_point_distance:
		var new_point = Vector2()
		new_point.x = upper_edge_points[0].x - WorldSettings.ground_point_distance
		var crnt_slope = -(upper_edge_points[1] - upper_edge_points[0]).angle()
		crnt_slope += rand_range(-slope_randomness, slope_randomness)
		crnt_slope = clamp(crnt_slope, -max_slope + ground_slope_bias,
			max_slope + ground_slope_bias)
		var height_offset = WorldSettings.ground_point_distance * atan(crnt_slope)
		new_point.y = upper_edge_points[0].y + height_offset
		upper_edge_points.push_front(new_point)
		
func delete_old_points():
	for point in upper_edge_points:
		if point.x < car_position.x - max_upper_edge_point_dist or \
			car_position.x + max_upper_edge_point_dist < point.x:
			upper_edge_points.erase(point)

func generate_polygons():
	var points = PoolVector2Array()
	for upper_edge_point in upper_edge_points:
		if car_position.x - screen_size.x < upper_edge_point.x and \
			upper_edge_point.x < car_position.x + screen_size.x:
			points.append(upper_edge_point)
		
	points.append(Vector2(upper_edge_points[-1].x,
		lower_edge_height + upper_edge_points[-1].y))
	points.append(Vector2(upper_edge_points[0].x,
		lower_edge_height + upper_edge_points[0].y))
	points.append(upper_edge_points[0])
	$Polygon2D.polygon = points
	$CollisionPolygon2D.polygon = points
