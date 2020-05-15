
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance = null; //singleton instance

    public GameObject board = null;

    public GameObject ghhhk = null;
    public GameObject grimtaash = null;
    public GameObject houjix = null;
    public GameObject kLorSlug = null;
    public GameObject kintanStrider = null;
    public GameObject mantellianSavrip = null;
    public GameObject monnok = null;
    public GameObject ngok = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
