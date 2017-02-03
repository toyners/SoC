
namespace Service.UnitTests.Messages
{
  using System;

  public class PersonalMessage : MessageBase
  {
    #region Construction
    public PersonalMessage(String sender, String text) : base(text)
    {
      this.Sender = sender;
    }
    #endregion

    #region Properties
    public String Sender { get; private set; }
    #endregion

    #region Methods
    public override Boolean IsSameAs(MessageBase messageBase)
    {
      if (!base.IsSameAs(messageBase))
      {
        return false;
      }

      return String.CompareOrdinal(this.Sender, ((PersonalMessage)messageBase).Sender) == 0;
    }
    #endregion
  }
}
