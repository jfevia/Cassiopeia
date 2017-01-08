using System;
using System.Collections.Generic;
using System.Linq;
using Cassiopeia.Collections.ObjectModel;
using Cassiopeia.Common;
using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class Option : ObservableObject
    {
        private readonly ObservableCollection<Option> _children;
        private readonly string _name;

        private bool _isExpanded;
        private bool _isMatch = true;
        private OptionType _type;

        public Option(string name, OptionType type, IEnumerable<Option> children)
        {
            Type = type;
            _name = name;
            _children = new ObservableCollection<Option>(children);
        }

        public Option(string name, IEnumerable<Option> children)
            : this(name, OptionType.None, children)
        {
        }

        public Option(string name, OptionType type)
            : this(name, type, Enumerable.Empty<Option>())
        {
        }

        public Option(string name)
            : this(name, OptionType.None)
        {
        }

        public OptionType Type
        {
            get { return _type; }
            set { Set(nameof(Type), ref _type, value); }
        }

        public IEnumerable<Option> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                Set(nameof(IsExpanded), ref _isExpanded, value);

                if (_isExpanded)
                    foreach (var child in Children)
                        child.IsMatch = true;
            }
        }

        public bool IsMatch
        {
            get { return _isMatch; }
            set { Set(nameof(IsMatch), ref _isMatch, value); }
        }

        public bool IsLeaf
        {
            get { return !Children.Any(); }
        }

        public override string ToString()
        {
            return _name;
        }

        private bool IsCriteriaMatched(string criteria)
        {
            return string.IsNullOrEmpty(criteria) || _name.Contains(criteria, StringComparison.InvariantCultureIgnoreCase);
        }

        private void ApplyCriteria(string criteria, Option parent)
        {
            foreach (var child in parent.Children)
            {
                if (child.IsLeaf && !child.IsCriteriaMatched(criteria))
                    child.IsMatch = false;
                ApplyCriteria(criteria, child);
            }
        }

        public void ApplyCriteria(string criteria, Stack<Option> ancestors)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatch = true;
                    ancestor.IsExpanded = !string.IsNullOrEmpty(criteria);
                    ApplyCriteria(criteria, ancestor);
                }
                IsExpanded = false;
            }
            else
            {
                IsMatch = false;
            }

            ancestors.Push(this);
            foreach (var child in Children)
                child.ApplyCriteria(criteria, ancestors);

            ancestors.Pop();
        }
    }
}