using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCompositionIdeo.Model.Prestashop
{
	public class PsProductAttributeRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        private bool ExistInShop(uint IDProductAttribute, uint IDShop)
        {
            return DBPrestashop.PsProductAttributeShop.Any(result => result.IDShop == IDShop && result.IDProductAttribute == IDProductAttribute);
        }

        public bool ExistInShopByReference(string reference)
        {
            return DBPrestashop.PsProductAttribute.Any(result =>  result.Reference == reference);
        }

        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttribute Obj, uint IDShop)
        {
            DBPrestashop.PsProductAttribute.InsertOnSubmit(Obj);
            Save();

            //Si le productattribute n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDProductAttribute, IDShop))
            {
                DBPrestashop.PsProductAttributeShop.InsertOnSubmit(new PsProductAttributeShop()
                {
                    AvailableDate = Obj.AvailableDate,
                    DefaultOn = Obj.DefaultOn,
                    EcOtAx = Obj.EcOtAx,
                    IDProductAttribute = Obj.IDProductAttribute,
                    IDShop = IDShop,
                    MinimalQuantity = Obj.MinimalQuantity,
                    Price = Obj.Price,
                    UnitPriceImpact = Obj.UnitPriceImpact,
                    Weight = Obj.Weight,
                    WholesalePrice = Obj.WholesalePrice,
#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172 || PRESTASHOP_VERSION_177 || PRESTASHOP_VERSION_178 || PRESTASHOP_VERSION_171 || PRESTASHOP_VERSION_80)
                    IDProduct = Obj.IDProduct,
#endif
                });
                DBPrestashop.SubmitChanges();
                new PsProductAttributeShopRepository().WriteDate(Obj.IDProductAttribute);
            }
        }

        public void Delete(PsProductAttribute Obj)
        {
            try
            {
                DBPrestashop.PsProductAttributeCombination.DeleteAllOnSubmit(DBPrestashop.PsProductAttributeCombination.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
                DBPrestashop.PsProductAttributeImage.DeleteAllOnSubmit(DBPrestashop.PsProductAttributeImage.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
                DBPrestashop.PsProductAttributeShop.DeleteAllOnSubmit(DBPrestashop.PsProductAttributeShop.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
                DBPrestashop.PsStockAvailable.DeleteAllOnSubmit(DBPrestashop.PsStockAvailable.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
                new Model.Prestashop.PsSpecificPriceRepository().DeleteFromProductAttribute(Obj.IDProduct, (int)Obj.IDProductAttribute);
                DBPrestashop.PsProductAttribute.DeleteOnSubmit(Obj);
                Save();
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString());
            }
        }

        public void EraseDefault(uint Product, uint ProductAttribute)
        {
            try
            {
                string TxtSQL = "update al_product_attribute "
                     + " set al_product_attribute.default_on = "
#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172 || PRESTASHOP_VERSION_177 || PRESTASHOP_VERSION_178 || PRESTASHOP_VERSION_171 || PRESTASHOP_VERSION_80)
                     + " null " 
#else
					 + " 0 " 
#endif
                     + " where al_product_attribute.id_product = " + Product
                     + " and al_product_attribute.id_product_attribute != " + ProductAttribute;
                DBPrestashop.ExecuteCommand(TxtSQL);

                new PsProductAttributeShopRepository().EraseDefault(Product, ProductAttribute);
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString());
            }
        }

        public bool ExistProductAttribute(uint ProductAttribute)
        {
			return DBPrestashop.PsProductAttribute.Any(Obj => Obj.IDProductAttribute == ProductAttribute);
        }

        public PsProductAttribute ReadProductAttribute(uint ProductAttribute)
        {
            return DBPrestashop.PsProductAttribute.FirstOrDefault(Obj => Obj.IDProductAttribute == ProductAttribute);
        }

        public PsProductAttribute ReadProductAttributeByReference(string reference)
        {
            return DBPrestashop.PsProductAttribute.FirstOrDefault(Obj => Obj.Reference == reference);
        }

        public List<PsProductAttribute> List(uint IdProduct)
        {
			return DBPrestashop.PsProductAttribute.Where(Table => Table.IDProduct == IdProduct).ToList();
        }

        public void WriteDate(uint Product)
        {
            string TxtSQL = "update ignore al_product_attribute "
                 + " set al_product_attribute.available_date = '0000-00-00 00:00:00' "
                 + " where al_product_attribute.id_product = " + Product
                 + " and al_product_attribute.available_date = '0001-01-01 00:00:00' ";
            DBPrestashop.ExecuteCommand(TxtSQL);
        }
    }
}
