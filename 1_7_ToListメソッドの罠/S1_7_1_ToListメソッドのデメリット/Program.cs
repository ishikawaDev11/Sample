using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;  // C# 6 の機能

class Program
{
  static void WriteTotalMemory(string header)
  {
    var totalMemory = GC.GetTotalMemory(true) / 1024.0 / 1024.0;
    WriteLine($"{header}: {totalMemory:0.0 MB}");
  }

  // 7.1 ToListメソッドのデメリット
  static void Main(string[] args)
  {
    // 1～100万（100万個）の整数を持つ配列（約4MB）
    int[] numbers = Enumerable.Range(1, 1000000).ToArray();
    WriteTotalMemory("配列確保後");
    WriteLine($"配列の要素数: {numbers.Length}\n");
    // 出力：
    // 配列確保後: 3.9 MB
    // 配列の要素数: 1000000

    // 偶数だけを取り出したコレクションを得る（2MBほど消費する?）
    IEnumerable<int> evenNumbers = numbers.Where(n => n % 2 == 0);
    WriteTotalMemory("偶数だけ取り出した後");
    // 出力：
    // 偶数だけ取り出した後: 3.9 MB

    // ToListを使う
    List<int> evenNumbersList = evenNumbers.ToList();
    WriteTotalMemory("ToList後");
    // 出力：
    // ToList後: 5.9 MB

    // 平均値を求める
    var ave = evenNumbersList.Average();
    WriteTotalMemory("計算後（LINQ）");
    WriteLine($"偶数だけの平均値（LINQ）: {ave}");
    // 出力：
    // 計算後（LINQ）: 5.9 MB
    // 偶数だけの平均値（LINQ）: 500001


#if DEBUG
    ReadKey();
#endif
  }
}

