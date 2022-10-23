using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BlockConverter.Tools
{

    //Таб-панель, с уникальным шаблоном для CollectionView.NewItemPlaceholder элемента
    public class TemplateSelectorByItemPlanceholder:DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate NewItemTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item == CollectionView.NewItemPlaceholder ? NewItemTemplate : DefaultTemplate;
        }
    }

    public class TabControlByItemPlanceholder:TabControl
    {
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            object oldSelectValue = null;
            if (e.RemovedItems.Count!=0)
                oldSelectValue = e.RemovedItems[0];
            base.OnSelectionChanged(e);
            foreach (var item in e.AddedItems)
            {
                if (item == CollectionView.NewItemPlaceholder)
                {
                    SelectedValue = oldSelectValue;
                    return;
                }
            }
            
        }
    }
}
