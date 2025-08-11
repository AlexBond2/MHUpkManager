using SharpGL;
using SharpGL.Shaders;

namespace MHUpkManager
{
    public class ModelShaders
    {
        private const string vertexNormal = @"#version 150 core

in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;
in vec3 aTangent;
out vec3 vNormal;
out vec2 vTexCoord;
out vec3 vTangent;
out vec3 vWorldPos;
uniform mat4 uProj;
uniform mat4 uView;
uniform mat4 uModel;
void main() {
    gl_Position = uProj * uView * uModel * vec4(aPosition, 1.0);
    vTexCoord = aTexCoord;
    vNormal = normalize((uModel * vec4(aNormal, 0.0)).xyz);
    vTangent = normalize((uModel * vec4(aTangent, 0.0)).xyz);
    vWorldPos = (uModel * vec4(aPosition, 1.0)).xyz;
}";

        private const string fragmentNormal = @"#version 150 core
in vec3 vNormal;
in vec2 vTexCoord;
in vec3 vTangent;
in vec3 vWorldPos;
out vec4 fragColor;
uniform sampler2D uDiffuseMap;
uniform sampler2D uNormalMap;
uniform vec3 uLightDir;
uniform vec3 uLight1Dir;
uniform vec3 uLight0Color;
uniform vec3 uLight1Color;
void main() {
    vec3 diffuseColor = texture(uDiffuseMap, vTexCoord).rgb;
    vec3 normalMapSample = texture(uNormalMap, vTexCoord).rgb;
    vec3 tangentNormal = normalize(normalMapSample * 2.0 - 1.0);
    vec3 N = normalize(vNormal);
    vec3 T_original = normalize(vTangent);   
    vec3 B = normalize(cross(N, T_original));    
    vec2 st0 = dFdx(vTexCoord);
    vec2 st1 = dFdy(vTexCoord);
    if (sign(st0.t * st1.s - st1.t * st0.s) < 0.0) {
        B = -B;
    }
    vec3 T = normalize(cross(B, N));  
    mat3 TBN = mat3(T, B, N);
    vec3 worldNormal = normalize(TBN * tangentNormal);
    float diff0 = max(dot(worldNormal, normalize(-uLightDir)), 0.0);
    float diff1 = max(dot(worldNormal, normalize(-uLight1Dir)), 0.0);    
    vec3 lighting = diffuseColor * (uLight0Color * diff0 + uLight1Color * diff1);    
    fragColor = vec4(lighting, 1.0);
}";

        private const string vertexFont = @"#version 150 core
in vec3 inPosition;
in vec2 inUV;

uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uOrtho;
uniform vec3 uStartPos;
uniform vec2 uViewportSize;
uniform float uScale;

out vec2 passUV;

void main()
{
    vec4 worldPos = vec4(uStartPos, 1.0);
    vec4 clipPos = uProjection * uView * worldPos;
    vec3 ndc = clipPos.xyz / clipPos.w;
    vec2 screenPixels = (ndc.xy * 0.5 + 0.5) * uViewportSize;
    screenPixels.y = uViewportSize.y - screenPixels.y;
    vec2 textOffset = inPosition.xy * uScale;
    vec2 finalScreenPos = screenPixels + textOffset;
    gl_Position = uOrtho * vec4(finalScreenPos, 0.0, 1.0);
    
    passUV = inUV;
}";
        private const string fragmentFont = @"#version 150 core
in vec2 passUV;
out vec4 FragColor;
uniform sampler2D uFontTexture;
uniform vec4 uTextColor;
void main()
{
    float alpha = texture(uFontTexture, passUV).r;
    FragColor = vec4(uTextColor.rgb, uTextColor.a * alpha);
}";
        private const string vertexColor = @"#version 150 core
in vec3 inPosition;
in vec4 inColor;
out vec4 vertColor;
uniform mat4 uProjection;
uniform mat4 uView;
void main()
{
    gl_Position = uProjection * uView * vec4(inPosition, 1.0);
    vertColor = inColor;
}";
        private const string fragmentColor = @"#version 150 core
in vec4 vertColor;
out vec4 fragColor;
void main()
{
    fragColor = vertColor;
}";
        private const string vertexColor1 = @"#version 150 core
in vec3 inPosition;
uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uModel;
void main()
{
    gl_Position = uProjection * uView * uModel * vec4(inPosition, 1.0);
}";
        private const string fragmentColor1 = @"#version 150 core
out vec4 outColor;
uniform vec4 uColor;
void main()
{
    outColor = uColor;
}";

        public ShaderProgram NormalShader;
        public ShaderProgram FontShader;
        public ShaderProgram ColorShader;
        public ShaderProgram ColorShader1;
        public bool Initialized { get; private set; }

        public void InitShaders(OpenGL gl)
        {
            Dictionary<uint, string> attributes = [];
            FontShader = new();
            attributes[0] = "inPosition";
            attributes[1] = "inUV";
            FontShader.Create(gl, vertexFont, fragmentFont, attributes);

            ColorShader = new();
            ColorShader.Create(gl, vertexColor, fragmentColor, null);

            ColorShader1 = new();
            ColorShader1.Create(gl, vertexColor1, fragmentColor1, null);

            NormalShader = new();
            attributes = [];
            attributes[0] = "aPosition";
            attributes[1] = "aNormal";
            attributes[2] = "aTexCoord";
            attributes[3] = "aTangent";
            NormalShader.Create(gl, vertexNormal, fragmentNormal, attributes);

            Initialized = true;
        }
    }
}
