using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;  // C# 6 の機能

// 「Sample」データ
public class Sample
{
  public string Kind { get; set; }
  public int Value { get; set; }
}

// LINQデータソース
public class SampleDataSource : IEnumerable<Sample>
{
  // IEnumerable<T>の実装
  public IEnumerator<Sample> GetEnumerator()
  {
    return GetCsvEnumerator();
  }
  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }


  // 外部からインスタンス化させない
  private SampleDataSource()
  {
    // (コード無し)
  }


  private string _csvFilePath;
  private string _kind;

  public static SampleDataSource ReadA(string csvFilePath)
  {
    return new SampleDataSource()
                {
                  _csvFilePath = csvFilePath,
                  _kind = "A",
                };
  }

  // ファイルを読み取り、Sampleオブジェクトを列挙するメソッド
  // ただし、返値の型に注目！
  private IEnumerator<Sample> GetCsvEnumerator()
  {
    // 検証用
    WriteLine("GetCsvEnumeratorメソッド開始");

    // 1行ずつ読み込んでループを回す
    foreach (var line in File.ReadLines(_csvFilePath))
    {
      // 検証用
      WriteLine($"Read: {line}");

      // カンマで分解する
      string[] data = line.Split(',');

      // データのKindをチェックする
      string kind = data[0].Trim();
      if (kind != _kind)
        continue;

      // データの数値を取得する
      int value = 0;
      int.TryParse(data[1].Trim(), out value);

      // 検証用
      WriteLine($"Create({kind}, {value})");

      // Sampleオブジェクトを生成して返す
      yield return new Sample(){ Kind = kind, Value = value, };
    }
  }

}


class Program
{
  static void Main(string[] args)
  {
    var samples = SampleDataSource.ReadA(@".\sample.csv");

    WriteLine("1回目");
    foreach (var s in samples)
      WriteLine($"☆ Kind={s.Kind}, Value={s.Value}");
    // 出力：
    // 1回目
    // GetCsvEnumeratorメソッド開始
    // Read: A, 100
    // Create(A, 100)
    // ☆ Kind = A, Value = 100
    // Read: B, 200
    // Read: A, 300
    // Create(A, 300)
    // ☆ Kind = A, Value = 300
    
    WriteLine();
    WriteLine("2回目");
    foreach (var s in samples)
      WriteLine($"☆ Kind={s.Kind}, Value={s.Value}");
    // 出力：
    // 2回目
    // (省略…1回目と同じ)

    WriteLine();
    WriteLine("100より大きいデータを抽出");
    var over100 = samples.Where(s => 
                            {
                              WriteLine($"Where:{s.Kind},{s.Value}");
                              return s.Value > 100;
                            });
    WriteLine("foreach開始");
    foreach (var s in over100)
      WriteLine($"☆ Kind={s.Kind}, Value={s.Value}");
    // 出力：
    // 100より大きいデータを抽出
    // foreach開始
    // GetCsvEnumeratorメソッド開始
    // Read: A, 100
    // Create(A, 100)
    // Where: A,100
    // Read: B, 200
    // Read: A, 300
    // Create(A, 300)
    // Where: A,300
    // ☆ Kind = A, Value = 300


#if DEBUG
    ReadKey();
#endif
  }
}
