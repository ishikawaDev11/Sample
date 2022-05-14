using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace UiControlByLinq
{
  public static class VisualTreeExtensions
  {
    // 指定した型の子孫要素を取得
    public static IEnumerable<T>
      Descendants<T>(this DependencyObject control)
        where T : DependencyObject
    {
      return control.Descendants().OfType<T>();
    }

    // 全ての子孫要素を取得
    public static IEnumerable<DependencyObject>
      Descendants(this DependencyObject control)
    {
      foreach (var child in control.Children())
      {
        yield return child;
        foreach (var c in child.Descendants())
          yield return c;
      }
    }

    // 直接の子要素を取得
    public static IEnumerable<DependencyObject>
      Children(this DependencyObject control)
    {
      var count = VisualTreeHelper.GetChildrenCount(control);
      for (int i = 0; i < count; i++)
        yield return VisualTreeHelper.GetChild(control, i);
    }
  }
}
