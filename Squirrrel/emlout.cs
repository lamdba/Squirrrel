using System;
using System.Collections.Generic;
using System.IO;

namespace Squirrrel
{

    abstract partial class Node     // 上下左右不是显示属性，就是模型的信息
    {
        abstract public void WriteEML(StreamWriter sw);
    }


    partial class N1 : Node
    {
        public override void WriteEML(StreamWriter sw)
        {
            sw.Write(string.Format("<s>\"{0}'", content));
        }
    }

    partial class N2 : Node
    {
        public override void WriteEML(StreamWriter sw)
        {
            sw.Write(string.Format("<{0}>(", typename));
            foreach (Node node in childrens)
            {
                node.WriteEML(sw);
            }
            sw.Write(")");
        }
    }

    partial class N3 : Node
    {
        public override void WriteEML(StreamWriter sw)
        {
            sw.Write(string.Format("<{0}>[", typename));
            foreach (Node node in childrens)
            {
                node.WriteEML(sw);
            }
            sw.Write("]");
        }
    }


    partial class NN : Node
    {
        public override void WriteEML(StreamWriter sw)
        {
            sw.Write("<_>_");
        }
    }


}
