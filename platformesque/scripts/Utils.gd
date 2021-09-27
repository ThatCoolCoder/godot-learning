static func converge_value(value, target, increment):
	var difference = target - value
	if abs(difference) < abs(increment):
		return target
	else:
		return value + sign(difference) * increment
