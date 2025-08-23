using System;
using System.Linq;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Material
{
    public class FMaterialUniformExpressionRef
    {
        public FName ExpressionType { get; set; }
        public FMaterialUniformExpression Expression { get; set; }

        public static FMaterialUniformExpressionRef ReadData(UBuffer buffer)
        {
            var expressionType = buffer.ReadName();

            return new FMaterialUniformExpressionRef
            {
                ExpressionType = expressionType,
                Expression = FMaterialUniformExpression.ReadData(buffer, expressionType)
            };
        }
    }

    public abstract class FMaterialUniformExpression
    {
        public static FMaterialUniformExpression ReadData(UBuffer buffer, FName expressionType)
        {
            var className = expressionType.Name;
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == className && t.IsSubclassOf(typeof(FMaterialUniformExpression)));

            if (type == null)
                throw new NotImplementedException($"Expression class not found: {className}");

            var expression = (FMaterialUniformExpression)Activator.CreateInstance(type);
            expression.ReadData(buffer);

            return expression;
        }

        protected abstract void ReadData(UBuffer buffer);
    }

    public class FMaterialUniformExpressionAppendVector : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef A { get; set; }
        public FMaterialUniformExpressionRef B { get; set; }
        public uint NumComponentsA { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            A = FMaterialUniformExpressionRef.ReadData(buffer);
            B = FMaterialUniformExpressionRef.ReadData(buffer);
            NumComponentsA = buffer.ReadUInt32();
        }
    }

    public class FMaterialUniformExpressionPeriodic : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }
    public class FMaterialUniformExpressionLength : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionSquareRoot : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionMin : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef A { get; set; }
        public FMaterialUniformExpressionRef B { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            A = FMaterialUniformExpressionRef.ReadData(buffer);
            B = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionMax : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef A { get; set; }
        public FMaterialUniformExpressionRef B { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            A = FMaterialUniformExpressionRef.ReadData(buffer);
            B = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionSine : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }
        public bool bIsCosine { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
            bIsCosine = buffer.ReadBool();
        }
    }
    public class FMaterialUniformExpressionClamp : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef Input { get; set; }
        public FMaterialUniformExpressionRef Min { get; set; }
        public FMaterialUniformExpressionRef Max { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            Input = FMaterialUniformExpressionRef.ReadData(buffer);
            Min = FMaterialUniformExpressionRef.ReadData(buffer);
            Max = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionFloor : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionCeil : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionFrac : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionFmod : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef A { get; set; }
        public FMaterialUniformExpressionRef B { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            A = FMaterialUniformExpressionRef.ReadData(buffer);
            B = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionAbs : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef X { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            X = FMaterialUniformExpressionRef.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionFoldedMath : FMaterialUniformExpression
    {
        public FMaterialUniformExpressionRef A { get; set; }
        public FMaterialUniformExpressionRef B { get; set; }
        public byte Op { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            A = FMaterialUniformExpressionRef.ReadData(buffer);
            B = FMaterialUniformExpressionRef.ReadData(buffer);
            Op = buffer.ReadByte();
        }
    }

    public class FMaterialUniformExpressionVectorParameter : FMaterialUniformExpression
    {
        public FName ParameterName { get; set; }
        public FLinearColor DefaultValue { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            ParameterName = buffer.ReadName();
            DefaultValue = FLinearColor.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionTime : FMaterialUniformExpression
    {
        protected override void ReadData(UBuffer buffer)
        {
        }
    }

    public class FMaterialUniformExpressionRealTime : FMaterialUniformExpression
    {
        protected override void ReadData(UBuffer buffer)
        {
        }
    }

    public class FMaterialUniformExpressionConstant : FMaterialUniformExpression
    {
        public FLinearColor Value { get; set; }
        public byte ValueType { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            Value = FLinearColor.ReadData(buffer);
            ValueType = buffer.ReadByte();
        }
    }

    public class FMaterialUniformExpressionScalarParameter : FMaterialUniformExpression
    {
        public FName ParameterName { get; set; }
        public float DefaultValue { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            ParameterName = buffer.ReadName();
            DefaultValue = buffer.ReadFloat();
        }
    }

    public class FMaterialUniformExpressionTexture : FMaterialUniformExpression
    {
        public int TextureIndex { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            TextureIndex = buffer.ReadInt32();
        }
    }

    public class FMaterialUniformExpressionTextureParameter : FMaterialUniformExpressionTexture
    {
        public FName ParameterName { get; set; }

        protected override void ReadData(UBuffer buffer)
        {
            ParameterName = buffer.ReadName();
            base.ReadData(buffer);
        }
    }

    public class FMaterialUniformExpressionFlipBookTextureParameter : FMaterialUniformExpressionTexture
    {
        protected override void ReadData(UBuffer buffer)
        {
            base.ReadData(buffer);
        }
    }

}
