using SharpGL;
using SharpGL.Shaders;

namespace MHUpkManager
{
    public class ModelShaders
    {
        private const string vertexNormal = @"#version 120
attribute vec3 aPosition;
attribute vec3 aNormal;
attribute vec2 aTexCoord;
attribute vec3 aTangent;
varying vec3 vNormal;
varying vec2 vTexCoord;
varying vec3 vTangent;
varying vec3 vWorldPos;
uniform mat4 uProj;
uniform mat4 uModel;
void main() {
    gl_Position = uProj * uModel * vec4(aPosition, 1.0);
    vTexCoord = aTexCoord;
    vNormal = normalize((uModel * vec4(aNormal, 0.0)).xyz);
    vTangent = normalize((uModel * vec4(aTangent, 0.0)).xyz);
    vWorldPos = (uModel * vec4(aPosition, 1.0)).xyz;
}";

        private const string fragmentNormal = @"#version 120
varying vec3 vNormal;
varying vec2 vTexCoord;
varying vec3 vTangent;
varying vec3 vWorldPos;
uniform sampler2D uDiffuseMap;
uniform sampler2D uNormalMap;
uniform vec3 uLightDir;
uniform vec3 uLight1Dir;
uniform vec3 uLight0Color;
uniform vec3 uLight1Color;
void main() {
    vec3 normalMap = texture2D(uNormalMap, vTexCoord).rgb * 2.0 - 1.0;
    vec3 N = normalize(vNormal);
    vec3 T = normalize(vTangent);
    vec3 B = normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);
    vec3 worldNormal = normalize(TBN * normalMap);
    float diff0 = max(dot(N, normalize(uLightDir)), 0.0);
    float diff1 = max(dot(N, normalize(uLight1Dir)), 0.0);
    vec4 diffuseColor = texture2D(uDiffuseMap, vTexCoord);
    vec3 lighting = diffuseColor.rgb * (uLight0Color * diff0 + uLight1Color * diff1);
    gl_FragColor = vec4(lighting, diffuseColor.a);
}";

        private const string fragmentTest = @"#version 120
varying vec3 vNormal;
uniform vec3 uLightDir;
uniform vec3 uLight1Dir;
uniform vec3 uLight0Color;
uniform vec3 uLight1Color;
void main() {
    vec3 normalMap = texture2D(uNormalMap, vTexCoord).rgb * 2.0 - 1.0;
    vec3 N = normalize(vNormal);
    vec3 T = normalize(vTangent);
    vec3 B = normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);
    vec3 worldNormal = normalize(TBN * normalMap);
    vec3 color = (worldNormal * 0.5) + 0.5;
    gl_FragColor = vec4(color, 1.0);
}";

        public ShaderProgram ShaderProgram = new();
        public bool Initialized { get; private set; }

        public void InitShaders(OpenGL gl)
        {
            ShaderProgram.Create(gl, vertexNormal, fragmentNormal, null);
            Initialized = true;
        }
    }
}
