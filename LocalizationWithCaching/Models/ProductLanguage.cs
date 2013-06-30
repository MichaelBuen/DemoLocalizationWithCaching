using System;

namespace LocalizationWithCaching.Models
{
    [Serializable]
    public class ProductLanguageCompositeKey
    {
        public virtual int ProductId { get; set; }
        public virtual string LanguageCode { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as ProductLanguageCompositeKey;
            if (t == null)
                return false;
            if (ProductId == t.ProductId && LanguageCode == t.LanguageCode)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return (ProductId + "|" + LanguageCode).GetHashCode();
        }
    }

    public class ProductLanguage
    {

        public virtual ProductLanguageCompositeKey ProductLanguageCompositeKey { get; set; }
        

        public ProductLanguage()
        {
            this.ProductLanguageCompositeKey = new ProductLanguageCompositeKey();
        }

        public virtual string ProductName { get; set; }
        public virtual string ProductDescription { get; set; }

        // A guide for the user, so he/she could know the source language of the untranslated string
        public virtual string ActualLanguageCode { get; set; }
    }
}