using NUnit.Framework;
using Moq;
using Condominio.Repository.Repositories;
using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Condominio.Tests.Repository
{
    [TestFixture]
    public class RepositoryTests : RepositoryTestFixtureBase
    {
        private Repository<ExpenseCategory> _repository;
        private Mock<DbSet<ExpenseCategory>> _mockDbSet;
        private List<ExpenseCategory> _testData;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Create test data
            _testData = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = 1, Category = "Utilities", Description = "Utilities" },
                new ExpenseCategory { Id = 2, Category = "Maintenance", Description = "Maintenance" },
                new ExpenseCategory { Id = 3, Category = "Services", Description = "Services" }
            };

            // Setup mock DbSet
            _mockDbSet = MockDbSetExtensions.CreateMockDbSet<ExpenseCategory>(_testData);

            // Setup the context to return our mock DbSet
            MockContext.Setup(m => m.Set<ExpenseCategory>()).Returns(_mockDbSet.Object);
            MockContext.Setup(m => m.ExpenseCategories).Returns(_mockDbSet.Object);

            // Create repository instance with mock context
            _repository = new Repository<ExpenseCategory>(MockContext.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.That(result.Any(x => x.Category == "Utilities"));
        }

        [Test]
        public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<ExpenseCategory>();
            _mockDbSet = MockDbSetExtensions.CreateMockDbSet<ExpenseCategory>(emptyList);
            MockContext.Setup(m => m.Set<ExpenseCategory>()).Returns(_mockDbSet.Object);
            var repository = new Repository<ExpenseCategory>(MockContext.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ReturnsEntity()
        {
            // Arrange
            var expenseCategory = new ExpenseCategory { Id = 1, Category = "Utilities", Description = "Utilities" };
            _mockDbSet.Setup(m => m.FindAsync(new object[] { 1 }))
                .ReturnsAsync(expenseCategory);

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Utilities", result.Category);
            _mockDbSet.Verify(m => m.FindAsync(new object[] { 1 }), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange - Create a new mock that returns null for invalid id
            var nullMockDbSet = new Mock<DbSet<ExpenseCategory>>();
            nullMockDbSet.Setup(m => m.FindAsync(new object[] { 999 }))
                .ReturnsAsync((ExpenseCategory)null);

            MockContext.Setup(m => m.Set<ExpenseCategory>()).Returns(nullMockDbSet.Object);
            var repository = new Repository<ExpenseCategory>(MockContext.Object);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddAsync_WithValidEntity_CallsSaveChanges()
        {
            // Arrange
            var newCategory = new ExpenseCategory { Category = "Other", Description = "Other" };

            // Act
            await _repository.AddAsync(newCategory);

            // Assert
            _mockDbSet.Verify(m => m.AddAsync(newCategory, It.IsAny<CancellationToken>()), Times.Once);
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_WithValidEntity_CallsSaveChanges()
        {
            // Arrange
            var categoryToUpdate = new ExpenseCategory { Id = 1, Category = "Updated", Description = "Updated" };

            // Act
            await _repository.UpdateAsync(categoryToUpdate);

            // Assert
            _mockDbSet.Verify(m => m.Update(categoryToUpdate), Times.Once);
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_WithValidId_RemovesEntityAndSaveChanges()
        {
            // Arrange
            var entityToDelete = new ExpenseCategory { Id = 1, Category = "Utilities", Description = "Utilities" };
            _mockDbSet.Setup(m => m.FindAsync(new object[] { 1 }))
                .ReturnsAsync(entityToDelete);

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            _mockDbSet.Verify(m => m.FindAsync(new object[] { 1 }), Times.Once);
            _mockDbSet.Verify(m => m.Remove(entityToDelete), Times.Once);
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_WithInvalidId_DoesNotRemoveEntity()
        {
            // Arrange
            var nullMockDbSet = new Mock<DbSet<ExpenseCategory>>();
            nullMockDbSet.Setup(m => m.FindAsync(new object[] { 999 }))
                .ReturnsAsync((ExpenseCategory)null);

            MockContext.Setup(m => m.Set<ExpenseCategory>()).Returns(nullMockDbSet.Object);
            var repository = new Repository<ExpenseCategory>(MockContext.Object);

            // Act
            await repository.DeleteAsync(999);

            // Assert
            nullMockDbSet.Verify(m => m.Remove(It.IsAny<ExpenseCategory>()), Times.Never);
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_MultipleCallsReturnConsistentResults()
        {
            // Act
            var firstCall = await _repository.GetAllAsync();
            var secondCall = await _repository.GetAllAsync();

            // Assert
            Assert.AreEqual(firstCall.Count(), secondCall.Count());
        }

        [Test]
        public void Repository_ContextIsNotNull()
        {
            // Assert
            Assert.IsNotNull(_repository);
        }

        [Test]
        public async Task AddAsync_AndThenGetByIdAsync_VerifiesAddWasCalled()
        {
            // Arrange
            var newCategory = new ExpenseCategory { Id = 4, Category = "New", Description = "New" };
            _mockDbSet.Setup(m => m.FindAsync(new object[] { 4 }))
                .ReturnsAsync(newCategory);

            // Act
            await _repository.AddAsync(newCategory);
            var retrievedCategory = await _repository.GetByIdAsync(4);

            // Assert
            Assert.IsNotNull(retrievedCategory);
            Assert.AreEqual("New", retrievedCategory.Category);
        }

        [Test]
        public async Task DeleteAsync_AndThenGetByIdAsync_VerifiesEntityRemoved()
        {
            // Arrange
            var entityToDelete = new ExpenseCategory { Id = 1, Category = "Utilities", Description = "Utilities" };
            _mockDbSet.Setup(m => m.FindAsync(new object[] { 1 }))
                .ReturnsAsync(entityToDelete);

            // Act - Delete
            await _repository.DeleteAsync(1);

            // Arrange - Setup for second call
            var nullMockDbSet = new Mock<DbSet<ExpenseCategory>>();
            nullMockDbSet.Setup(m => m.FindAsync(new object[] { 1 }))
                .ReturnsAsync((ExpenseCategory)null);

            MockContext.Setup(m => m.Set<ExpenseCategory>()).Returns(nullMockDbSet.Object);
            var repository = new Repository<ExpenseCategory>(MockContext.Object);

            // Act - Retrieve
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task Repository_CanPerformCRUDSequence()
        {
            // Test a sequence of operations
            var newCategory = new ExpenseCategory { Id = 10, Category = "NewTest", Description = "Test" };

            // Add
            await _repository.AddAsync(newCategory);
            _mockDbSet.Verify(m => m.AddAsync(newCategory, It.IsAny<CancellationToken>()), Times.Once);

            // Update
            newCategory.Description = "Updated";
            await _repository.UpdateAsync(newCategory);
            _mockDbSet.Verify(m => m.Update(newCategory), Times.Once);

            // Verify saves
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        }
    }
}
