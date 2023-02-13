## Notes about the organisation

In this godot project there is both a basic 2D sim (used to prototype the system of effectors), and a more advanced 3D sim. Generally 3D stuff says spatial or 3D to differentiate it.

## Notes about the physics:

(2d) Zero angle is to the right.

(3d) Z-positive is forward (yes I know this is opposite of Godot convention, this should be fixed before converting this to a playable game)

(2d) Positive angle of attack means that object is rotated further clockwise than the relative velocity to the airstream - eg moving to right but facing down-right is 45° of positive AOA.