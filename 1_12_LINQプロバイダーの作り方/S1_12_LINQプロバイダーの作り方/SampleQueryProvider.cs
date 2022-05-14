using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using static System.Console;  // C# 6 の機能


// 「Sample」データ
public class Sample
{
  public string Kind { get; set; }
  public int Value { get; set; }
}


// LINQプロバイダー

// IQueryProviderの実装
public class SampleQueryProvider : AbstractQueryProvider
{
  // 抽象メソッドの実装
  public override object Execute(Expression exp)
  {
    // SampleExpressionVisitorを使って、後続の拡張メソッドを解析する
    var visitor = new SampleExpressionVisitor();
    visitor.Visit(exp);

    // 解析したKindの指定を取り出す
    string kindFilter = visitor.KindFilter;

    // CSVファイルを読み込みつつ、Sampleオブジェクトを作って返す
    return ReadSampleCsvFile(kindFilter).AsQueryable();
  }

  public override string GetQueryText(Expression exp)
  {
    return string.Empty;
  }


  // 外部からインスタンス化させない
  private SampleQueryProvider()
  {
    // (コード無し)
  }

  public static IQueryable<Sample> CreateQuery(string csvFilePath)
  {
    var p = new SampleQueryProvider()
    {
      _csvFilePath = csvFilePath,
    };
    return new Query<Sample>(p);
  }


  private string _csvFilePath;

  // ファイルを読み取り、Sampleオブジェクトを列挙するメソッド
  private IEnumerable<Sample> ReadSampleCsvFile(string kindFilter)
  {
    // 検証用
    WriteLine("ReadSampleCsvFileメソッド開始");

    bool checkKind = (kindFilter != null);

    // 1行ずつ読み込んでループを回す
    foreach (var line in File.ReadLines(_csvFilePath))
    {
      // 検証用
      WriteLine($"Read: {line}");

      // カンマで分解する
      string[] data = line.Split(',');

      // データのKindをチェックする
      string kind = data[0].Trim();
      if (checkKind && kind != kindFilter)
        continue;

      // データの数値を取得する
      int value = 0;
      int.TryParse(data[1].Trim(), out value);

      // 検証用
      WriteLine($"Create({kind}, {value})");

      // Sampleオブジェクトを生成して返す
      yield return new Sample() { Kind = kind, Value = value, };
    }
  }
}