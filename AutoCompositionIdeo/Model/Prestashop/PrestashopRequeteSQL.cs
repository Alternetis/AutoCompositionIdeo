using AutoCompositionIdeo.Core;
using MySql.Data.MySqlClient;
using System;


namespace AutoCompositionIdeo.Model.Prestashop
{
    class PrestashopRequeteSQL
    {
        //private static int LangUs = Properties.Settings.Default.LangUs;

        // Groupe Attribut
        public static void CreationGroupeAttribut(string connectionPrestashopString, string NomAttribut, int isColorGroup = 0)
        {
            try
            {
                string Pp = Properties.Settings.Default.PrefixPrestashop;
                int Position = 0;
                uint IdAttributeGroupe;
                using (MySqlConnection ConnectionPrestashop = new MySqlConnection(connectionPrestashopString))
                {
                    ConnectionPrestashop.Open();
                    // Vérification SI Catégorie Groupe Attribut Existe
                    using (MySqlCommand CommandPrestashop = new MySqlCommand(string.Format("SELECT Count(*) FROM " + Pp + "attribute_group_lang " +
                        "WHERE name ='" + Formattage.EscapeSqlString(NomAttribut) + "' "), ConnectionPrestashop))
                    {
                        MySqlDataReader readerPrestashop = CommandPrestashop.ExecuteReader();

                        readerPrestashop.Read();
                        int Count = readerPrestashop.GetInt32(0);
                        readerPrestashop.Close();

                        if (Count == 0)
                        {
                            // Récupere La position Max
                            using (MySqlCommand RecupererPositionMax = new MySqlCommand(string.Format("SELECT MAX(position) FROM " + Pp + "attribute_group"), ConnectionPrestashop))
                            {
                                MySqlDataReader recupererReader = RecupererPositionMax.ExecuteReader();
                                recupererReader.Read();
                                if (recupererReader.IsDBNull(0) != true)
                                {
                                    Position = recupererReader.GetInt32(0) + 1;
                                }
                                else
                                {
                                    Position = 1;
                                }
                                recupererReader.Close();
                            }
                            
                            // Creation du Groupe Attribut
                            try
                            {
                                CreationG_Attribut(isColorGroup, Pp, Position, ConnectionPrestashop);
                            }
                            catch (Exception ex)
                            {
                                Core.Log.WriteLog(ex.ToString(), false);
                                Console.WriteLine("Erreur lors de la creation du G_Attribut");
                                Console.WriteLine(ex);
                            }

                            // Récupere l'id_Attribute Auto Géneré
                            IdAttributeGroupe = RecupererG_IdAttribute(Pp, Position, ConnectionPrestashop);
                            if (IdAttributeGroupe == 0)
                            {
                                Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + "Impossible de récupérer le groupe d'attribut envoyé, erreur critique!");
                                Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + "Impossible de récupérer le groupe d'attribut envoyé, erreur critique!");
                                throw new Exception("Impossible de récupérer le groupe d'attribut envoyé, erreur critique!");
                            }

                            try
                            {
                                // Creation du Groupe Attribut Langue
                                foreach (Model.Prestashop.PsShop psShop in new Model.Prestashop.PsShopRepository().List())
                                {
                                    // Ajout dans le shop Group Attribut
                                    if (SelectG_Attribute_Shop(Pp, IdAttributeGroupe, ConnectionPrestashop, (int)psShop.IDShop) != true)
                                    {
                                        CreationG_Attribute_Shop(Pp, IdAttributeGroupe, ConnectionPrestashop, (int)psShop.IDShop);
                                    }
                                    foreach (Model.Prestashop.PsLang psLang in new Model.Prestashop.PsLangRepository().ListActive(1, psShop.IDShop))
                                    {
                                        if (SelectG_Attribute_Langue(NomAttribut, Pp, IdAttributeGroupe, ConnectionPrestashop, (int)psLang.IDLang) != true)
                                        {
                                            CreationG_Attribute_Langue(NomAttribut, Pp, IdAttributeGroupe, ConnectionPrestashop, (int)psLang.IDLang);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Core.Log.WriteLog(ex.ToString(), false);
                                Console.WriteLine(ex);
                            }
                        }
                        else
                        {
                            Core.Log.WriteLog("Le group d'attribut " + NomAttribut + " éxiste déjà dans prestashop", false);
                            // Récupere l'id_Attribute Auto Géneré
                            uint IdAttributeGroupeCorr = (uint)RecupererG_IdAttributeFROM_Name(Pp, NomAttribut, connectionPrestashopString);
                            if (IdAttributeGroupeCorr != 0)
                            {
                                try
                                {
                                    // Creation du Groupe Attribut Langue
                                    foreach (Model.Prestashop.PsShop psShop in new Model.Prestashop.PsShopRepository().List())
                                    {
                                        // Ajout dans le shop Group Attribut
                                        if (SelectG_Attribute_Shop(Pp, IdAttributeGroupeCorr, ConnectionPrestashop, (int)psShop.IDShop) != true)
                                        {
                                            CreationG_Attribute_Shop(Pp, IdAttributeGroupeCorr, ConnectionPrestashop, (int)psShop.IDShop);
                                        }
                                        foreach (Model.Prestashop.PsLang psLang in new Model.Prestashop.PsLangRepository().ListActive(1, psShop.IDShop))
                                        {
                                            if (SelectG_Attribute_Langue(NomAttribut, Pp, IdAttributeGroupeCorr, ConnectionPrestashop, (int)psLang.IDLang) != true)
                                            {
                                                CreationG_Attribute_Langue(NomAttribut, Pp, IdAttributeGroupeCorr, ConnectionPrestashop, (int)psLang.IDLang);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Core.Log.WriteLog(ex.ToString(), false);
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(DateTime.Now + " - ERREUR lors de la création du groupe d'attribut dans Prestashop : " + ex.ToString());
                Core.Log.WriteLog(DateTime.Now + " - ERREUR lors de la création du groupe d'attribut dans Prestashop : " + ex.ToString());
            }
        }

        public static void CreationG_Attribut(int isColorGroup, string Pp, int Position, MySqlConnection ConnectionPrestashop)
        {
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand(string.Format("INSERT INTO " + Pp + "attribute_group (`is_color_group`, `group_type`, `position`) " +
                "VALUES(" + isColorGroup + ", 'select'," + Position + ")"), ConnectionPrestashop))
            {
                MySqlDataReader readerPrestashop = CommandPrestashop2.ExecuteReader();
                readerPrestashop.Close();
            }
        }

        public static bool SelectG_Attribute_Shop(string Pp, uint IdAttributeGroupe, MySqlConnection ConnectionPrestashop, int IDShop)
        {
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand(string.Format($@"SELECT * FROM {Pp}attribute_group_shop WHERE id_attribute_group = {IdAttributeGroupe} AND id_shop = {IDShop}"), ConnectionPrestashop))
            {
                using (MySqlDataReader readerSage2 = CommandPrestashop2.ExecuteReader())
                {
                    if (readerSage2.Read())
                    {
                        return true;
                    }
                    readerSage2.Close();
                }
            }
            return false;
        }

        public static bool SelectG_Attribute_Langue(string NomAttribut, string Pp, uint IdAttributeGroupe, MySqlConnection ConnectionPrestashop, int IDLang)
        {
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand(string.Format($@"SELECT * FROM {Pp}attribute_group_lang WHERE id_attribute_group = {IdAttributeGroupe} AND id_lang = {IDLang}"), ConnectionPrestashop))
            {
                using (MySqlDataReader readerSage2 = CommandPrestashop2.ExecuteReader())
                {
                    if (readerSage2.Read())
                    {
                        return true;
                    }
                    readerSage2.Close();
                }
            }
            return false;
        }

        public static void CreationG_Attribute_Shop(string Pp, uint IdAttributeGroupe, MySqlConnection ConnectionPrestashop, int IDShop)
        {
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand($@"INSERT INTO {Pp}attribute_group_shop VALUES({IdAttributeGroupe}, {IDShop})", ConnectionPrestashop))
            {
                MySqlDataReader readerSage2 = CommandPrestashop2.ExecuteReader();
                readerSage2.Close();
            }
        }
        public static void CreationG_Attribute_Langue(string NomAttribut, string Pp, uint IdAttributeGroupe, MySqlConnection ConnectionPrestashop, int IDLang)
        {
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand(string.Format("INSERT INTO " + Pp + "attribute_group_lang " +
                                        "VALUES(" + IdAttributeGroupe + ", " + IDLang + ",'" + NomAttribut + "','" + NomAttribut + "')"), ConnectionPrestashop))
            {
                MySqlDataReader readerSage2 = CommandPrestashop2.ExecuteReader();
                readerSage2.Close();
            }
        }
        public static uint RecupererG_IdAttribute(string Pp, int Position, MySqlConnection ConnectionPrestashop)
        {
            uint IdAttributeGroupe;
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand(string.Format("SELECT id_attribute_group FROM " + Pp + "attribute_group " +
                   "WHERE position = " + Position + " LIMIT 1"), ConnectionPrestashop))
            {
                Core.Log.WriteLog(CommandPrestashop2.CommandText);
                using (MySqlDataReader reader = CommandPrestashop2.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        IdAttributeGroupe = reader.GetUInt32(0);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return IdAttributeGroupe;
        }
        public static int RecupererG_IdAttributeFROM_Name(string Pp, string NomAttributeGroup, string connectionPrestashopString)
        {
            int IdAttributeGroupe = 0;
            using (MySqlConnection ConnectionPrestashop = new MySqlConnection(connectionPrestashopString))
            {
                ConnectionPrestashop.Open();
                using (MySqlCommand CommandPrestashop = new MySqlCommand(string.Format("SELECT id_attribute_group FROM " + Pp + "attribute_group_lang " +
                  "WHERE name = '" + Formattage.EscapeSqlString(NomAttributeGroup) + "' AND id_lang = 1 LIMIT 1"), ConnectionPrestashop))
                {
                    Core.Log.WriteLog("SELECT id_attribute_group FROM " + Pp + "attribute_group_lang " +
                  "WHERE name = '" + Formattage.EscapeSqlString(NomAttributeGroup) + "' AND id_lang = 1 LIMIT 1", false);
                    Console.WriteLine("SELECT id_attribute_group FROM " + Pp + "attribute_group_lang " +
                  "WHERE name = '" + Formattage.EscapeSqlString(NomAttributeGroup) + "' AND id_lang = 1 LIMIT 1");
                    using (MySqlDataReader readerPrestashop = CommandPrestashop.ExecuteReader())
                    {
                        if (readerPrestashop.Read())
                        {
                            IdAttributeGroupe = readerPrestashop.GetInt32(0);
                        }
                    }
                }
                return IdAttributeGroupe;
            }
        }

        // Attribut
        public static void CreationAttribut(string connectionPrestashopString, int IdAttributeGroup, string NomAttribut, int isColor)
        {
            string Pp = Properties.Settings.Default.PrefixPrestashop;
            int Position = 0;
            int IdAttribute = 0;

            using (MySqlConnection ConnectionPrestashop = new MySqlConnection(connectionPrestashopString))
            {
                ConnectionPrestashop.Open();
                using (MySqlCommand CommandPrestashop = new MySqlCommand(string.Format("SELECT Count(*) FROM " + Pp + "attribute_lang " +
                    "INNER JOIN " + Pp + "attribute ON " + Pp + "attribute.id_attribute = " + Pp + "attribute_lang.id_attribute " +
                      "WHERE name = '" + Formattage.EscapeSqlString(NomAttribut) + "' AND id_attribute_group = " + IdAttributeGroup), ConnectionPrestashop))
                {
                    MySqlDataReader readerPrestashop = CommandPrestashop.ExecuteReader();

                    readerPrestashop.Read();
                    int Count = readerPrestashop.GetInt32(0);
                    readerPrestashop.Close();

                    if (Count == 0)
                    {
                        Core.Log.WriteLog($"-> Creation valeur attribut : {NomAttribut}");
                        Console.WriteLine("CreationAttribut");
                        // Récupere La position Max +1
                        using (MySqlCommand RecupererPositionMax = new MySqlCommand(string.Format("SELECT MAX(position) FROM " + Pp + "attribute"), ConnectionPrestashop))
                        {
                            MySqlDataReader recupererReader = RecupererPositionMax.ExecuteReader();
                            recupererReader.Read();
                            if (recupererReader.IsDBNull(0) != true)
                            {
                                Position = recupererReader.GetInt32(0) + 1;
                            }
                            else
                            {
                                Position = 1;
                            }
                            recupererReader.Close();
                        }

                        if (Position != 0 && IdAttributeGroup != 0)
                        {
                            //Création de l'attribut
                            try
                            {
                                Creation_Attribut(IdAttributeGroup, isColor, Pp, Position, ConnectionPrestashop);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                Core.Log.WriteLog(ex.ToString());
                            }

                            try
                            {
                                // Récupere l'id_Attribute Auto Géneré
                                IdAttribute = Recuperer_IdAttribute(Pp, Position, ConnectionPrestashop, IdAttributeGroup);
                                // Creation duAttribut Langue
                                if (IdAttribute != 0)
                                {
                                    // Ajout dans le shopAttribut
                                    foreach (Model.Prestashop.PsShop PsShop in new Model.Prestashop.PsShopRepository().List())
                                    {
                                        if (!Get_Attribute_Shop(IdAttribute, Pp, (int)PsShop.IDShop, ConnectionPrestashop))
                                        {
                                            Console.WriteLine("Ajout dans le shop 1 de l' Attribut " + NomAttribut);
                                            Core.Log.WriteLog("Ajout dans le shop 1 de l' Attribut " + NomAttribut);
                                            Creation_Attribute_Shop(Pp, IdAttribute, ConnectionPrestashop, (int)PsShop.IDShop);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Attribut " + NomAttribut + " déjà insérée dans le shop " + (int)PsShop.IDShop + ".");
                                            Core.Log.WriteLog("Attribut " + NomAttribut + " déjà insérée dans le shop " + (int)PsShop.IDShop + ".");
                                        }
                                    }

                                    // Creation duAttribut Langue
                                    if (!Get_Attribute_Langue(IdAttribute, Pp, 1, ConnectionPrestashop))
                                    {
                                        Console.WriteLine("Création langue FR pour Attribut " + NomAttribut);
                                        Core.Log.WriteLog("Création langue FR pour Attribut " + NomAttribut);
                                        Creation_Attribute_Langue(NomAttribut, Pp, IdAttribute, ConnectionPrestashop, 1);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                Core.Log.WriteLog(ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        Core.Log.WriteLog($"-> La valeur d'attribut : {NomAttribut}, éxiste déjà dans Prestashop.");

                        try
                        {
                            // Récupere l'id_Attribute Auto Géneré
                            IdAttribute = Recuperer_IdAttributeByName(NomAttribut, Pp, connectionPrestashopString, IdAttributeGroup);
                            if (IdAttribute != 0)
                            {
                                // Ajout dans le shopAttribut
                                foreach (Model.Prestashop.PsShop PsShop in new Model.Prestashop.PsShopRepository().List())
                                {
                                    if (!Get_Attribute_Shop(IdAttribute, Pp, (int)PsShop.IDShop, ConnectionPrestashop))
                                    {
                                        Console.WriteLine("Ajout dans le shop " + PsShop.IDShop + " de l' Attribut " + NomAttribut);
                                        Core.Log.WriteLog("Ajout dans le shop " + PsShop.IDShop + " de l' Attribut " + NomAttribut);
                                        Creation_Attribute_Shop(Pp, IdAttribute, ConnectionPrestashop, (int)PsShop.IDShop);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Attribut " + NomAttribut + " déjà insérée dans le shop " + (int)PsShop.IDShop + ".");
                                        Core.Log.WriteLog("Attribut " + NomAttribut + " déjà insérée dans le shop " + (int)PsShop.IDShop + ".");
                                    }
                                }

                                // Creation duAttribut Langue
                                if (!Get_Attribute_Langue(IdAttribute, Pp, 1, ConnectionPrestashop))
                                {
                                    Console.WriteLine("Création langue FR pour Attribut " + NomAttribut);
                                    Core.Log.WriteLog("Création langue FR pour Attribut " + NomAttribut);
                                    Creation_Attribute_Langue(NomAttribut, Pp, IdAttribute, ConnectionPrestashop, 1);
                                }
                                else
                                {
                                    Console.WriteLine("langue FR pour Attribut " + NomAttribut + " déjà insérée.");
                                    Core.Log.WriteLog("langue FR pour Attribut " + NomAttribut + " déjà insérée.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            Core.Log.WriteLog(ex.ToString());
                        }
                    }
                }
            }
        }
        public static void Creation_Attribut(int IdAttributeGroup, int IsColor, string Pp, int Position, MySqlConnection ConnectionPrestashop)
        {
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand(string.Format("INSERT INTO " + Pp + "attribute (`id_attribute_group`, `color`, `position`) " +
                "VALUES(" + IdAttributeGroup + "," + IsColor + "," + Position + ")"), ConnectionPrestashop))
            {
                MySqlDataReader readerSage2 = CommandPrestashop2.ExecuteReader();
                readerSage2.Close();
            }
        }
        public static int Recuperer_IdAttribute(string Pp, int Position, MySqlConnection ConnectionPrestashop, int IdAttributeGroup)
        {
            int IdAttribute = 0;
            using (MySqlCommand CommandPrestashop2 = new MySqlCommand(string.Format("SELECT id_attribute FROM " + Pp + "attribute " +
                   "WHERE position = " + Position.ToString() + " AND id_attribute_group = " + IdAttributeGroup.ToString() + " LIMIT 1"), ConnectionPrestashop))
            {
                Console.WriteLine(CommandPrestashop2.CommandText);
                Core.Log.WriteLog(CommandPrestashop2.CommandText);
                IdAttribute = (int)CommandPrestashop2.ExecuteScalar();
            }

            return IdAttribute;
        }
        public static int Recuperer_IdAttributeByName(string NomAttribut, string Pp, string ConnectionPrestashopString, int IdAttributeGroup)
        {
            int idAttribute = 0;
            using (MySqlConnection ConnectionPrestashop = new MySqlConnection(ConnectionPrestashopString))
            {
                ConnectionPrestashop.Open();
                using (MySqlCommand CommandPrestashop = new MySqlCommand(string.Format("SELECT al.id_attribute FROM " + Pp + "attribute_lang al INNER JOIN " + Pp + "attribute a ON al.id_attribute = a.id_attribute " +
                " WHERE al.name = '" + Formattage.EscapeSqlString(NomAttribut) + "' AND a.id_attribute_group = " + IdAttributeGroup + " LIMIT 1"), ConnectionPrestashop))
                {
                    using (MySqlDataReader recupererReader = CommandPrestashop.ExecuteReader())
                    {
                        Console.WriteLine(CommandPrestashop.CommandText);
                        Core.Log.WriteLog(CommandPrestashop.CommandText);
                        if (recupererReader.Read())
                        {
                            if (recupererReader.IsDBNull(0) != true)
                            {
                                idAttribute = recupererReader.GetInt32(0);
                                Console.WriteLine("idAttribute = " + idAttribute);
                                Core.Log.WriteLog("idAttribute = " + idAttribute);
                            }
                        }
                        recupererReader.Close();
                    }
                }
                return idAttribute;
            }
        }

        public static bool Get_Attribute_Langue(int idAttribute, string Pp, int Lang, MySqlConnection ConnectionPrestashop)
        {
            using (MySqlCommand CommandPrestashop3 = new MySqlCommand(string.Format("SELECT id_attribute FROM " + Pp + "attribute_lang " +
                                        "WHERE id_attribute = " + idAttribute + " AND id_lang = " + Lang + " LIMIT 1"), ConnectionPrestashop))
            {
                Core.Log.WriteLog(CommandPrestashop3.CommandText);
                using (MySqlDataReader reader = CommandPrestashop3.ExecuteReader())
                {
                    int rowCount = 0;
                    while (reader.Read())
                    {
                        rowCount++;
                        // Traitez vos données ici.
                    }
                    Core.Log.WriteLog("Rows = " + rowCount);
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        public static void Creation_Attribute_Langue(string NomAttribut, string Pp, int idAttribute, MySqlConnection ConnectionPrestashop, int IDLang)
        {
            using (MySqlCommand CommandPrestashop4 = new MySqlCommand(string.Format("INSERT INTO " + Pp + "attribute_lang " +
                                        "VALUES(" + idAttribute + ", " + IDLang + ", '" + Formattage.EscapeSqlString(NomAttribut) + "')"), ConnectionPrestashop))
            {
                MySqlDataReader readerPrestashop4 = CommandPrestashop4.ExecuteReader();
                readerPrestashop4.Close();
            }
        }
        //public static void Creation_Attribute_LangueUs(string NomAttribut, string Pp, int idAttribute, MySqlConnection ConnectionPrestashop)
        //{
        //    using (MySqlCommand CommandPrestashop5 = new MySqlCommand(string.Format("INSERT INTO " + Pp + "attribute_lang " +
        //                                "VALUES(" + idAttribute + ", " + LangUs.ToString() + ", '" + Formattage.EscapeSqlString(NomAttribut) + "')"), ConnectionPrestashop))
        //    {
        //        MySqlDataReader readerPrestashop5 = CommandPrestashop5.ExecuteReader();
        //        readerPrestashop5.Close();
        //    }
        //}

        public static bool Get_Attribute_Shop(int idAttribute, string Pp, int Shop, MySqlConnection ConnectionPrestashop)
        {
            try
            {
                using (MySqlCommand CommandPrestashop6 = new MySqlCommand(string.Format("SELECT id_attribute FROM " + Pp + "attribute_shop " +
                                            "WHERE id_attribute = " + idAttribute + " and id_shop = " + Shop), ConnectionPrestashop))
                {
                    using (MySqlDataReader reader = CommandPrestashop6.ExecuteReader())
                    {
                        Core.Log.WriteLog(CommandPrestashop6.CommandText);
                        int rowCount = 0;
                        while (reader.Read())
                        {
                            rowCount++;
                            // Traitez vos données ici.
                        }
                        Core.Log.WriteLog("Rows = " + rowCount);
                        if (reader.HasRows)
                        {
                            reader.Close();
                            return true;
                        }
                        else
                        {
                            reader.Close();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Core.Log.WriteLog(ex.ToString());
                return false;
            }
        }

        public static void Creation_Attribute_Shop(string Pp, int IdAttribute, MySqlConnection ConnectionPrestashop, int IDShop)
        {
            using (MySqlCommand CommandPrestashop7 = new MySqlCommand(string.Format("INSERT INTO " + Pp + "attribute_shop " +
                                        "VALUES(" + IdAttribute + ", " + IDShop + ")"), ConnectionPrestashop))
            {
                MySqlDataReader readerPrestashop7 = CommandPrestashop7.ExecuteReader();
                readerPrestashop7.Close();
            }
        }
        //public static void Creation_Attribute_ShopUs(string Pp, int IdAttribute, MySqlConnection ConnectionPrestashop)
        //{
        //    using (MySqlCommand CommandPrestashop8 = new MySqlCommand(string.Format("INSERT INTO " + Pp + "attribute_shop " +
        //                                "VALUES(" + IdAttribute + ", " + LangUs.ToString() + ")"), ConnectionPrestashop))
        //    {
        //        MySqlDataReader readerPrestashop8 = CommandPrestashop8.ExecuteReader();
        //        readerPrestashop8.Close();
        //    }
        //}

        public static void ActiveArticle(string Pp, string connectionPrestashopString, int Pre_Id, int active)
        {
            using (MySqlConnection connectionPrestashop = new MySqlConnection(connectionPrestashopString))
            {
                connectionPrestashop.Open();
                using (MySqlCommand commandPrestashop = new MySqlCommand(string.Format($@"UPDATE {Pp}product SET active = {active} WHERE id_product = {Pre_Id}"), connectionPrestashop))
                {
                    MySqlDataReader readerPrestashop = commandPrestashop.ExecuteReader();
                    readerPrestashop.Close();
                }
            }
        }

        public static void ActiveArticleShop(string Pp, string connectionPrestashopString, int Pre_Id, int active, int IdShop)
        {
            using (MySqlConnection connectionPrestashop = new MySqlConnection(connectionPrestashopString))
            {
                connectionPrestashop.Open();
                using (MySqlCommand commandPrestashop = new MySqlCommand(string.Format($@"UPDATE {Pp}product_shop SET active = {active} WHERE id_product = {Pre_Id} AND id_shop = {IdShop}"), connectionPrestashop))
                {
                    MySqlDataReader readerPrestashop = commandPrestashop.ExecuteReader();
                    readerPrestashop.Close();
                }
            }
        }

        public static void UpdateMinimalQuantity(string Pp, uint idProductAttribute, int minimum, string ConnectionPrestashopString)
        {
            using (MySqlConnection ConnectionPrestashop = new MySqlConnection(ConnectionPrestashopString))
            {
                ConnectionPrestashop.Open();
                using (MySqlCommand CommandPrestashop9 = new MySqlCommand(string.Format("UPDATE " + Pp + "product_attribute " + " SET minimal_quantity = " + minimum +
                " WHERE id_product_attribute = " + idProductAttribute), ConnectionPrestashop))
                {
                    Core.Log.WriteLog("UPDATE " + Pp + "attribute " + " SET minimal_quantity = " + minimum + " WHERE id_product_attribute = " + idProductAttribute);
                    MySqlDataReader readerPrestashop9 = CommandPrestashop9.ExecuteReader();
                    readerPrestashop9.Close();
                }

                using (MySqlCommand CommandPrestashop10 = new MySqlCommand(string.Format("UPDATE " + Pp + "product_attribute_shop " + " SET minimal_quantity = " + minimum +
                    " WHERE id_product_attribute = " + idProductAttribute), ConnectionPrestashop))
                {
                    Core.Log.WriteLog("UPDATE " + Pp + "attribute_shop " + " SET minimal_quantity = " + minimum +
                    " WHERE id_product_attribute = " + idProductAttribute);
                    MySqlDataReader readerPrestashop10 = CommandPrestashop10.ExecuteReader();
                    readerPrestashop10.Close();
                }
            }
        }
    }
}
