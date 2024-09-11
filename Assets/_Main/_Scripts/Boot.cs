using System.Collections;
using _Main._Scripts.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    private SavesService _savesService;
    // private bool _billingSuccses;
    private IEnumerator Start()
    {
        // yield return YandexGamesSdk.Initialize();
        // yield return Cloud.Initialize();
        // Advertisement.Initialize();
        // WebApplication.Initialize(null);

        InitSavesService();

        // yield return Billing.Initialize();
        // yield return Consume();
        //
        // SetLanguage();
        // Advertisement.ShowInterstitialAd();
        LoadScene();
        yield return null;
    }

    private void InitSavesService()
    {
        _savesService = new SavesService();
        ServiceLocator.Instance.TryAddService(_savesService);
    }
    
    // private IEnumerator Consume()
    // {
    //     Billing.GetPurchasedProducts(UpdateProductCatalog);
    //     yield return new WaitUntil(() => _billingSuccses);
    // }
    //
    // private void UpdateProductCatalog(GetPurchasedProductsResponse response)
    // {
    //     _billingSuccses = true;
    //     PurchasedProduct[] purchaseProducts = response.purchasedProducts;
    //
    //     var countProducts = purchaseProducts.Length;
    //     for (var i = 0; i < countProducts; i++)
    //     {
    //         var product = purchaseProducts[i];
    //         if (product.productID.Equals(PurchaseIndexes.NoAD.ToString()))
    //             _savesService.Saves.BuyNoAd();
    //
    //
    //         Billing.ConsumeProduct(product.purchaseToken);
    //     }
    // }

//     private void SetLanguage()
//     {
// #if   UNITY_EDITOR
//         string lang = "ru";
// #endif
// #if   !UNITY_EDITOR
//              string lang = YandexGamesSdk.Environment.i18n.lang;
// #endif
//
//         if (Localization.SetLocalization(lang))
//         {
//             LocalizationTextBase.ApplyLocalizationDictionary();
//         }
//     }

    private void LoadScene() => SceneManager.LoadScene(sceneBuildIndex: 1);


    internal enum PurchaseIndexes
    {
        NoAD
    }
}
