using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squirrrel
{
    interface INodeComponent    // 在model.cs中没有使用本文件的任何功能，但本文件使用了model.cs中的功能。这是对Node系统的补充
    {
        void Show(int n);
        
    }

    abstract partial class Node : INodeComponent    // 接口下降到最终类，若是多了抽象类共用代码，就得一套override了
    {
        public int Highlight { get; set; }   // 不得不说，这些潜在的类型推理是编程之路上的大坑……（不要去分析它。） // 0无 1高亮 2半高亮

        abstract public void Show(int n); // 其实应该用渲染器类对象，但又想把代码分布到各对象

    }

    partial class N1      // 这里的继承问题在于，如果用修饰器模式，则对抽象类怎么修饰呢？
    {
        override public void Show(int n)
        {

            Writer.Movex(n * 4);
            string s = string.Format("<{0}>\"{1}\"", typename, content);
            if (Highlight == 1)
            {
                Writer.WhiteWrite(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.White);
            }
            else if (Highlight == 2)
            {
                Writer.DarkGrayWrite(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.DarkGray);
            }
            else
            {
                Writer.Write(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.Black);
            }
            Writer.NextLine();

        }

        
    }

    partial class N2
    {
        override public void Show(int n)
        {
            Writer.Movex(n * 4);

            string s = string.Format("<{0}>()", typename);
            if (Highlight == 1)
            {
                Writer.WhiteWrite(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.White);
            }
            else if (Highlight == 2)
            {
                Writer.DarkGrayWrite(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.DarkGray);
            }
            else
            {
                Writer.Write(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.Black);
            }
            Writer.NextLine();

            foreach (Node node in childrens)
            {
                node.Show(n + 1);
            }
        }

        
    }

    partial class N3
    {
        override public void Show(int n)
        {
            Writer.Movex(n * 4);

            string s = string.Format("<{0}>[]", typename);
            if (Highlight == 1)
            {
                Writer.WhiteWrite(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.White);
            }
            else if (Highlight == 2)
            {
                Writer.DarkGrayWrite(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.DarkGray);
            }
            else
            {
                Writer.Write(s);
                Writer.px++;
                Writer.ColorWrite(typename, ConsoleColor.Cyan, ConsoleColor.Black);
            }
            Writer.NextLine();

            foreach (Node node in childrens)
            {
                node.Show(n + 1);
            }

        }

        
    }

    partial class NN
    {
        override public void Show(int n)
        {

            //for (int i = 0; i < n; i++)
            //    Writer.Write("    ");
            Writer.Movex(n * 4);

            if (Highlight == 1)
                Writer.WhiteWrite("____");
            else if (Highlight == 2)
                Writer.DarkGrayWrite("____");
            else
                Writer.Write("____");
            Writer.NextLine();
        }

        


    }

    
}
