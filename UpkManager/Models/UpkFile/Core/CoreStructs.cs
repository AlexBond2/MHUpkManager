using System;
using System.Numerics;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    public interface IAtomicStruct
    {
        string Format { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AtomicStructAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }

    [AtomicStruct("Vector")]
    public class Vector : IAtomicStruct
    {
        [StructField]
        public float X { get; set; }

        [StructField]
        public float Y { get; set; }

        [StructField]
        public float Z { get; set; }

        public string Format => $"[{X:F4}; {Y:F4}; {Z:F4}]";

        public Vector() { }

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);

        public static Vector ReadData(UBuffer buffer)
        {
            var vector = new Vector
            {
                X = buffer.Reader.ReadSingle(),
                Y = buffer.Reader.ReadSingle(),
                Z = buffer.Reader.ReadSingle()
            };
            return vector;
        }
    }

    public class PackedNormal : IAtomicStruct
    {
        [StructField]
        public uint Packed { get; set; }

        public byte X => (byte)(Packed & 0xFF);
        public byte Y => (byte)((Packed >> 8) & 0xFF);
        public byte Z => (byte)((Packed >> 16) & 0xFF);
        public byte W => (byte)((Packed >> 24) & 0xFF);

        private const float Scale = 1.0f / 127.5f; 
        private const float Offset = -1.0f;

        public Vector ToVector()
        {            
            return new Vector(
                X * Scale + Offset,
                Y * Scale + Offset,
                Z * Scale + Offset
            );
        }

        public string Format => ToVector().Format;

        public static PackedNormal ReadData(UBuffer buffer)
        {
            PackedNormal normal = new()
            {
                Packed = buffer.Reader.ReadUInt32(),
            };

            return normal;
        }
    }

    public class PackedPosition : IAtomicStruct
    {
        [StructField]
        public uint Packed { get; set; }

        public int X => (int)(Packed << 0) << (32 - 11) >> (32 - 11);
        public int Y => (int)(Packed << 11) >> (32 - 11);
        public int Z => (int)(Packed << 22) >> (32 - 10);

        public Vector ToVector()
        {
            return new Vector(
                X / 1023.0f,
                Y / 1023.0f,
                Z / 511.0f
            );
        }

        public string Format => ToVector().Format;

        public static PackedPosition ReadData(UBuffer buffer)
        {
            PackedPosition normal = new()
            {
                Packed = buffer.Reader.ReadUInt32(),
            };

            return normal;
        }
    }

    public class Vector2D : IAtomicStruct
    {
        [StructField]
        public float X { get; set; }

        [StructField]
        public float Y { get; set; }

        public string Format => $"[{X:F4};{Y:F4}]";

        public Vector2D() { }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2D ReadData(UBuffer buffer)
        {
            var vector2D = new Vector2D
            {
                X = buffer.Reader.ReadSingle(),
                Y = buffer.Reader.ReadSingle()
            };
            return vector2D;
        }

        public Vector2 ToVector2() => new Vector2(X, Y);
    }

    public class Float16 : IAtomicStruct
    {
        [StructField]
        public ushort Encoded { get; set; }

        public float ToFloat()
        {
            int sign = (Encoded >> 15) & 0x1;
            int exponent = (Encoded >> 10) & 0x1F;
            int mantissa = Encoded & 0x3FF;

            uint result;

            if (exponent == 0)
            {
                result = (uint)(sign << 31);
            }
            else if (exponent == 0x1F)
            {
                result = ((uint)sign << 31) | ((uint)142 << 23) | 0x7FFFFF;
            }
            else
            {
                int newExp = exponent - 15 + 127; 
                int newMantissa = mantissa << 13;

                result = ((uint)sign << 31) | ((uint)newExp << 23) | (uint)newMantissa;
            }

            return BitConverter.Int32BitsToSingle((int)result);
        }

        public string Format => $"{ToFloat():F4}";

        public static Float16 ReadData(UBuffer buffer)
        {
            return new Float16
            {
                Encoded = buffer.Reader.ReadUInt16()
            };
        }
    }

    public class Vector2DHalf : IAtomicStruct
    {
        [StructField]
        public Float16 X { get; set; }

        [StructField]
        public Float16 Y { get; set; }

        public string Format => $"[{X.ToFloat():F4};{Y.ToFloat():F4}]";

        public static Vector2DHalf ReadData(UBuffer buffer)
        {
            var vector2D = new Vector2DHalf
            {
                X = Float16.ReadData(buffer),
                Y = Float16.ReadData(buffer)
            };
            return vector2D;
        }
    }

    [AtomicStruct("Quat")]
    public class Quat : IAtomicStruct
    {
        [StructField]
        public float X { get; set; }

        [StructField]
        public float Y { get; set; }

        [StructField]
        public float Z { get; set; }

        [StructField]
        public float W { get; set; }

        public string Format => $"[{X:F4}; {Y:F4}; {Z:F4}; {W:F4}]";

        public Quat() { }
        public Quat(float x, float y, float z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Quat ReadData(UBuffer buffer)
        {
            var quad = new Quat
            {
                X = buffer.Reader.ReadSingle(),
                Y = buffer.Reader.ReadSingle(),
                Z = buffer.Reader.ReadSingle(),
                W = buffer.Reader.ReadSingle()
            };
            return quad;
        }
    }

    [AtomicStruct("Guid")]
    public class Guid : IAtomicStruct
    {
        [StructField]
        public int A { get; set; }

        [StructField]
        public int B { get; set; }

        [StructField]
        public int C { get; set; }

        [StructField]
        public int D { get; set; }

        public System.Guid ToSystemGuid()
        {
            byte[] bytes = new byte[16];
            Buffer.BlockCopy(new[] { A, B, C, D }, 0, bytes, 0, 16);
            return new System.Guid(bytes);
        }

        public string Format => ToSystemGuid().ToString();
        public override string ToString() => ToSystemGuid().ToString();

        public static Guid ReadData(UBuffer buffer)
        {
            var guid = new Guid
            {
                A = buffer.Reader.ReadInt32(),
                B = buffer.Reader.ReadInt32(),
                C = buffer.Reader.ReadInt32(),
                D = buffer.Reader.ReadInt32()
            };
            return guid;
        }
    }

    [AtomicStruct("Rotator")]
    public class Rotator : IAtomicStruct
    {
        [StructField]
        public int Pitch { get; set; }

        [StructField]
        public int Yaw { get; set; }

        [StructField]
        public int Roll { get; set; }

        public string Format  => $"[{GetAngle(Pitch):F4}; {GetAngle(Yaw):F4}; {GetAngle(Roll):F4}]";

        public static float GetAngle(int value)
        {
            return value / 32768.0f * 180.0f;
        }

        public static Rotator ReadData(UBuffer buffer)
        {
            var rotator = new Rotator
            {
                Pitch = buffer.Reader.ReadInt32(),
                Yaw = buffer.Reader.ReadInt32(),
                Roll = buffer.Reader.ReadInt32()
            };
            return rotator;
        }
    }

    [AtomicStruct("Box")]
    public class Box : IAtomicStruct
    {
        [StructField]
        public Vector Min { get; set; }

        [StructField]
        public Vector Max { get; set; }

        [StructField]
        public bool IsValid { get; set; }

        public string Format => "";

        public static Box ReadData(UBuffer buffer)
        {
            var box = new Box
            {
                Min = Vector.ReadData(buffer),
                Max = Vector.ReadData(buffer),
                IsValid = buffer.ReadAtomicBool()
            };
            return box;
        }
    }

    [AtomicStruct("Plane")]
    public class Plane : IAtomicStruct
    {
        [StructField]
        public float W { get; set; }

        [StructField]
        public float X { get; set; }

        [StructField]
        public float Y { get; set; }

        [StructField]
        public float Z { get; set; }

        public string Format => $"[{X:F4}; {Y:F4}; {Z:F4}; {W:F4}]";

        public static Plane ReadData(UBuffer buffer)
        {
            var quad = new Plane
            {
                W = buffer.Reader.ReadSingle(),
                X = buffer.Reader.ReadSingle(),
                Y = buffer.Reader.ReadSingle(),
                Z = buffer.Reader.ReadSingle()
            };
            return quad;
        }
    }

    [AtomicStruct("Matrix")]
    public class Matrix : IAtomicStruct
    {
        [StructField]
        public Plane XPlane { get; set; }

        [StructField]
        public Plane YPlane { get; set; }

        [StructField]
        public Plane ZPlane { get; set; }

        [StructField]
        public Plane WPlane { get; set; }

        public string Format => "";

        public static Matrix ReadData(UBuffer buffer)
        {
            var matrix = new Matrix
            {
                XPlane = Plane.ReadData(buffer),
                YPlane = Plane.ReadData(buffer),
                ZPlane = Plane.ReadData(buffer),
                WPlane = Plane.ReadData(buffer),
            };
            return matrix;
        }
    }

    public class BoxSphereBounds : IAtomicStruct
    {
        [StructField]
        public Vector Origin { get; set; }

        [StructField]
        public Vector BoxExtent { get; set; }

        [StructField]
        public float SphereRadius { get; set; }

        public string Format => "";

        public static BoxSphereBounds ReadData(UBuffer buffer)
        {
            var bounds = new BoxSphereBounds
            {
                Origin = Vector.ReadData(buffer),
                BoxExtent = Vector.ReadData(buffer),
                SphereRadius = buffer.Reader.ReadSingle()
            };
            return bounds;
        }
    }

    [AtomicStruct("Color")]
    public class Color : IAtomicStruct
    {
        [StructField]
        public byte B { get; set; }

        [StructField]
        public byte G { get; set; }

        [StructField]
        public byte R { get; set; }

        [StructField]
        public byte A { get; set; }

        public string Format => $"[{R};{G};{B};{A}]";

        public static Color ReadData(UBuffer buffer)
        {
            var color = new Color
            {
                B = buffer.Reader.ReadByte(),
                G = buffer.Reader.ReadByte(),
                R = buffer.Reader.ReadByte(),
                A = buffer.Reader.ReadByte()
            };
            return color;
        }
    }

    [AtomicStruct("LinearColor")]
    public class LinearColor : IAtomicStruct
    {

        [StructField]
        public float R { get; set; }

        [StructField]
        public float G { get; set; }

        [StructField]
        public float B { get; set; }

        [StructField]
        public float A { get; set; }

        public string Format => $"[{R:F4}; {G:F4}; {B:F4}; {A:F4}]";

        public static LinearColor ReadData(UBuffer buffer)
        {
            var color = new LinearColor
            {
                R = buffer.Reader.ReadSingle(),
                G = buffer.Reader.ReadSingle(),
                B = buffer.Reader.ReadSingle(),
                A = buffer.Reader.ReadSingle()
            };
            return color;
        }
    }

    [UnrealStruct("RawDistribution")]
    public class RawDistribution
    {
        [StructField] 
        public byte Type { get; set; }

        [StructField]
        public byte Op { get; set; }

        [StructField]
        public byte LookupTableNumElements { get; set; }

        [StructField]
        public byte LookupTableChunkSize { get; set; }

        [StructField]
        public UArray<float> LookupTable { get; set; }

        [StructField]
        public float LookupTableTimeScale { get; set; }

        [StructField]
        public float LookupTableStartTime { get; set; }
    }

    [UnrealStruct("RawDistributionFloat")]
    public class RawDistributionFloat : RawDistribution
    {
        [StructField]
        public FObject Distribution { get; set; } // DistributionFloat
    }

    [UnrealStruct("RawDistributionVector")]
    public class RawDistributionVector : RawDistribution
    {
        [StructField]
        public FObject Distribution { get; set; } // DistributionVector
    }
}
