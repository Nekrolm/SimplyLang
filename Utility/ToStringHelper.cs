using ProgramTree;

namespace SimpleLang.Utility
{
    public static class ToStringHelper
    {
        public static string ToString(BinaryOpType binaryOpType)
        {
            switch (binaryOpType)
            {
                case BinaryOpType.Plus:
                    return "+";
                case BinaryOpType.Minus:
                    return "-";
                case BinaryOpType.Divides:
                    return "/";
                case BinaryOpType.Multiplies:
                    return "*";
                case BinaryOpType.Less:
                    return "<";
                case BinaryOpType.Greater:
                    return ">";
                case BinaryOpType.Equals:
                    return "==";
                case BinaryOpType.UnEquals:
                    return "!=";
                case BinaryOpType.LessOrEquals:
                    return "<=";
                case BinaryOpType.GreaterOrEquals:
                    return ">=";
                case BinaryOpType.And:
                    return "and";
                case BinaryOpType.Or:
                    return "or";
                case BinaryOpType.Not:
                    return "not";
                default:
                    return "";
            }
        }
    }
}