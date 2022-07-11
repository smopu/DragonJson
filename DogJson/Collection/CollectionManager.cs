using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{


    public static class CollectionManager
    {
        //static Dictionary<Type, IWriterObject> writeArrayMap = new Dictionary<Type, IWriterObject>();
        //static readonly Dictionary<Type, Type> writeArrayTypeMap = new Dictionary<Type, Type>();
        
        static Dictionary<Type, IWriterCollectionObject> writeObjectMap = new Dictionary<Type, IWriterCollectionObject>();
        static readonly Dictionary<Type, TypeCollectionMapItem> writeObjectTypeMap = new Dictionary<Type, TypeCollectionMapItem>();
        
        static Dictionary<Type, IWriterCollectionString> writeStringMap = new Dictionary<Type, IWriterCollectionString>();
        static readonly Dictionary<Type, TypeCollectionMapItem> writeStringTypeMap = new Dictionary<Type, TypeCollectionMapItem>();
        static readonly HashSet<Type> writeStringInheritedTypeMap = new HashSet<Type>();




        public static readonly Dictionary<Type, IReadCollectionObject> readObjectMap = new Dictionary<Type, IReadCollectionObject>();
        static readonly Dictionary<Type, TypeCollectionMapItem> readObjectTypeMap = new Dictionary<Type, TypeCollectionMapItem>();

        public static readonly Dictionary<Type, IReadCollectionObject> readArrayMap = new Dictionary<Type, IReadCollectionObject>();
        static readonly Dictionary<Type, TypeCollectionMapItem> readArrayTypeMap = new Dictionary<Type, TypeCollectionMapItem>();

        static readonly HashSet<Type> readInheritedTypeMap = new HashSet<Type>();
        static readonly HashSet<Type> writeInheritedTypeMap = new HashSet<Type>();

        static readonly Dictionary<Type, ReadCollectionLink> readArrayMapNew = new Dictionary<Type, ReadCollectionLink>();
        static readonly Dictionary<Type, TypeCollectionMapItem> readArrayTypeMapNew = new Dictionary<Type, TypeCollectionMapItem>();
        static readonly HashSet<Type> readInheritedTypeMapNew = new HashSet<Type>();


        public static ReadCollectionLink GetReadArrayCollectionLink(Type type)
        {
            return GetCollection2<ReadCollectionLink>(readArrayMapNew, readArrayTypeMapNew, type, readInheritedTypeMapNew);
        }


        class TypeCollectionMapItem
        {
            public Type type;
            public bool isSpecialCaseGeneric;
            public List<SpecialCaseGeneric> specialCaseGenerics;
        }


        //public static IWriterObject GetIWriterArray(Type type)
        //{
        //    return GetCollection<IWriterObject>(writeArrayMap, writeArrayTypeMap, type);
        //}
        public static IWriterCollectionObject GetWriterCollection(Type type)
        {
            return GetCollection<IWriterCollectionObject>(writeObjectMap, writeObjectTypeMap, type, writeInheritedTypeMap);
        }
        public static IWriterCollectionString GetWriterCollectionString(Type type)
        {
            return GetCollection<IWriterCollectionString>(writeStringMap, writeStringTypeMap, type, writeStringInheritedTypeMap);
        }

        public static IReadCollectionObject GetReadObjectCollection(Type type)
        {
            return GetCollection<IReadCollectionObject>(readObjectMap, readObjectTypeMap, type, readInheritedTypeMap);
        }
        public static IReadCollectionObject GetReadArrayCollection(Type type)
        {
            return GetCollection<IReadCollectionObject>(readArrayMap, readArrayTypeMap, type, readInheritedTypeMap);
        }
        
        static T GetCollection<T>(Dictionary<Type, T> map, Dictionary<Type, TypeCollectionMapItem> mapType, Type type, HashSet<Type> inheritedTypeMap)
            where T : class
        {
            bool isChakeInherited = false;
            T array = null;
            Start:
            if (map.TryGetValue(type, out array))
            {
            }
            else
            {
                TypeCollectionMapItem generic;
                if (type.IsGenericType && mapType.TryGetValue(type.GetGenericTypeDefinition(), out generic))
                {
                    if (generic.isSpecialCaseGeneric)
                    {
                        foreach (SpecialCaseGeneric item in generic.specialCaseGenerics)
                        {
                            Type tagteData;
                            if (item.IsSpecialCaseGeneric(type, out tagteData))
                            {
                                array = map[type] = (T)Activator.CreateInstance(tagteData);
                                return array;
                            }
                        }
                    }
                    if (generic.type == null)
                    {
                        return array;
                    }
                    array = map[type] = (T)Activator.CreateInstance(generic.type.MakeGenericType(type.GetGenericArguments()));
                }
                else
                {
                    if (!isChakeInherited)
                    {
                        type = type.BaseType;
                        if (inheritedTypeMap.Contains(type))
                        {
                            isChakeInherited = true;
                            goto Start;
                        }
                    }
                }
            }
            return array;
        }

        static T GetCollection2<T>(Dictionary<Type, T> map, Dictionary<Type, TypeCollectionMapItem> mapType, Type type, HashSet<Type> inheritedTypeMap)
            where T : class
        {
            bool isChakeInherited = false;
            T array = null;
        Start:
            if (map.TryGetValue(type, out array))
            {
            }
            else
            {
                TypeCollectionMapItem generic;
                if (type.IsGenericType && mapType.TryGetValue(type.GetGenericTypeDefinition(), out generic))
                {
                    if (generic.isSpecialCaseGeneric)
                    {
                        foreach (SpecialCaseGeneric item in generic.specialCaseGenerics)
                        {
                            Type tagteData;
                            if (item.IsSpecialCaseGeneric(type, out tagteData))
                            {
                                array = map[type] = (T)Activator.CreateInstance(tagteData);
                                return array;
                            }
                        }
                    }
                    if (generic.type == null)
                    {
                        return array;
                    }
                    array = map[type] = ((CreateTaget<T>)Activator.CreateInstance(generic.type.MakeGenericType(type.GetGenericArguments()))).Create();
                }
                else
                {
                    if (!isChakeInherited)
                    {
                        type = type.BaseType;
                        if (inheritedTypeMap.Contains(type))
                        {
                            isChakeInherited = true;
                            goto Start;
                        }
                    }
                }
            }
            return array;
        }



        static bool isStart = false;
        static object lockObj = new object();
        public static bool IsStart{ get{ return isStart; } }
        public static IJsonRenderToObject jsonRenderToObject;
        public static void Start(IJsonRenderToObject defaultJsonRenderToObject)
        {
            lock (lockObj)
            {
                lock (UnsafeOperation.dictionaryAllAssembly)
                {
                    if (isStart)
                    {
                        return;
                    }

                    jsonRenderToObject = defaultJsonRenderToObject;

                    var assembly = Assembly.GetAssembly(typeof(IReadCollectionObject));

                    string name = assembly.GetName().Name;
                    UnsafeOperation.dictionaryAllAssembly[name] = assembly;
                    ChakeAssembly(assembly);

                    var executingAssembly = Assembly.GetExecutingAssembly();
                    if (executingAssembly != null)
                    {
                        name = executingAssembly.GetName().Name;
                        if (!UnsafeOperation.dictionaryAllAssembly.ContainsKey(name))
                        {
                            UnsafeOperation.dictionaryAllAssembly[name] = executingAssembly;
                            ChakeAssembly(executingAssembly);
                        }
                    }

                    var callingAssembly = Assembly.GetCallingAssembly();
                    if (callingAssembly != null)
                    {
                        name = callingAssembly.GetName().Name;
                        if (!UnsafeOperation.dictionaryAllAssembly.ContainsKey(name))
                        {
                            ChakeAssembly(callingAssembly);
                        }
                    }

                    var entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        name = entryAssembly.GetName().Name;
                        if (!UnsafeOperation.dictionaryAllAssembly.ContainsKey(name))
                        {
                            ChakeAssembly(entryAssembly);
                        }
                    }

                    isStart = true;
                    return;
                }
            }
        }


        private static void ChakeAssembly(Assembly assembly)
        {
            foreach (var collectionType in assembly.GetTypes())
            {
                CollectionReadAttribute attribute = collectionType.GetCustomAttribute<CollectionReadAttribute>();
                if (attribute != null)
                {
                    if (attribute.isArrays)
                    {
                        ChakeType<IReadCollectionObject>(readArrayTypeMap, readArrayMap, collectionType, attribute.type);
                    }
                    else
                    {
                        ChakeType<IReadCollectionObject>(readObjectTypeMap, readObjectMap, collectionType, attribute.type);
                    }
                    if (attribute.inherited)
                    {
                        readInheritedTypeMap.Add(attribute.type);
                    }
                }

                IEnumerable<CollectionWriteAttribute> attributeWrite = collectionType.GetCustomAttributes<CollectionWriteAttribute>();
                foreach (var attributeType in attributeWrite)
                {
                    if (attributeType.inherited)
                    {
                        writeInheritedTypeMap.Add(attributeType.type);
                    }
                    if (attributeType.type.IsGenericType)
                    {
                        if (attributeType.type.BaseType == typeof(SpecialCaseGeneric))
                        {
                            SpecialCaseGeneric specialCaseGeneric = SpecialCaseGeneric.Create(attributeType.type, collectionType);
                            Type trueType = SpecialCaseGeneric.GetGenericType(attributeType.type);

                            TypeCollectionMapItem map;
                            if (writeObjectTypeMap.TryGetValue(trueType.GetGenericTypeDefinition(), out map))
                            {
                                if (map.isSpecialCaseGeneric)
                                {
                                    map.specialCaseGenerics.Add(specialCaseGeneric);
                                }
                                else
                                {
                                    map.isSpecialCaseGeneric = true;
                                    map.specialCaseGenerics = new List<SpecialCaseGeneric>();
                                    map.specialCaseGenerics.Add(specialCaseGeneric);
                                }
                            }
                            else
                            {
                                map = new TypeCollectionMapItem();
                                map.type = null;
                                map.isSpecialCaseGeneric = true;
                                map.specialCaseGenerics = new List<SpecialCaseGeneric>();
                                map.specialCaseGenerics.Add(specialCaseGeneric);
                                writeObjectTypeMap[trueType.GetGenericTypeDefinition()] = map;
                            }

                        }
                        else
                        {
                            TypeCollectionMapItem map;
                            if (writeObjectTypeMap.TryGetValue(attributeType.type, out map))
                            {
                                if (map.isSpecialCaseGeneric)
                                {
                                    map.type = collectionType;
                                }
                                else
                                {
                                    //重复
                                }
                            }
                            else
                            {
                                map = new TypeCollectionMapItem();
                                map.type = collectionType;
                                map.isSpecialCaseGeneric = false;
                                writeObjectTypeMap[attributeType.type] = map;
                            }
                        }
                    }
                    else
                    {
                        writeObjectMap[attributeType.type] = (IWriterCollectionObject)System.Activator.CreateInstance(collectionType);
                    }
                }


                IEnumerable<CollectionWriteStringAttribute> attributeWrite2 = collectionType.GetCustomAttributes<CollectionWriteStringAttribute>();
                foreach (var attributeType in attributeWrite2)
                {
                    if (attributeType.inherited)
                    {
                        writeStringInheritedTypeMap.Add(attributeType.type);
                    }
                    ChakeType<IWriterCollectionString>(writeStringTypeMap, writeStringMap, collectionType, attributeType.type);
                }


                ReadCollectionAttribute readCollectionAttribute = collectionType.GetCustomAttribute<ReadCollectionAttribute>();
                if (readCollectionAttribute != null)
                {
                    if (readCollectionAttribute.isArrays)
                    {
                        ChakeType2<ReadCollectionLink>(readArrayTypeMapNew, readArrayMapNew, collectionType, readCollectionAttribute.type);
                    }
                    else
                    {
                        //ChakeType<IReadCollectionObject>(readObjectTypeMap, readObjectMap, collectionType, attribute.type);
                    }
                    if (readCollectionAttribute.inherited)
                    {
                        readInheritedTypeMapNew.Add(attribute.type);
                    }
                }

            }
        }
        
        private static void ChakeType2<T>(
            Dictionary<Type, TypeCollectionMapItem> typeMap,
            Dictionary<Type, T> dataMap,
            Type collectionType, Type tagetType)
        {
            if (tagetType.IsGenericType)
            {
                if (tagetType.BaseType == typeof(SpecialCaseGeneric))
                {
                    SpecialCaseGeneric specialCaseGeneric = SpecialCaseGeneric.Create(tagetType, collectionType);
                    Type trueType = SpecialCaseGeneric.GetGenericType(tagetType);

                    TypeCollectionMapItem map;
                    if (typeMap.TryGetValue(trueType.GetGenericTypeDefinition(), out map))
                    {
                        if (map.isSpecialCaseGeneric)
                        {
                            map.specialCaseGenerics.Add(specialCaseGeneric);
                        }
                        else
                        {
                            map.isSpecialCaseGeneric = true;
                            map.specialCaseGenerics = new List<SpecialCaseGeneric>();
                            map.specialCaseGenerics.Add(specialCaseGeneric);
                        }
                    }
                    else
                    {
                        map = new TypeCollectionMapItem();
                        map.type = null;
                        map.isSpecialCaseGeneric = true;
                        map.specialCaseGenerics = new List<SpecialCaseGeneric>();
                        map.specialCaseGenerics.Add(specialCaseGeneric);
                        typeMap[trueType.GetGenericTypeDefinition()] = map;
                    }

                }
                else
                {
                    TypeCollectionMapItem map;
                    if (typeMap.TryGetValue(tagetType, out map))
                    {
                        if (map.isSpecialCaseGeneric)
                        {
                            map.type = collectionType;
                        }
                        else
                        {
                            //重复
                        }
                    }
                    else
                    {
                        map = new TypeCollectionMapItem();
                        map.type = collectionType;
                        map.isSpecialCaseGeneric = false;
                        typeMap[tagetType] = map;
                    }
                }
            }
            else
            {
                dataMap[tagetType] = ((CreateTaget<T>)System.Activator.CreateInstance(collectionType)).Create();
            }
        }


        private static void ChakeType<T>(
            Dictionary<Type, TypeCollectionMapItem> typeMap,
            Dictionary<Type, T> dataMap,
            Type collectionType, Type tagetType)
        {
            if (tagetType.IsGenericType)
            {
                if (tagetType.BaseType == typeof(SpecialCaseGeneric))
                {
                    SpecialCaseGeneric specialCaseGeneric = SpecialCaseGeneric.Create(tagetType, collectionType);
                    Type trueType = SpecialCaseGeneric.GetGenericType(tagetType);

                    TypeCollectionMapItem map;
                    if (typeMap.TryGetValue(trueType.GetGenericTypeDefinition(), out map))
                    {
                        if (map.isSpecialCaseGeneric)
                        {
                            map.specialCaseGenerics.Add(specialCaseGeneric);
                        }
                        else
                        {
                            map.isSpecialCaseGeneric = true;
                            map.specialCaseGenerics = new List<SpecialCaseGeneric>();
                            map.specialCaseGenerics.Add(specialCaseGeneric);
                        }
                    }
                    else
                    {
                        map = new TypeCollectionMapItem();
                        map.type = null;
                        map.isSpecialCaseGeneric = true;
                        map.specialCaseGenerics = new List<SpecialCaseGeneric>();
                        map.specialCaseGenerics.Add(specialCaseGeneric);
                        typeMap[trueType.GetGenericTypeDefinition()] = map;
                    }

                }
                else
                {
                    TypeCollectionMapItem map;
                    if (typeMap.TryGetValue(tagetType, out map))
                    {
                        if (map.isSpecialCaseGeneric)
                        {
                            map.type = collectionType;
                        }
                        else
                        {
                            //重复
                        }
                    }
                    else
                    {
                        map = new TypeCollectionMapItem();
                        map.type = collectionType;
                        map.isSpecialCaseGeneric = false;
                        typeMap[tagetType] = map;
                    }
                }
            }
            else
            {
                dataMap[tagetType] = (T)System.Activator.CreateInstance(collectionType);
            }
        }


        //public static readonly Dictionary<Type, Type> formatterArrayTypeMap = new Dictionary<Type, Type>();

        //public static readonly Dictionary<Type, ICollectionObjectBase> formatterAllArrayMap = new Dictionary<Type, ICollectionObjectBase>();




    }

}
