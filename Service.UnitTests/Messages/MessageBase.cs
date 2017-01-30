
namespace Service.UnitTests.Messages
{
  using System;

  public class MessageBase
  {
    public String MessageText { get; protected set; }

    public virtual Boolean IsSameAs(MessageBase messageBase)
    {
      if (this.Equals(messageBase))
      {
        throw new Exception("Same Object");
      }

      return (this.GetType() == messageBase.GetType() && String.CompareOrdinal(this.MessageText, messageBase.MessageText) == 0);
    }
  }
}
