
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Location_UnitTests
  {
    [Test]
    public void AddTrail_FirstTrail_TrailsHashsetCreated()
    {
      // Arrange
      var location1 = new Location();
      var location2 = new Location();

      var trail = new Trail(location1, location2);

      // Act
      location1.AddTrail(trail);

      // Arrange
      location1.Trails.ShouldNotBeNull();
    }

    [Test]
    public void AddTrail_FirstTrail_TrailAdded()
    {
      // Arrange
      var location1 = new Location();
      var location2 = new Location();

      var trail = new Trail(location1, location2);

      // Act
      location1.AddTrail(trail);

      // Arrange
      location1.Trails.Count.ShouldBe(1);
      location1.Trails.Contains(trail).ShouldBeTrue();
    }

    [Test]
    public void AddTrail_SubsequentTrail_TrailAdded()
    {
      // Arrange
      var location1 = new Location();
      var location2 = new Location();

      var firstTrail = new Trail(location1, location2);
      location1.AddTrail(firstTrail);

      var subsequentTrail = new Trail(location2, location1);

      // Act
      location1.AddTrail(subsequentTrail);

      // Arrange
      location1.Trails.Count.ShouldBe(2);
      location1.Trails.Contains(firstTrail).ShouldBeTrue();
      location1.Trails.Contains(subsequentTrail).ShouldBeTrue();
    }
  }
}
