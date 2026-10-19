using UnityEngine;

namespace LiveYourLife.CustomDocuments;

public interface IPlacement
{
    (Vector3? position, string? errorMessage) CreateWorldPosition();
    Vector3 CreateWorldRotation();
}