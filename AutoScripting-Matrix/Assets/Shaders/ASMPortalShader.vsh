#version 300 es
#ifdef GLSL

// <Semantic Name='POSITION' Attribute='a_position' />
// <Semantic Name='TEXCOORD' Attribute='a_texcoord' />

uniform mat4 u_worldViewProjectionMatrix;

in vec3 a_position;
in vec2 a_texcoord;
out vec2 v_texcoord;
out mat4 v_worldViewProjectionMatrix;
out vec3 v_position;

void main()
{
    vec4 clipSpacePos = u_worldViewProjectionMatrix * vec4(a_position.xyz, 1.0);

    gl_Position = clipSpacePos;
    v_position = a_position.xyz;
    v_worldViewProjectionMatrix = u_worldViewProjectionMatrix;
    v_texcoord = a_texcoord;

    OPENGL_POSITION_FIX;
}

#endif
