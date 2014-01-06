// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Clauses;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.Core.Clauses
{
  [TestFixture]
  public class SelectClauseTest
  {
    private Expression _selector;
    private SelectClause _selectClause;
    private CloneContext _cloneContext;

    [SetUp]
    public void SetUp ()
    {
      _selector = Expression.Constant (new Cook ());
      _selectClause = new SelectClause (_selector);
      _cloneContext = new CloneContext (new QuerySourceMapping());
    }

    [Test]
    public void InitializeWithExpression ()
    {
      Assert.That (_selectClause.Selector, Is.EqualTo (_selector));
    }

    [Test]
    public void Accept()
    {
      var queryModel = ExpressionHelper.CreateQueryModel_Cook ();
      var repository = new MockRepository();
      var visitorMock = repository.StrictMock<IQueryModelVisitor>();
      visitorMock.VisitSelectClause (_selectClause, queryModel);
      repository.ReplayAll();
      _selectClause.Accept (visitorMock, queryModel);
      repository.VerifyAll();
    }

    [Test]
    public void Clone ()
    {
      var clone = _selectClause.Clone (_cloneContext);

      Assert.That (clone, Is.Not.Null);
      Assert.That (clone, Is.Not.SameAs (_selectClause));
      Assert.That (clone.Selector, Is.SameAs (_selectClause.Selector));
    }

    [Test]
    public void TransformExpressions ()
    {
      var oldExpression = ExpressionHelper.CreateExpression ();
      var newExpression = ExpressionHelper.CreateExpression ();
      var clause = new SelectClause (oldExpression);

      clause.TransformExpressions (ex =>
      {
        Assert.That (ex, Is.SameAs (oldExpression));
        return newExpression;
      });

      Assert.That (clause.Selector, Is.SameAs (newExpression));
    }

    [Test]
    public new void ToString ()
    {
      var selectClause = new SelectClause (Expression.Constant (0));
      Assert.That (selectClause.ToString (), Is.EqualTo ("select 0"));
    }

    [Test]
    public void GetOutputDataInfo ()
    {
      var info = _selectClause.GetOutputDataInfo ();
      Assert.That (info.DataType, Is.SameAs (typeof (IQueryable<Cook>)));
      Assert.That (info.ItemExpression, Is.SameAs (_selectClause.Selector));
    }
  }
}
