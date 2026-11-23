using UnityEngine;

public class DeckController : MonoBehaviour
{
    public GameObject cardPrefab;
    public Material[] allCardMaterials;
    public HandManager handManager;

    private int cardsDrawn = 0;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    IzvuciKartu();
                    break;
                }
            }
        }
    }

    void IzvuciKartu()
    {
        if (allCardMaterials.Length == 0 || cardPrefab == null || handManager == null) return;

        cardsDrawn++;
        Debug.Log("Povučeno karata: " + cardsDrawn);

        GameObject newCard = Instantiate(cardPrefab);
        int randomIndex = Random.Range(0, allCardMaterials.Length);

        CardController cc = newCard.GetComponent<CardController>();
        if (cc != null && cc.faceRenderer != null)
        {
            Material odabrani = allCardMaterials[randomIndex];
            cc.faceRenderer.material = odabrani;

            string ime = odabrani.name.ToLower();

            if (ime.StartsWith("zu")) cc.bojaKarte = "Zuta";
            else if (ime.StartsWith("z")) cc.bojaKarte = "Zelena";
            else if (ime.StartsWith("c")) cc.bojaKarte = "Crvena";
            else if (ime.StartsWith("p")) cc.bojaKarte = "Plava";
            else cc.bojaKarte = "Specijalna"; 
            if (cc.bojaKarte == "Zuta") cc.znakKarte = ime.Substring(2); 
            else if (cc.bojaKarte == "Specijalna") cc.znakKarte = ime;
            else cc.znakKarte = ime.Substring(1); 
            newCard.name = cc.bojaKarte + "_" + cc.znakKarte;
        }

        handManager.AddCard(newCard);
    }
}