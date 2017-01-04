using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Cassiopeia.Windows.Controls
{
    public class TreeListView : ListView
    {
        private ITreeModel _model;

        public TreeListView()
        {
            Rows = new Collections.ObjectModel.ObservableCollection<TreeListViewNode>();
            Root = new TreeListViewNode(this, null) {IsExpanded = true};
            ItemsSource = Rows;
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
        }

        internal Collections.ObjectModel.ObservableCollection<TreeListViewNode> Rows { get; }

        public ITreeModel Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    Root.Children.Clear();
                    Rows.Clear();
                    CreateChildrenNodes(Root);
                }
            }
        }

        internal TreeListViewNode Root { get; }

        public ReadOnlyCollection<TreeListViewNode> Nodes
        {
            get { return Root.Nodes; }
        }

        internal TreeListViewNode PendingFocusNode { get; set; }

        public ICollection<TreeListViewNode> SelectedNodes
        {
            get
            {
                return SelectedItems.Cast<TreeListViewNode>()
                    .ToArray();
            }
        }

        public TreeListViewNode SelectedNode
        {
            get
            {
                if (SelectedItems.Count > 0)
                    return SelectedItems[0] as TreeListViewNode;
                return null;
            }
        }

        private void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && PendingFocusNode != null)
            {
                var item = ItemContainerGenerator.ContainerFromItem(PendingFocusNode) as TreeListViewItem;
                item?.Focus();
                PendingFocusNode = null;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var ti = element as TreeListViewItem;
            var node = item as TreeListViewNode;
            if (ti != null && node != null)
            {
                ti.Node = (TreeListViewNode) item;
                base.PrepareContainerForItemOverride(element, node.Tag);
            }
        }

        internal void SetIsExpanded(TreeListViewNode node, bool value)
        {
            if (value)
            {
                if (!node.IsExpandedOnce)
                {
                    node.IsExpandedOnce = true;
                    node.AssignIsExpanded(true);
                    CreateChildrenNodes(node);
                }
                else
                {
                    node.AssignIsExpanded(true);
                    CreateChildrenRows(node);
                }
            }
            else
            {
                DropChildrenRows(node, false);
                node.AssignIsExpanded(false);
            }
        }

        internal void CreateChildrenNodes(TreeListViewNode node)
        {
            var children = GetChildren(node);
            if (children != null)
            {
                var rowIndex = Rows.IndexOf(node);
                node.ChildrenSource = children as INotifyCollectionChanged;
                foreach (var obj in children)
                {
                    var child = new TreeListViewNode(this, obj);
                    child.HasChildren = HasChildren(child);
                    node.Children.Add(child);
                }
                Rows.InsertRange(rowIndex + 1, node.Children.ToArray());
            }
        }

        private void CreateChildrenRows(TreeListViewNode node)
        {
            var index = Rows.IndexOf(node);
            if (index >= 0 || node == Root) // ignore invisible nodes
            {
                var nodes = node.AllVisibleChildren.ToArray();
                Rows.InsertRange(index + 1, nodes);
            }
        }

        internal void DropChildrenRows(TreeListViewNode node, bool removeParent)
        {
            var start = Rows.IndexOf(node);
            if (start >= 0 || node == Root) // ignore invisible nodes
            {
                var count = node.VisibleChildrenCount;
                if (removeParent)
                    count++;
                else
                    start++;
                Rows.RemoveRange(start, count);
            }
        }

        private IEnumerable GetChildren(TreeListViewNode parent)
        {
            return Model?.GetChildren(parent.Tag);
        }

        private bool HasChildren(TreeListViewNode parent)
        {
            if (parent == Root)
                return true;
            if (Model != null)
                return Model.HasChildren(parent.Tag);
            return false;
        }

        internal void InsertNewNode(TreeListViewNode parent, object tag, int rowIndex, int index)
        {
            var node = new TreeListViewNode(this, tag);
            if (index >= 0 && index < parent.Children.Count)
            {
                parent.Children.Insert(index, node);
            }
            else
            {
                index = parent.Children.Count;
                parent.Children.Add(node);
            }
            Rows.Insert(rowIndex + index + 1, node);
        }
    }
}