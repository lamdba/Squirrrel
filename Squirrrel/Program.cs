using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



// Black
// DarkBlue.
// DarkGreen.
// DarkCyan.
// DarkRed.
// DarkMagenta.
// DarkYellow.
// Gray.
// DarkGray.
// Blue.
// Green.
// Cyan.
// Red.
// Magenta.
// Yellow.
// White.

namespace Squirrrel
{

    enum MODE
    {
        EXPLORE, INPUT, CHOOSE
    }

    interface IMode
    {
        void ScreenUpdate();
        void KeyMes(ConsoleKeyInfo cki);
    }

    interface Externs1
    {
        Node Mainnode { get; set; }
        void ChangeMode(MODE mode);
        //Node cursor;
    }   // 限制explore访问maincontroller

    interface Externs2    // 限制input访问explore
    {
        void InputString(string s);
    }

    interface Externs3 // 限制choose访问explore
    {
        void ChooseNode(Node node);
    }

    class ModeExplore: IMode, Externs2, Externs3
    {
        Externs1 externs;
        
        
        Node cursor = null;

        public ModeExplore(Externs1 externs1)      // writer 依然静态
        {
            externs = externs1;
            cursor = externs.Mainnode;
            cursor.Highlight = 1;
        }

        public void ScreenUpdate()
        {
            externs.Mainnode.Show(0);   // 这tm怎么搞……突出显示
            Writer.Flip();
        }

        public void KeyMes(ConsoleKeyInfo cki)
        {
            switch (cki.Key)
            {
                case ConsoleKey.W:
                {
                    //MoveToBefore()
                    Node node = cursor;
                    Node newnode = node.GetBefore();
                    if (newnode != null)
                    {
                        newnode.Highlight = 1;
                        node.Highlight = 2;
                        cursor = newnode;
                    }

                    break;
                }
                case ConsoleKey.S:
                {
                    //MoveToAfter()
                    Node node = cursor;
                    Node newnode = node.GetAfter();
                    if (newnode != null)
                    {
                        newnode.Highlight = 1;
                        node.Highlight = 2;
                        cursor = newnode;
                    }

                    break;
                }
                case ConsoleKey.Q:
                {
                    //MoveToParent()
                    Node node = cursor;
                    Node newnode = cursor.GetParent();
                    if (newnode != null)
                    {
                        foreach (Node sibling in node.GetSiblings())        // public //一定非null
                        {
                            sibling.Highlight = 0;
                        }

                        List<Node> siblings = newnode.GetSiblings();
                        if (siblings == null)
                            newnode.Highlight = 1;
                        else
                        {
                            foreach (Node sibling in newnode.GetSiblings())
                            {
                                sibling.Highlight = 2;
                            }
                            newnode.Highlight = 1;
                        }


                        cursor = newnode;
                    }

                    break;
                }
                case ConsoleKey.D:
                {
                    //MoveToChild()
                    Node node = cursor;
                    Node newnode = node.GetChild();
                    if (newnode != null)
                    {
                        List<Node> siblings = node.GetSiblings();
                        if (siblings == null)
                            node.Highlight = 0;
                        else
                        {
                            foreach (Node sibling in node.GetSiblings())        // public //一定非null
                            {
                                sibling.Highlight = 0;
                            }
                        }

                        foreach (Node sibling in newnode.GetSiblings())
                        {
                            sibling.Highlight = 2;
                        }
                        newnode.Highlight = 1;

                        cursor = newnode;
                    }

                    break;
                }
                case ConsoleKey.J:
                {
                    if (cursor.typeflag == TypeFlagNode.N1)
                    {
                        externs.ChangeMode(MODE.INPUT);     // 现在写入权限不在input而在explore，则input结束后explore需要处理input得到的内容（Node.ChangeMes()）
                    }
                    else if (cursor.typeflag == TypeFlagNode.NN)
                    {
                        externs.ChangeMode(MODE.CHOOSE);
                    }
                    break;
                }
                case ConsoleKey.K:
                {
                    //AddAtom()
                    if (cursor.childrens != null)
                    {
                        NN node = new NN();
                        node.parentandposition = new TreePos(cursor, cursor.childrens.Count);
                        cursor.childrens.Add(node);
                    }

                    break;
                }
                case ConsoleKey.L:
                {
                    if (cursor.parentandposition != null)
                    {
                        List<Node> list = cursor.GetSiblings();
                        list.RemoveAt(cursor.parentandposition.position);
                        for (int i = cursor.parentandposition.position; i < list.Count; i++)
                        {
                            list[i].parentandposition.position--;
                        }
                        cursor = cursor.GetParent();
                        cursor.Highlight = 1;
                    }

                    break;
                }
                case ConsoleKey.NumPad2:
                {
                    //Save()
                    FileStream stream = new FileStream("F:\\squirrrel-workspace\\default.txt", FileMode.Create, FileAccess.Write);
                    StreamWriter streamwriter = new StreamWriter(stream, Encoding.Default);
                    externs.Mainnode.WriteEML(streamwriter);
                    streamwriter.Close();

                    break;
                }
                case ConsoleKey.NumPad1:
                {
                    //Open()
                    FileStream stream = new FileStream("F:\\squirrrel-workspace\\default.txt", FileMode.Open, FileAccess.Read);
                    StreamReader streamreader = new StreamReader(stream, Encoding.Default);
                    string s = streamreader.ReadToEnd();
                    externs.Mainnode = Analyser.Analyse(s);
                    cursor = externs.Mainnode;
                    cursor.Highlight = 1;   // 这个逻辑显然不怎么好
                    streamreader.Close();

                    break;
                }


                default:
                break;
            }
        }

