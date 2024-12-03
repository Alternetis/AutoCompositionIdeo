using MySql.Data.MySqlClient;
using System.Linq;

namespace AutoCompositionIdeo.Model.Prestashop
{
	public class PsStockAvailableRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }

        public void Add(PsStockAvailable Obj)
        {
            DBPrestashop.PsStockAvailable.InsertOnSubmit(Obj);
            Save();
        }

        // <JG> 19/03/2013 correction maj stock par boutique
        public bool ExistProductAttributeShop(uint Product, uint Attribute, uint Shop)
        {
			return DBPrestashop.PsStockAvailable.Any(Obj => Obj.IDProduct == Product && Obj.IDProductAttribute == Attribute && Obj.IDShop == Shop);
        }

        // <JG> 19/03/2013 correction maj stock par boutique
        public PsStockAvailable ReadProductAttributeShop(uint Product, uint Attribute, uint Shop)
        {
            return DBPrestashop.PsStockAvailable.FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDProductAttribute == Attribute && Obj.IDShop == Shop);
        }
    }
}
