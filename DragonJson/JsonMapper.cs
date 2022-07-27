using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace DragonJson
{
    public delegate void ExporterFunc<T>(T obj, IJsonSerialize writer);
    public delegate TValue ImporterFunc<TValue>(TValue input);
    public delegate IJsonWrapper WrapperFactory();

    public interface IJsonWrapper : IList, ICollection, IEnumerable//, IDictionary//IOrderedDictionary,
    {
        bool IsString { get; }
        bool IsLong { get; }
        bool IsInt { get; }
        bool IsDouble { get; }
        bool IsBoolean { get; }
        bool IsArray { get; }
        bool IsObject { get; }

        bool GetBoolean();
        double GetDouble();
        int GetInt();
        JsonValueType GetJsonType();
        long GetLong();
        string GetString();
        void SetBoolean(bool val);
        void SetDouble(double val);
        void SetInt(int val);
        void SetLong(long val);
        void SetString(string val);
        string ToJson();
        void SetJsonType(JsonValueType type);

        //void ToJson(IJsonSerialize writer);
    }



    public interface IJsonSerialize
    {
        void Read(string json_text);
        void Read(TextReader reader);

        void Write(string json_text);
        void Write(TextReader reader);
    }

    public abstract class AJsonMapper
    {
        /// <summary>
        /// 扩展类型与其对应的解析方法
        /// </summary>
        //Dictionary<Type, Func<object, string>> ExtensionParseFuncDict = new Dictionary<Type, Func<object, string>>();
        protected Dictionary<Type, object> exporters = new Dictionary<Type, object>();

        /// <summary>
        /// 扩展类型与其对应的转换Json文本方法
        /// </summary>
        //Dictionary<Type, Func<string, object>> ExtensionToJsonFuncDict = new Dictionary<Type, Func<string, object>>();
        protected Dictionary<Type, object> importers = new Dictionary<Type, object>();

        public void Extension<T>(Func<T, string> exporter, Func<string, T> importer) where T : class
        {
            exporters[typeof(T)] = exporter;
            importers[typeof(T)] = importer;
        }

        public abstract string ToJson(object obj);
        public abstract string ToJson(JsonData jsonData);
        public abstract JsonData ToJsonData(string json);

        public JsonData ToObject(TextReader reader)
        {
            return ToJsonData(reader.ReadToEnd());
        }

        public T ToObject<T>(TextReader reader)
        {
            return (T)ToObject(reader.ReadToEnd(), typeof(T));
        }
        public abstract object ToObject(string json, Type ConvertType);

        public T ToObject<T>(string json)
        {
            return (T)ToObject(json, typeof(T));
        }
    }





}