        public void InputString(string s)  // callback!
        {
            cursor.ChangeMes(s);
            externs.ChangeMode(MODE.EXPLORE);
        }

        public void ChooseNode(Node node)
        {
            cursor.Lethimtakeyourplace(node);
            cursor = node;
            cursor.Highlight = 1;
            externs.ChangeMode(MODE.EXPLORE);
        }

    }

    class ModeInput : IMode
    {
        Externs2 externs;
        StringBuilder LineInput = null;

        public ModeInput(Externs2 externs2) 
        {
            externs = externs2;
            LineInput = new StringBuilder("");
            
        }


        public void ScreenUpdate()
        {

            ShowLineInput();
            Writer.Flip();
        }

        public void KeyMes(ConsoleKeyInfo cki)
        {
            if (cki.Key == ConsoleKey.Enter)
            {
                string s = LineInput.ToString();
                LineInput.Clear();
                externs.InputString(s);
            }
            else if (cki.Key == ConsoleKey.Backspace && LineInput.Length > 0)
                LineInput.Remove(LineInput.Length - 1, 1);
            else
            {
                LineInput.Append(cki.KeyChar);
            }
        }

        void ShowLineInput()
        {
            Writer.Jumpy(39);
            Writer.WhiteWrite("                                                                                ");
            Writer.WhiteWrite(LineInput.ToString());
        }

        
    }

    class ModeChoose : IMode
    {
        Externs3 externs;

        Tagpack pack;
        
        public ModeChoose(Externs3 externs3) 
        {
            externs = externs3;
            pack = new Tagpack();
            pack.AddTag("str", TypeFlagNode.N1);
            pack.AddTag("tuple", TypeFlagNode.N2);
            pack.AddTag("list", TypeFlagNode.N3);
            pack.AddTag("none(no change)", TypeFlagNode.NN);
            pack.AddTag("def", TypeFlagNode.N2);
            pack.AddTag("let", TypeFlagNode.N2);
            pack.AddTag("return", TypeFlagNode.N2);
            pack.AddTag("sign", TypeFlagNode.N1);

        }

       

        public void ScreenUpdate()
        {
            
            ShowChooseInput();
            Writer.Flip();
        }

        public void KeyMes(ConsoleKeyInfo cki)
        {
            //Node node;

            //if (cki.Key == ConsoleKey.Enter)
            //{
            //    externs.ChooseNode(new N1(""));
            //}
            //if (cki.Key == ConsoleKey.OemPlus)
            //else if (cki.Key == ConsoleKey.OemMinus)

            int n = 0;
            switch (cki.Key)
            {
                case ConsoleKey.D1: { n = 0; break; }
                case ConsoleKey.D2: { n = 1; break; }
                case ConsoleKey.D3: { n = 2; break; }
                case ConsoleKey.D4: { n = 3; break; }
                case ConsoleKey.D5: { n = 4; break; }
                case ConsoleKey.D6: { n = 5; break; }
                case ConsoleKey.D7: { n = 6; break; }
                case ConsoleKey.D8: { n = 7; break; }
                case ConsoleKey.D9: { n = 8; break; }
                default: return;
            }
            if (!(n < pack.tags.Count)) return;

            Tagpack.Record record = pack.tags[n];

            if (record.type == TypeFlagNode.N1)
            {
                externs.ChooseNode(new N1(record.name));
            }
            else if (record.type == TypeFlagNode.N2)
            {
                externs.ChooseNode(new N2(record.name));
            }
            else if (record.type == TypeFlagNode.N3)
            {
                externs.ChooseNode(new N3(record.name));
            }
            else if (record.type == TypeFlagNode.NN)
            {
                externs.ChooseNode(new NN());
            }
            else
            {
                externs.ChooseNode(new N1("what? bug"));
            }

        }

