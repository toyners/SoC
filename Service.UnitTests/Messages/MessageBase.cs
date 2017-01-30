
namespace Service.UnitTests.Messages
{
  using System;

  public class MessageBase
  {
    #region Construction
    public MessageBase() { }
    public MessageBase(String text)
    {
      this.MessageText = text;
    }
    #endregion

    #region Properties
    public String MessageText { get; private set; }
    #endregion

    #region Methods
    public virtual Boolean IsSameAs(MessageBase messageBase)
    {
      if (this == messageBase)
      {
        throw new Exception("Same Object");
      }

      return this.GetType() == messageBase.GetType() && 
        String.CompareOrdinal(this.MessageText, messageBase.MessageText) == 0;
    }
    #endregion
  }
}
