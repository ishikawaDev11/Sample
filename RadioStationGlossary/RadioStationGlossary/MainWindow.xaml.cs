using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Data;          //
using System.IO;
using RadioStationGlossary.Models;
using RadioStationGlossary.View;

namespace RadioStationGlossary
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ClsDatatable dtbl = null;
        ClsDatabase dbase = null;

        public MainWindow()
        {
            InitializeComponent();

            // データ初期化
            cbxDatabase.SelectedIndex = 0;
            InitializeData();

            #region TabSearch
            // 検索タブのイベント設定
            SetEventForSearch();
            #endregion

            #region TabRegist
            // 更新タブのイベント設定
            SetEventForRegist();
            #endregion

            this.Closed += (sender, e) =>
            {
                if (dbase != null)
                    dbase.Dispose();
            };
        }

        private void InitializeData(int idx=0)
        {
            // 表示用データの初期化
            dtbl = new ClsDatatable();                 // 表示用DataTableインスタンス化
            dtbl.Initialize();                         // 初期化(DataTable)
            lstGlossary.DataContext = dtbl.glTable;    // GridView に関連付け
            dbase = new ClsDatabase();                 // Databaseインスタンス化
            dbase.Initialize(idx);

            //if (dtbl != null)
            //{
            //    dtbl.Clear();
            //}
            //else
            //{
            //    dtbl = new ClsDatatable();                 // 表示用DataTableインスタンス化
            //    dtbl.Initialize();                         // 初期化(DataTable)
            //    lstGlossary.DataContext = dtbl.glTable;    // GridView に関連付け
            //}

            // データベースの初期化
            //if (dbase != null)
            //{
            //    dbase.Dispose();
            //}
            //dbase = new ClsDatabase();
            //dbase.Initialize(idx);
        }

        private void ChangeData(int idx)
        {
            dtbl?.Clear();
            dbase?.Initialize(idx);
        }

        #region TabSearch
        private void SetEventForSearch()
        {
            // 検索ボタンイベント定義
            btnSearch.Click += (sender, e) => Searchevent();

            // 検索順の設定イベント
            rbtnSearchOrderUp.Click += (sender, e) =>
            {
                dbase.Order = 0;
            };
            rbtnSearchOrderDown.Click += (sender, e) =>
            {
                dbase.Order = -1;
            };

            // 検索文字列入力の「Return」で検索
            tbxSearch.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Return)
                {
                    Searchevent();
                }

            };

            // リスト選択時の処理
            lstGlossary.SelectionChanged += (sender, e) =>
             {
                 var source = e.Source;
                 DataRowView dataRow = (DataRowView)lstGlossary.SelectedItem;
                 if (dataRow == null)
                 {
                     btnEdit.IsEnabled = false;
                     return;
                 }

                 btnEdit.IsEnabled = true;
                 string disp = dataRow.Row.ItemArray[2].ToString();
                 tbkDiscription.Text = disp;

                // 画像表示
                 var count = dataRow.Row.ItemArray.Count();
                 string filePath = dataRow.Row.ItemArray[4].ToString();
                 if (!String.IsNullOrWhiteSpace(filePath))
                 {
                     ImageSearchView(filePath);
                     imageDisplayData.Visibility = Visibility.Visible;
                 }
                 else
                     imageDisplayData.Visibility = Visibility.Hidden;
             };

            // 画像クリック時の処理
            imageDisplayData.MouseLeftButtonDown += (sender, e) =>
            {
                if (lstGlossary.SelectedIndex < 0)
                    return;

                DataRowView dataRow = (DataRowView)lstGlossary.SelectedItem;
                if (dataRow == null)
                    return;

                string filePath = dataRow.Row.ItemArray[4].ToString();
                if (!String.IsNullOrWhiteSpace(filePath))
                {
                    ImageViewWindow ivw = new ImageViewWindow(filePath);
                    ivw.Show();
                }
            };

            // リスト選択時の編集ボタンクリック
            btnEdit.Click += (sender, e) =>
            {
                DataRowView dataRow = (DataRowView)lstGlossary.SelectedItem;
                if (dataRow == null)
                    return;

                tabCtlDataase.SelectedIndex = 1;
                tbxInputName.Text = dataRow.Row.ItemArray[1].ToString();
                Glos gls = Search1Event();
                string filePath = gls.ImageData;

                //tbxInputName.Text = dataRow.Row.ItemArray[1].ToString();
                //tbxInputDiscription.Text = dataRow.Row.ItemArray[2].ToString();
                //var tmp = dataRow.Row.ItemArray[3];
                //if (null != tmp)
                //    tbxInputRemarks.Text = tmp.ToString();
                //
                //string filePath = dataRow.Row.ItemArray[4].ToString();
                if (!String.IsNullOrWhiteSpace(filePath))
                {
                    // 相対パスから絶対パスに変換
                    string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    tbxImageFile.Text = exePath + @"\" + filePath;
                    //ImageRegistView(filePath);
                    //imageRegistData.Visibility = Visibility.Visible;
                }
                else
                    imageRegistData.Visibility = Visibility.Hidden;

            };

            cbxDatabase.SelectionChanged += (sender, e) =>
            {
                int idx = cbxDatabase.SelectedIndex;
                ChangeData(idx);
            };
        }

        private void ImageSearchView(string filePath)
        {
            imageDisplayData.Source = ClsBitmap.GetBitmapImage(filePath);
            imageDisplayData.Visibility = Visibility.Visible;
        }

        private void Searchevent()
        {
            // Databaseの読込み
            string strSearch = tbxSearch.Text;
            var results = dbase.ReadDb(strSearch, out string emsg);
            if (results == null)
            {
                tbkDiscription.Text = emsg;
                return;
            }

            // Datatableへの展開
            dtbl.Load(results);

            // 詳細欄のクリア
            tbkDiscription.Text = "";
            imageDisplayData.Visibility = Visibility.Hidden;
        }

