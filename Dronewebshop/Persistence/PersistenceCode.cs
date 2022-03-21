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


    }
}
