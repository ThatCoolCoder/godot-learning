// IMPORTANT: when modifying the math, please modify Sea.cs to reflect the changes,
// or physics and graphics will desync!

shader_type spatial;
render_mode diffuse_lambert, specular_schlick_ggx, world_vertex_coords, cull_disabled, depth_draw_always;

uniform vec4 color: hint_color;
uniform float scale = 200.0;

uniform float height_scale = 1;
uniform sampler2D noise;
uniform float noise_size = 512.0;

uniform float global_time = 0.;
uniform float wave_map_size = 512.0;
uniform sampler2D wave_height_1;
uniform sampler2D wave_height_2;
uniform sampler2D wave_height_3;

uniform vec2 wave_angle_1 = vec2(1, 0);
uniform vec2 wave_angle_2 = vec2(0.5, 0.866);
uniform vec2 wave_angle_3 = vec2(-0.5, 0.866);
uniform float wave_speed = 1;
uniform float wave_height_scale = 0.1;

uniform sampler2D normal_detail_noise; // noise is not relevant to fluid simulation, is only for adding extra detail to the normal
uniform float normal_detail_scale = 1;
uniform float normal_detail_factor = 0.1;

uniform bool distance_fade_enabled = true;
uniform float distance_fade_start;
uniform float distance_fade_end;
varying float z_dist;

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
    height -= .5;
    height *= wave_height_scale / height_scale;
    return height;
}


float height_at_pos(vec2 pos, float time) {
    vec2 normalized_pos = texture_pos_from_world(pos);

    float height = texture(noise, normalized_pos).x - 0.5f;
    height += wave_height_offset(pos, wave_height_1, wave_angle_1, time);
    height += wave_height_offset(pos, wave_height_2, wave_angle_2, time);
    height += wave_height_offset(pos, wave_height_3, wave_angle_3, time);

    return height * height_scale;
    return 0.;
}

void vertex() {
    vec2 pos = VERTEX.xz;
    float height = height_at_pos(pos, global_time);
    VERTEX.y += height;

    float dist = 0.1;
    float height_a = height_at_pos(pos + vec2(dist, 0.0), global_time);
    float height_b = height_at_pos(pos + vec2(0.0, dist), global_time);

    NORMAL = normalize(vec3(height - height_a, dist, height - height_b));

    z_dist = -(MODELVIEW_MATRIX * vec4(VERTEX, 1.0)).z;
}

void fragment() {
    // Generate normals for flat shading based on other vertices 
    // NORMAL = normalize(cross(dFdx(VERTEX), dFdy(VERTEX)));


    vec3 world_vertex = (CAMERA_MATRIX * vec4(VERTEX, 1.0)).xyz;
    NORMAL = normalize(NORMAL + texture(normal_detail_noise, world_vertex.xz * normal_detail_scale).xyz * normal_detail_factor);

    float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));

    ALBEDO = color.rgb + (0.1 * fresnel);

    RIM = 0.2;
    METALLIC = 0.0;
    ROUGHNESS = 0.01 * (1.0 - fresnel);

    if (z_dist >= distance_fade_start && distance_fade_enabled)
    {
        float proportion = (z_dist - distance_fade_start) / (distance_fade_end - distance_fade_start);
        ALPHA = max(1. - proportion, 0.) * color.a;
    }
    else
    {
        ALPHA = color.a;
    }
}
