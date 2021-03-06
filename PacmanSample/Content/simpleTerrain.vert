#version 330

layout(std140) uniform PerFrame
{
  mat4 WorldViewProjection;
  vec3 CameraPosition;
  vec3 CameraViewDir;
  float Time;
};


// Vertex input.
layout(location = 0) in vec2 inPosition;
layout(location = 1) in vec2 inTexcoord;
layout(location = 2) in float inHeight;
layout(location = 3) in vec3 inNormal;

// Output = input for fragment shader.
out vec2 Texcoord;
out float Height;
out vec3 Normal;

void main(void)
{
  gl_Position = WorldViewProjection * vec4(inPosition.x, inHeight, inPosition.y, 1.0);

  // Simply pass through
  Texcoord = inTexcoord;
  Height = inHeight;
  Normal = inNormal;
}  