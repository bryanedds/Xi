using System;

namespace Xi
{
    /// <summary>
    /// Marks a property as browsable only when physics is enabled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PhysicsBrowseAttribute : Attribute { }
}
