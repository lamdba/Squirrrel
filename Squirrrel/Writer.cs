using System;

using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

namespace Squirrrel
{
    struct BF
    {
        public int n;  // 通过颜色变化区分每一段，这个n是该段结束处的坐标值
        public ConsoleColor bcolor;
        public ConsoleColor fcolor;

        public BF(int n_, ConsoleColor bcolor_, ConsoleColor fcolor_)
        {
            n = n_;
            bcolor = bcolor_;
            fcolor = fcolor_;
        }
    }

    static class Writer
    {
        // “显存”
        static char[] chars = new char[80 * 40];
        static List<BF> bfs = new List<BF>(40);     

        //static int x;
        //static int y;
        //static int width;
        //static int height;
        public static int px = 0;  // 当前列，允许超限
        public static int py = 0;  // 当前行，允许超限

        static Writer()
        {
            
            bfs.Add(new BF(80 * 40, ConsoleColor.Black, ConsoleColor.Gray));
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = ' ';
            }
        }

        static public void Write(string s)  // 并不会改变px和py
        {
            ColorWrite(s, ConsoleColor.Gray, ConsoleColor.Black);

        }

        static public void NextLine()
        {
            py++;
            px = 0;
        }

        static public void Movex(int n)
        {
            px = n;
        }

        static public void Jumpy(int n)
        {
            py = n;
        }

        static public void GrayWrite(string s)
        {
            ColorWrite(s, ConsoleColor.Black, ConsoleColor.Gray);
        }

        static public void DarkGrayWrite(string s)
        {
            ColorWrite(s, ConsoleColor.Black, ConsoleColor.DarkGray);
        }

        static public void WhiteWrite(string s)
        {
            ColorWrite(s, ConsoleColor.Black, ConsoleColor.White);
        }
        
        static public void ColorWrite(string s, ConsoleColor fc, ConsoleColor bc)
        {
            if (0 <= py && py < 40 && 0 <= px && px < 80)
            {
                int a = py * 80 + px;
                int len = Math.Min(80 - px, s.Length);
                int b = a + len;
                s.CopyTo(0, chars, a, len);

                int pos1 = -1;
                int pos2 = -1;
                bool flag = true;
                int i = 0;
                foreach (BF bf in bfs)       // 算了，遍历查找吧
                {
                    if (flag && bf.n >= a)// 找a
                    {
                        pos1 = i;
                        flag = false;
                    }

                    if (bf.n >= b)  // 找b
                    {
                        pos2 = i;
                        break;
                    }
                    i++;
                }

                //Console.WriteLine("a b {0} {1}", a, b);
                //Console.WriteLine("pos1 pos2 {0} {1}", pos1, pos2);


                BF newbf = new BF(a, bfs[pos1].bcolor, bfs[pos1].fcolor);
                bfs.Insert(pos1, newbf);
                pos2++;
                bfs.Insert(pos2, new BF(b, bc, fc));
                bfs.RemoveRange(pos1, pos2 - 1 - pos1);

                if (bfs[pos1].bcolor == bfs[pos1+1].bcolor && bfs[pos1].fcolor == bfs[pos1+1].fcolor)
                {
                    bfs.RemoveAt(pos1);
                    pos1--;
                }
                if (bfs[pos1+2].bcolor == bfs[pos1 + 1].bcolor && bfs[pos1+2].fcolor == bfs[pos1 + 1].fcolor)
                {
                    bfs.RemoveAt(pos1+1);
                }

                //foreach (BF _ in bfs)
                //{
                //    Console.Write("{0} ", _.n);
                //}
                //Console.WriteLine();


            }
        }

        static public void Flip()
        {


            string temp = new string(chars);
            Console.Clear();
            
            int lastn = 0;
            foreach (BF bf in bfs)
            {
                Console.BackgroundColor = bf.bcolor;
                Console.ForegroundColor = bf.fcolor;
                Console.Write(temp.Substring(lastn, bf.n-lastn));
                if (lastn > bf.n) { throw new Exception(string.Format("bfs的n不成序 {0} !>= {1}", lastn, bf.n)); }
                lastn = bf.n;
            }
            
            px = 0;
            py = 0;

            bfs.Clear();
            bfs.Add(new BF(80 * 40, ConsoleColor.Black, ConsoleColor.Gray));
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = ' ';
            }



        }
    }
}
