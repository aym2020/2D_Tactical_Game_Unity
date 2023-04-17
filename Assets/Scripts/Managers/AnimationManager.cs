using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    [SerializeField] private Transform damagePopupPrefab;
    [SerializeField] private Transform spellAnimationPrefab;


    public void Start()
    {
        // CreateDamagePopup(new Vector3(0, 0, 0), 9999);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            bool isCriticalHit = Random.Range(0, 100) < 30;
            // CreateDamagePopup(GetMouseWorldPosition(), 9999, isCriticalHit);
        }
    }

    public void CreateDamagePopup(Vector3 position, int damageAmount, bool isCriticalHit)
    {
        DamagePopup.Create(damagePopupPrefab, position, damageAmount, isCriticalHit);
    }

    private Vector3 GetMouseWorldPosition() 
    {
        Vector3 mouseWorldPosition = GridManager.Instance._mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        return mouseWorldPosition;
    }

    // Animation for when a spell is cast
    public void CreateSpellAnimation(Vector3 position)
    {
        Transform spellAnimationTransform = Instantiate(spellAnimationPrefab, position, Quaternion.identity);
    }

}
