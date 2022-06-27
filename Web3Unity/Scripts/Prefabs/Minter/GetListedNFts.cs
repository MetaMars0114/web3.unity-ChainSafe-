using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetListedNFts : MonoBehaviour
{
    public string chain = "ethereum";
    public Renderer textureObject;
    public string network = "goerli";
    public Text price;
    public Text seller;
    public Text description;
    public Text listPercentage;
    public Text contractAddr;
    public Text tokenId;
    public Text itemId;
    private string _itemPrice = "";
    private string _itemNum;
    // Start is called before the first frame update
    async void Start()
    {
        List<GetNftListModel.Response> response = await EVM.GetNftMarket(chain, network);
        

            print("Price: " + response[0].price);
            price.text = response[0].price;
            print("Seller: " + response[0].seller);
            seller.text = response[0].seller;
            if (response[0].uri.StartsWith("ipfs://"))
            {
                response[0].uri = response[0].uri.Replace("ipfs://", "https://ipfs.io/ipfs/");
            }
            UnityWebRequest webRequest = UnityWebRequest.Get(response[0].uri);
            await webRequest.SendWebRequest();
            RootGetNFT data = JsonConvert.DeserializeObject<RootGetNFT>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));
            print("Description : " + data.description);
            description.text = data.description;
            
            // parse json to get image uri
            string imageUri = data.image;
            if (imageUri.StartsWith("ipfs://"))
            {
                imageUri = imageUri.Replace("ipfs://", "https://ipfs.io/ipfs/");
                Debug.Log("Image" + imageUri);
                StartCoroutine(DownloadImage(imageUri));
            }
            if (data.properties != null)
            {
                print("Properties : " + data.properties);
                foreach (var prop in data.properties.additionalFiles)
                {
                    if (prop.StartsWith("ipfs://"))
                    {
                        var additionalURi = prop.Replace("ipfs://", "https://ipfs.io/ipfs/");
                        Debug.Log("Additional Files: " + additionalURi);
                    }
                }
            }

            print("Listed Percentage: " + response[0].listedPercentage);
            listPercentage.text = response[0].listedPercentage;
            print("Contract: " + response[0].nftContract);
            contractAddr.text = response[0].nftContract;
            print("Token ID: " + response[0].tokenId);
            print("Item ID: " + response[0].itemId);
            itemId.text = response[0].itemId;
            _itemNum = response[0].itemId;
            _itemPrice = response[0].price;
            tokenId.text = response[0].tokenId;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else{
            Texture2D webTexture = ((DownloadHandlerTexture)request.downloadHandler).texture as Texture2D;
            Sprite webSprite = SpriteFromTexture2D(webTexture);
            textureObject.GetComponent<Image>().sprite = webSprite;
   
        }
    }
    Sprite SpriteFromTexture2D(Texture2D texture) {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    
    public async void PurchaseItem()
    {
        Debug.Log("Chain PurchaseItem: " + chain);
        Debug.Log("Network PurchaseItem: " + network);
        Debug.Log("Account PurchaseItem: " + PlayerPrefs.GetString("Account"));
        Debug.Log("ItenID PurchaseItem: " + _itemNum);
        Debug.Log("ItemPrice PurchaseItem: " + _itemPrice);
        BuyNFT.Response response = await EVM.CreatePurchaseNftTransaction(chain, network, PlayerPrefs.GetString("Account"), _itemNum, _itemPrice);
        Debug.Log("Account : " + response.tx.account);
        Debug.Log("To : " + response.tx.to);
        Debug.Log("Value : " + response.tx.value);
        Debug.Log("Data : " + response.tx.data);
        Debug.Log("Gas Price : " + response.tx.gasPrice);
        Debug.Log("Gas Limit : " + response.tx.gasLimit);
        
       try
        {
            string responseNft = await Web3Wallet.SendTransaction("5", response.tx.to, response.tx.value, response.tx.data, response.tx.gasLimit, response.tx.gasPrice);
            print(responseNft);
            Debug.Log(responseNft);
        } catch (Exception e) {
            Debug.LogException(e, this);
        }
    }
}
