using System.Collections.Generic;
using static System.Console;  // C# 6 の機能


// 遅延実行しないLINQデータソース
// この例は、コンストラクタ引数で与えられた個数だけの偶数を、
// 2,4,6,8,…と生成して内部に保持する。
// NumbersプロパティがLINQデータソースになっている。
public class EvenNumbers
{
  private List<int> _evenNumbers = new List<int>();

  public EvenNumbers(int count)
  {
    for (int n = 1; n <= count; n++)
      _evenNumbers.Add(n * 2);
  }

  public IEnumerable<int> Numbers
  {
    get
    {
      return new System.Collections.ObjectModel
                  .ReadOnlyCollection<int>(_evenNumbers);
    }
  }
}


class Program
{
  static void Main(string[] args)
  {
    var en = new EvenNumbers(5);
    foreach (var n in en.Numbers)
      WriteLine(n);
    // 出力：
    // 2
    // 4
    // 6
    // 8
    // 10

#if DEBUG
    ReadKey();
#endif
  }
}
