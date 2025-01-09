using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RepositionEnemy : MonoBehaviour, IRepositionable
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Area"))
        {
            Reposition();
        }
    }

    public void Reposition()
    {
        Vector3 playerDirection = GameManager.Instance.player.InputVector;
        
        if (_collider.enabled)
        {
            transform.Translate(playerDirection * 40 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
        }
    }
}
