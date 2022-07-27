using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DragonJson
{
    [StructLayout(LayoutKind.Explicit)]
    public class JsonData : IEquatable<JsonData>//, IEnumerable<KeyValuePair<string, JsonData>>
    {
        #region Fields
        [FieldOffset(16)]
        private List<JsonData> _array;
        [FieldOffset(8)]
        private bool _boolean;
        [FieldOffset(8)]
        public double _double;
        [FieldOffset(8)]
        private int _int;
        [FieldOffset(8)]
        private long _long;
        [FieldOffset(8)]
        private int _size;
        [FieldOffset(16)]
        private Dictionary<string, JsonData> _object;
        [FieldOffset(16)]
        public string _string;
        [FieldOffset(0)]
        private JsonValueType type;
        #endregion
        public object ValueOobject
        {
            get
            {
                switch (type)
                {
                    case JsonValueType.None:
                        return null;
                    case JsonValueType.Object:
                        return _object;
                    case JsonValueType.Array:
                        return _array;
                    case JsonValueType.String:
                        return _string;
                    case JsonValueType.Long:
                        return _long;
                    case JsonValueType.Double:
                        return _double;
                    case JsonValueType.Boolean:
                        return _boolean;
                }
                return null;
            }
        }

        public JsonValueType GetJsonType()
        {
            return type;
        }
        public JsonValueType Type
        {
            get
            {
                return type;
            }
        }
        public bool Bool
        {
            get
            {
                return _boolean;
            }
        }
        public double Double
        {
            get
            {
                return _double;
            }
        }

        public int Int
        {
            get
            {
                return _int;
            }
        }

        public long Long
        {
            get
            {
                return _long;
            }
        }

        public long Size
        {
            get
            {
                return _size;
            }
        }

        public Dictionary<string, JsonData> Object
        {
            get
            {
                return _object;
            }
        }
        public List<JsonData> Arrray
        {
            get
            {
                return _array;
            }
        }
        public string String
        {
            get
            {
                return _string;
            }
        }


        public int ArrayCount
        {
            get { return _array.Count; }
        }

        public int Count
        {
            get { return EnsureCollection().Count; }
        }

        public bool IsArray
        {
            get { return type == JsonValueType.Array; }
        }

        public bool IsBoolean
        {
            get { return type == JsonValueType.Boolean; }
        }

        public bool IsDouble
        {
            get { return type == JsonValueType.Double; }
        }

        public bool IsInt
        {
            get { return type == JsonValueType.Long && _long < int.MaxValue && int.MinValue < _long; }
        }

        public bool IsLong
        {
            get { return type == JsonValueType.Long; }
        }

        public bool IsObject
        {
            get { return type == JsonValueType.Object; }
        }

        public bool IsString
        {
            get { return type == JsonValueType.String; }
        }

        public ICollection<string> Keys
        {
            get { EnsureDictionary(); return _object.Keys; }
        }

        public Boolean ContainsKey(String key)
        {
            EnsureDictionary();
            return this._object.ContainsKey(key);
        }

        public void Add(JsonData jsonData)
        {
            if (type != JsonValueType.Array)
                throw new ArgumentException(
                    "此数据不是数组，而是" + type);
            _array.Add(jsonData);
        }

        public JsonData this[string prop_name]
        {
            get
            {
                if (type != JsonValueType.Object)
                    throw new ArgumentException(
                        "此数据不是对象，而是" + type);
                return _object[prop_name];
            }

            set
            {
                if (type != JsonValueType.Object)
                    throw new ArgumentException(
                        "此数据不是对象，而是" + type);
                _object[prop_name] = value;
            }
        }

        public JsonData this[int index]
        {
            get
            {
                if (type == JsonValueType.Array)
                    return _array[index];

                if (type == JsonValueType.Object)
                    return _object[index.ToString()];

                throw new ArgumentException(
                    "此数据不是数组也不是字符串，而是" + type);
            }

            set
            {
                if (type == JsonValueType.Array)
                    _array[index] = value;

                if (type == JsonValueType.Object)
                    _object[index.ToString()] = value;
                throw new ArgumentException(
                    "此数据不是数组也不是字符串，而是" + type);
            }
        }

        public JsonData()
        {
            type = JsonValueType.None;
        }

        public JsonData(bool boolean)
        {
            type = JsonValueType.Boolean;
            _boolean = boolean;
        }

        public JsonData(double number)
        {
            type = JsonValueType.Double;
            _double = number;
        }

        public JsonData(int number)
        {
            type = JsonValueType.Long;
            _int = number;
        }

        public JsonData(long number)
        {
            type = JsonValueType.Long;
            _long = number;
        }

        public JsonData(string str)
        {
            type = JsonValueType.String;
            _string = str;
        }

        public JsonData(Dictionary<string, JsonData> argObject, int size = 0)
        {
            type = JsonValueType.Object;
            this._object = argObject;
            this._size = size;
        }

        public JsonData(List<JsonData> argArray, int size = 0)
        {
            type = JsonValueType.Array;
            this._array = argArray;
            this._size = size;
        }

        public JsonData(object obj)
        {
            if (obj is Boolean)
            {
                type = JsonValueType.Boolean;
                _boolean = (bool)obj;
                return;
            }

            if (obj is Double)
            {
                type = JsonValueType.Double;
                _double = (double)obj;
                return;
            }

            if (obj is Int32)
            {
                type = JsonValueType.Long;
                _int = (int)obj;
                return;
            }

            if (obj is Int64)
            {
                type = JsonValueType.Long;
                _long = (long)obj;
                return;
            }

            if (obj is String)
            {
                type = JsonValueType.String;
                _string = (string)obj;
                return;
            }

            throw new ArgumentException(
                "Unable to wrap the given object with JsonData");
        }

        public static implicit operator JsonData(Boolean data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(Double data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(Int32 data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(Int64 data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(String data)
        {
            return new JsonData(data);
        }

        public static explicit operator Boolean(JsonData data)
        {
            if (data.type != JsonValueType.Boolean)
                throw new InvalidCastException("类型不匹配");

            return data._boolean;
        }

        public static explicit operator Double(JsonData data)
        {
            if (data.type != JsonValueType.Double)
                throw new InvalidCastException("类型不匹配");

            return data._double;
        }

        public static explicit operator Int32(JsonData data)
        {
            if (data.type != JsonValueType.Long && data.type != JsonValueType.Long)
            {
                throw new InvalidCastException("类型不匹配");
            }

            // cast may truncate data... but that's up to the user to consider
            return data.type == JsonValueType.Long ? data._int : (int)data._long;
        }

        public static explicit operator Int64(JsonData data)
        {
            if (data.type != JsonValueType.Long && data.type != JsonValueType.Long)
            {
                throw new InvalidCastException("类型不匹配");
            }

            return data.type == JsonValueType.Long ? data._long : data._int;
        }

        public static explicit operator String(JsonData data)
        {
            if (data.type != JsonValueType.String)
                throw new InvalidCastException("类型不匹配");

            return data._string;
        }

        #region Private Methods
        private ICollection EnsureCollection()
        {
            if (type == JsonValueType.Array)
                return (ICollection)_array;

            if (type == JsonValueType.Object)
                return (ICollection)_object;

            throw new InvalidOperationException(
                "The JsonData instance has to be initialized first");
        }
        private IDictionary EnsureDictionary()
        {
            if (type == JsonValueType.Object)
                return (IDictionary)_object;

            throw new InvalidOperationException(
                "数据不是对象");
        }

        private IList EnsureList()
        {
            if (type == JsonValueType.Array)
                return (IList)_array;

            if (type != JsonValueType.None)
                throw new InvalidOperationException(
                    "Instance of JsonData is not a list");

            type = JsonValueType.Array;
            _array = new List<JsonData>();

            return (IList)_array;
        }

        private JsonData ToJsonData(object obj)
        {
            if (obj == null)
                return null;

            if (obj is JsonData)
                return (JsonData)obj;

            return new JsonData(obj);
        }

        #endregion

        public void Clear()
        {
            if (IsObject)
            {
                ((IDictionary)this).Clear();
                return;
            }

            if (IsArray)
            {
                ((IList)this).Clear();
                return;
            }
        }

        public bool Equals(JsonData x)
        {
            if (x == null)
                return false;

            if (x.type != this.type)
            {
                // further check to see if this is a long to int comparison
                if ((x.type != JsonValueType.Long && x.type != JsonValueType.Long)
                    || (this.type != JsonValueType.Long && this.type != JsonValueType.Long))
                {
                    return false;
                }
            }

            switch (this.type)
            {
                case JsonValueType.None:
                    return true;

                case JsonValueType.Object:
                    return this._object.Equals(x._object);

                case JsonValueType.Array:
                    return this._array.Equals(x._array);

                case JsonValueType.String:
                    return this._string.Equals(x._string);

                case JsonValueType.Long:
                    {
                        if (x.IsInt)
                        {
                            return x._int.Equals(this._int);
                        }
                        if (x.IsLong)
                        {
                            return this._long.Equals(x._long);
                        }
                        return this._int.Equals(x._int);
                    }

                case JsonValueType.Double:
                    return this._double.Equals(x._double);

                case JsonValueType.Boolean:
                    return this._boolean.Equals(x._boolean);
            }

            return false;
        }

        public void SetJsonType(JsonValueType type)
        {
            if (this.type == type)
                return;

            switch (type)
            {
                case JsonValueType.None:
                    break;

                case JsonValueType.Object:
                    _object = new Dictionary<string, JsonData>();
                    break;

                case JsonValueType.Array:
                    _array = new List<JsonData>();
                    break;

                case JsonValueType.String:
                    _string = default(String);
                    break;

                case JsonValueType.Long:
                    _long = default(Int64);
                    break;

                case JsonValueType.Double:
                    _double = default(Double);
                    break;

                case JsonValueType.Boolean:
                    _boolean = default(Boolean);
                    break;
            }

            this.type = type;
        }

        public string ToJson()
        {
            return ToJson(0);
        }

        public override string ToString()
        {
            switch (type)
            {
                case JsonValueType.None:
                    return "null";
                case JsonValueType.Object:
                    {
                        StringBuilder str = new StringBuilder();
                        int i = 0;
                        str.Append("{");
                        foreach (var item in _object)
                        {
                            i++;
                            str.Append("\"" + item.Key + "\"" + " : " + item.Value.ToString());
                            if (i < _object.Count)
                            {
                                str.Append(", ");
                            }
                        }
                        str.Append("}");
                        return str.ToString();
                    }
                case JsonValueType.Array:
                    {
                        StringBuilder str = new StringBuilder();
                        str.Append("[");
                        int count = 0;
                        foreach (var item in _array)
                        {
                            count++;
                            str.Append(item.ToString());
                            if (count < _array.Count)
                            {
                                str.AppendLine(", ");
                            }
                        }
                        str.Append("]");
                        return str.ToString();
                    }
                case JsonValueType.String:
                    return "\"" + _string + "\"";
                case JsonValueType.Long:
                    return _long.ToString();
                case JsonValueType.Double:
                    return _double.ToString();
                case JsonValueType.Boolean:
                    return _boolean.ToString();
                default:
                    break;
            }
            return base.ToString();
        }
        public string ToJson(int size)
        {
            switch (type)
            {
                case JsonValueType.None:
                    return "null";
                case JsonValueType.Object:
                    {
                        StringBuilder str = new StringBuilder();
                        StringBuilder strNum = new StringBuilder();
                        for (int j = 0; j < size; j++)
                        {
                            strNum.Append("\t");
                        }
                        int i = 0;
                        str.AppendLine();
                        str.Append(strNum);
                        str.AppendLine("{");
                        foreach (var item in _object)
                        {
                            i++;
                            str.Append(strNum);
                            str.Append("\t");
                            str.Append("\"" + item.Key + "\"" + " : " + item.Value.ToJson(size + 1));
                            if (i < _object.Count)
                            {
                                str.AppendLine(", ");
                            }
                        }
                        str.AppendLine();
                        str.Append(strNum);
                        str.Append("}");
                        return str.ToString();
                    }
                case JsonValueType.Array:
                    {
                        StringBuilder str = new StringBuilder();
                        StringBuilder strNum = new StringBuilder();
                        for (int j = 0; j < size; j++)
                        {
                            strNum.Append("\t");
                        }
                        int i = 0;
                        str.AppendLine();
                        str.Append(strNum);
                        str.AppendLine("[");
                        int count = 0;
                        foreach (var item in _array)
                        {
                            count++;
                            str.Append(strNum);
                            str.Append("\t");
                            str.Append(item.ToJson(size + 1));
                            if (count < _array.Count)
                            {
                                str.AppendLine(", ");
                            }
                        }
                        str.AppendLine();
                        str.Append(strNum);
                        str.Append("]");
                        return str.ToString();
                    }
                case JsonValueType.String:
                    return "\"" + _string + "\"";
                case JsonValueType.Long:
                    return _int.ToString();
                case JsonValueType.Double:
                    return _double.ToString();
                case JsonValueType.Boolean:
                    return _boolean.ToString();
                default:
                    break;
            }
            return base.ToString();
        }


    }


}