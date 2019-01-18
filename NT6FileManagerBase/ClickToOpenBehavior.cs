using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using WindowsSharp.DiskItems;

namespace NT6FileManagerBase
{
    public class ClickToOpenBehavior : Behavior<Panel>
    {
        /*public DiskItem TargetItem
        {
            get => (DiskItem)GetValue(TargetItemProperty);
            set => SetValue(TargetItemProperty, value);
        }

        public static readonly DependencyProperty TargetItemProperty =
            DependencyProperty.Register("TargetItem", typeof(DiskItem), typeof(ClickToOpenBehavior), new PropertyMetadata(null));*/

        public ListViewItem ParentListViewItem
        {
            get => (ListViewItem)GetValue(ParentListViewItemProperty);
            set => SetValue(ParentListViewItemProperty, value);
        }

        public static readonly DependencyProperty ParentListViewItemProperty =
            DependencyProperty.Register("ParentListViewItem", typeof(ListViewItem), typeof(ClickToOpenBehavior), new PropertyMetadata(null, OnParentListViewItemChangedCallback));

        static void OnParentListViewItemChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var sned = sender as ClickToOpenBehavior;
            if (e.NewValue != null)
                sned.ParentListViewItem.PreviewMouseDoubleClick += sned.ParentListViewItem_PreviewMouseDoubleClick;

            if (e.OldValue != null)
                (e.OldValue as ListViewItem).PreviewMouseDoubleClick -= sned.ParentListViewItem_PreviewMouseDoubleClick;
        }

        public FileManagerBase ManagerBase
        {
            get => (FileManagerBase)GetValue(ManagerBaseProperty);
            set => SetValue(ManagerBaseProperty, value);
        }

        public static readonly DependencyProperty ManagerBaseProperty =
            DependencyProperty.Register("ManagerBase", typeof(FileManagerBase), typeof(ClickToOpenBehavior), new PropertyMetadata(null));

        //ListViewItem _item;
        /*protected override void OnAttached()
        {
            base.OnAttached();
        }*/

        private void ParentListViewItem_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ManagerBase.OpenSelection();
        }
    }
}
