using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Tests.Services
{
    [TestClass]
    public class PathFinderServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            // Clear the paths pool before each test to ensure test isolation
            Paths.Instance.Pool.Clear();
        }

        [TestMethod]
        public void FindNextPathToTraverse_WithNoTraversedPaths_ReturnsFirstEligiblePath()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: false);
            var path2 = new TestPath(traverseOrder: 20, canTraverse: true, traversed: false);
            
            Paths.Instance.Pool.Add(path2);
            Paths.Instance.Pool.Add(path1); // Add in reverse order to test ordering

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert
            result.Should().BeSameAs(path1, "path1 has the lowest TraverseOrder");
        }

        [TestMethod]
        public void FindNextPathToTraverse_WithNoEligiblePaths_ReturnsNull()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: false, traversed: false);
            var path2 = new TestPath(traverseOrder: 20, canTraverse: false, traversed: false);
            
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert
            result.Should().BeNull("no paths can traverse the impulse");
        }

        [TestMethod]
        public void FindNextPathToTraverse_SkipsAlreadyTraversedPaths()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: true);
            var path2 = new TestPath(traverseOrder: 20, canTraverse: true, traversed: false);
            
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert
            result.Should().BeSameAs(path2, "path1 was already traversed");
        }

        [TestMethod]
        public void FindNextPathToTraverse_WithTraversedPaths_ReturnsNextPathAfterLastTraversedOrder()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: false);
            var path2 = new TestPath(traverseOrder: 20, canTraverse: true, traversed: false);
            var path3 = new TestPath(traverseOrder: 30, canTraverse: true, traversed: false);
            
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);
            Paths.Instance.Pool.Add(path3);
            
            // Simulate that path1 was already traversed
            impulse.TraversedPaths.Enqueue(path1);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert - path1 is returned because >= comparison includes paths with same order
            // and path1 is first in the ordered list with TraverseOrder >= 10
            result.Should().BeSameAs(path1, "path1 has TraverseOrder >= last traversed (10) and is first eligible");
        }

        [TestMethod]
        public void FindNextPathToTraverse_WithMultipleTraversedPaths_ConsidersLastTraversedPath()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: false);
            var path2 = new TestPath(traverseOrder: 20, canTraverse: true, traversed: false);
            var path3 = new TestPath(traverseOrder: 30, canTraverse: true, traversed: false);
            
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);
            Paths.Instance.Pool.Add(path3);
            
            // Simulate that path1 and path2 were already traversed
            impulse.TraversedPaths.Enqueue(path1);
            impulse.TraversedPaths.Enqueue(path2);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert - path2 is returned because >= comparison and it's the first eligible with TraverseOrder >= 20
            result.Should().BeSameAs(path2, "path2 has TraverseOrder >= last traversed (20) and is first eligible");
        }

        [TestMethod]
        public void FindNextPathToTraverse_OrdersByTraverseOrder_Ascending()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 50, canTraverse: true, traversed: false);
            var path2 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: false);
            var path3 = new TestPath(traverseOrder: 30, canTraverse: true, traversed: false);
            
            // Add in random order
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);
            Paths.Instance.Pool.Add(path3);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert
            result.Should().BeSameAs(path2, "path2 has the lowest TraverseOrder value");
        }

        [TestMethod]
        public void FindNextPathToTraverse_WithSameTraverseOrder_ReturnsFirstMatching()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: false);
            var path2 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: false);
            
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert
            result.Should().BeSameAs(path1, "path1 was added first");
        }

        [TestMethod]
        public void FindNextPathToTraverse_WithEmptyPool_ReturnsNull()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert
            result.Should().BeNull("no paths are available in the pool");
        }

        [TestMethod]
        public void FindNextPathToTraverse_FiltersOutPathsThatCannotTraverse()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: false, traversed: false);
            var path2 = new TestPath(traverseOrder: 20, canTraverse: true, traversed: false);
            var path3 = new TestPath(traverseOrder: 30, canTraverse: false, traversed: false);
            
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);
            Paths.Instance.Pool.Add(path3);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert
            result.Should().BeSameAs(path2, "only path2 can traverse the impulse");
        }

        [TestMethod]
        public void FindNextPathToTraverse_WithTraversedPaths_IncludesPathsWithSameOrder()
        {
            // Arrange
            var pathFinder = new PathFinderService(new ApplicationSettings());
            var impulse = new Impulse();
            
            var path1 = new TestPath(traverseOrder: 10, canTraverse: true, traversed: false);
            var path2 = new TestPath(traverseOrder: 20, canTraverse: true, traversed: false);
            var path3 = new TestPath(traverseOrder: 15, canTraverse: true, traversed: false);
            
            Paths.Instance.Pool.Add(path1);
            Paths.Instance.Pool.Add(path2);
            Paths.Instance.Pool.Add(path3);
            
            // Simulate that path2 (order 20) was traversed
            impulse.TraversedPaths.Enqueue(path2);

            // Act
            var result = pathFinder.FindNextPathToTraverse(impulse);

            // Assert - path2 itself can be returned if it passes CanTraverse and !Traversed filters
            result.Should().BeSameAs(path2, "path2 has TraverseOrder >= 20 and passes all filters");
        }

        /// <summary>
        /// Test implementation of IPath for use in unit tests.
        /// This mock path allows full control over CanTraverse and Traversed behavior.
        /// </summary>
        private class TestPath : IPath
        {
            private readonly bool _canTraverse;
            private readonly bool _traversed;

            public TestPath(int traverseOrder, bool canTraverse, bool traversed)
            {
                TraverseOrder = traverseOrder;
                _canTraverse = canTraverse;
                _traversed = traversed;
                PathName = $"TestPath{traverseOrder}";
            }

            public int TraverseOrder { get; }
            public string PathName { get; }

            public bool CanTraverse(Impulse impulse) => _canTraverse;
            public bool Traversed(Impulse impulse) => _traversed;
            
            public Task Traverse(Impulse impulse) => Task.CompletedTask;
            public Task Emit(Impulse impulse) => Task.CompletedTask;
        }
    }
}
