using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Item
{
    int getType();
    Vector2Int GetPosition();
    void SetPosition(Vector2Int newPosition);
    void Activate(BoardManager boardManager);
}
