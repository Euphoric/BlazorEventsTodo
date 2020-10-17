using System;
using System.ComponentModel;
using System.Globalization;

namespace BlazorEventsTodo.Todo
{
    [TypeConverter(typeof(EntityIdConverter))]
    public record EntityId(string Value)
    {
        public override string ToString()
        {
            return Value;
        }
    }

    class EntityIdConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string val)
            {
                return new EntityId(val);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
