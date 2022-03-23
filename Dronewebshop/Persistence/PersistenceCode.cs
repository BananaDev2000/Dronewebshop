using Dronewebshop.Models;
using MySql.Data.MySqlClient;
namespace Dronewebshop.Persistence

{
    public class PersistenceCode
    {

    string connStr = "server=localhost; user id=root; password=Test123; database=dbwebshop";

        public List<Product> loadProducten()
    {
        MySqlConnection conn = new MySqlConnection(connStr);
        conn.Open();
        string qry = "select * from tblproducten";
        MySqlCommand cmd = new MySqlCommand(qry, conn);
        MySqlDataReader dtr = cmd.ExecuteReader();
        List<Product> list = new List<Product>();
        while (dtr.Read())
        {
            Product product = new Product();
            product.ArtNr = Convert.ToInt32(dtr["ArtNr"]);
            product.Naam = Convert.ToString(dtr["Naam"]);
            product.Fotonaam = Convert.ToString(dtr["Foto"]);
            product.Prijs = Convert.ToDouble(dtr["Prijs"]);
            product.Voorraad = Convert.ToInt32(dtr["Voorraad"]);
            list.Add(product);
        }
        conn.Close();
            return list;
    }

    public Product loadProduct(int ArtNr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select * from tblproducten where ArtNr=" + ArtNr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Product product = new Product();
            while (dtr.Read())
            {
                product.ArtNr = Convert.ToInt32(dtr["ArtNr"]);
                product.Naam = Convert.ToString(dtr["Naam"]);
                product.Fotonaam = Convert.ToString(dtr["Foto"]);
                product.Prijs = Convert.ToDouble(dtr["Prijs"]);
                product.Voorraad = Convert.ToInt32(dtr["Voorraad"]);
            }
            conn.Close();
            return product;
        }

        // Winkelmand

        public int haalVoorraad(int ArtNr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select voorraad from tblproducten where ArtNr=" + ArtNr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            int voorraad = 0;

            while (dtr.Read())
            {
                voorraad = Convert.ToInt32(dtr["Voorraad"]);
            }
            conn.Close();
            return voorraad;
        }

        public void voegToe (WinkelmandItem winkelmandItem)
        {
            MySqlConnection conn1 = new MySqlConnection(connStr);
            conn1.Open();
            string qry1 = "select * from tblwinkelmand where ArtNr=" + winkelmandItem.ArtNr + " and KlantNr=" + winkelmandItem.KlantID;
            MySqlCommand cmd1 = new MySqlCommand(qry1, conn1);
            MySqlDataReader dtr1 = cmd1.ExecuteReader();
            if (dtr1.HasRows)
            {
                MySqlConnection conn2 = new MySqlConnection(connStr);
                conn2.Open();
                string qry = "update tblwinkelmand set Aantal=(Aantal+" + winkelmandItem.Aantal + ") where KlantNr=" + winkelmandItem.KlantID + " and ArtNr=" + winkelmandItem.ArtNr;
                MySqlCommand cmd2 = new MySqlCommand(qry, conn2);
                cmd2.ExecuteNonQuery();
                conn2.Close();

                MySqlConnection conn3 = new MySqlConnection(connStr);
                conn3.Open();

                string qry2 = "update tblproducten set Voorraad=(voorraad-" + winkelmandItem.Aantal + ") where ArtNr=" + winkelmandItem.ArtNr;
                MySqlCommand cmd3 = new MySqlCommand(qry2, conn3);
                cmd3.ExecuteNonQuery();
                conn3.Close();
            }
            else
            {
                MySqlConnection conn2 = new MySqlConnection(connStr);
                conn2.Open();

                string qry = "insert into tblwinkelmand (artnr,klantnr,aantal) values (" + winkelmandItem.ArtNr + "," + winkelmandItem.KlantID + "," + winkelmandItem.Aantal + ")";
                MySqlCommand cmd2 = new MySqlCommand(qry, conn2);

                cmd2.ExecuteNonQuery();
                conn2.Close();

                MySqlConnection conn3 = new MySqlConnection(connStr);
                conn3.Open();

                string qry2 = "update tblproducten set Voorraad=(voorraad-" + winkelmandItem.Aantal + ") where ArtNr=" + winkelmandItem.ArtNr;
                MySqlCommand cmd3 = new MySqlCommand(qry2,conn3);
                cmd3.ExecuteNonQuery();
                conn3.Close();
            }
            conn1.Close();
        }

        public List<WinkelmandItem> loadWinkelitems(int KlantNr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "SELECT tblwinkelmand.ArtNr as ArtNr, Naam, KlantNr, Foto, Prijs, Prijs*Aantal as Totaal, Aantal FROM tblwinkelmand INNER JOIN tblproducten ON tblwinkelmand.ArtNr = tblproducten.ArtNr where KlantNr=" + KlantNr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            List<WinkelmandItem> list = new List<WinkelmandItem>();
            while (dtr.Read())
            {
                WinkelmandItem winkelmandItem = new WinkelmandItem();
                winkelmandItem.ArtNr = Convert.ToInt32(dtr["ArtNr"]);
                winkelmandItem.KlantID = Convert.ToInt32(dtr["KlantNr"]);
                winkelmandItem.Aantal = Convert.ToInt32(dtr["Aantal"]);
                winkelmandItem.Prijs = Convert.ToDouble(dtr["Prijs"]);
                winkelmandItem.Naam = Convert.ToString(dtr["Naam"]);
                winkelmandItem.Totaal = Convert.ToDouble(dtr["Totaal"]);
                winkelmandItem.FotoNaam = Convert.ToString(dtr["Foto"]);
                list.Add(winkelmandItem);
            }
            conn.Close();
            return list;
        }

        public Gebruiker haalGebruiker(int KlantNr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select * from tblklanten where KlantNr=" + KlantNr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Gebruiker gebruiker = new Gebruiker();
            while (dtr.Read())
            {
                gebruiker.KlantNr = Convert.ToInt32(dtr["KlantNr"]);
                gebruiker.Naam = Convert.ToString(dtr["Naam"]);
                gebruiker.Voornaam = Convert.ToString(dtr["Voornaam"]);
                gebruiker.Adres = Convert.ToString(dtr["Adres"]);
                gebruiker.Postcode = Convert.ToString(dtr["Postcode"]);
                gebruiker.Gemeente = Convert.ToString(dtr["Gemeente"]);
            }
            conn.Close();
            return gebruiker;
        }

        public void Verwijder (WinkelmandItem winkelmandItem)
        {
            MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            string qry1 = "update tblproducten set Voorraad=(voorraad+" + winkelmandItem.Aantal + ") where ArtNr=" + winkelmandItem.ArtNr;
            MySqlCommand cmd1 = new MySqlCommand(qry1, con);
                cmd1.ExecuteNonQuery();
            con.Close();
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "delete from tblwinkelmand where (ArtNr=" + winkelmandItem.ArtNr + ") and (KlantNr="+ winkelmandItem.KlantID + ")";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
