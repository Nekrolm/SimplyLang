using System;
using ThreeAddr;

// Считается, что перед этой оптимизацией выполнена обработка констант.
namespace SimpleLang.Optimizations
{
    public class AlgebraIdentity : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            bool answer = false; // Индикатор того, что хоть один раз, но оптимизация была выполнена.
            foreach (var line in bblock.Code) // Проход по всему базовому блоку.
            {
                if (ThreeAddrOpType.Computable.Contains(line.OpType))
                {
                    answer |= Recognize(line); // Выполняем оптимизацию
                }
            }
            return answer;
        }
		private bool Recognize(ThreeAddrLine line) // метод получает строку трехадресного кода, конвертирует операнты и записывает результат.
		{
			int a = 0, b = 0;
			bool isaconst = int.TryParse(line.LeftOp, out a); // Получаем левый оперант.
			bool isbconst = int.TryParse(line.RightOp, out b);

			if (line.LeftOp == null)
			{
				isaconst = true;
				a = 0;
			}
			string res=null;
			if (!isaconst && !isbconst && (line.LeftOp != line.RightOp)) return false; // обе переменны, которые не равны друг другу, то не наш случай

            // Конвертируем операнды и считаем результат
            if (!isaconst && !isbconst) { res = ComputeBothVar(line.OpType); }
                //if (line.OpType=="-") res="0"; }
			if (isaconst && (a == 1 || a == 0)) { res = ComputeVarRigth(a, line.RightOp, line.OpType); }
			if (isbconst && (b == 1 || b == 0)) { res = ComputeVarLeft(line.LeftOp, b, line.OpType);  }

            if (res != null)
            {
                line.LeftOp = null; // Просто зануляем.
                line.OpType = ThreeAddrOpType.Assign; // записываем в тип операции assign.
                line.RightOp = res;
                return true;
            }
            return false;
        }
        private string ComputeVarLeft(string MyVar, int MyConst, string OpType) // Метод в зависимости от операции выполняет вычисление и возвращает значение.
        {
            switch (OpType)
            {
                case "+": if (MyConst == 0) return MyVar; else return null;
                case "-": if (MyConst == 0) return MyVar; else return null;
                case "*": if (MyConst == 0) return "0"; else return MyVar;
                case "/": if (MyConst == 1) return MyVar; else return null;
                case ThreeAddrOpType.And:
					if (MyConst == 0) return "0"; else return null;
                case ThreeAddrOpType.Or:
                    if (MyConst == 1) return "1"; else return null;
				default:
                    return null; 
            }
        }
		private string ComputeVarRigth(int MyConst, string MyVar, string OpType) // Метод в зависимости от операции выполняет вычисление и возвращает значение.
		{
			switch (OpType)
			{
				case "+": if (MyConst == 0) return MyVar; else return null;
				case "*": if (MyConst == 0) return "0"; else return MyVar;
				case ThreeAddrOpType.And:
					if (MyConst == 0) return "0"; else return null;
				case ThreeAddrOpType.Or:
					if (MyConst == 1) return "1"; else return null;
				default:
					return null;
			}
		}
        private string ComputeBothVar(string OpType) // Метод в зависимости от операции выполняет вычисление и возвращает значение.
        {
            switch (OpType)
            {
                case "-": return "0";
                case "<": return "0"; 
                case ">": return "0";
                case "<=": return "1";
                case ">=": return "1";
                case "==": return "1";
                case "!=": return "1";
                default:
                    return null;
            }
        }
    }
}