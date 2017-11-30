
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Diagnostics;

  [DebuggerDisplay("id = {id}, creationDateTime = {creationDateTime}")]
  public class TurnToken
  {
    private readonly Guid id;
    private readonly DateTime creationDateTime;

    public TurnToken()
    {
      this.id = new Guid();
      this.creationDateTime = DateTime.Now;
    }

    public override Boolean Equals(Object obj)
    {
      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      var other = obj as TurnToken;
      if (other == null)
      {
        return false;
      }

      if (this.id == other.id && this.creationDateTime == other.creationDateTime)
      {
        return true;
      }

      return false;
    }

    public override Int32 GetHashCode()
    {
      return this.id.GetHashCode() + this.creationDateTime.GetHashCode();
    }
  }
}
