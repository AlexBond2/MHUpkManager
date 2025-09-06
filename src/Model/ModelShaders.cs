using SharpGL;
using SharpGL.Shaders;

namespace MHUpkManager.Model
{
    public class ModelShaders
    {
        // Vertex Shader
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
out vec3 vViewDir;

uniform mat4 uProj;
uniform mat4 uView;
uniform mat4 uModel;
uniform vec3 uViewPos;

void main() {
    gl_Position = uProj * uView * uModel * vec4(aPosition, 1.0);
    vTexCoord = aTexCoord;
    
    // Transform normals to world space
    mat3 normalMatrix = mat3(transpose(inverse(uModel)));
    vNormal = normalize(normalMatrix * aNormal);
    vTangent = normalize(normalMatrix * aTangent);
    vBitangent = normalize(normalMatrix * aBitangent);
    
    vWorldPos = (uModel * vec4(aPosition, 1.0)).xyz;
    vViewDir = normalize(uViewPos - vWorldPos);
}";

        // Fragment Shader
        private const string fragmentNormal = @"#version 150 core
in vec3 vNormal;
in vec2 vTexCoord;
in vec3 vTangent;
in vec3 vBitangent;
in vec3 vWorldPos;
in vec3 vViewDir;

out vec4 fragColor;

// Textures
uniform sampler2D uDiffuseMap;
uniform sampler2D uNormalMap;
uniform sampler2D uSMSPSKMap;      // R=SpecMult, G=SpecPow, B=SkinMask, A=Reflectivity
uniform sampler2D uESPAMap;        // Emissive_SpecPower_Ambient
uniform sampler2D uSMRRMap;        // SpecMult_RimMask_Reflection
uniform sampler2D uRampMap;         // tf2_ramp
uniform sampler2D uSpecColorMap;    // Specular color texture

// Texture flags
uniform float uHasDiffuseMap = 1.0;
uniform float uHasNormalMap = 1.0;
uniform float uHasSMSPSK = 0.0;
uniform float uHasESPA = 0.0;
uniform float uHasSMRR = 0.0;
uniform float uHasSpecColorMap = 0.0;

// Lighting
uniform vec3 uLightDir;
uniform vec3 uLight1Dir;
uniform vec3 uLight0Color;
uniform vec3 uLight1Color;
uniform vec3 uViewPos;

// Material scalar parameters from UE3
uniform float uLambertDiffusePower = 1.0;
uniform float uPhongDiffusePower = 1.0;
uniform float uLightingAmbient = 0.1;
uniform float uShadowAmbientMult = 1.0;
uniform float uNormalStrength = 1.0;
uniform float uReflectionMult = 1.0;
uniform float uRimColorMult = 0.0;
uniform float uRimFalloff = 2.0;
uniform float uScreenLightAmount = 0.0;
uniform float uScreenLightMult = 1.0;
uniform float uScreenLightPower = 1.0;
uniform float uSpecMult = 1.0;
uniform float uSpecMultLQ = 0.5;
uniform float uSpecularPower = 15.0;
uniform float uSpecularPowerMask = 1.0;

// Material vector parameters from UE3
uniform vec3 uLambertAmbient = vec3(0.1);
uniform vec3 uShadowAmbientColor = vec3(0.05);
uniform vec3 uFillLightColor = vec3(0.2, 0.19, 0.18);
uniform vec3 uDiffuseColor = vec3(0.5);
uniform vec3 uSpecularColor = vec3(0.5);  // Used when no spec color map

// Subsurface scattering (from material properties)
uniform vec3 uSubsurfaceInscatteringColor = vec3(1.0, 1.0, 1.0);
uniform vec3 uSubsurfaceAbsorptionColor = vec3(0.902, 0.784, 0.784);
uniform float uImageReflectionNormalDampening = 5.0;

// Character material specific
uniform float uSkinScatterStrength = 0.5;
uniform float uTwoSidedLighting = 0.0;

uniform float uAlphaTest = 0.0;

struct MaterialMasks {
    float specMult;
    float specPower;
    float skinMask;
    float reflectivity;
    float emissive;
    float ambientOcclusion;
    float rimMask;
};

MaterialMasks getMaterialMasks() {
    MaterialMasks masks;
    
    if (uHasSMSPSK > 0.5) {
        // 1: SMSPSK
        vec4 smspsk = texture(uSMSPSKMap, vTexCoord);
        masks.specMult = smspsk.r;
        masks.specPower = smspsk.g;
        masks.skinMask = smspsk.b;
        masks.reflectivity = smspsk.a;
        masks.emissive = 0.0;
        masks.ambientOcclusion = 1.0;
        masks.rimMask = 1.0;
    }
    else if (uHasESPA > 0.5 && uHasSMRR > 0.5) {
        // 2: ESPA + SMRR
        vec4 espa = texture(uESPAMap, vTexCoord);
        vec4 smrr = uHasSMRR > 0.5 ? texture(uSMRRMap, vTexCoord) : vec4(0.0);
        
        masks.specMult = smrr.r;
        masks.rimMask = smrr.g; 
        masks.reflectivity = smrr.b;

        masks.skinMask = espa.b;
        masks.specPower = espa.g;

        masks.emissive = espa.r;
        masks.ambientOcclusion = 1.0;
    }
    else {
        // Defoult values if no maps are present
        masks.specMult = 0.0;
        masks.specPower = 0.0;
        masks.skinMask = 0.0;
        masks.reflectivity = 0.0;
        masks.emissive = 0.0;
        masks.ambientOcclusion = 1.0;
        masks.rimMask = 1.0;
    }
    
    return masks;
}

vec3 calculateNormal() {
    vec3 N = normalize(vNormal);
    
    if (uHasNormalMap > 0.5) {
        vec3 normalMapSample = texture(uNormalMap, vTexCoord).rgb;
        vec3 tangentNormal = normalize((normalMapSample * 2.0 - 1.0) * vec3(1.0, 1.0, uNormalStrength));
        
        vec3 T = normalize(vTangent);
        vec3 B = normalize(vBitangent);
        mat3 TBN = mat3(T, B, N);
        
        return normalize(TBN * tangentNormal);
    }
    
    return N;
}

vec3 getSpecularColor() {
    if (uHasSpecColorMap > 0.5) {
        return texture(uSpecColorMap, vTexCoord).rgb;
    }
    return uSpecularColor;
}

vec3 calculateDiffuse(vec3 normal, vec3 lightDir, float skinMask) {
    vec3 L = normalize(-lightDir);
    float NdotL = max(dot(normal, L), 0.0);
    
    // Lambert diffuse with power control
    float lambert = pow(NdotL, uLambertDiffusePower);
    
    // Phong diffuse for more control
    float phong = pow(max(0.0, dot(normal, L)), uPhongDiffusePower);
    
    // Mix between lambert and phong based on material settings
    float diffuse = mix(lambert, phong, 0.5);
    
    // Two-sided lighting for thin surfaces (ears, etc)
    if (uTwoSidedLighting > 0.5) {
        float backLight = max(0.0, dot(-normal, L));
        diffuse = max(diffuse, backLight * 0.5);
    }
    
    // Subsurface scattering for skin
    if (uHasSMSPSK > 0.5 && skinMask > 0.0) {
        float scatter = pow(max(0.0, dot(-normal, L)), 2.0);
        vec3 subsurface = uSubsurfaceInscatteringColor * scatter * skinMask * uSkinScatterStrength;
        
        // Apply absorption color
        subsurface *= uSubsurfaceAbsorptionColor;
        
        return vec3(diffuse) + subsurface;
    }
    
    return vec3(diffuse);
}

vec3 calculateSpecular(vec3 normal, vec3 lightDir, vec3 viewDir, float specMult, float specPower, float skinMask) {
    vec3 L = normalize(-lightDir);
    vec3 H = normalize(L + viewDir);
    float NdotH = max(dot(normal, H), 0.0);
    
    // Adjust specular power based on SMSPSK map and parameters
    float finalSpecPower = uSpecularPower;
    if (uHasSMSPSK > 0.5) {
        finalSpecPower = mix(uSpecularPower, uSpecularPower * 4.0, specPower) * uSpecularPowerMask;
        
        // Skin has different specular characteristics
        if (skinMask > 0.0) {
            finalSpecPower *= mix(1.0, 2.0, skinMask);
        }
    }
    
    // Calculate specular intensity
    float spec = pow(NdotH, finalSpecPower);
    
    // Apply multipliers
    float finalSpecMult = uSpecMult;
    if (uHasSMSPSK > 0.5) {
        // Lower quality specular option
        finalSpecMult = mix(finalSpecMult, uSpecMultLQ, 0.0);
    }
    
    vec3 specColor = getSpecularColor();
    return specColor * spec * finalSpecMult * specMult;
}

vec3 calculateRimLighting(vec3 normal, vec3 viewDir) {
    if (uRimColorMult <= 0.0) return vec3(0.0);
    
    float rim = 1.0 - max(dot(normal, viewDir), 0.0);
    rim = pow(rim, uRimFalloff) * uRimColorMult;
    
    return uFillLightColor * rim;
}

vec3 calculateScreenSpaceLighting(vec3 normal) {
    if (uScreenLightAmount <= 0.0) return vec3(0.0);
    
    // Simple screen-space ambient
    vec3 screenNormal = normal * 0.5 + 0.5;
    float screenLight = pow(screenNormal.y, uScreenLightPower) * uScreenLightMult;
    
    return vec3(screenLight * uScreenLightAmount);
}

void main() {
    // Get material masks
    MaterialMasks masks = getMaterialMasks();
    float specularMultiplier = masks.specMult;
    float specularPower = masks.specPower;
    float skinMask = masks.skinMask;
    float reflectivity = masks.reflectivity * uReflectionMult;
    float emissive = masks.emissive;
    float ambientOcclusion = masks.ambientOcclusion;
    float rimMask = masks.rimMask;

    // Sample textures
    vec4 diffuseSample = uHasDiffuseMap > 0.5 ? texture(uDiffuseMap, vTexCoord) : vec4(uDiffuseColor, 1.0);
    vec3 diffuseColor = diffuseSample.rgb;

    if (uAlphaTest > 0){
        float alpha = textureLod(uDiffuseMap, vTexCoord, 0.0).a;
        if (alpha < 0.5) discard;
    }

    diffuseColor *= ambientOcclusion;
    
    // Calculate world normal
    vec3 worldNormal = calculateNormal(); 
    vec3 viewDir = normalize(vViewDir);
    
    // Ambient lighting
    vec3 ambient = uLambertAmbient * uLightingAmbient;
    ambient += uShadowAmbientColor * uShadowAmbientMult;
    
    // Main light
    vec3 diffuse0 = calculateDiffuse(worldNormal, uLightDir, skinMask) * uLight0Color;
    vec3 specular0 = calculateSpecular(worldNormal, uLightDir, viewDir, 
                                      specularMultiplier, specularPower, skinMask) * uLight0Color;
    
    // Secondary light
    vec3 diffuse1 = calculateDiffuse(worldNormal, uLight1Dir, skinMask) * uLight1Color;
    vec3 specular1 = calculateSpecular(worldNormal, uLight1Dir, viewDir,
                                      specularMultiplier, specularPower, skinMask) * uLight1Color;
    
    // Fill light
    vec3 fillLight = uFillLightColor * max(0.0, dot(worldNormal, vec3(0.0, 1.0, 0.0))) * 0.5;
    
    // Rim lighting
    vec3 rimLight = calculateRimLighting(worldNormal, viewDir) * rimMask;
    
    // Screen space lighting
    vec3 screenLight = calculateScreenSpaceLighting(worldNormal);
    
    // Combine all lighting
    vec3 totalDiffuse = ambient + diffuse0 + diffuse1 + fillLight;
    vec3 totalSpecular = specular0 + specular1;
    
    // Apply to diffuse color
    vec3 finalColor = diffuseColor * totalDiffuse;
    finalColor += totalSpecular;
    finalColor += rimLight;
    finalColor += screenLight;
    
    // Simple reflection based on reflectivity
    if (reflectivity > 0.0) {
        vec3 reflectDir = reflect(-viewDir, worldNormal);
        // Dampen reflection based on normal
        float reflectAmount = reflectivity / (1.0 + uImageReflectionNormalDampening);
        finalColor += vec3(reflectAmount) * 0.2; // Simplified reflection
    }

    if (emissive > 0.0) {
        finalColor += diffuseColor * emissive * 2.0;
    }    
    finalColor = clamp(finalColor, 0.0, 1.0);

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