        void ShowChooseInput()
        {
            int startline = 35;
            
            for (int i = 0; i < 5; i++)
            {
                Writer.Jumpy(startline + i);
                Writer.GrayWrite("                                                                                ");
                if (i < pack.tags.Count)
                {
                    Writer.GrayWrite(String.Format("{0,-3}", (i + 1).ToString() + ".") + pack.tags[i].name);
                }
            }
            Writer.Movex(40);

            for (int i = 0; i < 5; i++)
            {
                int n = i + 5;
                Writer.Jumpy(startline + i);
                if (n < pack.tags.Count)
                {
                    Writer.GrayWrite(String.Format("{0,-3}", (n + 1).ToString() + ".") + pack.tags[n].name);
                }
            }

        }
    }

    class MainController: Externs1
    {
        ModeExplore modeexplore;
        ModeInput modeinput;
        ModeChoose modechoose;
        IMode mode;

        Node mainnode;

        public MainController()
        {

            
            mainnode = TreeMaker.MakeTree();// 数据结构（指向树的起始）

            modeexplore = new ModeExplore((Externs1)this);
            modeinput = new ModeInput((Externs2)modeexplore);
            modechoose = new ModeChoose((Externs3)modeexplore);
            mode = modeexplore;
        }

        public void Run()
        {
            while (true)
            {
                mode.ScreenUpdate();
                mode.KeyMes(Console.ReadKey(true));
            }
        }

        public Node Mainnode { get { return mainnode; } set { mainnode = value; } }

        public void ChangeMode(MODE newmode)
        {
            switch (newmode)
            {
                case MODE.EXPLORE: { mode = modeexplore; break; }
                case MODE.INPUT: { mode = modeinput; break; }
                case MODE.CHOOSE: { mode = modechoose; break; }
            }
        }

    }

    class Program   // 是静态的    ，主程序类，是一个控制器，总控器
    {
        const int WIDTH = 80;   // const是编译时常量，不需static
        const int HEIGHT = 41;

        public static void Main(string[] args)
        {
            
            Console.SetWindowSize(WIDTH, HEIGHT);
            Console.SetBufferSize(WIDTH, HEIGHT);
            Console.CursorVisible = false;

            MainController maincontroller = new MainController();
            maincontroller.Run();

        }

       

    }

    class TreeMaker
    {
        static public Node MakeTree()
        {

            //Analyser.Analyse("<define>(<s>\"\"<fargs>[<s>\"\"<s>\"\"]<func>[<let>(<s>\"\"<mul-operator>(<s>\"\"<s>\"\"))<let>(<s>\"\"<pow-operator>(<s>\"\"<s>\"\"))<return>(<add-operator>(<s>\"\"<s>\"\"))])");
            //Node _ = Analyser.Analyse("<s>\"hello'");
            //mainnode = _;

            N2 n2_, n2__;
            N3 n3_, n3__;

            N2 thenode = new N2("def");

            thenode.Add(new NN());
            thenode.Add(new N1("f"));
            n3_ = new N3("fargs");
            n3_.Add(new N1("sign").AddContent("x"));
            n3_.Add(new N1("sign").AddContent("y"));
            thenode.Add(n3_);

            n3_ = new N3("func-content");

            n2_ = new N2("let");
            n2_.Add(new N1("sign").AddContent("x_square"));
            n2__ = new N2("mul-operator");
            n2__.Add(new N1("sign").AddContent("x"));
            n2__.Add(new N1("sign").AddContent("x"));
            n2_.Add(n2__);
            n3_.Add(n2_);

            n2_ = new N2("let");
            n2_.Add(new N1("sign").AddContent("y_square"));
            n2__ = new N2("pow-operator");
            n2__.Add(new N1("sign").AddContent("y"));
            n2__.Add(new N1("sign").AddContent("2"));
            n2_.Add(n2__);
            n3_.Add(n2_);

            n2_ = new N2("return");
            n2__ = new N2("add-operator");
            n2__.Add(new N1("sign").AddContent("x_square"));
            n2__.Add(new N1("sign").AddContent("y_square"));
            n2_.Add(n2__);
            n3_.Add(n2_);

            thenode.Add(n3_);

            return thenode;
        }
    }

    

    class Tagpack
    {
        public struct Record
        {
            public string name;
            public TypeFlagNode type;
        }

        public List<Record> tags;

        public Tagpack()
        {
            tags = new List<Record>();
        }

        public void AddTag(string s, TypeFlagNode flag)
        {
            tags.Add(new Record { name = s, type = flag});
        }

    }
}



