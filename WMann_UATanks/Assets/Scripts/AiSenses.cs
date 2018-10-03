using UnityEngine;

public class AiSenses : MonoBehaviour
{
    AiController aiController;

    // Use this for initialization
    void Start()
    {
        aiController = gameObject.GetComponentInParent<AiController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.ToLower().Contains("player"))
        {
            aiController.TargetWasDetected();
        }
    }
}
