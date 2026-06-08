using NUnit.Framework;

public class PlayerHealthTests
{
    [Test]
    public void Damage_Reduces_Health()
    {
        int health = 5;

        health -= 3;

        Assert.AreEqual(2, health);
    }
}