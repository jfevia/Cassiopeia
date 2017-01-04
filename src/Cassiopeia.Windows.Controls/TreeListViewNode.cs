using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Cassiopeia.Windows.Controls
{
    public sealed class TreeListViewNode : INotifyPropertyChanged
    {
        internal TreeListViewNode(TreeListView tree, object tag)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            Tree = tree;
            Children = new NodeCollection(this);
            Nodes = new ReadOnlyCollection<TreeListViewNode>(Children);
            Tag = tag;
        }

        public override string ToString()
        {
            if (Tag != null)
                return Tag.ToString();
            return base.ToString();
        }

        private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        var index = e.NewStartingIndex;
                        int rowIndex = Tree.Rows.IndexOf(this);
                        foreach (var obj in e.NewItems)
                        {
                            Tree.InsertNewNode(this, obj, rowIndex, index);
                            index++;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (Children.Count > e.OldStartingIndex)
                        RemoveChildAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    while (Children.Count > 0)
                        RemoveChildAt(0);
                    Tree.CreateChildrenNodes(this);
                    break;
            }
            HasChildren = Children.Count > 0;
            OnPropertyChanged("IsExpandable");
        }

        private void RemoveChildAt(int index)
        {
            var child = Children[index];
            Tree.DropChildrenRows(child, true);
            ClearChildrenSource(child);
            Children.RemoveAt(index);
        }

        private void ClearChildrenSource(TreeListViewNode node)
        {
            node.ChildrenSource = null;
            foreach (var n in node.Children)
                ClearChildrenSource(n);
        }

        #region NodeCollection

        private class NodeCollection : Collection<TreeListViewNode>
        {
            private readonly TreeListViewNode _owner;

            public NodeCollection(TreeListViewNode owner)
            {
                _owner = owner;
            }

            protected override void ClearItems()
            {
                while (Count != 0)
                    RemoveAt(Count - 1);
            }

            protected override void InsertItem(int index, TreeListViewNode item)
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item));

                if (item.Parent != _owner)
                {
                    item.Parent?.Children.Remove(item);
                    item.Parent = _owner;
                    item.Index = index;
                    for (var i = index; i < Count; i++)
                        this[i].Index++;
                    base.InsertItem(index, item);
                }
            }

            protected override void RemoveItem(int index)
            {
                var item = this[index];
                item.Parent = null;
                item.Index = -1;
                for (var i = index + 1; i < Count; i++)
                    this[i].Index--;
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, TreeListViewNode item)
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item));
                RemoveAt(index);
                InsertItem(index, item);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Properties

        internal TreeListView Tree { get; }

        private INotifyCollectionChanged _childrenSource;

        internal INotifyCollectionChanged ChildrenSource
        {
            get { return _childrenSource; }
            set
            {
                if (_childrenSource != null)
                    _childrenSource.CollectionChanged -= ChildrenChanged;

                _childrenSource = value;

                if (_childrenSource != null)
                    _childrenSource.CollectionChanged += ChildrenChanged;
            }
        }

        public int Index { get; private set; } = -1;

        /// <summary>
        ///     Returns true if all parent nodes of this node are expanded.
        /// </summary>
        internal bool IsVisible
        {
            get
            {
                var node = Parent;
                while (node != null)
                {
                    if (!node.IsExpanded)
                        return false;
                    node = node.Parent;
                }
                return true;
            }
        }

        public bool IsExpandedOnce { get; internal set; }

        public bool HasChildren { get; internal set; }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != IsExpanded)
                {
                    Tree.SetIsExpanded(this, value);
                    OnPropertyChanged("IsExpanded");
                    OnPropertyChanged("IsExpandable");
                }
            }
        }

        internal void AssignIsExpanded(bool value)
        {
            _isExpanded = value;
        }

        public bool IsExpandable
        {
            get { return HasChildren && !IsExpandedOnce || Nodes.Count > 0; }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }


        public TreeListViewNode Parent { get; private set; }

        public int Level
        {
            get
            {
                if (Parent == null)
                    return -1;
                return Parent.Level + 1;
            }
        }

        public TreeListViewNode PreviousNode
        {
            get
            {
                if (Parent != null)
                {
                    var index = Index;
                    if (index > 0)
                        return Parent.Nodes[index - 1];
                }
                return null;
            }
        }

        public TreeListViewNode NextNode
        {
            get
            {
                if (Parent != null)
                {
                    var index = Index;
                    if (index < Parent.Nodes.Count - 1)
                        return Parent.Nodes[index + 1];
                }
                return null;
            }
        }

        internal TreeListViewNode BottomNode
        {
            get
            {
                var parent = Parent;
                if (parent != null)
                    if (parent.NextNode != null)
                        return parent.NextNode;
                    else
                        return parent.BottomNode;
                return null;
            }
        }

        internal TreeListViewNode NextVisibleNode
        {
            get
            {
                if (IsExpanded && Nodes.Count > 0)
                    return Nodes[0];
                var nn = NextNode;
                return nn ?? BottomNode;
            }
        }

        public int VisibleChildrenCount
        {
            get { return AllVisibleChildren.Count(); }
        }

        public IEnumerable<TreeListViewNode> AllVisibleChildren
        {
            get
            {
                var level = Level;
                var node = this;
                while (true)
                {
                    node = node.NextVisibleNode;
                    if (node != null && node.Level > level)
                        yield return node;
                    else
                        break;
                }
            }
        }

        public object Tag { get; }

        internal Collection<TreeListViewNode> Children { get; }

        public ReadOnlyCollection<TreeListViewNode> Nodes { get; }

        #endregion
    }
}