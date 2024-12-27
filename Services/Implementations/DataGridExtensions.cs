using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

public static class DataGridExtensions
{
    public static DataGridCell GetCell(this DataGridColumn column, DataGridRow row)
    {
        var presenter = FindVisualChild<DataGridCellsPresenter>(row);
        if (presenter == null)
        {
            var dataGrid = GetParent<DataGrid>(row); // Find the DataGrid
            dataGrid?.ScrollIntoView(row.Item, column);
            presenter = FindVisualChild<DataGridCellsPresenter>(row);
        }

        return presenter?.ItemContainerGenerator.ContainerFromIndex(column.DisplayIndex) as DataGridCell;
    }

    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T tChild) return tChild;

            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null) return childOfChild;
        }
        return null;
    }

    private static T GetParent<T>(DependencyObject obj) where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(obj);
        while (parent != null && !(parent is T))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as T;
    }
}
