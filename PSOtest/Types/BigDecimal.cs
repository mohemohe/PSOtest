using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PSOtest.Types
{
    class BigDecimal
    {
        private const int dSize = 256;
        private const int prefixVal = 3;
        private string dPadding;
        private BigInteger I;
        private BigInteger D;

        private void InitializeBigDecimal()
        {
            dPadding = "#.";
            for (var i = 0; i < dSize; i++)
            {
                dPadding += "#";
            }
        }

        public BigDecimal(double a)
        {
            InitializeBigDecimal();

            this.I = ParseToBigInteger(Math.Truncate(a).ToString().Split('.')[0]);
            this.D = ParseToBigDecimalsDecimal((a % 1.0).ToString(dPadding).Split('.')[1]);
        }

        public BigDecimal(BigInteger a)
        {
            InitializeBigDecimal();

            I = a;
            D = ParseToBigDecimalsDecimal(0.ToString(dPadding).Split('.')[1]);
        }

        public BigDecimal(string a)
        {
            InitializeBigDecimal();

            var decimalStruct = a.Split('.');
            I = new BigInteger(Convert.ToInt64(decimalStruct[0]));
            D = NormalizeDecimal(ParseToBigDecimalsDecimal(decimalStruct[1]));
        }

        public new string ToString()
        {
            return  I.ToString() + "." + D.ToString().Substring(1);
        }

        private static BigInteger ParseToBigInteger(string str)
        {
            BigInteger bi = new BigInteger(0);

            var pos = 0;
            for (var i = str.Length - 1; i >= 0; i--)
            {
                BigInteger tmp = Convert.ToInt32(str.Substring(i, 1));
                for (var j = 0; j < pos; j++)
                {
                    tmp = BigInteger.Multiply(tmp, 10);
                }
                bi += tmp;
                pos++;
            }

            return bi;
        }

        private static BigInteger ParseToBigDecimalsDecimal(string str)
        {
            var tmp = prefixVal.ToString() + str;
            return ParseToBigInteger(tmp);
        }

        private static BigInteger NormalizeDecimal(BigInteger bi)
        {
            var tmp = bi.ToString();
            for (var i = tmp.Length; i <= dSize + 2; i++)
            {
                tmp += "0";
            }
            return ParseToBigInteger(tmp);
        }

        private static BigDecimal ChangeNegative(BigDecimal a)
        {
            var tmp = a;
            a.I = -a.I;

            return tmp;
        }

        public static BigDecimal Add(BigDecimal a, BigDecimal b)
        {
            if (b.I.Sign < 0)
            {
                return Sub(a, ChangeNegative(b));
            }

            var result = a;
            var add = b;
            add.D = ParseToBigInteger("1" + result.D.ToString().Substring(1));

            if (a.I.Sign < 0)
            {
                
                result.I -= b.I;
                result.D -= b.D;

                if (result.D.ToString().Substring(0, 1) != (prefixVal - 1).ToString())
                {
                    result.I++;
                    result.D = ParseToBigDecimalsDecimal(result.D.ToString().Substring(1));
                }
            }
            else
            {
                result.I += b.I;
                result.D += b.D;

                if (result.D.ToString().Substring(0, 1) != (prefixVal + 1).ToString())
                {
                    result.I++;
                    result.D = ParseToBigDecimalsDecimal(result.D.ToString().Substring(1));
                }
            }

            return result;
        }

        public static BigDecimal Sub(BigDecimal a, BigDecimal b)
        {
            if (b.I.Sign < 0)
            {
                return Add(a, ChangeNegative(b));
            }

            var result = a;
            var sub = b;
            sub.D = ParseToBigInteger("1" + result.D.ToString().Substring(1));

            if (a.I.Sign < 0)
            {
                result.I += sub.I;
                result.D += sub.D;

                if (result.D.ToString().Substring(0, 1) != (prefixVal + 1).ToString())
                {
                    result.I++;
                    result.D = ParseToBigDecimalsDecimal(result.D.ToString().Substring(1));
                }
            }
            else
            {
                result.I -= sub.I;
                result.D -= sub.D;

                if (result.D.ToString().Substring(0, 1) != (prefixVal - 1).ToString())
                {
                    result.I--;
                    result.D = ParseToBigDecimalsDecimal(result.D.ToString().Substring(1));
                }
            }

            return result;
        }
    }
}
