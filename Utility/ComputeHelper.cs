using System;
using ThreeAddr;
namespace SimpleLang.Utility
{
    public static class ComputeHelper
    {
        public static bool IsConst(String v)
        {
            return int.TryParse(v, out var b);
        }

        public static int? Calculate(int a, int b, string OpType) // Метод в зависимости от операции выполняет вычисление и возвращает значение.
        {
            switch (OpType)
            {
                case "+": return a + b; // Если найден плюс: вернётся сумма.
                case "-": return a - b; // Если минус: то разность.
                case "*": return a * b;
                case "/": return a / b;
                case "<": if (a < b) return 1; else return 0; // Если a меньше b, верн\тся 1: иначе 0.
                case ">": if (a > b) return 1; else return 0;
                case "<=": if (a <= b) return 1; else return 0; // Если a меньше b, верн\тся 1: иначе 0.
                case ">=": if (a >= b) return 1; else return 0;

                case ThreeAddrOpType.Eq:
                    if (a == b) return 1; else return 0;
                case ThreeAddrOpType.UnEq:
                    if (a != b) return 1; else return 0;
                case ThreeAddrOpType.Not:
                    if (b == 0) return 1; else return 0;
                case ThreeAddrOpType.And:
                    if (Convert.ToBoolean(a) && Convert.ToBoolean(b)) return 1; else return 0;
                case ThreeAddrOpType.Or:
                    if (Convert.ToBoolean(a) || Convert.ToBoolean(b)) return 1; else return 0;
                default:
                    return null; // В остальных случаях нельзя вычислить
            }
        }


    }
}
