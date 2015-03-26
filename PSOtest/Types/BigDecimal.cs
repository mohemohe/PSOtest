using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PSOtest.Types
{
    class BigDecimal
    {
        private const int dSize = 256;

        /// <summary>
        /// 0.0を表します。このフィールドは定数です。
        /// </summary>
        public static readonly BigDecimal Zero = new BigDecimal(0);

        public BigInteger Value { get; private set; }
        public int DecimalIndex { get; private set; }

        #region コンストラクタ

        public BigDecimal(int a)
        {
            Value = a;
            DecimalIndex = 0;
        }

        public BigDecimal(uint a)
        {
            Value = a;
            DecimalIndex = 0;
        }

        public BigDecimal(long a)
        {
            Value = a;
            DecimalIndex = 0;
        }

        public BigDecimal(ulong a)
        {
            Value = a;
            DecimalIndex = 0;
        }

        public BigDecimal(double a)
        {
            var decimalFormat = "#.";
            for (var i = 0; i < dSize; i++)
            {
                decimalFormat += "#";
            }

            var tmp = a.ToString(decimalFormat);
            Value = ParseToBigInteger(tmp.Replace(".", ""));
            DecimalIndex = tmp.Length - 1 - tmp.IndexOf(".");
        }

        public BigDecimal(string a)
        {
            Value = ParseToBigInteger(a.Replace(".", ""));
            if (a.Contains("."))
            {
                DecimalIndex = a.Length - 1 - a.IndexOf(".");
            }
            else
            {
                DecimalIndex = 0;
            }
        }

        public BigDecimal(BigInteger a)
        {
            Value = a;
            DecimalIndex = 0;
        }

        public BigDecimal(BigInteger a, int decimalIndex)
        {
            Value = a;
            DecimalIndex = decimalIndex;
        }

        public BigDecimal(BigDecimal a)
        {
            Value = a.Value;
            DecimalIndex = a.DecimalIndex;
        }

        #endregion コンストラクタ

        #region overrides

        new public string ToString()
        {
            var tmp = Value.ToString();
            if (DecimalIndex != 0)
            {
                tmp = tmp.Insert(tmp.Length - DecimalIndex, ".");
            }
            return tmp;
        }

        #endregion overrides

        #region operators

        public static BigDecimal operator +(BigDecimal a, BigDecimal b)
        {
            return Add(a, b);
        }

        public static BigDecimal operator -(BigDecimal a, BigDecimal b)
        {
            return Sub(a, b);
        }

        public static BigDecimal operator *(BigDecimal a, BigDecimal b)
        {
            return Mul(a, b);
        }

        public static BigDecimal operator /(BigDecimal a, BigDecimal b)
        {
            return Div(a, b);
        }

        public static bool operator ==(BigDecimal a, BigDecimal b)
        {
            return (a.Value == b.Value && a.DecimalIndex == b.DecimalIndex);
        }

        public static bool operator !=(BigDecimal a, BigDecimal b)
        {
            return (a.Value != b.Value || a.DecimalIndex != b.DecimalIndex);
        }

        public static BigDecimal operator ++(BigDecimal a)
        {
            return new BigDecimal(a.Value + 1, a.DecimalIndex);
        }

        public static BigDecimal operator --(BigDecimal a)
        {
            return new BigDecimal(a.Value - 1, a.DecimalIndex);
        }

        #endregion operators

        #region helper methods

        private static BigInteger ParseToBigInteger(string str)
        {
            var result = new BigInteger(0);

            var negativeFlag = false;
            if (str.StartsWith("-"))
            {
                negativeFlag = true;
                str = str.Remove(0, 1);
            }

            var pos = 0;
            for (var i = str.Length - 1; i >= 0; i--)
            {
                BigInteger tmp = Convert.ToInt32(str.Substring(i, 1));
                if (tmp != 0)
                {
                    for (var j = 0; j < pos; j++)
                    {
                        tmp = BigInteger.Multiply(tmp, 10);
                    }
                
                    result += tmp;
                }
                pos++;
            }

            if (negativeFlag)
            {
                result *= -1;
            }

            return result;
        }

        private static BigDecimal NormalizeDecimal(BigDecimal a, int decimalLength)
        {
            var currentLength = a.DecimalIndex;
            var delta = decimalLength - currentLength;

            var result = a.Value;

            for (var i = 0; i < delta; i++)
            {
                result = BigInteger.Multiply(result, 10);
            }

            return new BigDecimal(result, decimalLength);
        }

        #endregion helper methods

        #region static methods

        public static BigDecimal Add(BigDecimal a, BigDecimal b)
        {
            int indexLength;
            if (a.DecimalIndex < b.DecimalIndex)
            {
                indexLength = b.DecimalIndex;
                a = NormalizeDecimal(a, b.DecimalIndex);
            }
            else
            {
                indexLength = a.DecimalIndex;
                b = NormalizeDecimal(b, a.DecimalIndex);
            }

            return new BigDecimal(a.Value + b.Value, indexLength);
        }

        public static BigDecimal Sub(BigDecimal a, BigDecimal b)
        {
            var b1 = new BigDecimal(-b.Value, b.DecimalIndex);

            return Add(a, b1);
        }

        public static BigDecimal Mul(BigDecimal a, BigDecimal b)
        {
            return new BigDecimal(a.Value * b.Value, a.DecimalIndex + b.DecimalIndex);
        }

        /// <summary>
        /// 非常に計算コストが重いので乗法の使用を推奨
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static BigDecimal Div(BigDecimal a, BigDecimal b)
        {
            int indexLength;
            if (a.DecimalIndex < b.DecimalIndex)
            {
                indexLength = b.DecimalIndex;
                a = NormalizeDecimal(a, b.DecimalIndex);
            }
            else
            {
                indexLength = a.DecimalIndex;
                b = NormalizeDecimal(b, a.DecimalIndex);
            }

            var mod = a.Value; 
            var i = new BigInteger(0);

            while (mod - b.Value >= 0)
            {
                mod = mod - b.Value;
                i++;
            }

            var d = "";
            var index = 0;
            mod = BigInteger.Multiply(mod, 10);
            while(index < dSize)
            {
                var k = 0;
                if (mod == 0)
                {
                    break;
                }
                if (mod > b.Value)
                {
                    
                    while (mod - b.Value > 0)
                    {
                        mod = mod - b.Value;
                        k++;
                    }
                }
                d += k.ToString();
                mod = BigInteger.Multiply(mod, 10);
                index++;
            }

            return new BigDecimal(ParseToBigInteger(i.ToString() + d.ToString()), index);
        }

        #endregion static methods
    }
}
