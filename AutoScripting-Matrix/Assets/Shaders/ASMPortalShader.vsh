#ifdef GLSL

// <Semantic Name='POSITION' Attribute='a_position' />
// <Semantic Name='TEXCOORD' Attribute='a_texcoord' />

uniform mat4 u_worldViewProjectionMatrix;

attribute vec3 a_position;
attribute vec2 a_texcoord;
varying vec2 v_texcoord;
varying mat4 v_worldViewProjectionMatrix;
varying vec3 v_position;

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
