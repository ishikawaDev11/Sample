using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;  // C# 6 の機能

public class Sample
{
  public string Kind { get; set; }
  public int Value { get; set; }
}

public static class SapmleExtensions
{
  public static int SumValues(this IEnumerable<Sample> samples)
  {
    WriteLine("※SumValues開始");

    int sum = 0;
    foreach (var s in samples)
    {
      sum += s.Value;
      WriteLine($"[3] Sum: {s.Kind}, {s.Value} <sum={sum}>");
    }

    WriteLine("※SumValues終了");
    return sum;
  }
}


class Program
{
  static Sample CreateFromCsvLine(string line)
  {
    // 冒頭に「using static System.Console;」が必要
    WriteLine($"[1] Select: {line}");

    // カンマで分解する
    string[] items = line.Split(',');

    // Sampleオブジェクトを作って返す
    return new Sample()
    {
      Kind = items[0].Trim(),
      Value = int.Parse(items[1].Trim()),
    };
  }

  static bool IsKindA(Sample s)
  {
    bool isKindA = s.Kind == "A";
    WriteLine($"[2] Where: {s.Kind}, {s.Value} ({isKindA})");
    return isKindA;
  }


  // 7-2. ToListメソッドのメリット
  static void Main(string[] args)
  {
    // 冒頭に「using static System.Console;」が必要

    // 「Sample.csv」ファイルから1行ずつ最後まで読む
    IEnumerable<string> lines = File.ReadLines(@".\sample.csv");
    WriteLine("※ReadLines完了");

    // CSVファイルの各行からSampleオブジェクトを作る
    // ※ここで一旦「実体化」する
    List<Sample> samples
      = lines.Select(line => CreateFromCsvLine(line))
             .ToList(); //⇒「[1] Select:…」
    WriteLine("※Select完了");

    // Kindが「A」のデータだけを抜き出す
    IEnumerable<Sample> selectedA
      = samples.Where(s => IsKindA(s)); //⇒「[2] Where: …」
    WriteLine("※Where完了");

    // Valueを合計する
    int sumA = selectedA.SumValues(); //⇒「[3] Sum: …」
    WriteLine("※Sum完了");

    WriteLine($"Aだけの合計は {sumA}");



    // 続けて、Kindが「B」のデータだけを抜き出して合計する。

    WriteLine();
    WriteLine("残りのデータも合計する");

    // Kindが「A」ではないデータだけを抜き出す
    IEnumerable<Sample> selectedB
      = samples.Where(s => !IsKindA(s));
    WriteLine("※Where完了");

    // Valueを合計する
    int sumB = selectedB.SumValues();
    WriteLine("※Sum完了");

    WriteLine($"Bだけの合計は {sumB}");

#if DEBUG
    ReadKey();
#endif
  }
}
