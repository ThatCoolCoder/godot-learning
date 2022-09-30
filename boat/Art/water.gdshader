shader_type spatial;
render_mode diffuse_lambert, specular_schlick_ggx;

uniform vec4 color: hint_color;
uniform float scale = 200.0;
uniform float vertex_distance = 1.0;

uniform float height_scale = 1;
uniform sampler2D noise;
uniform float noise_size = 512.0;
uniform sampler2D normal_map;


uniform float wave_map_size = 512.0;
uniform sampler2D wave_height_1;
uniform sampler2D wave_height_2;
uniform sampler2D wave_height_3;

const vec2 wave_angle_1 = vec2(1, 0);
const vec2 wave_angle_2 = vec2(0.5, 0.866);
const vec2 wave_angle_3 = vec2(-0.5, 0.866);
uniform float wave_speed = 1;
uniform float wave_height_scale = 0.1;


vec2 texture_pos_from_world(vec2 pos) {
    pos /= scale * 2.;
    pos += 0.5;
    return pos;
}

float wave_height_offset(vec2 pos, sampler2D height_map, vec2 wave_direction, float time) {
    vec2 movement = normalize(wave_direction) * time * wave_speed;
    pos += movement;
    vec2 normalized_pos = pos / wave_map_size;

    float height = texture(height_map, normalized_pos).x;
    height = pow(height, 2.);
    height *= wave_height_scale / height_scale;
    return height;
}


float height_at_pos(vec2 pos, float time) {
    vec2 normalized_pos = texture_pos_from_world(pos);

    float height = texture(noise, normalized_pos).x;
    height += wave_height_offset(pos, wave_height_1, wave_angle_1, time);
    height += wave_height_offset(pos, wave_height_2, wave_angle_2, time);
    height += wave_height_offset(pos, wave_height_3, wave_angle_3, time);

    return (height - 0.5) * height_scale;
}

void vertex() {
    vec2 pos = VERTEX.xz;
    float height = height_at_pos(pos, TIME);
    VERTEX.y += height;
}

void fragment() {
    // Generate normals for flat shading based on other vertices 
    NORMAL = normalize(cross(dFdx(VERTEX), dFdy(VERTEX)));

    float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
    // fresnel = 0.;
    RIM = 0.2;
    METALLIC = 0.0;
    ROUGHNESS = 0.01 * (1.0 - fresnel);
    // ROUGHNESS = 0.3;
    ALBEDO = color.rgb + (0.1 * fresnel);
}
