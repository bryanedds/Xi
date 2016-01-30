namespace System.ComponentModel
{
    /// <summary>
    /// A fake class definition of EditorAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class EditorAttribute : Attribute
    {
        public EditorAttribute() { }
        public EditorAttribute(string typeName, string baseTypeName) { }
        public EditorAttribute(string typeName, Type baseType) { }
        public EditorAttribute(Type type, Type baseType) { }
        public string EditorBaseTypeName { get { return string.Empty; } }
        public string EditorTypeName { get { return string.Empty; } }
    }
}
