using UnityEngine;

// Small component to attach to player attack prefabs to carry damage value
public class AttackData : MonoBehaviour
{
    public int damage = 10;
    // marcar si el ataque pertenece a un enemigo (true) o al jugador (false)
    public bool isEnemy = false;
}
