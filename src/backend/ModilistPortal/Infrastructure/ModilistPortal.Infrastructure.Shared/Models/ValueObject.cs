using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Models
{
    public interface IValueObject { }

    public abstract class ValueObject<T> : IEquatable<T>, IValueObject
        where T : ValueObject<T>
    {
        public static bool operator ==(ValueObject<T> x, ValueObject<T> y)
        {
            if ((object)x == null)
            {
                return (object)y == null;
            }

            return x.Equals(y);
        }

        public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as T;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            var fields = GetFields();
            int startValue = 17;
            int multiplier = 59;
            int hashCode = startValue;

            foreach (var field in fields)
            {
                object value = field.GetValue(this);
                if (value != null)
                {
                    hashCode = (hashCode * multiplier) + value.GetHashCode();
                }
            }

            return hashCode;
        }

        public virtual bool Equals(T other)
        {
            var type = GetType();
            var otherType = other?.GetType();
            if (otherType == null)
            {
                return false;
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var property in properties)
            {
                object value1 = property.GetValue(other);
                object value2 = property.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                    {
                        return false;
                    }
                }
                else if (!value1.Equals(value2))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            var type = GetType();
            var fields = new List<FieldInfo>();
            while (type != typeof(object))
            {
                fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                type = type.BaseType;
            }

            return fields;
        }
    }
}
