using System;
using System.Text;
using System.Collections.Generic;

// “绝对不要在继承中覆盖任何东西”

namespace Squirrrel
{
    class TreePos   // 如果使用struct，则 == null 式判断失效（可空判断又不会）
    {
        public Node parent;
        public int position;
        public TreePos (Node theparent, int theposition)
        {
            parent = theparent;
            position = theposition;
        }

    }

    enum TypeFlagNode
    {
        N1,
        N2, 
        N3,
        NN
    }

    abstract partial class Node     // 上下左右不是显示属性，就是模型的信息
    {
        public TypeFlagNode typeflag;
        public TreePos parentandposition;  // 可null // 固定托管范式。无法用C#完美描述
        public List<Node> childrens; // 可null    // protected的语义是，子类的新加部分（相对于重写部分）相当于非子类，不能访问继承来的成员
        public List<Node> GetSiblings() { return (parentandposition == null) ? null : parentandposition.parent.childrens; } // 是引用

        public Node()
        {
            parentandposition = null;
            childrens = null;
        }

        public Node GetBefore()
        {
            if (parentandposition == null)
                return null;
            else
            {
                int n = parentandposition.position - 1;
                List<Node> children = parentandposition.parent.childrens;
                // 根据程序表意，children 必定不为空，不需检测
                if ((0 <= n) && (n < children.Count))
                    return children[n];
                else
                    return null;    // 数组出界 // 即使考虑可空也难处理出界啊……
            }
        }

        public Node GetAfter()
        {
            if (parentandposition == null)
                return null;
            else
            {
                int n = parentandposition.position + 1;
                List<Node> children = parentandposition.parent.childrens;
                if ((0 <= n) && (n < children.Count))
                    return children[n];
                else
                    return null;
            }
        }

        public Node GetParent()
        {
            if (parentandposition == null)
                return null;
            else
                return parentandposition.parent;
        }

        public Node GetChild()
        {
            if (childrens == null)
                return null;
            else
            {
                if (childrens.Count > 0)
                    return childrens[0];
                else
                    return null;
            }
        }

        public void Lethimtakeyourplace(Node newnode)
        {
            if (parentandposition == null)
            {
            }
            else
            {
                Node parent = parentandposition.parent;
                newnode.parentandposition = parentandposition;
                int position = parentandposition.position;
                parent.childrens[position] = newnode;
            }
        }

        abstract public void ChangeMes(string s);

        
    }

    partial class N1 : Node
    {
        string typename;  //
        string content = "";

        public N1(string thetypename):base()
        {
            typename = thetypename;
            typeflag = TypeFlagNode.N1;
        }

        public override void ChangeMes(string s)
        {
            this.content = s;
        }

        public N1 AddContent(string s)
        {
            content = s;
            return this;
        }

    }

    partial class N2 : Node
    {
        string typename;
        public N2(string typename):base()
        {
            this.typename = typename;
            this.childrens = new List<Node>();
            typeflag = TypeFlagNode.N2;
        }

        public void Add(Node node)
        {
            node.parentandposition = new TreePos(this, childrens.Count);
            childrens.Add(node);
        }

        public void Fillone(int pos, Node node)
        {
            childrens[pos] = node;
        }

        public override void ChangeMes(string s)
        {
            this.typename = s;
        }
    }

    partial class N3 : Node
    {
        string typename;

        public N3(string typename) : base()
        {
            this.typename = typename;
            this.childrens = new List<Node>();
            typeflag = TypeFlagNode.N3;
        }

        public void Add(Node node)
        {
            node.parentandposition = new TreePos(this, childrens.Count);
            childrens.Add(node);
        }

        public override void ChangeMes(string s)
        {
            this.typename = s;
        }
    }

    partial class NN : Node
    {
        public NN()
        {
            typeflag = TypeFlagNode.NN;
        }

        public override void ChangeMes(string s)
        {
            
        }
    }


}
