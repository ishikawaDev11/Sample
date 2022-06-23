using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;     //
using LiteDB;                               //

namespace RadioStationGlossary.Models
{
    public class ClsDatabase : IDisposable
    {
        //private LiteDatabase _context = null;
        //private string _database = "./data.db";
        //private string _dbname = "glossary";
        private int _idx = 0;
        private LiteDatabase[] _contexts = { null, null };
        private string[] _databases = { "./data.db", "./awsData.db" };
        private string[] _dbnames = { "glossary", "awscontents" };
        private Glos _glos = new Glos();
        private int _order = 0;     // 0:default=昇順
                                    // -1：降順

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public void Initialize(int idx)
        {
            //if (_context != null)
            //{
            //    _context.Dispose();
            //    _context = null;
            //}
            _idx = idx;
            //_database = _databases[_idx];
            //_dbname = _dbnames[_idx];
        }

        public bool WriteDb(Glos glos, out string msg)
        {
            bool retsts = false;
            msg = "■名称がありません";

            if (glos?.Name == "") return retsts;

            int id = _glos.Id;                              // 更新時に必要
            _glos = glos;

            try
            {
                // DB接続
                if (_contexts[_idx] == null)
                    _contexts[_idx] = new LiteDatabase(_databases[_idx]);

                // コレクション取得
                var db = _contexts[_idx].GetCollection<Glos>(_dbnames[_idx]);

                // 画像データのファイル整形
                if (_glos.ImageData != "")
                {
                    // ※image/yyyymm/yyyymmdd_hhmmss.jpg としてコピーする。解像度変換等の圧縮しなおしはしない。
                    DateTime dt = DateTime.Now;
                    string dirname = "image" + @"\" + dt.ToString("yyyyMM");
                    string filename = dt.ToString("yyyyMMdd_hhmmss") + ".jpg";
                    string imagedata = dirname + @"\" + filename;
                    string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string dstFile = exePath + @"\" + imagedata;
                    try
                    {
                        string distDir = System.IO.Path.GetDirectoryName(dstFile);
                        if (!System.IO.Directory.Exists(distDir))
                        {
                            System.IO.Directory.CreateDirectory(distDir);
                        }

                        System.IO.File.Copy(_glos.ImageData, dstFile, true);
                        _glos.ImageData = imagedata;
                    }
                    catch (Exception ex)
                    {   // コピーに失敗したらイメージ画像なしとする
                        Console.WriteLine(ex.Message);
                        _glos.ImageData = "";
                    }
                }

                // 同じキーを探して登録か更新かを判定する。
                var record = db.FindOne(x => x.Name == glos.Name);
                if (record == null)
                {   // DB新規追加
                    // Nameをユニークインデックスにする
                    db.EnsureIndex(x => x.Name, true);

                    // 作成
                    db.Insert(glos);

                    msg = $"■Database書込み終了しました({glos.Name})";
                    retsts = true;
                }
                else
                {   // DB更新
                    DialogResult result = MessageBox.Show($"{glos.Name} を更新しますか？", "DB更新確認", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        glos.Id = id;
                        if (db.Update(glos) == true)
                        {
                            msg = $"■Database更新終了しました({glos.Name})";
                            retsts = true;
                        }
                        else
                        {
                            msg = $"■Database更新できませんでした";
                            msg += $"\r\n {glos.Id}";
                            msg += $"\r\n {glos.Name}";
                            msg += $"\r\n {glos.Discription}";
                            //retsts = false;
                        }
                    }
                    else
                    {
                        msg = $"■Database更新キャンセルしました({glos.Name})";
                        //retsts = false;
                    }
                }
            }
            catch(Exception ex)
            {
                msg = $"■Databaseの書き込みに失敗しました({glos.Name})\r\n" + ex.Message;
            }

            return retsts;
        }

        public bool DeleteDb(Glos glos, out string msg)
        {
            bool retsts = false;
            msg = "";

            DialogResult result = MessageBox.Show($"{glos.Name} を削除しますか？", "DB削除確認", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    // DB接続
                    if (_contexts[_idx] == null)
                        _contexts[_idx] = new LiteDatabase(_databases[_idx]);

                    // コレクション取得
                    var db = _contexts[_idx].GetCollection<Glos>(_dbnames[_idx]);

                    // 削除
                    db.Delete(glos.Id);

                    if (!String.IsNullOrWhiteSpace(glos.ImageData))
                    {
                        try
                        {
                            System.IO.File.Delete(glos.ImageData);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("画像削除失敗：" + ex.Message);
                        }
                    }

                    msg = $"■Database削除終了しました({glos.Name})";
                    retsts = true;
                }
                catch (Exception ex)
                {
                    msg = $"■Databaseの削除に失敗しました({glos.Name})\r\n" + ex.Message;
                }
            }
            else
            {
                msg = $"■Database削除キャンセルしました({glos.Name})";
                retsts = true;
            }

            return retsts;
        }

        public IEnumerable<Glos> ReadDb(string strSearch, out string msg)
        {
            msg = "";
            IEnumerable<Glos> gls = null;

            try
            {
                // DB接続
                if (_contexts[_idx] == null)
                    _contexts[_idx] = new LiteDatabase(_databases[_idx]);
                var db = _contexts[_idx].GetCollection<Glos>(_dbnames[_idx]);

                if (strSearch == "")
                {
                    if (_order < 0)     // マイナスなので降順
                        gls = db.FindAll().OrderByDescending(xx => xx.Name);
                    else                // 0以上なので昇順
                        gls = db.FindAll().OrderBy(xx => xx.Name);

                    var cnt = gls.Count();
                }
                else
                {
                    if (_order < 0)
                        gls = db.Find(x => x.Name.StartsWith(strSearch)).OrderByDescending(x => x.Name);
                    else
                        gls = db.Find(x => x.Name.StartsWith(strSearch)).OrderBy(x => x.Name);
                }
            }
            catch(Exception ex)
            {
                msg = $"■Databaseの読み込みに失敗しました\r\n" + ex.Message;
            }
            return gls;
        }

        public Glos ReadDb1(string strSearch, out string msg)
        {
            msg = "";
            IEnumerable<Glos> gls = null;

            try
            {
                // DB接続
                if (_contexts[_idx] == null)
                    _contexts[_idx] = new LiteDatabase(_databases[_idx]);
                var db = _contexts[_idx].GetCollection<Glos>(_dbnames[_idx]);

                if (strSearch != "")
                {
                    if (db.Count(x => x.Name == strSearch) > 0)
                    {
                        gls = db.Find(x => x.Name == strSearch);
                        _glos = gls.First();
                    }
                    else
                    {
                        ClearGros();
                    }
                }
            }
            catch(Exception ex)
            {
                msg = $"■Databaseの読み込みに失敗しました\r\n" + ex.Message;
            }
            return _glos;
        }

        private void ClearGros()
        {
            _glos.Id = 0;
            _glos.Name = "";
            _glos.Discription = "";
            _glos.Remarks = "";
            _glos.ImageData = "";
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                        for (int id = 0; id < _contexts.Length; id++)
                        {
                            _contexts[id]?.Commit();
                            _contexts[id]?.Dispose();
                            _contexts[id] = null;
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~ClsDatabase() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
