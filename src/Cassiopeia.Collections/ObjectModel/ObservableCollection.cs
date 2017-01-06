using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Cassiopeia.Collections.ObjectModel
{
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public ObservableCollection()
        {
        }

        public ObservableCollection(IEnumerable<T> collection) : base
            (collection)
        {
        }

        public ObservableCollection(List<T> collection) : base
            (collection)
        {
        }

        public void RemoveRange(int index, int count)
        {
            CheckReentrancy();
            var items = Items as List<T>;
            items?.RemoveRange(index, count);
            OnReset();
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            CheckReentrancy();
            var items = Items as List<T>;
            items?.InsertRange(index, collection);
            OnReset();
        }

        private void OnReset()
        {
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void AddRange(IEnumerable<T> collection)
        {
            CheckReentrancy();
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var i in collection) Items.Add(i);
            OnReset();
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            CheckReentrancy();
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var i in collection) Items.Remove(i);
            OnReset();
        }

        public void Replace(T item)
        {
            CheckReentrancy();
            ReplaceRange(new[] {item});
        }

        public void ReplaceRange(IEnumerable<T> collection)
        {
            CheckReentrancy();
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Items.Clear();
            foreach (var i in collection) Items.Add(i);
            OnReset();
        }
    }
}