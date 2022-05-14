using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UiControlByLinq
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    // イベントハンドラー

    // ウィンドウが表示されるとき
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      StoreControls();
    }

    // ［UIの有効 / 無効］ボタンがONになったとき
    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
      // ボタンがONになった → すべてのボタンを有効にする
      _buttons?.ForEach(b => b.IsEnabled = true);
      _radioButtons?.ForEach(b => b.IsEnabled = true);
    }

    // ［UIの有効 / 無効］ボタンがOFFになったとき
    private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
    {
      // ボタンがOFFになった → すべてのボタンを無効にする
      _buttons?.ForEach(b => b.IsEnabled = false);
      _radioButtons?.ForEach(b => b.IsEnabled = false);
    }

    // リストボックスの選択が変わったとき
    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // 選択されたリスト項目のTagプロパティ
      object tag = (e.AddedItems[0] as ListBoxItem).Tag;

      // ラジオボタンを選択する
      _radioButtons.ForEach(r => 
        r.IsChecked = object.Equals(r.Tag, tag));
    }


    // UIコントロールをListコレクションに入れて管理する
    private List<Button> _buttons;
    private List<RadioButton> _radioButtons;
    private void StoreControls()
    {
      // このウィンドウが持っているすべてのButtonコントロール
      _buttons = this.Descendants<Button>().ToList();

      // このウィンドウが持っているすべてのRadioButtonコントロール
      _radioButtons = this.Descendants<RadioButton>().ToList();
    }

  }
}
