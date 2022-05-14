using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;  // C# 6 の機能


public static class MyLinqExtensions
{
  // 検証用：現在の要素をコンソールに表示する
  public static IEnumerable<T> WriteCurrent<T>(this IEnumerable<T> c, string s)
  {
    int n = 0;
    foreach (var item in c)
    {
      WriteLine($"[{s}]{n++}: {item}");
      yield return item;
    }
  }



  // 10-2. LINQを使うLINQ拡張メソッド
  public static IEnumerable<T> LessThan1<T>(
                                  this IEnumerable<T> numbers,
                                  T threshold) where T : IComparable
  {
    return numbers.Where(n => n.CompareTo(threshold) < 0);
  }



  // 10-3. foreachを使うLINQ拡張メソッド（その1－失敗）
  public static IEnumerable<T> LessThan2<T>(
                                this IEnumerable<T> numbers, 
                                T threshold) where T : IComparable
  {
    List<T> result = new List<T>(); // ← 「ToListの罠」
    foreach (var n in numbers)
    {
      if (n.CompareTo(threshold) < 0)
        result.Add(n);
    }
    return result;
  }

  // 10-3. foreachを使うLINQ拡張メソッド（その2－成功）
  public static IEnumerable<T> LessThan3<T>(
                                this IEnumerable<T> numbers,
                                T threshold) where T : IComparable
  {
    foreach (var n in numbers)
    {
      if (n.CompareTo(threshold) < 0)
        yield return n;
    }
  }



  // 10-4. ラムダ式を受け取るLINQ拡張メソッド
  // 与えられた条件を満たすときだけ、数値を文字列にして「"」で囲む
  public static IEnumerable<string> ToQuotedString<TSource>(
                                this IEnumerable<TSource> numbers,
                                Func<TSource, bool> predicate
                                ) where TSource : IFormattable
  {
    foreach (var n in numbers)
    {
      bool match = predicate(n);
      if (match)
        yield return $"\"{n.ToString()}\"";
    }
  }
  // 注：実際にはラムダ式不要の拡張メソッドも用意しておくべき
  public static IEnumerable<string> ToQuotedString<TSource>(
                                this IEnumerable<TSource> numbers
                                ) where TSource : IFormattable
  {
    foreach (var n in numbers)
      yield return $"\"{n.ToString()}\"";
  }

}

class Program
{
  // コレクション内の全ての要素を表示するメソッド ⇒ 2-1参照
  private static void WriteNumbers<T>(IEnumerable<T> items, string header) 
  {
    Write($"{header}:");  // C# 6 の機能 (冒頭に using static System.Console; が必要)
    foreach (var n in items)
      Write($" {n}");
    WriteLine();
  }

  static void Main(string[] args)
  {
    // 数値のコレクション
    IEnumerable<int> numbers = Enumerable.Range(0, 10);
    double[] doubles = { 0.0, 1.0, 2.5, 3.9, 4.0, 4.1, };

    // 標準のLINQ拡張メソッド（Where）を使う
    {
      IEnumerable<int> evens1 = numbers.Where(n => n < 4 );
      IEnumerable<double> evens2 = doubles.Where(n => n < 4.0);
      WriteNumbers(evens1, "Where（int）");
      WriteNumbers(evens2, "Where（double）");
    }

    WriteLine();

    // LINQを使ったLINQ拡張メソッド
    {
      IEnumerable<int> evens1 = numbers.LessThan1(4);
      IEnumerable<double> evens2 = doubles.LessThan1(4.0);
      WriteNumbers(evens1, "LessThan1（int）");
      WriteNumbers(evens2, "LessThan1（double）");
    }

    WriteLine();

    // 10-3. foreachを使うLINQ拡張メソッド（その1－失敗）
    {
      IEnumerable<int> evens1 = numbers.LessThan2(4);
      IEnumerable<double> evens2 = doubles.LessThan2(4.0);
      WriteNumbers(evens1, "LessThan2（int）");
      WriteNumbers(evens2, "LessThan2（double）");

      // 結果は正しく出るが…

      WriteLine($"evens1の型は{evens1.GetType().Name}");
      // 出力：
      // evens1の型はList`1
      // ※これはList<T>型だということ。

      //int sum = numbers.WriteCurrent("A").LessThan2(4).WriteCurrent("B").Sum();
      // ※ Aが全部走ってからBが走る
      //    ＝ループの分解／再構成が行われていない
      //    ＝遅延実行していない
    }

    WriteLine();

    // 10-3. foreachを使うLINQ拡張メソッド（その2－成功）
    {
      IEnumerable<int> evens1 = numbers.LessThan3(4);
      IEnumerable<double> evens2 = doubles.LessThan3(4.0);
      WriteNumbers(evens1, "LessThan3（int）");
      WriteNumbers(evens2, "LessThan3（double）");

      WriteLine($"LessThan3<int>メソッドが返す型: {evens1.GetType().Name}");
      // 出力:
      // LessThan3<int>メソッドが返す型: <LessThan3>d__3`1
      // ※これは自動生成されたクラス

      //int sum = numbers.WriteCurrent("A").LessThan3(4).WriteCurrent("B").Sum();
      // ※ A→B→A→B→…と実行される＝遅延実行されている
    }

    WriteLine();

    // 10-4. ラムダ式を受け取るLINQ拡張メソッド
    {
      IEnumerable<string> result1 = numbers.ToQuotedString(n => n >= 5);
      IEnumerable<string> result2 = doubles.ToQuotedString(d => d < 4.0);
      WriteNumbers(result1, "5以上のint");
      WriteNumbers(result2, "4未満のdouble");
    }


#if DEBUG
    ReadKey();
#endif
  }
}
