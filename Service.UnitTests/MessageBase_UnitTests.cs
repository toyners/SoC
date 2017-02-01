
namespace Service.UnitTests
{
  using System;
  using Messages;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class MessageBase_UnitTests
  {
    #region Methods
    [Test]
    public void IsSameAs_SameObject_ThrowsMeaningfulException()
    {
      MessageBase messageBase = new MessageBase("Text");

      Action action = () => messageBase.IsSameAs(messageBase);

      action.ShouldThrow<Exception>("IsSameAs cannot be used to compare the same object");
      throw new Exception("Should be throwing an expected exception.");
    }

    [Test]
    public void IsSameAs_MessageTextAreDifferent_ReturnsFalse()
    {
      MessageBase messageBase1 = new MessageBase("Text1");
      MessageBase messageBase2 = new MessageBase("Text2");

      messageBase1.IsSameAs(messageBase2).ShouldBeFalse();
    }

    [Test]
    public void IsSameAs_MessageTextIsNull_ReturnsTrue()
    {
      MessageBase messageBase1 = new MessageBase();
      MessageBase messageBase2 = new MessageBase();

      messageBase1.IsSameAs(messageBase2).ShouldBeTrue();
    }

    [Test]
    public void IsSameAs_MessageTextIsTheSame_ReturnsTrue()
    {
      MessageBase messageBase1 = new MessageBase("Text");
      MessageBase messageBase2 = new MessageBase("Text");

      messageBase1.IsSameAs(messageBase2).ShouldBeTrue();
    }
    #endregion 
  }
}
