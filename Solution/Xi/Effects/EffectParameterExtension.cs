using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// An extension class for EffectParameter.
    /// </summary>
    public static class EffectParameterExtension
    {
        public static bool TryGetValueBoolean(this EffectParameter parameter) { return parameter == null ? false : parameter.GetValueBoolean(); }
        public static bool[] TryGetValueBooleanArray(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueBooleanArray(count); }
        public static int TryGetValueInt32(this EffectParameter parameter) { return parameter == null ? 0 : parameter.GetValueInt32(); }
        public static int[] TryGetValueInt32Array(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueInt32Array(count); }
        public static Matrix TryGetValueMatrix(this EffectParameter parameter) { return parameter == null ? new Matrix() : parameter.GetValueMatrix(); }
        public static Matrix[] TryGetValueMatrixArray(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueMatrixArray(count); }
        public static Matrix TryGetValueMatrixTranspose(this EffectParameter parameter) { return parameter == null ? new Matrix() : parameter.GetValueMatrix(); }
        public static Matrix[] TryGetValueMatrixTransposeArray(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueMatrixArray(count); }
        public static Quaternion TryGetValueQuaternion(this EffectParameter parameter) { return parameter == null ? new Quaternion() : parameter.GetValueQuaternion(); }
        public static Quaternion[] TryGetValueQuaternionArray(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueQuaternionArray(count); }
        public static float TryGetValueSingle(this EffectParameter parameter) { return parameter == null ? 0 : parameter.GetValueSingle(); }
        public static float[] TryGetValueSingleArray(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueSingleArray(count); }
        public static string TryGetValueString(this EffectParameter parameter) { return parameter == null ? null : parameter.GetValueString(); }
        public static Texture2D TryGetValueTexture2D(this EffectParameter parameter) { return parameter == null ? null : parameter.GetValueTexture2D(); }
        public static Texture3D TryGetValueTexture3D(this EffectParameter parameter) { return parameter == null ? null : parameter.GetValueTexture3D(); }
        public static TextureCube TryGetValueTextureCube(this EffectParameter parameter) { return parameter == null ? null : parameter.GetValueTextureCube(); }
        public static Vector2 TryGetValueVector2(this EffectParameter parameter) { return parameter == null ? new Vector2() : parameter.GetValueVector2(); }
        public static Vector2[] TryGetValueVector2Array(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueVector2Array(count); }
        public static Vector3 TryGetValueVector3(this EffectParameter parameter) { return parameter == null ? new Vector3() : parameter.GetValueVector3(); }
        public static Vector3[] TryGetValueVector3Array(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueVector3Array(count); }
        public static Vector4 TryGetValueVector4(this EffectParameter parameter) { return parameter == null ? new Vector4() : parameter.GetValueVector4(); }
        public static Vector4[] TryGetValueVector4Array(this EffectParameter parameter, int count) { return parameter == null ? null : parameter.GetValueVector4Array(count); }
        public static void TrySetArrayRange(this EffectParameter parameter, int start, int end) { if (parameter != null) parameter.SetArrayRange(start, end); }
        public static void TrySetValue(this EffectParameter parameter, bool value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, bool[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, float value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, float[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, int value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, int[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Matrix value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Matrix[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Quaternion value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Quaternion[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, string value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Texture value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Vector2 value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Vector2[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Vector3 value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Vector3[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Vector4 value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValue(this EffectParameter parameter, Vector4[] value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValueTranspose(this EffectParameter parameter, Matrix value) { if (parameter != null) parameter.SetValue(value); }
        public static void TrySetValueTranspose(this EffectParameter parameter, Matrix[] value) { if (parameter != null) parameter.SetValue(value); }
    }
}
