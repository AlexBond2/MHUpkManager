using System.IO;
using System.Text;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Material
{
    public class DumpExpressionSet
    {
        public static void DumpUniformExpressionSetToFile(FUniformExpressionSet expressionSet, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("=== UNIFORM EXPRESSION SET DUMP ===");
                writer.WriteLine();

                // Pixel Expressions
                writer.WriteLine("=== PIXEL EXPRESSIONS ===");
                DumpFrequencyExpressionsToFile(writer, "PIXEL", expressionSet.PixelExpressions);

                // Vertex Expressions 
                writer.WriteLine();
                writer.WriteLine("=== VERTEX EXPRESSIONS ===");
                DumpFrequencyExpressionsToFile(writer, "VERTEX", expressionSet.VertexExpressions);

                // Cube Texture Expressions
                writer.WriteLine();
                writer.WriteLine("=== CUBE TEXTURE EXPRESSIONS ===");
                DumpExpressionArrayToFile(writer, "CubeTexture", expressionSet.UniformCubeTextureExpressions);

                writer.WriteLine();
                writer.WriteLine("=== END DUMP ===");
            }
        }

        private static void DumpFrequencyExpressionsToFile(StreamWriter writer, string frequency, FShaderFrequencyUniformExpressions expressions)
        {
            writer.WriteLine($"--- {frequency} VECTOR EXPRESSIONS (PConstFloat[]) ---");
            DumpExpressionArrayToFile(writer, $"{frequency}_Vector", expressions.UniformVectorExpressions);

            writer.WriteLine($"--- {frequency} SCALAR EXPRESSIONS ---");
            DumpExpressionArrayToFile(writer, $"{frequency}_Scalar", expressions.UniformScalarExpressions);

            writer.WriteLine($"--- {frequency} 2D TEXTURE EXPRESSIONS (PSampler[]) ---");
            DumpExpressionArrayToFile(writer, $"{frequency}_Texture2D", expressions.Uniform2DTextureExpressions);
        }

        private static void DumpExpressionArrayToFile(StreamWriter writer, string prefix, UArray<FMaterialUniformExpressionRef> expressions)
        {
            for (int i = 0; i < expressions.Count; i++)
            {
                var expr = expressions[i];
                writer.WriteLine($"{prefix}[{i}]: {expr.ExpressionType}");
                DumpExpressionRecursiveToFile(writer, expr.Expression, 1);
                writer.WriteLine();
            }
        }

        private static void DumpExpressionRecursiveToFile(StreamWriter writer, FMaterialUniformExpression expr, int indent)
        {
            string indentStr = new string(' ', indent * 2);

            switch (expr)
            {
                case FMaterialUniformExpressionVectorParameter vec:
                    writer.WriteLine($"{indentStr}VectorParam: {vec.ParameterName} = ({vec.DefaultValue.R:F3}, {vec.DefaultValue.G:F3}, {vec.DefaultValue.B:F3}, {vec.DefaultValue.A:F3})");
                    break;

                case FMaterialUniformExpressionScalarParameter scalar:
                    writer.WriteLine($"{indentStr}ScalarParam: {scalar.ParameterName} = {scalar.DefaultValue:F3}");
                    break;

                case FMaterialUniformExpressionTextureParameter tex:
                    writer.WriteLine($"{indentStr}TextureParam: {tex.ParameterName}, Index: {tex.TextureIndex}");
                    break;

                case FMaterialUniformExpressionConstant constant:
                    writer.WriteLine($"{indentStr}Constant: ({constant.Value.R:F3}, {constant.Value.G:F3}, {constant.Value.B:F3}, {constant.Value.A:F3}), Type: {constant.ValueType}");
                    break;

                case FMaterialUniformExpressionAppendVector append:
                    writer.WriteLine($"{indentStr}AppendVector: NumComponentsA={append.NumComponentsA}");
                    writer.WriteLine($"{indentStr}  A:");
                    DumpExpressionRecursiveToFile(writer, append.A.Expression, indent + 2);
                    writer.WriteLine($"{indentStr}  B:");
                    DumpExpressionRecursiveToFile(writer, append.B.Expression, indent + 2);
                    break;

                case FMaterialUniformExpressionFoldedMath math:
                    writer.WriteLine($"{indentStr}FoldedMath: Op={math.Op}");
                    writer.WriteLine($"{indentStr}  A:");
                    DumpExpressionRecursiveToFile(writer, math.A.Expression, indent + 2);
                    writer.WriteLine($"{indentStr}  B:");
                    DumpExpressionRecursiveToFile(writer, math.B.Expression, indent + 2);
                    break;

                default:
                    writer.WriteLine($"{indentStr}{expr.GetType().Name}");
                    break;
            }
        }
    }
}
