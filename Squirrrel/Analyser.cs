using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squirrrel
{
    class Analyser 
    {
        public static Node Analyse(string code)     // to_structure
        {
            Stack<StackRecord> stack = new Stack<StackRecord>();
            char flag = ' ';
            StringBuilder temptypename = new StringBuilder(7);     // 暂存一下类型名，这样创建对象时就有确定的类型名了
            foreach (char ch in code)
            {
                //Console.WriteLine("before the switch, ch:{0}, flag:{1}", ch, flag);
                switch (ch)
                {
                    case '<':
                    {
                        if (!" \"[(".Contains(flag))
                            throw new Exception(flag.ToString());
                        stack.Push(new StackRecord());
                        temptypename.Clear();
                        flag = '<';
                        break;
                    }
                    case '>':
                    {
                        if (flag != '<')
                            throw new Exception();
                        flag = '>';
                        break;
                    }
                    case '"':
                    {
                        if (flag != '>')
                            throw new Exception();
                        stack.Peek().obj = new MyAtom(temptypename.ToString());
                        flag = '"';
                        stack.Peek().reg_state = flag;
                        break;
                    }
                    case '(':
                    {
                        if (flag != '>')
                            throw new Exception();
                        stack.Peek().obj = new MyTuple(temptypename.ToString());
                        flag = '(';
                        stack.Peek().reg_state = flag;
                        break;
                    }
                    case '[':
                    {
                        if (flag != '>')
                            throw new Exception();
                        stack.Peek().obj = new MyList(temptypename.ToString());
                        flag = '[';
                        stack.Peek().reg_state = flag;
                        break;
                    }
                    case '\'':              // 前引号用" 后引号用'
                    {
                        StackRecord stackrecord = stack.Pop();
                        flag = stackrecord.reg_state;
                        Obj obj = stackrecord.obj;
                        if (flag != '"')
                            throw new Exception();
                        //Console.WriteLine("here {0}",stack.Count);
                        if (stack.Count == 0)
                            return obj.CreateNode();            // 读取完第一个对象，其后的任何东西无论何物全部被忽略
                        else
                            stack.Peek().obj.AddObj(obj);
                        break;
                    }
                    case ')':
                    {
                        StackRecord stackrecord = stack.Pop();
                        flag = stackrecord.reg_state;
                        Obj obj = stackrecord.obj;
                        if (flag != '(')
                            throw new Exception();
                        if (stack.Count == 0)
                            return obj.CreateNode();
                        else
                            stack.Peek().obj.AddObj(obj);
                        break;
                    }
                    case ']':
                    {
                        StackRecord stackrecord = stack.Pop();
                        flag = stackrecord.reg_state;
                        Obj obj = stackrecord.obj;
                        if (flag != '[')
                            throw new Exception();
                        if (stack.Count == 0)
                            return obj.CreateNode();
                        else
                            stack.Peek().obj.AddObj(obj);
                        break;
                    }
                    case (' '):
                    case ('\n'):
                    case ('\r'):
                    case ('\t'):
                    break;
                    default:
                    {
                        switch (flag)
                        {
                            case '<':
                            {
                                temptypename.Append(ch);
                                break;
                            }

                            case '"':
                            {
                                stack.Peek().obj.AddChar(ch);
                                break;
                            }
                            default:
                            throw new Exception();
                        }
                        // print(ch, flag)        # debug用
                        break;
                    }
                }
                //Console.WriteLine("after the switch, ch:{0}, flag:{1}",ch, flag);
            }
            throw new Exception();
        }



        // python 代码
        //        class list2(list):
        //    def __repr__(self):
        //        return repr(tuple(self))


        //class Obj :
        //    def __init__(self):
        //        self.head = ""
        //        self.unit = None
        //
        //    def __repr__(self):
        //        return '<{0}>{1}'.format(self.head, repr(self.unit))

        // 该方案下字符串中的空格也将被无视

        interface IObj
        {
            void AddChar(char ch);
            void AddObj(Obj obj);
            Node CreateNode();
        }

        abstract class Obj : IObj       // Obj类包装Node，仅使用AddChar和AddObj操作
        {
            abstract public void AddChar(char ch);
            abstract public void AddObj(Obj obj);
            abstract public Node CreateNode();
        }

        class MyAtom : Obj
        {
            StringBuilder content = new StringBuilder(7);
            N1 n1;

            public MyAtom(string thehead)
            {
                n1 = new N1(thehead);
            }

            override public void AddChar(char ch)
            {
                content.Append(ch);
            }

            override public void AddObj(Obj obj) { }

            public override Node CreateNode()
            {
                n1.ChangeMes(content.ToString());
                return n1;
            }
        }

        class MyTuple : Obj
        {
            N2 n2;

            public MyTuple(string thehead)
            {
                n2 = new N2(thehead);
            }

            override public void AddChar(char ch) { }

            override public void AddObj(Obj obj)
            {
                n2.Add(obj.CreateNode());
            }

            override public Node CreateNode()
            {
                return n2;
            }
        }

        class MyList : Obj
        {
            N3 n3;

            public MyList(string thehead)
            {
                n3 = new N3(thehead);
            }

            override public void AddChar(char ch) { }

            override public void AddObj(Obj obj)
            {
                n3.Add(obj.CreateNode());
            }

            override public Node CreateNode()
            {
                return n3;
            }
        }


        class StackRecord
        {
            public char reg_state;      // 都是等待填入（仅一次），如果不信任其外的任何代码，则此处要复杂化为一次写模式
            public Obj obj;
        }

    }
}