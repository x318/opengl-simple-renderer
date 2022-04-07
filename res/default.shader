#vertex
#version 330 core

layout(location = 0) in vec3 aPosition;

layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    texCoord = aTexCoord;

    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}

#fragment
#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);
}