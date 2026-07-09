using Condominio.Data.Mysql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Condominio.Tests.Repository
{
    /// <summary>
    /// Helper class for setting up mock DbSet<T> objects for repository testing
    /// </summary>
    public static class MockDbSetExtensions
    {
        /// <summary>
        /// Creates a mock DbSet<T> with queryable and async enumerable support
        /// </summary>
        public static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var asyncEnumerable = data.ToAsyncEnumerable();

            var mockDbSet = new Mock<DbSet<T>>();

            // Setup IQueryable members to support LINQ queries
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            // Setup IAsyncEnumerable to support async enumeration
            mockDbSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(asyncEnumerable.GetAsyncEnumerator);

            // Setup FindAsync - returns ValueTask<T> as required by EF Core
            mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(ids =>
                {
                    var id = ids.FirstOrDefault();
                    T result = id == null ? null : queryableData.FirstOrDefault();
                    return new ValueTask<T>(result);
                });

            // Setup AddAsync - returns ValueTask<EntityEntry<T>>
            mockDbSet.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityEntry<T>)null);

            // Setup Update and Remove as callbacks
            mockDbSet.Setup(m => m.Update(It.IsAny<T>())).Callback<T>(entity => { });
            mockDbSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>(entity => { });

            return mockDbSet;
        }

        /// <summary>
        /// Creates a mock CondominioContext with mocked DbSets
        /// </summary>
        public static Mock<CondominioContext> CreateMockContext()
        {
            var mockContext = new Mock<CondominioContext>();

            // Setup SaveChangesAsync to return 1 by default
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            return mockContext;
        }
    }

    /// <summary>
    /// Base test fixture for repository tests
    /// </summary>
    public class RepositoryTestFixtureBase
    {
        protected Mock<CondominioContext> MockContext { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            MockContext = MockDbSetExtensions.CreateMockContext();
        }

        /// <summary>
        /// Helper to setup mock DbSet on the mock context
        /// </summary>
        protected void SetupMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var mockDbSet = MockDbSetExtensions.CreateMockDbSet(data);
            MockContext.Setup(m => m.Set<T>()).Returns(mockDbSet.Object);
        }
    }
}
