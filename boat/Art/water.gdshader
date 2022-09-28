shader_type spatial;
render_mode specular_toon;

uniform vec4 color: hint_color;
uniform float height_scale = 1;
uniform sampler2D noise;
uniform sampler2D normal_map;
uniform float size = 20.0;

varying vec2 tex_position;

void vertex() {
  tex_position = VERTEX.xz / size + 0.5;
  float height = texture(noise, tex_position).x - 0.5;
  VERTEX.y += height * height_scale;
}

void fragment() {

  float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
	RIM = 0.2;
	METALLIC = 0.0;
	ROUGHNESS = 0.01 * (1.0 - fresnel);
	ALBEDO = color.rgb + (0.1 * fresnel);

  // Give us normals
  NORMALMAP = texture(normal_map, tex_position).xyz;
}
