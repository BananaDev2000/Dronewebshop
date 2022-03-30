using Dronewebshop.Models;
using MySql.Data.MySqlClient;
namespace Dronewebshop.Persistence

{
    public class PersistenceCode
    {

    string connStr = "server=localhost; user id=root; password=Test123; database=dbwebshop";

        //Het stukje code hieronder wordt gebruikt om de producten in te laden zodra index ook geladen wordt.
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

        //Hier halen we het product op vanop het moment dat er wordt geklikt op een artikel.
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

        // We halen de voorraad op uit de databank en tonen deze.

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

        //Als er op de knop wordt geduwd voeg toe aan winkelmand in de view toevoegen wordt dit stuk code uitgevoerd
        public void voegToe (WinkelmandItem winkelmandItem)
        {
            //We halen het artikelNr en klantNr op
            MySqlConnection conn1 = new MySqlConnection(connStr);
            conn1.Open();
            string qry1 = "select * from tblwinkelmand where ArtNr=" + winkelmandItem.ArtNr + " and KlantNr=" + winkelmandItem.KlantID;
            MySqlCommand cmd1 = new MySqlCommand(qry1, conn1);
            MySqlDataReader dtr1 = cmd1.ExecuteReader();
            if (dtr1.HasRows)
            {
                //Het aantal producten die er in het winkelmandje zitten wordt aangepast in de databank
                MySqlConnection conn2 = new MySqlConnection(connStr);
                conn2.Open();
                string qry = "update tblwinkelmand set Aantal=(Aantal+" + winkelmandItem.Aantal + ") where KlantNr=" + winkelmandItem.KlantID + " and ArtNr=" + winkelmandItem.ArtNr;
                MySqlCommand cmd2 = new MySqlCommand(qry, conn2);
                cmd2.ExecuteNonQuery();
                conn2.Close();

                MySqlConnection conn3 = new MySqlConnection(connStr);
                conn3.Open();
                //De voorraad van het product wordt gewijzigd zodra deze in het winkelmandje gestoken wordt.
                string qry2 = "update tblproducten set Voorraad=(voorraad-" + winkelmandItem.Aantal + ") where ArtNr=" + winkelmandItem.ArtNr;
                MySqlCommand cmd3 = new MySqlCommand(qry2, conn3);
                cmd3.ExecuteNonQuery();
                conn3.Close();
            }
            else
            {
                //Als dit product zich nog niet in het winkemandje bevindt of het winkelmandje is leeg dan wordt deze code uitgevoerd.
                //Hieronder wordt het product toegevoegd.
                MySqlConnection conn2 = new MySqlConnection(connStr);
                conn2.Open();

                string qry = "insert into tblwinkelmand (artnr,klantnr,aantal) values (" + winkelmandItem.ArtNr + "," + winkelmandItem.KlantID + "," + winkelmandItem.Aantal + ")";
                MySqlCommand cmd2 = new MySqlCommand(qry, conn2);

                cmd2.ExecuteNonQuery();
                conn2.Close();
                
                //Hier wordt de voorraad aangepast.
                MySqlConnection conn3 = new MySqlConnection(connStr);
                conn3.Open();

                string qry2 = "update tblproducten set Voorraad=(voorraad-" + winkelmandItem.Aantal + ") where ArtNr=" + winkelmandItem.ArtNr;
                MySqlCommand cmd3 = new MySqlCommand(qry2,conn3);
                cmd3.ExecuteNonQuery();
                conn3.Close();
            }
            conn1.Close();
        }

        //We tonen en vullen het winkelmandje met de informatie die we hierboven hebben gekregen.
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

        //De gebruiker wordt opgehaald zodat de informatie overeen komt voor een specifieke gebruiker.
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

        //Als er op het vuilnisbakje geklikt wordt gaat deze code uitgevoerd worden en ervoor zorgen dat de voorraad terug aangepast wordt en het product uit het winkelmandje verdwijnt.
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

