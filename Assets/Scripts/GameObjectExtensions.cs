using UnityEngine;

public static class GameObjectExtensions
{
    public static void ApplyCorrectMaterial(this GameObject gameObject)
    {
        GameManager.Instance.ColorObject(gameObject, GameManager.Instance.CorrectMaterial);
    }

    public static void ApplyIncorrectMaterial(this GameObject gameObject)
    {
        GameManager.Instance.ColorObject(gameObject, GameManager.Instance.IncorrectMaterial);
    }

    public static void ApplyPathMaterial(this GameObject gameObject)
    {
        GameManager.Instance.ColorObject(gameObject, GameManager.Instance.PathMaterial);
    }

    public static void ApplyOriginalMaterial(this GameObject gameObject)
    {
        Material originalMaterial = null;

        // Determine the original material based on the object's variant
        if (gameObject.transform.parent.parent.name.ToLower().Contains("city"))
        {
            if(gameObject.tag == "Lite"){
                originalMaterial = GameManager.Instance.CityLite;
            }else 
            originalMaterial = GameManager.Instance.CityMaterial;
        }
        else if (gameObject.transform.parent.parent.name.ToLower().Contains("base"))
        {
            if(gameObject.tag == "Lite"){
                originalMaterial = GameManager.Instance.BaseLite;
            } else
            originalMaterial = GameManager.Instance.BaseMaterial;
        }
        else if (gameObject.transform.parent.parent.name.ToLower().Contains("minefield"))
        {
            if(gameObject.tag == "Dark"){
                originalMaterial = GameManager.Instance.MinefieldDark;
            } else 
            originalMaterial = GameManager.Instance.MinefieldMaterial;
        }
        else if (gameObject.transform.parent.parent.name.ToLower().Contains("safe"))
        {
            if(gameObject.tag == "Dark"){
                originalMaterial = GameManager.Instance.SafeDark;
            }else 
            originalMaterial = GameManager.Instance.SafeMaterial;
        }

        if (originalMaterial != null)
        {
            // Apply the original material using the ColorObject method
            GameManager.Instance.ColorObject(gameObject, originalMaterial);
        }
    }

    public static void TakeDamage(this GameObject gameObject, float damage) {
        Health health = gameObject.GetComponent<Health>();
        if (health) {
            health.RecieveDamage(damage);
        }
    }

}
