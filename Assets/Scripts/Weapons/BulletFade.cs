using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletFade : MonoBehaviour
{

    [SerializeField]
    private float VisibleDuration = 20;
    [SerializeField]
    private float FadeDuration = 2;

    private DecalProjector _decalProjector;

    private void OnEnable()
    {
        _decalProjector = GetComponent<DecalProjector>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // wait out visible duration
        yield return new WaitForSeconds(VisibleDuration);

        float elapsed = 0;
        float initialFactor = _decalProjector.fadeFactor;

        while (elapsed < 1)
        {
            _decalProjector.fadeFactor = Mathf.Lerp(initialFactor, 0, elapsed);
            // lerps between fade factor and being transparent (0)
            elapsed += Time.deltaTime / FadeDuration;
            // when elapsed = 1, that means that the fade is complete
            yield return null;
        }
        // once faded, delete the prefab
        Destroy(gameObject);
    }
}