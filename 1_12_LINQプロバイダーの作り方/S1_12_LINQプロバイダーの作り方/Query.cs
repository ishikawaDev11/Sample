// MSDN Blogs - LINQ: Building an IQueryable provider series
// 第1回：LINQ: Building an IQueryable Provider - Part I
// http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx

// original source code:
// http://iqtoolkit.codeplex.com/SourceControl/latest#Source/IQToolkit/Query.cs
// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class Query<T> : IQueryable<T>, IQueryable,
                        IEnumerable<T>, IEnumerable,
                        IOrderedQueryable<T>, IOrderedQueryable
{
  AbstractQueryProvider provider;
  Expression expression;

  public Query(AbstractQueryProvider provider)
  {
    if (provider == null)
      throw new ArgumentNullException("provider");

    this.provider = provider;
    this.expression = Expression.Constant(this);
  }

  public Query(AbstractQueryProvider provider, Expression expression)
  {
    if (provider == null)
      throw new ArgumentNullException("provider");

    if (expression == null)
      throw new ArgumentNullException("expression");

    if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
      throw new ArgumentOutOfRangeException("expression");

    this.provider = provider;
    this.expression = expression;
  }

  Expression IQueryable.Expression => this.expression;

  Type IQueryable.ElementType => typeof(T);

  IQueryProvider IQueryable.Provider => this.provider; 

  public IEnumerator<T> GetEnumerator()
    => ((IEnumerable<T>)this.provider.Execute(this.expression))
                                     .GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
    => ((IEnumerable)this.provider.Execute(this.expression))
                                  .GetEnumerator();

  public override string ToString()
    => this.provider.GetQueryText(this.expression);
}
