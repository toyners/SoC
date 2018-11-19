
namespace Jabberwocky.SoC.Library
{
    using System;

    public class Connection
    {
        #region Fields
        public readonly uint Location1;
        public readonly uint Location2;
        #endregion

        #region Construction
        public Connection(uint location1, uint location2)
        {
            if (location1 == location2)
            {
                throw new ArgumentException("Locations cannot be the same.");
            }

            this.Location1 = location1;
            this.Location2 = location2;
        }
        #endregion

        #region Methods
        public static bool operator ==(Connection connection1, Connection connection2)
        {
            if (Object.ReferenceEquals(connection1, null) && Object.ReferenceEquals(connection2, null))
            {
                return true;
            }

            if (Object.ReferenceEquals(connection1, null) || Object.ReferenceEquals(connection2, null))
            {
                return false;
            }

            if (connection1.Location1 == connection2.Location1 && connection1.Location2 == connection2.Location2)
            {
                return true;
            }

            if (connection1.Location1 == connection2.Location2 && connection1.Location2 == connection2.Location1)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(Connection connection1, Connection connection2)
        {
            return !(connection1 == connection2);
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Connection))
            {
                return false;
            }

            return this == (Connection)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns true if another connection is connected (i.e. shares only one location) to this connection.
        /// If the other connection is the same as this connection then false is returned.
        /// </summary>
        /// <param name="road">Another road segment.</param>
        /// <returns>True if connected; otherwise false.</returns>
        public bool IsConnected(Connection connection)
        {
            if (this == connection)
            {
                return false;
            }

            return this.Location1 == connection.Location1 || this.Location1 == connection.Location2 || this.Location2 == connection.Location1 || this.Location2 == connection.Location2;
        }

        /// <summary>
        /// Returns true if this road segment is on the location passed in.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <returns>True if this road segment is on the location; otherwise false.</returns>
        public bool IsOnLocation(uint location)
        {
            return this.Location1 == location || this.Location2 == location;
        }
        #endregion
    }
}
