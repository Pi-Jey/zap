using UnityEngine;
using UnityEngine.Events;

public class CollectObjects : MonoBehaviour
{

    public GameObject HintSpace;

    [Header("Events")]
    [Space]

    public UnityEvent OnPickUpKiwiEvent;

    public UnityEvent OnPickUpBananasEvent;

    public UnityEvent OnPickUpPineappleEvent;

    public UnityEvent OnFinishEnterEvent;
    private void Awake()
    {
        HintSpace = GameObject.Find("Canvas");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "AbilityToShoot":
                Destroy(collision.gameObject);
                OnPickUpKiwiEvent.Invoke();
                if (Application.platform == RuntimePlatform.Android)
                {
                    HintSpace.SendMessage("GetHint", "Tap the new button to RangeAttack");
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    HintSpace.SendMessage("GetHint", "Click \"V\" to RangeAttack");
                }
                break;
            case "AbilityToDJump":
                Destroy(collision.gameObject);
                OnPickUpBananasEvent.Invoke();
                if (Application.platform == RuntimePlatform.Android)
                {
                    HintSpace.SendMessage("GetHint", "Tap the new button to DoubleJump");
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    HintSpace.SendMessage("GetHint", "Click \"Z\" to DoubleJump");
                }
                break;
            case "AbilityToDash":
                Destroy(collision.gameObject);
                OnPickUpPineappleEvent.Invoke();
                if (Application.platform == RuntimePlatform.Android)
                {
                    HintSpace.SendMessage("GetHint", "Tap the new button to Dash");
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    HintSpace.SendMessage("GetHint", "Click \"C\" to Dash");
                }
                break;
            case "Finish":
                OnFinishEnterEvent.Invoke();
                break;
        }

    }
}
