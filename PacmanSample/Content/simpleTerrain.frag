#version 330

uniform sampler2D DiffuseTexture;

// Input = output from vertex shader.
in vec2 Texcoord;
in float Height;
in vec3 Normal;

out vec4 OutputColor;

void main()  
{ 
  OutputColor = vec4(Normal*0.5 + 0.5, 1); //(Height*0.25+0.5)*2.0 * texture(DiffuseTexture, Texcoord*10.0) + (Height*0.25)*2.0 * vec4(1,1,1,1);  
}