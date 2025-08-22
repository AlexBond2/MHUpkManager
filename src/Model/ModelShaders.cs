using SharpGL;
using SharpGL.Shaders;

namespace MHUpkManager.Model
{
    public class ModelShaders
    {
        private const string vertexNormal = @"#version 150 core
in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoord;
in vec3 aTangent;
in vec3 aBitangent;
out vec3 vNormal;
out vec2 vTexCoord;
out vec3 vTangent;
out vec3 vBitangent;
out vec3 vWorldPos;
uniform mat4 uProj;
uniform mat4 uView;
uniform mat4 uModel;
void main() {
    gl_Position = uProj * uView * uModel * vec4(aPosition, 1.0);
    vTexCoord = aTexCoord;
    vNormal = normalize((uModel * vec4(aNormal, 0.0)).xyz);
    vTangent = normalize((uModel * vec4(aTangent, 0.0)).xyz);
    vBitangent = normalize((uModel * vec4(aBitangent, 0.0)).xyz);
    vWorldPos = (uModel * vec4(aPosition, 1.0)).xyz;
}";

        private const string fragmentNormal = @"#version 150 core
in vec3 vNormal;
in vec2 vTexCoord;
in vec3 vTangent;
in vec3 vBitangent;
in vec3 vWorldPos;
out vec4 fragColor;

uniform sampler2D uDiffuseMap;
uniform sampler2D uNormalMap;
uniform sampler2D uSMSPSKMap;

uniform vec3 uLightDir;
uniform vec3 uLight1Dir;
uniform vec3 uLight0Color;
uniform vec3 uLight1Color;

uniform vec3 uViewPos;
uniform float uSkinScatterStrength = 0.5;
uniform float uSkinSpecularPower = 32.0;
uniform vec3 uSkinSubsurfaceColor = vec3(1.0, 0.4, 0.3);
uniform float uHasSMSPSK = 0.0;

vec3 calculateLighting(vec3 normal, vec3 lightDir, vec3 lightColor, vec3 viewDir, 
                      float specMult, float specPower, float skinMask, float hasSMSPSK)
{
    vec3 L = normalize(-lightDir);
    float NdotL = max(dot(normal, L), 0.0);
    
    vec3 diffuse = vec3(NdotL) * lightColor;
    
    float subsurfaceAmount = hasSMSPSK * skinMask * uSkinScatterStrength;
    float subsurface = pow(max(0.0, dot(-normal, L)), 2.0) * subsurfaceAmount;
    vec3 subsurfaceContrib = uSkinSubsurfaceColor * subsurface;
    
    vec3 halfDir = normalize(L + viewDir);
    float NdotH = max(dot(normal, halfDir), 0.0);
    
    float finalSpecPower = mix(16.0, mix(uSkinSpecularPower, uSkinSpecularPower * 2.0, specPower), hasSMSPSK);
    float finalSpecMult = mix(0.2, specMult, hasSMSPSK);
    
    float spec = pow(NdotH, finalSpecPower) * finalSpecMult;
    vec3 specular = vec3(spec) * lightColor;
    
    return diffuse + subsurfaceContrib + specular;
}

void main() {
    vec3 diffuseColor = texture(uDiffuseMap, vTexCoord).rgb;
    vec3 normalMapSample = texture(uNormalMap, vTexCoord).rgb;
    vec3 tangentNormal = normalize(normalMapSample * 2.0 - 1.0);    
    vec3 N = normalize(vNormal);
    vec3 T = normalize(vTangent);
    vec3 B = normalize(vBitangent);   
    mat3 TBN = mat3(T, B, N);
    vec3 worldNormal = normalize(TBN * tangentNormal);

    vec4 smspskData = texture(uSMSPSKMap, vTexCoord);
    float specularMultiplier = smspskData.r;
    float specularPower = smspskData.g;
    float skinMask = smspskData.b;
    vec3 viewDir = normalize(uViewPos - vWorldPos);

    vec3 lighting = vec3(0.0);
    
    lighting += calculateLighting(worldNormal, uLightDir, uLight0Color, viewDir,
                                specularMultiplier, specularPower, skinMask, uHasSMSPSK);
    
    lighting += calculateLighting(worldNormal, uLight1Dir, uLight1Color, viewDir,
                                specularMultiplier, specularPower, skinMask, uHasSMSPSK);
    
    vec3 finalColor = diffuseColor * lighting;
    fragColor = vec4(finalColor, 1.0);
}";

        private const string vertexFont = @"#version 150 core
in vec3 inPosition;
in vec2 inUV;
uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uModel;
uniform mat4 uOrtho;
uniform vec3 uStartPos;
uniform vec2 uViewportSize;
uniform float uScale;
out vec2 passUV;
void main()
{
    vec4 worldPos = vec4(uStartPos, 1.0);
    vec4 clipPos = uProjection * uView * uModel * worldPos;
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
uniform mat4 uModel;
void main()
{
    gl_Position = uProjection * uView * uModel * vec4(inPosition, 1.0);
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
            attributes[4] = "aBitangent";
            NormalShader.Create(gl, vertexNormal, fragmentNormal, attributes);

            Initialized = true;
        }

        public void DestroyShaders(OpenGL gl)
        {
            if (Initialized)
            {
                NormalShader.Delete(gl);
                FontShader.Delete(gl);
                ColorShader.Delete(gl);
                ColorShader1.Delete(gl);
                Initialized = false;
            }
        }
    }
}