#endregion

        #region TabRegist
        private void SetEventForRegist()
        {
            // 検索ボタンイベント定義
            btnWrite.Click += (sender, e) =>
            {
                var datarow = new Glos();
                datarow.Name = tbxInputName.Text;
                datarow.Discription = tbxInputDiscription.Text;
                datarow.ImageData = tbxImageFile.Text;
                datarow.Remarks = tbxInputRemarks.Text;
                if (dbase.WriteDb(datarow, out string msg) == true)
                {   // 正常に書き込み
                    tbkResult.Foreground = Brushes.Black;
                    ClearInputData();
                    tbxInputName.Focus();       // フォーカスを名称入力にして効率よく入力できるようにする
                }
                else
                {   // 書き込み失敗など
                    tbkResult.Foreground = Brushes.Red;
                }
                tbkResult.Text = msg;
            };

            btnWdClear.Click += (sender, e) => ClearAllData();

            btnDelete.Click += (sender, e) =>
            {
                Glos glos = Search1Event();
                if (glos != null)
                {
                    if(dbase.DeleteDb(glos, out string msg) == true)
                    {
                        tbkResult.Foreground = Brushes.Black;
                        ClearInputData();
                        tbxInputName.Focus();       // フォーカスを名称入力にして効率よく入力できるようにする
                    }
                    else
                    {
                        tbkResult.Foreground = Brushes.Red;
                    }
                    tbkResult.Text = msg;
                }
            };

            //btnRdSet.Click += (sender, e) => Search1Event();

            // テキストボックス定義
            tbxInputName.TextChanged += (sender, e) => ClearResultData();
            tbxInputName.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Return)
                {
                    Glos gls = Search1Event();
                }
            };

            tbxInputDiscription.KeyDown += (sender, e) =>
            {
                if(e.Key == Key.Return)
                {
                    TextBox textBox = (TextBox)sender;
                    if(textBox.Text=="")
                        return;

                    int pos = textBox.CaretIndex;
                    string buf = textBox.Text;
                    textBox.Text = buf.Insert(pos, Environment.NewLine);
                    textBox.CaretIndex = pos + 2;

#if false
                    int pos = textBox.CaretIndex;
                    string buf = textBox.Text;
                    buf += Environment.NewLine;
                    textBox.Text = buf;
                    textBox.CaretIndex = buf.Length;
#endif
                }
            };

            tbxInputRemarks.TextChanged += (sender, e) => ClearResultData();

            tbxImageFile.PreviewDragOver += (sender, e) =>
            {
                if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
                {
                    e.Effects = System.Windows.DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = System.Windows.DragDropEffects.None;
                }
                e.Handled = true;
            };

            tbxImageFile.PreviewDrop += (sender, e) =>
            {
                var dropFiles = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
                if (dropFiles == null) return;
                string filename = dropFiles[0];
                tbxImageFile.Text = filename;
                //string folderName = Path.GetDirectoryName(filename);
                ImageRegistView(filename);
            };
        }

        private void ImageRegistView(string filePath)
        {
            imageRegistData.Source = ClsBitmap.GetBitmapImage(filePath);
            imageRegistData.Visibility = Visibility.Visible;
        }

        private Glos Search1Event()
        {
            Glos gl = null;
            string keyname = tbxInputName.Text;
            if (keyname == "")
                return gl;

            ClearInfoData();
            ClearResultData();
            gl = dbase.ReadDb1(keyname, out string emsg);
            if (gl != null)
            {
                if (gl.Id > 0)
                {
                    tbxInputName.Text = gl.Name;
                    tbxInputDiscription.Text = gl.Discription;
                    tbxInputRemarks.Text = gl.Remarks;
                    tbxImageFile.Text = gl.ImageData;
                    if (!String.IsNullOrWhiteSpace(gl.ImageData))
                    {
                        ImageRegistView(gl.ImageData);
                    }
                }
            }
            else
            {
                tbkResult.Text = emsg;
            }
            return gl;
        }

        private void ClearAllData()
        {
            ClearInputData();
            ClearResultData();
        }

        private void ClearInputData()
        {
            tbxInputName.Clear();
            ClearInfoData();
            ClearImageData();
        }

        private void ClearInfoData()
        {
            tbxInputDiscription.Clear();
            tbxInputRemarks.Clear();
        }

        private void ClearImageData()
        {
            tbxImageFile.Clear();
            imageRegistData.Visibility = Visibility.Hidden;
        }

        private void ClearResultData()
        {
            tbkResult.Text = "";
        }

        #endregion

    }


}
