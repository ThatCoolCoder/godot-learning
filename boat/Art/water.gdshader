shader_type spatial;
render_mode diffuse_lambert, specular_schlick_ggx;

uniform vec4 color: hint_color;
uniform float scale = 20.0;

uniform float height_scale = 1;
uniform sampler2D noise;
uniform sampler2D normal_map;

uniform sampler2D wave_height_1;
uniform sampler2D wave_height_2;
uniform sampler2D wave_height_3;
// const vec2 wave_angle_1 = vec2(-0.017, 0.99);
// const vec2 wave_angle_2 = vec2(-0.642, 0.766);
// const vec2 wave_angle_3 = vec2(-0.358, 0.933);

const vec2 wave_angle_1 = vec2(1, 0);
const vec2 wave_angle_2 = vec2(0.5, 0.866);
const vec2 wave_angle_3 = vec2(-0.5, 0.866);
uniform float wave_speed = 1;
uniform float wave_height_scale = 0.1;


float wave_height_offset(vec2 pos, sampler2D height_map, vec2 wave_direction, float time) {
    vec2 movement = normalize(wave_direction) * time * wave_speed;
    vec2 final_pos = pos / scale + movement;
    float height = texture(height_map, final_pos).x;
    height = pow(height, 2.);
    //height -= 0.5;
    height *= wave_height_scale;
    return height;
}

float height_at_pos(vec2 pos, float time) {
    vec2 final_pos = pos;
    final_pos /= scale;
    final_pos -= 0.5;

    float height = texture(noise, final_pos).x;
    height += wave_height_offset(pos, wave_height_1, wave_angle_1, time);
    height += wave_height_offset(pos, wave_height_2, wave_angle_2, time);
    height += wave_height_offset(pos, wave_height_3, wave_angle_3, time);

    height -= 0.5;
    height *= height_scale;
    
    return height;
}

void vertex() {
    vec2 pos = VERTEX.xz;
    float height = height_at_pos(pos, TIME);
    NORMAL = normalize(
        vec3(
            height - height_at_pos(pos + vec2(0.1, 0.0), TIME),
            0.1,
            height - height_at_pos(pos + vec2(0.0, 0.1), TIME)
        )
    );
    VERTEX.y += height;
}

void fragment() {
    float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
    fresnel = 0.;
    // RIM = 0.2;
    METALLIC = 0.0;
    // ROUGHNESS = 0.01 * (1.0 - fresnel);
    ROUGHNESS = 0.3;
    ALBEDO = color.rgb + (0.1 * fresnel);
}
