using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public float chechRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI prompText;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        //True, joka "checkRate" sekunti
        if (Time.time - lastCheckTime > chechRate)
        {
            lastCheckTime = Time.time;

            //Luo näytön keskeltä säde, joka osoittaa katselusuuntaan
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            //Tallentaa raycast-hitin
            RaycastHit hit;

            //Osuimmeko johonkin?
            if(Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                //Tarkista, onko tämä meidän nykyinen vuorovaikutteinen?
                //Jos on, aseta se nykyiseksi vuorovaikutteiseksi
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }//Jos ei osunut yhtään mihinkään
            else
            {
                curInteractGameObject = null;
                curInteractable = null;

                //Poista prompText käytöstä
                prompText.gameObject.SetActive(false);
            }

        }
    }

    void SetPromptText()
    {
        prompText.gameObject.SetActive(true);
        prompText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt());
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            prompText.gameObject.SetActive(false);
        }
    }
}

public interface IInteractable
{
    //string funktio, joka saa mukautetun vuorovaikutusviestin näkyviin, kun hiiren osoitin viedään kohteen päälle
    string GetInteractPrompt();

    //OnInteract kutsutaan, kun olemme vuorovaikutuksessa kohteen kanssa
    void OnInteract();
}
