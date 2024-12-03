using AutoCompositionIdeo.Core;
using AutoCompositionIdeo.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using static AutoCompositionIdeo.Program;

namespace AutoCompositionIdeo.Model.Sage
{
    class SageRequeteSQL
    {
        //public static void ListCompostion(List<Composition> Compositions)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(Settings.Default.SageConnection))
        //        {
        //            connection.Open();
        //            string CodeRegroupe = Settings.Default.CodeRegroupement;
        //            string CodeRegroupe2 = Settings.Default.CodeRegroupement2;
        //            string query = $"SELECT DISTINCT {CodeRegroupe} ";
        //            if (!string.IsNullOrEmpty(CodeRegroupe2))
        //            {
        //                query += $", {CodeRegroupe2} "; 
        //            }
        //            query += $" FROM F_ARTICLE WHERE {CodeRegroupe} IS Not Null AND {CodeRegroupe} != '' ";//AND AR_Publie = 1
        //            if (!string.IsNullOrEmpty(CodeRegroupe2))
        //            {
        //                query += $" AND {CodeRegroupe2} IS Not Null AND {CodeRegroupe2} != '' ";
        //            }
        //            //query += $" AND [Produit] = 'COUSSIN' ";//test sur structure
        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        string modele = reader.GetString(1) + " " + reader.GetString(0);
        //                        if (!string.IsNullOrWhiteSpace(modele))
        //                        {
        //                            bool modeleExiste = false;

        //                            foreach (Composition composition in Compositions)
        //                            {
        //                                if (composition.Modele == modele)
        //                                {
        //                                    modeleExiste = true;
        //                                    break;
        //                                }
        //                            }

        //                            if (!modeleExiste)
        //                            {
        //                                Console.WriteLine($"Ajout du Code de regroupement : {modele}");
        //                                Core.Log.WriteLog($"Ajout du Code de regroupement : {modele}");
        //                                Compositions.Add(new Composition
        //                                {
        //                                    Modele = modele,
        //                                    Articles = new List<Article>()
        //                                });
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Erreur Ajout du Code de regroupement
        //        Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
        //        Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
        //    }
        //}

