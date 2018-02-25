using System;

namespace ThreeAddr
{
    public class ThreeAddrLine
    {
        public string Label { get; set; }
        public string Accum { get; set; }
        public string LeftOp { get; set; }
        public string RightOp { get; set; }
        public string OpType { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Label) &&
                   string.IsNullOrEmpty(Accum) &&
                   string.IsNullOrEmpty(LeftOp) &&
                   string.IsNullOrEmpty(RightOp) &&
                   string.IsNullOrEmpty(OpType);
        }
    }
}