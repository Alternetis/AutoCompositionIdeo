using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AutoCompositionIdeo.Model.Prestashop
{
	public class PsShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        public List<PsShop> List()
        {
			return DBPrestashop.PsShop.ToList();
        }
        
        public int Count()
        {
            return DBPrestashop.PsShop.Count();
        }
    }
}

