
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

    public GameObject Instantiate(PieceType type)
    {
        GameObject src = null;
        switch (type)
        {
        case PieceType.Ghhhk:
            src = ghhhk;
            break;
        case PieceType.Grimtaash:
            src = grimtaash;
            break;
        case PieceType.Houjix:
            src = houjix;
            break;
        case PieceType.KintanStrider:
            src = kintanStrider;
            break;
        case PieceType.KLorSlug:
            src = kLorSlug;
            break;
        case PieceType.MantellianSavrip:
            src = ghhhk;
            break;
        case PieceType.Monnok:
            src = monnok;
            break;
        case PieceType.NgOk:
            src = ngok;
            break;
        }

        var dst = GameObject.Instantiate(src);
        return dst;
    }
}
