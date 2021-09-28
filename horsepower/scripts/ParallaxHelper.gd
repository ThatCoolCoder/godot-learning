extends ParallaxLayer

func _ready():
	motion_mirroring = $Sprite.texture.get_size().rotated($Sprite.global_rotation)
