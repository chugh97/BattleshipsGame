using System;

namespace Battleships.Game
{
    public class Ship
    {
        /// <summary>
        /// Size of Ship
        /// </summary>
        public virtual int Size { get; set; }

        /// <summary>
        /// Unique name of the ship
        /// </summary>
        public virtual Guid ShipName{ get; set; }

        public Ship()
        {
            ShipName = System.Guid.NewGuid();
        }
    }
}