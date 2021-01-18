using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace packages
{
    public class Bomberman_Package : Package
    {

        public Package asPackage()
        {
            Package p = new Package(this.XML);
            return p;
        }

        /// <summary>
        /// Wysyłane przez serwer, ustala id mapy
        /// </summary>
        /// <param name="id"></param>
        public void SetTypeMAP_ID(int id)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>MAP_ID</type>";
            this.XML += "<arg1>" + id + "</arg1>";
            this.XML += "</PACKAGE>";
        }
        /// <summary>
        /// Wysyłane przez klienta celem przedstawienia się
        /// </summary>
        /// <param name="name">Nick gracza</param>
        public void SetTypePLAYER_INFO(String name)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>PLAYER_ID</type>";
            this.XML += "<arg1>" + name + "</arg1>";
            this.XML += "</PACKAGE>";
        }
        /// <summary>
        /// Wysyłane przez serwer, informuje o id i nickach gracza
        /// </summary>
        /// <param name="id">Nowe id gracza</param>
        /// <param name="name">Nickname gracza połączone z id</param>
        public void SetTypeSET_PLAYER_ID(int id, String name)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>SET_PLAYER_ID</type>";
            this.XML += "<arg1>" + id + "</arg1>";
            this.XML += "<arg1>" + name + "</arg1>";
            this.XML += "</PACKAGE>";
        }
        /// <summary>
        /// Wysyłane przez serwer i gracza, ustala pozycję gracza na mapie
        /// </summary>
        /// <param name="id">id gracza</param>
        /// <param name="x">pozycja x</param>
        /// <param name="y">pozycja y</param>
        public void SetTypePLAYER_POSITION(int id, float x, float y)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>PLAYER_POSITION</type>";
            this.XML += "<arg1>" + id + "</arg1>";
            this.XML += "<arg2>" + x + "</arg2>";
            this.XML += "<arg3>" + y + "</arg3>";
            this.XML += "</PACKAGE>";
        }
        /// <summary>
        /// Wysyłane przez serwer, informuje o postawieniu bomby z czasem detonacji ttl w milisekundach
        /// </summary>
        /// <param name="x">pozycja x</param>
        /// <param name="y">pozycja y</param>
        /// <param name="ttl">czas do detonacj z ms</param>
        public void SetTypeBOMB_POSITION(int x, int y, int ttl)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>BOMB_POSITION</type>";
            this.XML += "<arg1>" + x + "</arg1>";
            this.XML += "<arg2>" + y + "</arg2>";
            this.XML += "<arg3>" + ttl + "</arg3>";
            this.XML += "</PACKAGE>";
        }
        /// <summary>
        /// Wysyłane przez serwer, przenosi informację o wybuchu bomby
        /// </summary>
        /// <param name="x">pozycja x</param>
        /// <param name="y">pozycja y</param>
        /// <param name="range">zasięg wybuchu</param>
        public void SetTypeBOMB_EXPLOSION(int id, int x, int y, int range)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>BOMB_EXPLOSION</type>";
            this.XML += "<arg1>" + id + "</arg1>";
            this.XML += "<arg2>" + x + "</arg2>";
            this.XML += "<arg3>" + y + "</arg3>";
            this.XML += "<arg4>" + range + "</arg4>";
            this.XML += "</PACKAGE>";
        }

        /// <summary>
        /// Wysyłane przez serwer, informuje o zniszczeniu ściany
        /// </summary>
        /// <param name="x">pozycja x</param>
        /// <param name="y">pozycja y</param>
        public void SetTypeDESTROY_WALL(int x, int y)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>DESTROY_WALL</type>";
            this.XML += "<arg1>" + x + "</arg1>";
            this.XML += "<arg2>" + y + "</arg2>";
            this.XML += "</PACKAGE>";
        }
        /// <summary>
        /// Wysyłane przez serwer informuje o uszkodzeniu ściany
        /// </summary>
        /// <param name="x">pozycja x</param>
        /// <param name="y">pozycja y</param>
        /// <param name="hpLeft">pozostałe punkty życia</param>
        public void SetTypeDAMAGE_WALL(int x, int y, int hpLeft)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>DAMAGE_WALL</type>";
            this.XML += "<arg1>" + x + "</arg1>";
            this.XML += "<arg2>" + y + "</arg2>";
            this.XML += "<arg3>" + hpLeft + "</arg3>";
            this.XML += "</PACKAGE>";
        }
        /// <summary>
        /// Wysyłane przez serwer, informuje o śmierci gracza
        /// </summary>
        /// <param name="id">id gracza</param>
        public void SetTypeDEAD(int id)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>DEAD</type>";
            this.XML += "<arg1>" + id + "</arg1>";
            this.XML += "</PACKAGE>";
        }
        public void SetTypeASSIGN_ID(int id)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>ASSIGN_ID</type>";
            this.XML += "<arg1>" + id + "</arg1>";
            this.XML += "</PACKAGE>";
        }
        public void SetTypeSTART()
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>START</type>";
            this.XML += "</PACKAGE>";
        }
        public void SetTypePLAYER_INFO(int id,String sid)
        {
            this.XML = "<PACKAGE>";
            this.XML += "<type>PLAYER_INFO</type>";
            this.XML += "<arg1>" + id + "</arg1>";
            this.XML += "<arg2>" + sid + "</arg2>";
            this.XML += "</PACKAGE>";
        }
    }
}
