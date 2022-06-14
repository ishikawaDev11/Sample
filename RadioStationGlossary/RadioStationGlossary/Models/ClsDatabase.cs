using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;     //
using LiteDB;                               //

namespace RadioStationGlossary.Models
{
    public class ClsDatabase
    {
        private LiteDatabase _context = null;
        private string _database = "./data.db";
        private string _dbname = "glossary";
        private Glos _glos = new Glos();
        private int _order = 0;     // 0:default=昇順
                                    // -1：降順

        public int Order
        {
            get { return _order; }
            set { _order = value; }
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
                if (_context == null)
                    _context = new LiteDatabase(_database);

                // コレクション取得
                var db = _context.GetCollection<Glos>(_dbname);

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
                    if (_context == null)
                        _context = new LiteDatabase(_database);

                    // コレクション取得
                    var db = _context.GetCollection<Glos>(_dbname);

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
                            Console.WriteLine("画像削除失敗");
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

        public IEnumerable<Glos> ReadDb(string strSearch)
        {

            // DB接続
            if (_context == null)
                _context = new LiteDatabase(_database);
            var db = _context.GetCollection<Glos>(_dbname);
            IEnumerable<Glos> gls = null;

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
            return gls;
        }

        public Glos ReadDb1(string strSearch)
        {
           
            // DB接続
            if (_context == null)
                _context = new LiteDatabase(_database);
            var db = _context.GetCollection<Glos>(_dbname);
            IEnumerable<Glos> gls = null;

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

    }
}
