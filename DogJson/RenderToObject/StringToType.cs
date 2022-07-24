using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.RenderToObject
{
    public class StringToType
    {
        public StringToType(IDictionary<string, Type> allType)
        { 
            this.allType = allType;
        }
        IDictionary<string, Type> allType;
        struct TypeWords
        {
            public enum WordsEnum
            {
                type,
                generic,
                startGeneric,
                endGeneric,
                array,
            }
            public WordsEnum termsEnum;
            public string name;
            public override string ToString()
            {
                return name;
            }
            public int genericArgSize;
            public int arrayRank;
        }

        class TypeSentence
        {
            public Type type;
            public int genericArgSize;
            public int chakeGenericArgSize;
            public List<int> arrayLengths;
            public override string ToString()
            {
                if (arrayLengths != null)
                {
                    string str = type.ToString();
                    for (int i = 0; i < arrayLengths.Count; i++)
                    {
                        str += "[" + arrayLengths[i] + "]";
                    }
                    return str;
                }
                return type.ToString();
            }
        }

        public unsafe Type GetType(string str)
        {
            Type type;
            type = AppDomainGetType(str);
            if (type != null)
            {
                return type;
            }

            //词法分析
            List<TypeWords> typeTerms = GetTypeLexicalAnalysis(str);
            //语法分析
            Stack<TypeSentence> typeSentences = GetTypeGrammaAnalysis(typeTerms);
            //语义分析
            type = GetTypeSemanticAnalysis(typeSentences);

            return type;
        }

        /// <summary>
        /// 词法分析
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        unsafe List<TypeWords> GetTypeLexicalAnalysis(string str)
        {
            //词法分析
            List<TypeWords> typeWords = new List<TypeWords>();
            fixed (char* s = str)
            {
                int start = 0;
                int length = str.Length;
                for (int i = 0; i < length; i++)
                {
                Loop:
                    char now = s[i];
                    switch (now)
                    {
                        //case '`':
                        //    //泛型
                        //    int size = 0;
                        //    ++i;
                        //    for (; i < length; i++)
                        //    {
                        //        now = s[i];
                        //        if ('0' > now || now > '9')
                        //        {
                        //            break;
                        //        }
                        //        else
                        //        {
                        //            size *= 10;
                        //            size += now - '0';
                        //        }
                        //    }
                        //    TypeWords typeTerms1 = new TypeWords();
                        //    typeTerms1.name = new string(s, start, i - start);
                        //    typeTerms1.termsEnum = TypeWords.WordsEnum.generic;
                        //    typeTerms1.genericArgSize = size;
                        //    typeWords.Add(typeTerms1);
                        //    if (now == '[')
                        //    {
                        //        start = i;
                        //        goto case '[';
                        //    }
                        //    else if (now == '+')
                        //    {

                        //    }
                        //    else
                        //    {
                        //        throw new Exception("泛型词法错误，泛型数字后面没有[或+" + new string(s, 0, i));
                        //    }
                        //    start = i;
                        //    goto Loop;
                        //    break;
                        case '[':
                            if (i - start > 0)
                            {
                                TypeWords typeTerms3 = new TypeWords();
                                typeTerms3.name = new string(s, start, i - start);
                                typeTerms3.termsEnum = TypeWords.WordsEnum.type;
                                typeWords.Add(typeTerms3);
                            }
                            start = i;
                            ++i;
                            if (i == length) { throw new Exception("数组词法错误，" + new string(s, 0, i)); }
                            now = s[i];
                            if (now == ']')
                            {
                                TypeWords typeTerms2 = new TypeWords();
                                typeTerms2.name = new string(s, start, i - start + 1);
                                typeTerms2.termsEnum = TypeWords.WordsEnum.array;
                                typeTerms2.arrayRank = 1;
                                typeWords.Add(typeTerms2);
                                start = i + 1;
                                break;
                            }
                            else if (now == ',')
                            {
                                int arrayRank = 1;
                                do
                                {
                                    ++arrayRank;
                                    ++i;
                                    if (i == length) { throw new Exception("数组词法错误，" + new string(s, 0, i)); }
                                    now = s[i];
                                } while (now == ',');

                                if (now != ']')
                                {
                                    throw new Exception("数组词法错误，" + new string(s, 0, i));
                                }
                                TypeWords typeTerms2 = new TypeWords();
                                typeTerms2.name = new string(s, start, i - start + 1);
                                typeTerms2.termsEnum = TypeWords.WordsEnum.array;
                                typeTerms2.arrayRank = arrayRank;
                                typeWords.Add(typeTerms2);
                                start = i + 1;
                                break;
                            }
                            else
                            {
                                TypeWords typeTerms2 = new TypeWords();
                                typeTerms2.name = "[";
                                typeTerms2.termsEnum = TypeWords.WordsEnum.startGeneric;
                                typeWords.Add(typeTerms2);
                                start = i;
                                goto Loop;
                            }
                            break;
                        case ',':
                            if (i - start > 0)
                            {
                                TypeWords typeTerms3 = new TypeWords();
                                typeTerms3.name = new string(s, start, i - start);
                                typeTerms3.termsEnum = TypeWords.WordsEnum.type;
                                typeWords.Add(typeTerms3);
                            }
                            start = i + 1;
                            break;
                        //goto Loop;
                        case ']':
                            if (i - start > 0)
                            {
                                TypeWords typeTerms3 = new TypeWords();
                                typeTerms3.name = new string(s, start, i - start);
                                typeTerms3.termsEnum = TypeWords.WordsEnum.type;
                                typeWords.Add(typeTerms3);

                                TypeWords typeTerms2 = new TypeWords();
                                typeTerms2.name = "]";
                                typeTerms2.termsEnum = TypeWords.WordsEnum.endGeneric;
                                typeWords.Add(typeTerms2);

                                start = i + 1;
                                break;
                            }
                            else
                            {
                                TypeWords typeTerms2 = new TypeWords();
                                typeTerms2.name = "]";
                                typeTerms2.termsEnum = TypeWords.WordsEnum.endGeneric;
                                typeWords.Add(typeTerms2);
                                start = i + 1;
                                break;
                            }
                            break;
                        default:
                            //if (!isRread)
                            //{
                            //    isRread = true;
                            //    start = i;
                            //}
                            break;
                    }
                }
            }
            return typeWords;
        }

        /// <summary>
        /// 语法分析
        /// </summary>
        /// <param name="typeWords"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        unsafe Stack<TypeSentence> GetTypeGrammaAnalysis(List<TypeWords> typeWords)
        {
            //语法分析
            Stack<TypeSentence> stack = new Stack<TypeSentence>();
            Stack<TypeSentence> stackGeneric = new Stack<TypeSentence>();
            TypeSentence now = null;
            for (int i = 0; i < typeWords.Count; i++)
            {
                TypeSentence typeSentence1;
                TypeWords nowTerms = typeWords[i];
                switch (nowTerms.termsEnum)
                {
                    case TypeWords.WordsEnum.type:
                        if (stackGeneric.Count > 0) { stackGeneric.Peek().chakeGenericArgSize++; }
                        typeSentence1 = new TypeSentence();
                        typeSentence1.type = AppDomainGetType(nowTerms.name);
                        if (typeSentence1.type.IsGenericType)
                        {
                            typeSentence1.genericArgSize = typeSentence1.type.GetGenericArguments().Length;
                        }
                        stack.Push(typeSentence1);
                        now = typeSentence1;
                        break;
                    //case TypeWords.WordsEnum.generic:
                    //    if (stackGeneric.Count > 0) { stackGeneric.Peek().chakeGenericArgSize++; }
                    //    typeSentence1 = new TypeSentence();
                    //    typeSentence1.type = AppDomainGetType(nowTerms.name);
                    //    typeSentence1.genericArgSize = nowTerms.genericArgSize;
                    //    stack.Push(typeSentence1);
                    //    now = typeSentence1;
                    //    break;
                    case TypeWords.WordsEnum.startGeneric:
                        stackGeneric.Push(now);
                        now = null;
                        break;
                    case TypeWords.WordsEnum.endGeneric:
                        now = stackGeneric.Pop();
                        if (now.genericArgSize != now.chakeGenericArgSize)
                        {
                            throw new Exception("泛型语法错误，" + nowTerms.name);
                        }
                        break;
                    case TypeWords.WordsEnum.array:
                        if (now.arrayLengths == null)
                        {
                            now.arrayLengths = new List<int>();
                        }
                        now.arrayLengths.Add(nowTerms.arrayRank);
                        break;
                    default:
                        break;
                }
            }
            if (stackGeneric.Count > 0)
            {
                throw new Exception("语法错误，泛型未完成");
            }
            return stack;
        }

        /// <summary>
        /// 语义分析
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        unsafe Type GetTypeSemanticAnalysis(Stack<TypeSentence> stack)
        {
            Stack<Type> stack2 = new Stack<Type>();
            while (true)
            {
                TypeSentence data = stack.Pop();
                if (data.genericArgSize > 0)
                {
                    Type[] arg = new Type[data.genericArgSize];
                    for (int j = 0; j < data.genericArgSize; j++)
                    {
                        arg[j] = stack2.Pop();
                    }
                    data.type = data.type.MakeGenericType(arg);
                    if (data.arrayLengths != null)
                    {
                        for (int i = 0; i < data.arrayLengths.Count; i++)
                        {
                            if (data.arrayLengths[i] == 1)
                            {
                                data.type = data.type.MakeArrayType();
                            }
                            else
                            {
                                data.type = data.type.MakeArrayType(data.arrayLengths[i]);
                            }
                        }
                    }
                    if (stack.Count == 0)
                    {
                        return data.type;
                    }
                    stack2.Push(data.type);
                }
                else
                {
                    if (data.arrayLengths != null)
                    {
                        for (int i = 0; i < data.arrayLengths.Count; i++)
                        {
                            if (data.arrayLengths[i] == 1)
                            {
                                data.type = data.type.MakeArrayType();
                            }
                            else
                            {
                                data.type = data.type.MakeArrayType(data.arrayLengths[i]);
                            }
                        }
                    }
                    if (stack.Count == 0)
                    {
                        return data.type;
                    }
                    stack2.Push(data.type);
                }
            }

        }


       Type AppDomainGetType(string str)
        {
            Type type;
            if (allType.TryGetValue(str, out type))
            {
                return type;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                type = item.GetType(str);
                if (type != null)
                {
                    break;
                }
            }

            allType[str] = type;
            return type;
        }

    }
}
