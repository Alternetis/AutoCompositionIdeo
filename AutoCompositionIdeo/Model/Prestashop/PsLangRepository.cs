using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AutoCompositionIdeo.Model.Prestashop
{
    public class PsLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PrestashopConnection));

        public List<PsLang> ListActive(byte Active, uint IDShop)
        {
            //System.Linq.IQueryable<PsLang> Return = from Table in DBPrestashop.PsLang
            //                                        where Table.Active == Active 
            //                                        select Table;

            //return Return.ToList();

            return DBPrestashop.ExecuteQuery<PsLang>(
                "SELECT DISTINCT L.id_lang, L.name, L.active, L.iso_code, L.language_code, L.date_format_lite, L.date_format_full, L.is_rtl FROM al_lang L " +
                " INNER JOIN al_lang_shop LS ON LS.id_lang = L.id_lang " +
                " WHERE L.active = {0} AND LS.id_shop = {1} " +
                " ", Active, IDShop).ToList();
        }

        public bool ExistId(uint Id)
        {
            return DBPrestashop.PsLang.Any(Obj => Obj.IDLang == Id);
        }

        public PsLang ReadId(uint Id)
        {
            return DBPrestashop.PsLang.FirstOrDefault(Obj => Obj.IDLang == Id);
        }

        public bool ExistIso(string Iso)
        {
            return DBPrestashop.PsLang.Any(Obj => Obj.IsoCode == Iso);
        }

        public PsLang ReadIso(string Iso)
        {
            return DBPrestashop.PsLang.FirstOrDefault(Obj => Obj.IsoCode == Iso);
        }
    }
}
