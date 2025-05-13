using UnityEngine;

public class BulletHoleManager : MonoBehaviour
{
    public static BulletHoleManager Instance;

    [SerializeField] private GameObject bulletHolePrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // called in from the weapons' shoot function, takes in the rays because weapon spread is already decided
    public void SpawnBulletHole(RaycastHit hit)
    {
        if (bulletHolePrefab == null) return; // error prevention

        Vector3 holePosition = hit.point + hit.normal * -0.01f;
        Quaternion holeRotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

        GameObject hole = Instantiate(bulletHolePrefab, holePosition, holeRotation);
        hole.transform.SetParent(hit.collider.transform); // makes the hole a container of whatever it hit
        hole.transform.Rotate(Vector3.forward, Random.Range(0f, 360f)); // random rotation to make the decal look different instead of it being an obviously repeated texture

        //Destroy(hole, bulletTimer); // it goes after that many seconds
        //destroy logic moved to BulletFade.cs 
    }
}
