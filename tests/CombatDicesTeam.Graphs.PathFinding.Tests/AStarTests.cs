using Moq;

namespace CombatDicesTeam.Graphs.PathFinding.Tests;

public class AStarTests
{
    [Test]
    public void Run_MinimalForkedGraph_ExpectedPath()
    {
        // ARRANGE
        var graph = new DirectedGraph<TestGraphPayload>();

        var graphNode1 = new GraphNode<TestGraphPayload>(new TestGraphPayload(1));
        graph.AddNode(graphNode1);
        var graphNode2 = new GraphNode<TestGraphPayload>(new TestGraphPayload(2));
        graph.AddNode(graphNode2);
        var graphNode3 = new GraphNode<TestGraphPayload>(new TestGraphPayload(3));
        graph.AddNode(graphNode3);

        graph.ConnectNodes(graphNode1, graphNode2);
        graph.ConnectNodes(graphNode1, graphNode3);

        var contextMock = new Mock<IAStarContext<TestGraphPayload>>();

        contextMock.Setup(x => x.GetDistanceBetween(It.IsAny<IGraphNode<TestGraphPayload>>(),
            It.IsAny<IGraphNode<TestGraphPayload>>())).Returns(1);
        contextMock.Setup(x => x.GetNext(It.IsAny<IGraphNode<TestGraphPayload>>()))
            .Returns<IGraphNode<TestGraphPayload>>(current => graph.GetNext(current));

        var context = contextMock.Object;

        var aStar = new AStar<TestGraphPayload>(context, graphNode1, graphNode2);

        var expectedPath = new[] { graphNode1, graphNode2 };

        // ACT
        var factState = aStar.Run();

        // ASSERT

        factState.Should().Be(State.GoalFound);

        var factPath = aStar.GetPath();
        factPath.Should().BeEquivalentTo(expectedPath);
    }
}
