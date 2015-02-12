using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpLib
{
    public class Tree<T> where T : class
    {
        #region Свойства

        public T Value { get; set; }

        public Tree<T> Parent { get; private set; }

        public Tree<T> Root
        {
            get
            {
                Tree<T> curr = this;

                while (curr != null)
                {
                    if (curr.Parent == null)
                    {
                        return curr;
                    }
                    curr = curr.Parent;
                }

                return null;
            }
        }

        public List<Tree<T>> Childs { get; private set; }

        public Boolean HasChilds
        {
            get { return (Childs.Count > 0); }
        }

        #endregion

        #region Конструктор

        public Tree()
            : this(null, null)
        {
        }

        public Tree(T value)
            : this(value, null)
        {
        }

        public Tree(T value, Tree<T> parent)
        {
            Value = value;
            Parent = parent;
            Childs = new List<Tree<T>>();
        }

        #endregion

        #region Методы

        private Tree<T> SearchRecurce(Tree<T> parent, T value)
        {
            foreach (Tree<T> item in parent.Childs)
            {
                if (item.Value == value)
                {
                    return item;
                }

                if (item.HasChilds)
                {
                    Tree<T> result = SearchRecurce(item, value);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private void ToListRecurce(Tree<T> parent, List<T> list)
        {
            foreach (Tree<T> item in parent.Childs)
            {
                list.Add(item.Value);

                if (item.HasChilds)
                {
                    ToListRecurce(item, list);
                }
            }
        }

        public Tree<T> AddChild(T value)
        {
            Tree<T> tree = new Tree<T>(value, this);
            Childs.Add(tree);

            return tree;
        }

        public void RemoveChild(T value)
        {
            foreach (Tree<T> child in Childs)
            {
                if (child.Value == value)
                {
                    Childs.Remove(child);
                    return;
                }
            }
        }

        public Tree<T> Search(T value)
        {
            Tree<T> root = Root;
            Tree<T> result = SearchRecurce(root, value);

            return result;
        }

        public Tree<T> SearchParent(T value)
        {
            Tree<T> result = Search(value);

            return result.Parent;
        }

        public void Clear()
        {
            Childs.Clear();
            Parent = null;
            Value = null;
        }

        public List<T> ToList()
        {
            List<T> list = new List<T>();

            ToListRecurce(Root, list);

            return list;
        }

        private void PrintRecourse(Tree<T> root, int level)
        {
            if (root != null)
            {
                String spaces = "".ExpandRightEx(level * 2);
                String result = String.Format("{0}{1}", spaces, root.Value);
                Debug.WriteLine(result);

                foreach (Tree<T> child in root.Childs)
                {
                    PrintRecourse(child, level + 1);
                }
            }
        }

        public void Print(String caption = "\r\n=================== Print Tree ===================")
        {
            Debug.WriteLine(caption);

            PrintRecourse(this, 0);
        }

        #endregion
    }
}