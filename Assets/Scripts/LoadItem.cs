using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class LoadItem : MonoBehaviour
{
    public GameObject ItemBoxPrefab;
    public enum LoadItemMode
    {
        Specific,
        RandomFromCategory
    }
    public LoadItemMode Mode;
    [Header("Specific Mode")]
    public ItemObject item;
    [Header("Random From Category Mode")]
    public ItemCategory ItemCategory;
    private GameObject current;
    private ItemObject currentitem;
    bool ListenForE;
    bool Purchased;

    void Start()
    {
        // Specific Mode
        if (Mode == LoadItemMode.Specific)
        {
            LoadItemInfo(item);
        }
        // Random From Category Mode
        if (Mode == LoadItemMode.RandomFromCategory)
        {
            int luckynumber = Random.Range(0, ItemCategory.Items.Length);
            LoadItemInfo(ItemCategory.Items[luckynumber]);
        }
    }
    void LoadItemInfo(ItemObject Item)
    {
        GameObject prefab = Instantiate(ItemBoxPrefab, gameObject.transform);
        prefab.transform.localPosition = new Vector2(0, 11);
        prefab.transform.localScale = Vector3.one;
        current = prefab;
        prefab.GetComponent<Canvas>().sortingLayerName = "Canvas";
        currentitem = Item;

        prefab.transform.Find("ItemSprite").GetComponent<SpriteRenderer>().sprite = Item.ItemImage;
        prefab.transform.Find("ItemSprite").transform.localPosition += Item.WorldOffset;
        prefab.transform.Find("ItemImage").GetComponent<Image>().sprite = Item.ItemImage;
        prefab.transform.Find("ItemImage").transform.localPosition += Item.ImageOffset;
        prefab.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = Item.ItemName;
        prefab.transform.Find("ItemStock").GetComponent<TextMeshProUGUI>().text = "Stock: " + Item.Stock.ToString();
        prefab.transform.Find("ItemCost").GetComponent<TextMeshProUGUI>().text = "Cost: " + Item.ItemCost.ToString();
        if (Item.ItemCost == ItemObject.CostCategory.Low)
        {
            prefab.transform.Find("ItemMoney").GetComponent<TextMeshProUGUI>().text = "$<color=#C0C0C0>$$$$";
        }
        if (Item.ItemCost == ItemObject.CostCategory.Medium)
        {
            prefab.transform.Find("ItemMoney").GetComponent<TextMeshProUGUI>().text = "$$<color=#C0C0C0>$$$";
        }
        if (Item.ItemCost == ItemObject.CostCategory.High)
        {
            prefab.transform.Find("ItemMoney").GetComponent<TextMeshProUGUI>().text = "$$$<color=#C0C0C0>$$";
        }
        if (Item.ItemCost == ItemObject.CostCategory.Expensive)
        {
            prefab.transform.Find("ItemMoney").GetComponent<TextMeshProUGUI>().text = "$$$$<color=#C0C0C0>$";
        }
        if (Item.ItemCost == ItemObject.CostCategory.Priceless)
        {
            prefab.transform.Find("ItemMoney").GetComponent<TextMeshProUGUI>().text = "$<color=#C0C0C0>$$$$";
        }
        prefab.transform.parent.Find("StockCount").GetComponentInChildren<TextMeshProUGUI>().text = item.Stock.ToString();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Circle")
        {
            ListenForE = true;
            current.GetComponent<CanvasGroup>().DOFade(1, 1).SetEase(Ease.OutExpo);
            current.transform.parent.Find("ButtonE").GetComponent<SpriteRenderer>().DOFade(1, 1).SetEase(Ease.OutExpo);
            current.transform.parent.Find("StockCount").transform.DOLocalMove(new Vector3(5, 13.75f, 0), 1).SetEase(Ease.OutExpo);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Circle")
        {
            ListenForE = false;
            current.GetComponent<CanvasGroup>().DOFade(0, 1).SetEase(Ease.OutExpo);
            current.transform.parent.Find("ButtonE").GetComponent<SpriteRenderer>().DOFade(0, 1).SetEase(Ease.OutExpo);
            current.transform.parent.Find("StockCount").transform.DOLocalMove(new Vector3(0.1f, 9.5f, 0), 1).SetEase(Ease.OutExpo);
        }
    }
    private void Update()
    {
        if (ListenForE && !Purchased)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Backspace))
            {
                // Purchase
                GameObject cartitem = Instantiate(new GameObject("CartItem"), GameObject.FindGameObjectWithTag("Circle").transform);
                cartitem.transform.localPosition = new Vector2(-0.0122f, 0.9238f);
                cartitem.transform.localScale = Vector3.one;
                cartitem.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
                cartitem.AddComponent<Image>().sprite = item.ItemImage;
                cartitem.GetComponent<Image>().rectTransform.sizeDelta = Vector2.one;
                cartitem.GetComponent<Image>().preserveAspect = true;
                current.transform.Find("Purchased").GetComponent<CanvasGroup>().alpha = 1;
                current.transform.Find("Purchased").transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                current.transform.Find("Purchased").transform.DOScale(1, 0.5f);
                current.transform.parent.Find("StockCount").Find("Circle").GetComponent<Image>().color = new Color(0.168608f, 0.7509433f, 0.4204296f);
                current.transform.parent.Find("StockCount").Find("Check").GetComponent<Image>().color = Color.white;
                current.transform.parent.Find("StockCount").Find("Stock").GetComponent<TextMeshProUGUI>().text = "";
                item.Stock--;
                GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().ItemsPurchased.Add(currentitem);
                GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().AddScore(20, "Item Purchased");
                GameObject.FindGameObjectWithTag("PurchaseText").transform.parent.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                GameObject.FindGameObjectWithTag("PurchaseText").transform.parent.transform.DOScale(1, 0.5f);
                GameObject.FindGameObjectWithTag("PurchaseText").GetComponent<TextMeshProUGUI>().text = GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().ItemsPurchased.Count + "<size=40> purchased";
                LoadStock();
                HideItemInfo();
                current.transform.parent.Find("StockCount").Find("Stock").GetComponent<TextMeshProUGUI>().text = "";
                Purchased = true;
            }
        }
    }

    public void LoadStock()
    {
        current.transform.Find("ItemStock").GetComponent<TextMeshProUGUI>().text = "Stock: " + item.Stock.ToString();
        current.transform.parent.Find("StockCount").GetComponentInChildren<TextMeshProUGUI>().text = item.Stock.ToString();
    }
    public void HideItemInfo()
    {
        current.transform.Find("ItemImage").GetComponent<Image>().color = Color.clear;
        current.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().color = Color.clear;
        current.transform.Find("ItemStock").GetComponent<TextMeshProUGUI>().color = Color.clear;
        current.transform.Find("ItemCost").GetComponent<TextMeshProUGUI>().text = "";
        current.transform.Find("ItemCost").GetComponent<TextMeshProUGUI>().color = Color.clear;
        current.transform.Find("InfoBox").GetComponent<CanvasGroup>().alpha = 0;
    }
}
