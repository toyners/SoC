
namespace Jabberwocky.SoC.Service.Messages
{
  using System;

  internal class PersonalMessage : GameSessionMessage
  {
    public readonly String Text;

    public PersonalMessage(IClientCallback client, String text) : base(Types.Personal, client)
    {
      this.Text = text;
    }
  }
}
