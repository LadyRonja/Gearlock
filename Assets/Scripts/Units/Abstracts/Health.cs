using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    public int HealthMax = 5;
    public int HealthCur = 5;

    public Health(int max, int cur)
    {
        HealthMax = Mathf.Max(max, 1);
        HealthCur = Mathf.Max(cur, 0);
    }

    public Health()
    {
        
    }

    public virtual void TakeDamage(int amount)
    {
        HealthCur -= amount;
        if(HealthCur < 0 )
        {
            HealthCur = 0;
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
