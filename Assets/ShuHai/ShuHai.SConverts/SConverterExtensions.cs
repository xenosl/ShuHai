using System;

namespace ShuHai.SConverts
{
    public static class SConverterExtensions
    {
        public static bool CanConvert(this ISConverter self, object value)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            
            var tt = self.TargetType;
            return tt.IsValueType
                ? CanConvert(tt, value.GetType())
                : value == null || CanConvert(tt, value.GetType());
        }

        public static bool CanConvert(this ISConverter self, Type valueType)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return CanConvert(self.TargetType, valueType);
        }

        private static bool CanConvert(Type targetType, Type valueType)
        {
            return targetType.IsAssignableFrom(valueType);
        }
    }
}