
namespace Jabberwocky.SoC.Library
{
    using System;

    public class GameToken
    {
        private readonly Guid id;
        private readonly DateTime creationDateTime;

        public GameToken()
        {
            this.id = Guid.NewGuid();
            this.creationDateTime = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (!(obj is GameToken other) || this.id != other.id)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode() + this.creationDateTime.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{this.id}, {this.creationDateTime}]";
        }
    }
}
