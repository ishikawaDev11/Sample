// MSDN Blogs - LINQ: Building an IQueryable provider series
// 第1回：LINQ: Building an IQueryable Provider - Part I
// http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx

// original source code:
// http://iqtoolkit.codeplex.com/SourceControl/latest#Source/IQToolkit/QueryProvider.cs
// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)


using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public abstract class AbstractQueryProvider : IQueryProvider
{
  protected AbstractQueryProvider()
  {
    // (コード無し)
  }

  // IQueryProviderの実装：ここから

  IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
     => new Query<T>(this, expression);

  IQueryable IQueryProvider.CreateQuery(Expression expression)
  {
    Type elementType = TypeSystem.GetElementType(expression.Type);
    try
    {
      return (IQueryable)Activator.CreateInstance(typeof(Query<>)
              .MakeGenericType(
                elementType),
                new object[] { this, expression });
    }
    catch (TargetInvocationException tie)
    {
      throw tie.InnerException;
    }
  }

  T IQueryProvider.Execute<T>(Expression expression)
    => (T)this.Execute(expression);
  object IQueryProvider.Execute(Expression expression)
    => this.Execute(expression);

  // IQueryProviderの実装：ここまで


  // 実際に使うクラスで実装すべきメソッド
  // LINQプロバイダを実装するには、
  // この抽象クラスを継承し、
  // 次のExecute／GetQueryTextメソッドを実装する
  public abstract string GetQueryText(Expression expression);
  public abstract object Execute(Expression expression);
}