        public static void ListArticleDeclinaisons(List<Article> ListArticle, List<Composition> Compositions)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Settings.Default.SageConnection))
                {
                    connection.Open();
                    string query = "SELECT cbMarq, AR_Ref, AR_Design" +
                                           ", ATT1, ATT2, ATT3, ATT4" +
                                           ", DEC1, DEC2, DEC3, DEC4" +
                                           ", CL_No1, CL_No2, CL_No3, CL_No4" +
                                           ", Groupe, AR_Sommeil " +
                                           " FROM F_ARTICLE WHERE AR_Ref LIKE 'C%'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                string _InfoProduit = reader.IsDBNull(reader.GetOrdinal("AR_Ref")) ? "" : (string)reader["AR_Ref"];
                                _InfoProduit = _InfoProduit.Substring(1);
                                _InfoProduit = _InfoProduit.Split('-')[0];
                                Article article = new Article()
                                {
                                    cbMarq = (int)reader["cbMarq"],
                                    AR_Ref = reader.IsDBNull(reader.GetOrdinal("AR_Ref")) ? "" : (string)reader["AR_Ref"],
                                    AR_Design = reader.IsDBNull(reader.GetOrdinal("AR_Design")) ? "" : (string)reader["AR_Design"],
                                    Att1 = reader.IsDBNull(reader.GetOrdinal("ATT1")) ? "" : (string)reader["ATT1"],
                                    Att2 = reader.IsDBNull(reader.GetOrdinal("ATT2")) ? "" : (string)reader["ATT2"],
                                    Att3 = reader.IsDBNull(reader.GetOrdinal("ATT3")) ? "" : (string)reader["ATT3"],
                                    Att4 = reader.IsDBNull(reader.GetOrdinal("ATT2")) ? "" : (string)reader["ATT4"],
                                    Dec1 = reader.IsDBNull(reader.GetOrdinal("DEC1")) ? "" : (string)reader["DEC1"],
                                    Dec2 = reader.IsDBNull(reader.GetOrdinal("DEC2")) ? "" : (string)reader["DEC2"],
                                    Dec3 = reader.IsDBNull(reader.GetOrdinal("DEC3")) ? "" : (string)reader["DEC3"],
                                    Dec4 = reader.IsDBNull(reader.GetOrdinal("DEC4")) ? "" : (string)reader["DEC4"],
                                    CL_No1 = reader.IsDBNull(reader.GetOrdinal("CL_No1")) ? 0 : (int)reader["CL_No1"],
                                    CL_No2 = reader.IsDBNull(reader.GetOrdinal("CL_No2")) ? 0 : (int)reader["CL_No2"],
                                    CL_No3 = reader.IsDBNull(reader.GetOrdinal("CL_No3")) ? 0 : (int)reader["CL_No3"],
                                    CL_No4 = reader.IsDBNull(reader.GetOrdinal("CL_No4")) ? 0 : (int)reader["CL_No4"],
                                    Active = (short)reader["AR_Sommeil"] == 0 ? true : false,
                                    Groupe = reader.IsDBNull(reader.GetOrdinal("Groupe")) ? "" : (string)reader["Groupe"],
                                    InfoProduit = _InfoProduit
                                };

                                if (!string.IsNullOrEmpty(article.Groupe) && !string.IsNullOrWhiteSpace(article.Att1) && !string.IsNullOrWhiteSpace(article.Dec1)) //déclinaison d'article
                                {
                                    Composition composition = null;
                                    foreach (Composition compo in Compositions)
                                    {
                                        if (compo.Groupe == article.Groupe) //l'article de regroupement existe
                                        {
                                            composition = compo;
                                            break;
                                        }
                                    }

                                    if (composition != null)
                                    {
                                        composition.Articles.Add(article);
                                    }
                                    else
                                    {
                                        composition = new Composition()
                                        {
                                            CL_No = article.CL_No4 != 0 ? article.CL_No4 : article.CL_No3 != 0 ? article.CL_No3 : article.CL_No2 != 0 ? article.CL_No2 : article.CL_No1 != 0 ? article.CL_No1 : 0,
                                            InfoProduit = article.InfoProduit,
                                            Groupe = article.Groupe,
                                            Articles = new List<Article>()
                                        };

                                        composition.Articles.Add(article);
                                        Compositions.Add(composition);
                                    }
                                }
                                else // article simple
                                {
                                    ListArticle.Add(article);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Erreur Ajout du Code de regroupement
                Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - Erreur lors de la récupération des articles : " + ex.ToString());
                Console.WriteLine(DateTime.Now.ToShortDateString() + " - Erreur lors de la récupération des articles : " + ex.ToString());
            }
        }

        //public static void SearchDesign_CLNo1(List<Composition> Compositions)
        //{
        //    try
        //    {
        //        string CodeR = "";
        //        string CodeRegroupement2 = Settings.Default.CodeRegroupement2;
        //        if (!string.IsNullOrEmpty(Settings.Default.CodeRegroupement2))
        //        {
        //            CodeR = Settings.Default.CodeRegroupement2 + " + ' ' + " + Settings.Default.CodeRegroupement;
        //        }
        //        else
        //        {
        //            CodeR = Settings.Default.CodeRegroupement;
        //        }
        //        foreach (var composition in Compositions)
        //        {
        //            using (SqlConnection connection = new SqlConnection(Settings.Default.SageConnection))
        //            {
        //                connection.Open();
        //                string query = $"SELECT TOP 1 AR_DESIGN, Ar_Ref, CL_NO1 ";
        //
        //                if (Settings.Default.CodeRegroupement2 != null)
        //                {
        //                    query += $", {Settings.Default.CodeRegroupement2} ";
        //                }
        //
        //                query += $" FROM F_ARTICLE where {CodeR} = '{Formattage.EscapeSqlString(composition.InfoProduit)}' ";/*AND AR_Publie = 1*/
        //                using (SqlCommand command = new SqlCommand(query, connection))
        //                {
        //                    using (SqlDataReader reader = command.ExecuteReader())
        //                    {
        //                        if (reader.Read())
        //                        {
        //                            Console.WriteLine($"Ajout de la designation pour :{composition.InfoProduit}");
        //                            Core.Log.WriteLog($"Ajout de la designation pour :{composition.InfoProduit}");
        //                            composition.CL_No = (int)reader["CL_NO1"];
        //                            composition.InfoProduit = (string)reader[CodeRegroupement2];
        //                            string Compo = (string)reader["AR_Ref"];
        //                            Compo = Compo.Split('_')[0];
        //                            composition.AR_Compo = Compo;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Erreur Ajout Designation
        //        Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
        //        Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
        //    }
        //}

        //public static void SearchInfoLibreArticle_cbMarq(string infoLibre1, string infoLibre2, string infoLibre2Us, string infoLibre3,
        //    string infoLibre4, string infoIndicateur, string infoLibreMinimum, List<Composition> Compositions)
        //{
        //    try
        //    {
        //        string CodeR = "";
        //        string CodeRegroupement2 = Settings.Default.CodeRegroupement2;
        //        if (!string.IsNullOrEmpty(Settings.Default.CodeRegroupement2))
        //        {
        //            CodeR = Settings.Default.CodeRegroupement2 + " + ' ' + " + Settings.Default.CodeRegroupement;
        //        }
        //        else
        //        {
        //            CodeR = Settings.Default.CodeRegroupement;
        //        }
        //        foreach (var composition in Compositions)
        //        {
        //            if (!string.IsNullOrEmpty(composition.Modele))
        //            {
        //                using (SqlConnection connection = new SqlConnection(Settings.Default.SageConnection))
        //                {
        //                    connection.Open();
        //                    string query = $"SELECT cbMarq, AR_Ref ";
        //
        //                    if (!string.IsNullOrWhiteSpace(Settings.Default.InfoLibre1))
        //                    {
        //                        query += $", [{Settings.Default.InfoLibre1}] ";
        //                    }
        //                    if (!string.IsNullOrWhiteSpace(Settings.Default.InfoLibre2))
        //                    {
        //                        query += $",[{Settings.Default.InfoLibre2}] ";
        //                    }
        //                    if (!string.IsNullOrWhiteSpace(Settings.Default.InfoLibre2Us))
        //                    {
        //                        query += $",[{Settings.Default.InfoLibre2Us}] ";
        //                    }
        //                    if (!string.IsNullOrWhiteSpace(Settings.Default.InfoLibre3))
        //                    {
        //                        query += $",[{Settings.Default.InfoLibre3}] ";
        //                    }
        //                    if (!string.IsNullOrWhiteSpace(Settings.Default.InfoLibre4Color))
        //                    {
        //                        query += $",[{Settings.Default.InfoLibre4Color}] ";
        //                    }
        //                    if (!string.IsNullOrWhiteSpace(Settings.Default.IndicateurActivation))
        //                    {
        //                        query += $",[{Settings.Default.IndicateurActivation}] ";
        //                    }
        //                    if (!string.IsNullOrWhiteSpace(Settings.Default.InfolibreMinimalQuantity))
        //                    {
        //                        query += $",[{Settings.Default.InfolibreMinimalQuantity}] ";
        //                    }
        //
        //                    query += $", AR_Sommeil FROM F_ARTICLE where {CodeR} = '{Formattage.EscapeSqlString(composition.Modele)}'"; // AND AR_Sommeil = 0";// AND AR_Publie = 1
        //
        //                    using (SqlCommand command = new SqlCommand(query, connection))
        //                    {
        //                        Console.WriteLine(command.CommandText);
        //                        Core.Log.WriteLog(command.CommandText);
        //                        using (SqlDataReader reader = command.ExecuteReader())
        //                        {
        //                            while (reader.Read())
        //                            {
        //                                int minimum = 1;
        //                                if (int.TryParse(reader[infoLibreMinimum].ToString(), out int result))
        //                                {
        //                                    minimum = result;
        //                                }
        //
        //                                int _cbMarq = (int)reader["cbMarq"];
        //                                string _AR_Ref = (string)reader["AR_Ref"];
        //                                string _Info1 = !string.IsNullOrEmpty(infoLibre1) ? (string)reader[infoLibre1] : null;
        //                                string _Info2 = !string.IsNullOrEmpty(infoLibre2) ? (string)reader[infoLibre2] : null;
        //                                string _Info2Us = !string.IsNullOrEmpty(infoLibre2Us) ? (string)reader[infoLibre2Us] : null;
        //                                string _Info3 = !string.IsNullOrEmpty(infoLibre3) ? (string)reader[infoLibre3] : null;
        //                                string _Info4 = !string.IsNullOrEmpty(infoLibre4) ? (string)reader[infoLibre4] : null;
        //                                string _InfoIndicateur = !string.IsNullOrEmpty(infoIndicateur) ? (string)reader[infoIndicateur] : null;
        //                                int _InfoLibreMinimum = minimum;
        //                                bool _Active = (short)reader["AR_Sommeil"] == 0 ? true : false;
        //
        //                                Article article = new Article
        //                                {
        //                                    cbMarq = _cbMarq,
        //                                    AR_Ref = _AR_Ref,
        //                                    Info1 = !string.IsNullOrEmpty(_Info1) ? _Info1 : null,
        //                                    Info2 = !string.IsNullOrEmpty(_Info2) ? _Info2 : null,
        //                                    Info2Us = !string.IsNullOrEmpty(_Info2Us) ? _Info2Us : null,
        //                                    Info3 = !string.IsNullOrEmpty(_Info3) ? _Info3 : null,
        //                                    Info4 = !string.IsNullOrEmpty(_Info4) ? _Info4 : null,
        //                                    InfoIndicateur = !string.IsNullOrEmpty(_InfoIndicateur) ? _InfoIndicateur : null,
        //                                    InfoLibreMinimum = minimum,
        //                                    Active = _Active
        //                                };
        //                                composition.Articles.Add(article);
        //                                Console.WriteLine($"Ajout de l'article {article.AR_Ref} pour :{composition.Modele}");
        //                                Core.Log.WriteLog($"Ajout de l'article {article.AR_Ref} pour :{composition.Modele}");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Erreur Ajout Designation
        //        Core.Log.WriteLog(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
        //        Console.WriteLine(DateTime.Now.ToShortDateString() + " - " + ex.ToString());
        //    }
        //}

        //public static int SearchSageId(string connectionSageString, Composition composition)
        //{
        //    int Sag_Id = 0;
        //    string CodeR = "";
        //    if (!string.IsNullOrEmpty(Settings.Default.CodeRegroupement2))
        //    {
        //        CodeR = Settings.Default.CodeRegroupement2 + " + ' ' + " + Settings.Default.CodeRegroupement;
        //    }
        //    else
        //    {
        //        CodeR = Settings.Default.CodeRegroupement;
        //    }
        //    using (SqlConnection connectionSage = new SqlConnection(connectionSageString))
        //    {
        //        connectionSage.Open();
        //        using (SqlCommand commandPrestaconnect = new SqlCommand(string.Format($"Select TOP 1 cbMarq FROM F_ARTICLE " +
        //            $"WHERE {CodeR} = '{Formattage.EscapeSqlString(composition.Modele)}'"), connectionSage))
        //        {
        //            Core.Log.WriteLog(commandPrestaconnect.CommandText);
        //            using (SqlDataReader readerSage = commandPrestaconnect.ExecuteReader())
        //            {
        //                if (readerSage.Read())
        //                {
        //                    Sag_Id = readerSage.GetInt32(0);
        //                }
        //            }
        //        }
        //    }
        //
        //    return Sag_Id;
        //}
    }
}
