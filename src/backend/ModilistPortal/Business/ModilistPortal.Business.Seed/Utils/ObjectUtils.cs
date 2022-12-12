using System.Reflection;

namespace ModilistPortal.Business.Seed.Utils
{
    internal static class ObjectUtils
    {
        public static void SetPrivateProperty(this object @object, string name, object value)
        {
            var objectType = @object.GetType();
            var propertyInfo = objectType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"Property not found with name {name} for {objectType.Name}");
            }

            propertyInfo.SetValue(@object, value);
        }

        public static void SetBasePrivateProperty(this object @object, string name, object value)
        {
            var objectType = @object.GetType().BaseType;
            var propertyInfo = objectType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"Property not found with name {name} for {objectType.Name}");
            }

            propertyInfo.SetValue(@object, value);
        }
    }
}
