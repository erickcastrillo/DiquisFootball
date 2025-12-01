using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Diquis.Application.Common.Specification;
using Xunit;

namespace Diquis.Application.Tests.Common.Specification
{
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public NestedEntity Nested { get; set; }
    }

    public class NestedEntity
    {
        public string NestedName { get; set; }
    }

    public class TestSpecification : Specification<TestEntity>
    {
        public TestSpecification(string orderBy)
        {
            Query.OrderBy(orderBy);
        }
    }

    public class ArdalisSpecificationExtensionsTests
    {
        private readonly List<TestEntity> _testData;

        public ArdalisSpecificationExtensionsTests()
        {
            _testData = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "B", Date = DateTime.Now.AddDays(-1), Nested = new NestedEntity { NestedName = "Z" } },
                new TestEntity { Id = 2, Name = "A", Date = DateTime.Now, Nested = new NestedEntity { NestedName = "Y" } },
                new TestEntity { Id = 3, Name = "C", Date = DateTime.Now.AddDays(-2), Nested = new NestedEntity { NestedName = "X" } }
            };
        }

        [Fact]
        public void OrderBy_SingleField_SortsCorrectly()
        {
            // Arrange
            var spec = new Specification<TestEntity>();
            spec.Query.OrderBy("Name");
            var evaluator = new SpecificationEvaluator();

            // Act
            var result = evaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

            // Assert
            Assert.Equal("A", result.First().Name);
            Assert.Equal("C", result.Last().Name);
        }

        [Fact]
        public void OrderBy_SingleFieldDescending_SortsCorrectly()
        {
            // Arrange
            var spec = new Specification<TestEntity>();
            spec.Query.OrderBy("-Name");
            var evaluator = new SpecificationEvaluator();

            // Act
            var result = evaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

            // Assert
            Assert.Equal("C", result.First().Name);
            Assert.Equal("A", result.Last().Name);
        }

        [Fact]
        public void OrderBy_MultipleFields_SortsCorrectly()
        {
            // Arrange
            _testData.Add(new TestEntity { Id = 4, Name = "A", Date = DateTime.Now.AddDays(-1) });
            var spec = new Specification<TestEntity>();
            spec.Query.OrderBy("Name,-Date");
            var evaluator = new SpecificationEvaluator();

            // Act
            var result = evaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

            // Assert
            Assert.Equal("A", result.First().Name);
            Assert.True(result.First().Date > result.Skip(1).First().Date);
        }
        
        [Fact]
        public void OrderBy_NestedProperty_SortsCorrectly()
        {
            // Arrange
            var spec = new Specification<TestEntity>();
            spec.Query.OrderBy("Nested.NestedName");
            var evaluator = new SpecificationEvaluator();

            // Act
            var result = evaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

            // Assert
            Assert.Equal("X", result.First().Nested.NestedName);
            Assert.Equal("Z", result.Last().Nested.NestedName);
        }

        [Fact]
        public void OrderBy_NonExistentProperty_ThrowsArgumentException()
        {
            // Arrange
            var spec = new Specification<TestEntity>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => spec.Query.OrderBy("NonExistentProperty"));
        }

        [Fact]
        public void FindNestedProperty_TopLevel_FindsProperty()
        {
            // Act
            var prop = ArdalisSpecificationExtensions.FindNestedProperty(typeof(TestEntity), "Name");

            // Assert
            Assert.NotNull(prop);
            Assert.Equal("Name", prop.Name);
        }

        [Fact]
        public void FindNestedProperty_Nested_FindsProperty()
        {
            // Act
            var prop = ArdalisSpecificationExtensions.FindNestedProperty(typeof(TestEntity), "Nested.NestedName");

            // Assert
            Assert.NotNull(prop);
            Assert.Equal("NestedName", prop.Name);
        }

        [Fact]
        public void FindNestedProperty_NonExistent_ReturnsNull()
        {
            // Act
            var prop = ArdalisSpecificationExtensions.FindNestedProperty(typeof(TestEntity), "Nested.NonExistent");

            // Assert
            Assert.Null(prop);
        }
    }
}
