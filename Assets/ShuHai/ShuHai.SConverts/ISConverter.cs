using System;

namespace ShuHai.SConverts
{
    public interface ISConverter
    {
        Type TargetType { get; }
        
        string ToString(object value);
        object ToValue(string str);
    }

    public interface ISConverter<T> : ISConverter
    {
        string ToString(T value);
        new T ToValue(string str);
    }
}