#version 330 core

uniform sampler2D uTextures[8];

in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
out vec4 oColor;


void main() {
    if (fTexID > 0) 
    {
        int id = int(fTexID);
        oColor = fColor * texture(uTextures[id], fTexCoords); 
    }
    else {
        oColor = fColor; 
    }

}