using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


// 式ツリーの全ノードを処理するクラス

public  class SampleExpressionVisitor : ExpressionVisitor
{
  // 「Where(s => s.Kind == "?")」の"?"に該当する文字列を保持するメンバー
  public string KindFilter { get; private set; }

  // メソッド呼び出しのノードを処理する
  protected override Expression VisitMethodCall(MethodCallExpression expression)
  {
    // 実験的な実装として、
    // Where(s => s.Kind == "?")
    // だけを解釈する

    switch (expression.Method.Name)
    {
      case "Where":
        // Where拡張メソッドのラムダ式
        var lambda = (expression.Arguments[1] as UnaryExpression)
                      ?.Operand as LambdaExpression;
        if (lambda.Parameters.Count > 0
            && lambda.Parameters[0].Type == typeof(Sample))
        // ラムダ式のパラメータ（「=>」の左）がSampleオブジェクトで…
        {
          var bodyExpression = lambda.Body as BinaryExpression;
          if (bodyExpression?.NodeType == ExpressionType.Equal)
          // ラムダ式の本体（「=>」の右）が等価比較で…
          {
            var left = bodyExpression.Left;
            if (left.NodeType == ExpressionType.MemberAccess
                && (left as MemberExpression)?.Member.Name == "Kind")
            // 等価比較の左辺はプロパティでその名前は"Kind"で…
            {
              var right = bodyExpression.Right;
              if (right.NodeType == ExpressionType.Constant)
              // 等価比較の右辺が定数ならば、
              {
                if (KindFilter != null)
                  throw new InvalidOperationException("Whereをチェーンできません");

                // KindFilterにその定数をセットする
                KindFilter = (right as ConstantExpression).Value as string;
              }
            }
          }
        }
        break;

      default:
        throw new NotImplementedException(
          $"{expression.Method.Name}はサポートしていません");
    }
    return base.VisitMethodCall(expression);
  }
}
