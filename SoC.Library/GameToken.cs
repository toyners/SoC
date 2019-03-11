
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Diagnostics;

  [DebuggerDisplay("id = {id}, creationDateTime = {creationDateTime}")]
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
      {
        return true;
      }

      var other = obj as GameToken;
      if (other == null || this.id != other.id)
      {
        return false;
      }

      return true;
    }

    public override int GetHashCode()
    {
      return this.id.GetHashCode() + this.creationDateTime.GetHashCode();
    }
  }
}
