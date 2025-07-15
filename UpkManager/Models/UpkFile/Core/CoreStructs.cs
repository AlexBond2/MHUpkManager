using System;

namespace UpkManager.Models.UpkFile.Core
{
    public interface IAtomicStruct
    {
        string Format { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class StructFieldAttribute : Attribute
    {
    }

    public class Vector : IAtomicStruct
    {
        [StructField]
        public float X { get; set; }

        [StructField]
        public float Y { get; set; }

        [StructField]
        public float Z { get; set; }

        public string Format => $"[{X:F4}; {Y:F4}; {Z:F4}]";
    }

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

        public string Format => $"{A:X}-{B:X}-{C:X}-{D:X}";
    }

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
    }

    public class Box : IAtomicStruct
    {
        [StructField]
        public Vector Min { get; set; }

        [StructField]
        public Vector Max { get; set; }

        [StructField]
        public bool IsValid { get; set; }

        public string Format => ""; 
    }

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
    }

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
    }

}
