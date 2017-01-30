
namespace Service.UnitTests.Messages
{
  using System;

  public class MessageBase
  {
    public MessageBase() { }
    public MessageBase(String text)
    {
      this.MessageText = text;
    }

    public String MessageText { get; private set; }

    public virtual Boolean IsSameAs(MessageBase messageBase)
    {
      if (this == messageBase)
      {
        throw new Exception("Same Object");
      }

      return this.GetType() == messageBase.GetType() && 
        String.CompareOrdinal(this.MessageText, messageBase.MessageText) == 0;
    }
  }
}
