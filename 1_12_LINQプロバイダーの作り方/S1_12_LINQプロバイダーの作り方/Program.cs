using System.Collections.Generic;
using System.Linq;
using static System.Console;  // C# 6 の機能



class Program
{
  static void Main(string[] args)
  {
    IQueryable<Sample> q 
      = SampleQueryProvider.CreateQuery(@".\sample.csv");

    WriteLine("【全件取得】foreach開始");
    foreach (var s in q)
      WriteLine($"☆ Kind={s.Kind}, Value={s.Value}");
    // 出力：
    //【全件取得】foreach開始
    //ReadSampleCsvFileメソッド開始
    //Read: A, 100
    //Create(A, 100)
    //☆ Kind = A, Value = 100
    //Read: B, 200
    //Create(B, 200)
    //☆ Kind = B, Value = 200
    //Read: A, 300
    //Create(A, 300)
    //☆ Kind = A, Value = 300

    WriteLine();

    IQueryable<Sample> q1 = q.Where(s => s.Kind == "A");
    WriteLine("【Aのみ】foreach開始");
    foreach (var s in q1)
      WriteLine($"☆ Kind={s.Kind}, Value={s.Value}");
    // 出力：
    //【Aのみ】foreach開始
    //ReadSampleCsvFileメソッド開始
    //Read: A, 100
    //Create(A, 100)
    //☆ Kind = A, Value = 100
    //Read: B, 200
    //Read: A, 300
    //Create(A, 300)
    //☆ Kind = A, Value = 300

    WriteLine();

    // このプロバイダーは「Where(s => s.Kind == "?")」だけにしか対応していない
    // それ以外の拡張メソッドを使うには、IEnumerableに変換してからチェーンする
    IEnumerable<Sample> e1 = q.Where(s => s.Kind == "A")
                              .AsEnumerable() //これ以降はIEnumerableのチェーン
                              .Where(s => s.Value == 100);
    WriteLine("【AかつValue=100】foreach開始");
    foreach (var s in e1)
      WriteLine($"☆ Kind={s.Kind}, Value={s.Value}");
    // 出力：
    //【AかつValue = 100】foreach開始
    //ReadSampleCsvFileメソッド開始
    //Read: A, 100
    //Create(A, 100)
    //☆ Kind = A, Value = 100
    //Read: B, 200
    //Read: A, 300
    //Create(A, 300)

    WriteLine();

    // IEnumerableに変換しても、ループの打ち切りはちゃんと伝わる
    IEnumerable<Sample> e2 = q.Where(s => s.Kind == "B")
                              .AsEnumerable()
                              .Take(1);  //コレクションの先頭から1個だけを取り出す
    WriteLine("【B、ただし最初の1件のみ】foreach開始");
    foreach (var s in e2)
      WriteLine($"☆ Kind={s.Kind}, Value={s.Value}");
    // 出力：
    //【B、ただし最初の1件のみ】foreach開始
    //ReadSampleCsvFileメソッド開始
    //Read: A, 100
    //Read: B, 200
    //Create(B, 200)
    //☆ Kind = B, Value = 200

#if DEBUG
    ReadKey();
#endif
  }
}
