using System.Collections.Generic;
using System.Linq;

using static System.Console;

namespace S1_6_2_LINQはメモリを節約する
{
  class Program
  {
    // 6-2. LINQはメモリーを節約する
    static void Main(string[] args)
    {
      // 冒頭に「using static System.Console;」が必要

      // foreach文の動作

      // 列挙対象のIEnumerable<T>型コレクション
      // 1～10までの整数
      IEnumerable<int> numbers = Enumerable.Range(1, 10);

      // 最初にIEnumerator<T>オブジェクトを取得する
      IEnumerator<int> enumerator = numbers.GetEnumerator();

      // MoveNextメソッドがtrueを返す間、ループを回す
      while (enumerator.MoveNext())
      {
        // ループ変数を取り出す
        int current = enumerator.Current;

        // ループ変数を使った処理
        WriteLine($"{current}");
      }

#if DEBUG
      ReadKey();
#endif
    }
  }
}

