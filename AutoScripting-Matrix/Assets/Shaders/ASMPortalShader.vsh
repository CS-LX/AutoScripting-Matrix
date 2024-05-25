#ifdef GLSL

// <Semantic Name='POSITION' Attribute='a_position' />
// <Semantic Name='TEXCOORD' Attribute='a_texcoord' />

uniform mat4 u_worldViewProjectionMatrix;

attribute vec3 a_position;
attribute vec2 a_texcoord;
varying vec2 v_texcoord;

void main()
{
    vec4 clipSpacePos = u_worldViewProjectionMatrix * vec4(a_position.xyz, 1.0);

    vec3 ndc = clipSpacePos.xyz / clipSpacePos.w;
    vec2 ScreenCoord = ndc.xy * 0.5 + 0.5;
    ScreenCoord = vec2(ScreenCoord.x, 1 - ScreenCoord.y);

    gl_Position = clipSpacePos ;
    v_texcoord = ScreenCoord;

    OPENGL_POSITION_FIX;
}

#endif
