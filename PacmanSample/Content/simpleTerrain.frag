#version 330

uniform sampler2D DiffuseTexture;

// Input = output from vertex shader.
in vec2 Texcoord;
in float Height;
in vec3 Normal;

out vec4 OutputColor;

void main()  
{ 
vec3 lightDir = normalize(vec3(1,-1,1));
vec3 normNormal = normalize(Normal);
float diffuse = dot(normNormal, lightDir);
  OutputColor = (diffuse + 0.2) * texture(DiffuseTexture, Texcoord*10.0);// *(Height*0.25+0.5)*2.0  + (Height*0.25)*2.0 * vec4(1,1,1,1);  
}