using System;
namespace ThreeAddr
{
    public struct ThreeAddrLine
    {
        public string Label { get; set; }
        public string SrcDst { get; set; }
        public string LeftOp { get; set; }
        public string RightOp { get; set; }
        public string OpType { get; set; }
    }
}
