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



    }
}