        //We maken het order aan.
        public Order maakOrder(int KlantNr)
        {
            //De datum en het klantNr worden doorgegeven.
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "insert into tblorder (orderdatum,klantnr) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + KlantNr + "')";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

            //Het order krijgt een unieke ID.
            MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            string qry1 = "select max(ordernr) as laatste from tblorder where klantnr=" + KlantNr;
            MySqlCommand cmd1 = new MySqlCommand(qry1, con);
            MySqlDataReader dtr1 = cmd1.ExecuteReader();
             int OrderNr = 0;
            while (dtr1.Read())
            {
                OrderNr = Convert.ToInt32(dtr1["laatste"]);
            }
            con.Close();
            
            //De informatie uit het winkelmandje wordt opgehaald.
            MySqlConnection conn2 = new MySqlConnection(connStr);
            conn2.Open();
            string qry2 = "select tblwinkelmand.ArtNr,Aantal,Prijs from tblwinkelmand inner join tblproducten on tblwinkelmand.ArtNr = tblproducten.ArtNr where klantnr=" + KlantNr;
            MySqlCommand cmd2 = new MySqlCommand(qry2, conn2);
            MySqlDataReader dtr2 = cmd2.ExecuteReader();
            List<WinkelmandItem> lijst = new List<WinkelmandItem>();
            while (dtr2.Read())
            {
                WinkelmandItem wmi = new WinkelmandItem();
                wmi.ArtNr = Convert.ToInt32(dtr2["ArtNr"]);
                wmi.Aantal = Convert.ToInt32(dtr2["Aantal"]);
                wmi.Prijs = Convert.ToDouble(dtr2["Prijs"]); 
                lijst.Add(wmi);
            }
            conn2.Close();

            foreach (var wmi in lijst)
            {
                //Per product in winkelmand wordt de informatie bijgehouden in het order.
                MySqlConnection conn3 = new MySqlConnection(connStr);
                conn3.Open();
                string corrPrijs = wmi.Prijs.ToString();
                corrPrijs = corrPrijs.Replace(",",".");
                string qry3 = "insert into tblorderinfo (ArtNr,OrderNr,Aantal,Prijs) values ('" + wmi.ArtNr + "','" + OrderNr + "','" + wmi.Aantal + "','" + corrPrijs + "')";
                MySqlCommand cmd3 = new MySqlCommand(qry3, conn3);
                cmd3.ExecuteNonQuery();
                conn3.Close();  
            }

            //De totale prijs van de specifieke klant wordt berekend en 
            MySqlConnection conn5 = new MySqlConnection(connStr);
            conn5.Open();
            string qry5 = "select (sum(aantal*prijs)*1.21) as totaal from tblwinkelmand inner join tblproducten on tblwinkelmand.ArtNr = tblproducten.ArtNr where klantNr = " + KlantNr;
            MySqlCommand cmd5 = new MySqlCommand(qry5, conn5);
            MySqlDataReader dtr5 = cmd5.ExecuteReader();
            Order order = new Order();
            while (dtr5.Read())
            {
                order.totaal = Convert.ToDouble(dtr5["totaal"]);
            }
            conn5.Close();
            order.OrderNr = OrderNr;

            //Als er iets besteld is wordt alles uit het winkelmandje verwijdert.
            MySqlConnection conn4 = new MySqlConnection(connStr);
            conn4.Open();
            string qry4 = "delete from tblwinkelmand where klantnr=" + KlantNr;
            MySqlCommand cmd4 = new MySqlCommand(qry4, conn4);
            cmd4.ExecuteNonQuery();
            conn4.Close();

            //Het mailadres van de klant wordt opgehaald uit de databank.
            MySqlConnection conn6 = new MySqlConnection(connStr);
            conn6.Open();
            string qry6 = "select Email from tblklanten where klantnr=" + KlantNr;
            MySqlCommand cmd6 = new MySqlCommand(qry6, conn6);
            MySqlDataReader dtr6 = cmd6.ExecuteReader();
            while (dtr6.Read())
            {
                order.mail = Convert.ToString(dtr6["Email"]);
            }
            conn6.Close();

            //     foreach (var wmi in lijst)
            //  {
            //      Order order = new Order();
            //    order.Aantal = wmi.Aantal;
            //  order.OudePrijs = wmi.Prijs;
            //order.ArtNr = wmi.ArtNr;
            //order.OrderNr = OrderNr;                     
            //}

            return order;
        }

        //De logingegevens worden nagekeken en als deze juist zijn mag de klant naar de webshop verwezen worden.
        public int checkCredentials(LoginCredentials loginCredentials)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(connStr);
            mySqlConnection.Open();
            string sql = "select klantnr from tblklanten where Login ='" + loginCredentials.gebruikersnaam + "' and binary Wachtwoord='" + loginCredentials.wachtwoord + "'";
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            MySqlDataReader dr = mySqlCommand.ExecuteReader();
            int usrID = -1;
            while (dr.Read())
            {
                usrID = Convert.ToInt32(dr["KlantNr"]);
            }
            mySqlConnection.Close();
            return usrID;
        }
    }
}
