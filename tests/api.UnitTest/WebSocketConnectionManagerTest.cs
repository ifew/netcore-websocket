using System;
using Xunit;
using api.WebSocketManager;
using System.Threading.Tasks;

namespace api.UnitTest
{
    public class WebSocketConnectionManagerTest
    {
        private readonly WebSocketConnectionManager _manager;

        public WebSocketConnectionManagerTest()
        {
            _manager = new WebSocketConnectionManager();
        }

        public class GetSocketById : WebSocketConnectionManagerTest
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("foo")]
            public void WhenNonExistentId_ShouldReturnNull(string id)
            {
                var socket = _manager.GetSocketById(id);

                Assert.Null(socket);
            }

            [Fact]
            public void WhenExistingId_ShouldReturnSocket()
            {
                var socket = new FakeSocket();

                _manager.AddSocket(socket);
                var id = _manager.GetId(socket);

                Assert.Same(socket, _manager.GetSocketById(id));
            }
        }

        public class GetAll : WebSocketConnectionManagerTest
        {
            [Fact]
            public void WhenEmpty_ShouldReturnZero()
            {
                Assert.Empty(_manager.GetAll());
            }

            [Fact]
            public void WhenOneSocket_ShouldReturnOne()
            {
                _manager.AddSocket(new FakeSocket());

                Assert.Single(_manager.GetAll());
            }
        }

        public class GetAllFromGroup : WebSocketConnectionManagerTest
        {
            private string GroupName = "FakeGroup";

            [Fact]
            public void WhenNonExistingGroup_ShouldReturnNull()
            {
                Assert.Null(_manager.GetAllFromGroup(GroupName));
            }

            [Fact]
            public void WhenOneSocketInGroup_ShouldReturnOne()
            {
                var socket = new FakeSocket();
                _manager.AddSocket(socket);
                var socketID = _manager.GetId(socket);
                _manager.AddToGroup(socketID, GroupName);

                Assert.Single(_manager.GetAllFromGroup(GroupName));
            }
        }

        public class GetId : WebSocketConnectionManagerTest
        {
            [Fact]
            public void WhenNull_ShouldReturnNull()
            {
                var id = _manager.GetId(null);

                Assert.Null(id);
            }

            [Fact]
            public void WhenUntrackedInstance_ShouldReturnNull()
            {
                var id = _manager.GetId(new FakeSocket());

                Assert.Null(id);
            }

            [Fact]
            public void WhenTrackedInstance_ShouldReturnId()
            {
                var socket = new FakeSocket();
                _manager.AddSocket(socket);

                var id = _manager.GetId(socket);

                Assert.NotNull(id);
            }
        }

        public class AddSocket : WebSocketConnectionManagerTest
        {
            //[Fact(Skip = "At the moment the implementation allows adding null references")]
            public void WhenNull_ShouldNotNotContainSocket()
            {
                _manager.AddSocket(null);

                Assert.Empty(_manager.GetAll());
            }

            [Fact]
            public void WhenInstance_ShouldContainSocket()
            {
                _manager.AddSocket(new FakeSocket());

                Assert.Single(_manager.GetAll());
            }
        }

        public class RemoveSocket : WebSocketConnectionManagerTest
        {
            //[Theory(Skip = "Currently it doesn't check if the socket was removed or not, so we get an NRE")]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("foo")]
            public async Task WhenNonExistentId_ShouldNotThrowException(string id)
            {
                await _manager.RemoveSocket(id);
            }
        }

        public class RemoveFromGroup : WebSocketConnectionManagerTest
        {
            //[Theory(Skip = "Currently it doesn't check for non existing sockets")]
            [InlineData("FakeGroup")]
            public void WhenRemoveNonExisting_ShouldNotThrowException(string GroupName)
            {
                _manager.RemoveFromGroup("", GroupName);
            }
        }
    }
}