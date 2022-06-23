using NUnit.Framework;

namespace Challenge.Service.Test.Services
{
    public class LobbyManagerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void On_Create_ThrowException_When_PlayerNotFound()
        {
            Assert.Pass();
        }

        [Test]
        public void On_Create_ThrowException_When_PlayerAlreadyInLobby()
        {
            Assert.Pass();
        }

        [Test]
        public void On_Join_ThrowException_When_PlayerNotFound()
        {
            Assert.Pass();
        }

        [Test]
        public void On_Join_ThrowException_When_PlayerAlreadyInLobby()
        {
            Assert.Pass();
        }

        [Test]
        public void On_Join_ThrowException_When_LobbyNotFound()
        {
            Assert.Pass();
        }

        [Test]
        public void On_Leave_ThrowException_When_PlayerNotFound()
        {
            Assert.Pass();
        }
    }
}