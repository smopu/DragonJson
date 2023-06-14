using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtrReflection
{
    /// <summary>
    /// 字符串转数字
    /// </summary>
    public unsafe class StringToNumber
    {
        static double[] pow309 = new double[309];
        static double[] pow309negative = new double[309];
        private static bool initializedPow = false;
        //public static bool isStart = false;
        public static void Start()
        {
            if (!initializedPow)
            {
                lock (pow309)
                {
                    lock (pow309negative)
                    {
                        if (!initializedPow)
                        {
                            for (int i = 0; i < 309; i++)
                            {
                                pow309[i] = Math.Pow(10, i);
                                pow309negative[i] = Math.Pow(10, -i);
                            }
                            initializedPow = true;
                        }
                    }
                }
            }
        }

        public static bool ToNumber(ref char* now, ref int i, int length, ref long v_long, ref double v_double)
        {
            if (!initializedPow)
            {
                Start();
            }
            bool fu = false;
            if (*now < '0' || *now > '9')
            {
                if (*now == '-')
                {
                    fu = true;
                    //负号后面可以加空格
                    for (++i, ++now; i < length; ++i, ++now)
                    {
                        if (*now != ' ')
                        {
                            break;
                        }
                    }
                }
                else if (*now == '+')
                {
                    ++i; ++now;
                }
                else
                {
                    throw new Exception("无法解析" + *now);
                }
            }
            v_long = (*now - '0');
            for (++i, ++now; i < length; ++i, ++now)
            {
                if (*now < '0' || *now > '9')
                {
                    if (*now == '.')
                    {
                        goto Dot;
                    }
                    else if (*now == 'E' || *now == 'e')
                    {
                        --now;
                        --i;
                        goto Dot;
                    }
                    else if (v_long == 0 && (*now == 'x' || *now == 'X'))
                    {
                        //16进制
                        for (++i, ++now; i < length; ++i, ++now)
                        {
                            switch (*now)
                            {
                                case '0':
                                    v_long <<= 4;
                                    break;
                                case '1':
                                    v_long <<= 4;
                                    v_long += 1;
                                    break;
                                case '2':
                                    v_long <<= 4;
                                    v_long += 2;
                                    break;
                                case '3':
                                    v_long <<= 4;
                                    v_long += 3;
                                    break;
                                case '4':
                                    v_long <<= 4;
                                    v_long += 4;
                                    break;
                                case '5':
                                    v_long <<= 4;
                                    v_long += 5;
                                    break;
                                case '6':
                                    v_long <<= 4;
                                    v_long += 6;
                                    break;
                                case '7':
                                    v_long <<= 4;
                                    v_long += 7;
                                    break;
                                case '8':
                                    v_long <<= 4;
                                    v_long += 8;
                                    break;
                                case '9':
                                    v_long <<= 4;
                                    v_long += 9;
                                    break;
                                case 'a':
                                case 'A':
                                    v_long <<= 4;
                                    v_long += 10;
                                    break;
                                case 'b':
                                case 'B':
                                    v_long <<= 4;
                                    v_long += 11;
                                    break;
                                case 'c':
                                case 'C':
                                    v_long <<= 4;
                                    v_long += 12;
                                    break;
                                case 'd':
                                case 'D':
                                    v_long <<= 4;
                                    v_long += 13;
                                    break;
                                case 'e':
                                case 'E':
                                    v_long <<= 4;
                                    v_long += 14;
                                    break;
                                case 'f':
                                case 'F':
                                    v_long <<= 4;
                                    v_long += 15;
                                    break;
                                default:
                                    v_long = fu ? -v_long : v_long;
                                    return true;
                            }
                        }
                    }
                    else
                    {
                        v_long = fu ? -v_long : v_long;
                        return true;
                    }
                }
                else
                {
                    v_long *= 10;
                    v_long += (*now - '0');
                }
            }
            //long
            return true;

            Dot:
            v_double = v_long;
            //点
            var v_decimal = 0.1;
            for (++i, ++now; i < length; ++i, ++now)
            {
                if (*now < '0' || *now > '9')
                {
                    if (*now == 'E' || *now == 'e')
                    {
                        ++now; ++i;
                        if (*now == '-')
                        {
                            int ex = 0;

                            for (++i, ++now; i < length; ++i, ++now)
                            {
                                if (*now < '0' || *now > '9')
                                {
                                    break;
                                }
                                else
                                {
                                    ex *= 10;
                                    ex += (*now - '0');
                                }
                            }
                            v_double *= pow309negative[ex];
                            v_double = fu ? -v_double : v_double;
                            return false;
                        }
                        else if (*now == '+')
                        {
                            int ex = 0;

                            for (++i, ++now; i < length; ++i, ++now)
                            {
                                if (*now < '0' || *now > '9')
                                {
                                    break;
                                }
                                else
                                {
                                    ex *= 10;
                                    ex += (*now - '0');
                                }
                            }
                            v_double *= pow309[ex];
                            v_double = fu ? -v_double : v_double;
                            return false;
                        }

                    }
                    break;
                }
                else
                {
                    v_double += (*now - '0') * v_decimal;
                    v_decimal *= 0.1;
                }
            }
            v_double = fu ? -v_double : v_double;
            return false;

        }

    }
}
