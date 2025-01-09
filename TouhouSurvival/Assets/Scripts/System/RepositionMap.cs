using System;
using UnityEngine;

public class RepositionMap : MonoBehaviour, IRepositionable
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Area"))
        {
            Reposition();
        }
    }

    public void Reposition()
    {
        Vector3 playerPosition = GameManager.Instance.player.transform.position;
        float diffX = Mathf.Abs(playerPosition.x - transform.position.x);
        float diffY = Mathf.Abs(playerPosition.y - transform.position.y);

        Vector3 playerDirection = GameManager.Instance.player.InputVector;
        float dirX = playerDirection.x < 0 ? -1 : 1;
        float dirY = playerDirection.y < 0 ? -1 : 1;
        
        if (diffX > diffY)
        {
            transform.Translate(Vector3.right * dirX * 80);
        }
        else if (diffX < diffY)
        {
            transform.Translate(Vector3.up * dirY * 80);
        }
    }
}
