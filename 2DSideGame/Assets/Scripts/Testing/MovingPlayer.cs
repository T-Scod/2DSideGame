using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlayer : MonoBehaviour
{
    public float speed;

    public TestPlayerProjectile projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 translation = input * speed * Time.deltaTime;
        transform.Translate(translation);

        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePosition - transform.position).normalized;
            TestPlayerProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Fire(direction);
        }
    }
}
