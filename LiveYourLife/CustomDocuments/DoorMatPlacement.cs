using UnityEngine;

namespace LiveYourLife.CustomDocuments;

public class DoorMatPlacement(NewAddress address) : IPlacement
{
    public (Vector3? position, string? errorMessage) CreateWorldPosition()
    {
        var node = FindEntranceNode();
        if (node == null)
        {
            return (null, $"no placement node at {address.name}.");
        }

        return (node.position + new Vector3(0f, 0.045f, 0f), null);
    }

    private NewNode? FindEntranceNode()
    {
        var entrance = address.GetMainEntrance();
        if (entrance == null)
            return address.anchorNode;

        if (entrance.fromNode.gameLocation == address)
            return entrance.toNode;

        if (entrance.toNode.gameLocation == address)
            return entrance.fromNode;

        return address.anchorNode;
    }

    public Vector3 CreateWorldRotation()
    {
        return new Vector3(0f, 90f, 0f);
    }
}