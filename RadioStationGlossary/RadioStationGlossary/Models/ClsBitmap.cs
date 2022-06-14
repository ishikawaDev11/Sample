using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;          //
using System.IO;
using System.Windows.Media.Imaging;



namespace RadioStationGlossary.Models
{
    public class ClsBitmap
    {

        public static BitmapImage GetBitmapImage(string filePath)
        {
            BitmapImage bmp = null;
            try
            {
                // 選択したファイルをメモリにコピーする
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    fs.CopyTo(ms);      // FileStreamの内容をメモリストリームにコピーします。
                }

                ms.Seek(0, SeekOrigin.Begin);
                bmp = new BitmapImage();
                bmp.BeginInit();                                // BitmapImage の初期化の開始を通知します。
                bmp.CacheOption = BitmapCacheOption.OnLoad;     // プロセスでファイルロックしないようにする
                bmp.StreamSource = ms;                          // BitmapImage のストリーム ソースを設定します。
                bmp.EndInit();                                  // BitmapImage の初期化の終了を通知します。
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return bmp;
        }
    }
}
