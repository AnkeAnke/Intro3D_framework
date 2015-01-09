#version 330

uniform sampler2D DiffuseTexture;

layout(std140) uniform PerFrame
{
  mat4 ViewProjection;
  vec3 CameraPosition;
  vec3 CameraViewDir;
  float Time;
};

// Input = output from vertex shader.
in vec3 Normal;
in vec2 Texcoord;

out vec4 OutputColor;

void main()  
{     
   vec3 textureColor = texture(DiffuseTexture, Texcoord).rgb;

  vec3 lightDir = normalize(vec3(1,-1,1));
  vec3 normNormal = normalize(-Normal);
  vec3 halfVec = normalize(lightDir - CameraViewDir);

  float specular = pow(abs(dot(normNormal, halfVec)), 4.0); // intentionally wrong abs, just because it looks "cooler"
  float diffuse = dot(normNormal, lightDir);

  OutputColor.rgb = textureColor * diffuse + specular * vec3(0.8, 0.8, 1.0);
  OutputColor.a = 1.0; 
}