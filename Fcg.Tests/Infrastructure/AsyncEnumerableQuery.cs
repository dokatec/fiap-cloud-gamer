// MockDbSetExtensions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fcg.Infrastructure.Tests.Mocks
{
    // Classe para simular IAsyncEnumerable
    public class AsyncEnumerableQuery<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerableQuery(IEnumerable<T> enumerable) : base(enumerable) { }
        public AsyncEnumerableQuery(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumeratorWrapper<T>(this.AsEnumerable().GetEnumerator());
        }
    }

    // Classe para envolver IEnumerator em IAsyncEnumerator
    internal class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumeratorWrapper(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }
    }

    // Classe para simular IQueryProvider com IAsyncQueryProvider
    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerableQuery<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerableQuery<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var enumerationResult = Execute<IQueryable<TEntity>>(expression).GetEnumerator();

            // Para simular FirstOrDefaultAsync, ToListAsync etc.
            if (expression.ToString().Contains(".FirstOrDefaultAsync("))
            {
                return (TResult)(object)Task.FromResult(enumerationResult.MoveNext() ? enumerationResult.Current : default(TEntity));
            }
            else if (expression.ToString().Contains(".ToListAsync("))
            {
                var listResult = ((IEnumerable<TEntity>)_inner.Execute(expression)).ToList();
                return (TResult)(object)Task.FromResult(listResult);
            }
            // Adicione mais simulações conforme necessário para outros métodos assíncronos
            // Este é um exemplo simplificado, pode precisar de mais refinamento dependendo do LINQ complexo.
            throw new NotSupportedException($"Only FirstOrDefaultAsync and ToListAsync are currently supported for mocking. Expression: {expression}");
        }
    }

    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            // Configurações básicas para IQueryable
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumeratorWrapper<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider)); // Usa o provedor assíncrono

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        // Sobrecarga para facilitar quando os dados já são uma lista
        public static Mock<DbSet<T>> BuildMockDbSet<T>(this List<T> data) where T : class
        {
            return data.AsQueryable().BuildMockDbSet();
        }
    }
}