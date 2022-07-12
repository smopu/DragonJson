using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DogJson
{
    public class StringTable
    {
        const char Mo = '\"';  //'\0';

        internal class StringTableLive
        {
            internal struct Chake
            {
                public int chakeIndex;

                public char chakeValue;

                public unsafe Chake* next;

                public unsafe Chake* no;

                public unsafe char* data;

                public unsafe char* noData;

                public int dataIndex;

                public int noDataIndex;
            }

            internal class ChakeTest
            {
                public int chakeIndex;

                public char chakeValue;

                public ChakeTest next;

                public ChakeTest no;

                public unsafe char* data;

                public unsafe char* noData;

                public int dataIndex;

                public int noDataIndex;

                public int testIndex;
            }

            internal class ChakeNode
            {
                private struct Data
                {
                    public string str;

                    public int index;

                    public Data(string str, int index)
                    {
                        this.str = str;
                        this.index = index;
                    }
                }

                public List<ChakeNode> nodes = new List<ChakeNode>();

                public int chakeIndex = -1;

                public char chakeValue;

                public ChakeNode next;

                public ChakeNode no;

                public bool isLast;

                public string[] strs;

                public int index;

                public int dataIndex = -1;

                public int charIndex = -1;

                public ChakeNode()
                {
                }

                public override string ToString()
                {
                    if (isLast)
                    {
                        return chakeValue + " " + chakeIndex + " last";
                    }

                    return chakeValue + " " + chakeIndex;
                }

                public ChakeNode(char chakeValue, int chakeIndex)
                {
                    this.chakeValue = chakeValue;
                    this.chakeIndex = chakeIndex;
                }

                public void Division()
                {
                    int num = chakeIndex;
                    Dictionary<char, List<string>> dictionary = new Dictionary<char, List<string>>();
                    //_ = strs[0].Length;
                    do
                    {
                        num++;
                        dictionary = new Dictionary<char, List<string>>();
                        for (int i = 0; i < strs.Length; i++)
                        {
                            char key = strs[i][num];
                            if (dictionary.ContainsKey(key))
                            {
                                dictionary[key].Add(strs[i]);
                                continue;
                            }

                            dictionary[key] = new List<string>();
                            dictionary[key].Add(strs[i]);
                        }
                    }
                    while (dictionary.Count == 1);
                    dictionary = dictionary.OrderByDescending((KeyValuePair<char, List<string>> x) => x.Value.Count).ToDictionary((KeyValuePair<char, List<string>> x) => x.Key, (KeyValuePair<char, List<string>> x) => x.Value);
                    foreach (KeyValuePair<char, List<string>> item in dictionary)
                    {
                        ChakeNode chakeNode = new ChakeNode();
                        chakeNode.chakeIndex = num;
                        chakeNode.chakeValue = item.Key;
                        chakeNode.strs = item.Value.ToArray();
                        if (chakeNode.strs.Length > 1)
                        {
                            chakeNode.Division();
                        }

                        nodes.Add(chakeNode);
                    }
                }

                public void Assignment(List<ChakeNode> list)
                {
                    if (nodes.Count <= 0)
                    {
                        return;
                    }

                    for (int i = 0; i < nodes.Count - 1; i++)
                    {
                        nodes[i].no = nodes[i + 1];
                        if (nodes[i].nodes.Count > 0)
                        {
                            nodes[i].next = nodes[i].nodes[0];
                        }

                        list.Add(nodes[i]);
                        nodes[i].Assignment(list);
                    }

                    if (nodes[nodes.Count - 1].nodes.Count > 0)
                    {
                        nodes[nodes.Count - 1].next = nodes[nodes.Count - 1].nodes[0];
                    }

                    list.Add(nodes[nodes.Count - 1]);
                    nodes[nodes.Count - 1].isLast = true;
                    nodes[nodes.Count - 1].Assignment(list);
                }
            }

            internal IntPtr stackIntPtr;

            internal IntPtr allCharPtr;

            internal unsafe Chake* chakes;

            internal int num;

            private int oneData;

            private bool isOne;

            private unsafe char* oneChar;

            ~StringTableLive()
            {
                if (isOne)
                {
                    Marshal.FreeHGlobal(allCharPtr);
                    return;
                }

                Marshal.FreeHGlobal(stackIntPtr);
                Marshal.FreeHGlobal(allCharPtr);
            }

            public unsafe StringTableLive(Dictionary<string, int> stringToIndex, int length)
            {
                if (stringToIndex.Count == 1)
                {
                    isOne = true;
                    this.num = length;
                    oneData = stringToIndex.First().Value;
                    allCharPtr = Marshal.AllocHGlobal((length + 1) * 2);
                    oneChar = (char*)allCharPtr.ToPointer();
                    string key = stringToIndex.First().Key;
                    for (int i = 0; i < key.Length; i++)
                    {
                        oneChar[i] = key[i];
                    }

                    oneChar[length] = Mo;
                    return;
                }

                string[] array = new string[stringToIndex.Count];
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                int num = 0;
                foreach (KeyValuePair<string, int> item in stringToIndex)
                {
                    dictionary[item.Key] = num;
                    array[num] = item.Key;
                    num++;
                }

                ChakeNode chakeNode = new ChakeNode();
                chakeNode.strs = stringToIndex.Keys.ToArray();
                chakeNode.Division();
                List<ChakeNode> list = new List<ChakeNode>();
                List<ChakeNode> list2 = new List<ChakeNode>();
                chakeNode.Assignment(list);
                int num2 = 0;
                foreach (ChakeNode item2 in list)
                {
                    if (item2.strs.Length == 1)
                    {
                        item2.dataIndex = stringToIndex[item2.strs[0]];
                        item2.charIndex = dictionary[item2.strs[0]];
                    }

                    if (!item2.isLast)
                    {
                        item2.index = list2.Count;
                        list2.Add(item2);
                        num2++;
                    }
                }

                this.num = length;
                int num3 = this.num + 1;
                stackIntPtr = Marshal.AllocHGlobal(num2 * Marshal.SizeOf(typeof(Chake)));
                chakes = (Chake*)stackIntPtr.ToPointer();
                allCharPtr = Marshal.AllocHGlobal(stringToIndex.Count * num3 * 2);
                char* ptr = (char*)allCharPtr.ToPointer();
                for (int j = 0; j < stringToIndex.Count; j++)
                {
                    for (int k = 0; k < num3 - 1; k++)
                    {
                        char c = array[j][k];
                        ptr[j * num3 + k] = c;
                    }

                    ptr[j * num3 + num3 - 1] = Mo;
                }

                ChakeTest[] array2 = new ChakeTest[num2];
                for (int l = 0; l < array2.Length; l++)
                {
                    array2[l] = new ChakeTest();
                    array2[l].testIndex = l;
                }

                for (int m = 0; m < list2.Count; m++)
                {
                    ChakeNode chakeNode2 = list2[m];
                    array2[m].chakeIndex = chakeNode2.chakeIndex;
                    array2[m].chakeValue = chakeNode2.chakeValue;
                    if (chakeNode2.next == null)
                    {
                        array2[m].next = null;
                        array2[m].data = ptr + chakeNode2.charIndex;
                        array2[m].dataIndex = chakeNode2.dataIndex;
                    }
                    else
                    {
                        array2[m].next = array2[chakeNode2.next.index];
                    }

                    if (chakeNode2.no.next == null && chakeNode2.no.no == null)
                    {
                        array2[m].no = null;
                        array2[m].noData = ptr + chakeNode2.no.charIndex;
                        array2[m].noDataIndex = chakeNode2.no.dataIndex;
                    }
                    else if (chakeNode2.no.isLast)
                    {
                        array2[m].no = array2[chakeNode2.no.next.index];
                    }
                    else
                    {
                        array2[m].no = array2[chakeNode2.no.index];
                    }
                }

                for (int n = 0; n < list2.Count; n++)
                {
                    ChakeNode chakeNode3 = list2[n];
                    chakes[n].chakeIndex = chakeNode3.chakeIndex;
                    chakes[n].chakeValue = chakeNode3.chakeValue;
                    if (chakeNode3.next == null)
                    {
                        chakes[n].next = null;
                        chakes[n].data = ptr + num3 * chakeNode3.charIndex;
                        chakes[n].dataIndex = chakeNode3.dataIndex;
                    }
                    else
                    {
                        chakes[n].next = chakes + chakeNode3.next.index;
                    }

                    if (chakeNode3.no.next == null && chakeNode3.no.no == null)
                    {
                        chakes[n].no = null;
                        chakes[n].noData = ptr + num3 * chakeNode3.no.charIndex;
                        chakes[n].noDataIndex = chakeNode3.no.dataIndex;
                    }
                    else if (chakeNode3.no.isLast)
                    {
                        chakes[n].no = chakes + chakeNode3.no.next.index;
                    }
                    else
                    {
                        chakes[n].no = chakes + chakeNode3.no.index;
                    }
                }
            }

            public unsafe int Run(char* cha)
            {
                if (isOne)
                {
                    if (EqualsHelper(oneChar, cha))
                    {
                        return oneData;
                    }

                    return -1;
                }

                Chake* ptr = chakes;
                while (true)
                {
                    if (cha[ptr->chakeIndex] == ptr->chakeValue)
                    {
                        if (ptr->next == null)
                        {
                            if (EqualsHelper(ptr->data, cha))
                            {
                                return ptr->dataIndex;
                            }

                            return -1;
                        }

                        ptr = ptr->next;
                    }
                    else
                    {
                        if (ptr->no == null)
                        {
                            break;
                        }

                        ptr = ptr->no;
                    }
                }

                if (EqualsHelper(ptr->noData, cha))
                {
                    return ptr->noDataIndex;
                }

                return -1;
            }

            private unsafe bool EqualsHelper(char* ptr, char* ptr3)
            {
                long* ptr4 = (long*)ptr;
                long* ptr5 = (long*)ptr3;
                while (num >= 12)
                {
                    if (*ptr4 != *ptr5)
                    {
                        return false;
                    }

                    if (ptr4[1] != ptr5[1])
                    {
                        return false;
                    }

                    if (ptr4[2] != ptr5[2])
                    {
                        return false;
                    }

                    ptr4 += 3;
                    ptr5 += 3;
                    num -= 12;
                }

                while (num >= 4)
                {
                    if (*ptr4 != *ptr5)
                    {
                        return false;
                    }

                    ptr4++;
                    ptr5++;
                    num -= 4;
                }

                if (num == 0)
                {
                    return true;
                }

                if (num == 3)
                {
                    if (*(int*)ptr4 != *(int*)ptr5)
                    {
                        return false;
                    }

                    if (*(int*)((byte*)ptr4 + 4) != *(int*)((byte*)ptr5 + 4))
                    {
                        return false;
                    }
                }
                else if (*(int*)ptr4 != *(int*)ptr5)
                {
                    return false;
                }

                return true;
            }
        }

        private StringTableLive[] lives;

        public StringTable(Dictionary<string, int> strs)
        {
            int num = 0;
            Dictionary<int, Dictionary<string, int>> dictionary = new Dictionary<int, Dictionary<string, int>>();
            foreach (KeyValuePair<string, int> str in strs)
            {
                if (!dictionary.ContainsKey(str.Key.Length))
                {
                    dictionary[str.Key.Length] = new Dictionary<string, int>();
                }

                dictionary[str.Key.Length][str.Key] = str.Value;
                if (num < str.Key.Length)
                {
                    num = str.Key.Length;
                }
            }

            lives = new StringTableLive[num + 1];
            foreach (KeyValuePair<int, Dictionary<string, int>> item in dictionary)
            {
                lives[item.Key] = new StringTableLive(item.Value, item.Key);
            }
        }

        public unsafe int Find(char* d, int length)
        {
            if (length >= lives.Length) 
            {
                return -1;
            }
            if (lives[length] != null)
            {
                return lives[length].Run(d);
            }
            return -1;
        }
    }
}