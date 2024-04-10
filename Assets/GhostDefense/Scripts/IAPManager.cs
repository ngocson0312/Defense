using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace UDEV.GhostDefense
{
    [System.Serializable]
    public class IAPItem
    {
        public ProductType productType;
        public string id;
        public int amount;
        public float localPrice;
        public string fbLogEvent;
    }

    public class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
    {
        public string noadsId;
        public float noadsLocalPrice;
        public List<IAPItem> items;
        private ConfigurationBuilder m_builder;

        public IStoreController controller { get; private set; }
        public IExtensionProvider extensions { get; private set; }

        public UnityEvent OnProccessing;
        public UnityEvent OnAdsBuying;
        public UnityEvent<IAPItem> OnItemBuying;
        public UnityEvent<string> OnBuyingFailed;

        private void Start()
        {
            if (items == null) return;
            m_builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            m_builder.AddProduct(noadsId, ProductType.NonConsumable);
            foreach (var item in items)
            {
                m_builder.AddProduct(item.id, item.productType);
            }

            UnityPurchasing.Initialize(this, m_builder);
        }

        public IAPManager()
        {

        }

        public void OnInitialized(IStoreController storeController, IExtensionProvider extensionProvider)
        {
            controller = storeController;
            extensions = extensionProvider;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            var purchasedProductId = e.purchasedProduct.definition.id;

            if (IsNoAdsItem(purchasedProductId))
            {
                OnAdsBuying?.Invoke();
            }
            else
            {
                var buyingItem = items.Find(x => x.id == purchasedProductId);
                OnItemBuying?.Invoke(buyingItem);
            }

            return PurchaseProcessingResult.Complete;
        }

        private bool IsNoAdsItem(string productId)
        {
            return string.Compare(productId, noadsId) == 0;
        }

        public void MakeItemBuying(string itemId)
        {
            if (controller == null) return;

            OnProccessing?.Invoke();

            controller.InitiatePurchase(itemId);
        }

        public string GetPriceString(IAPItem item, string currencySymbol = "$")
        {
            var product = controller.products.WithID(item.id);
            if (product == null) return "";
            if (product.metadata.localizedPrice <= new decimal(0.01))
            {
                return currencySymbol + item.localPrice.ToString();
            }
            return product.metadata.localizedPriceString;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            string reasonMessage = GetPurchaseFailureReasonString(reason);
            OnBuyingFailed?.Invoke(reasonMessage);
        }

        private string GetPurchaseFailureReasonString(PurchaseFailureReason reason)
        {
            switch (reason)
            {
                case PurchaseFailureReason.DuplicateTransaction:
                    return "Duplicate transaction.";

                case PurchaseFailureReason.ExistingPurchasePending:
                    return "Existing purchase pending.";

                case PurchaseFailureReason.PaymentDeclined:
                    return "Payment was declined.";

                case PurchaseFailureReason.ProductUnavailable:
                    return "Product is not available.";

                case PurchaseFailureReason.PurchasingUnavailable:
                    return "Purchasing is not available.";

                case PurchaseFailureReason.SignatureInvalid:
                    return "Invalid signature.";

                case PurchaseFailureReason.Unknown:
                    return "Unknown error.";

                case PurchaseFailureReason.UserCancelled:
                    return "User cancelled.";

            }

            return "Unknown error.";
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {

        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {

        }
    }
}
