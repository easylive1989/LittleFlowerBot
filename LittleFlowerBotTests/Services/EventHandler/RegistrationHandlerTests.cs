using isRock.LineBot;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Services.EventHandler;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;

namespace LittleFlowerBotTests.Services.EventHandler
{
    [TestFixture]
    public class RegistrationHandlerTests
    {
        private IMessage _message;
        private Event _event;
        private StubRegistrationHandler _registrationHandler;
        private string _replyToken = "qwer";
        private string _userId = "123";
        private RegistrationCache _registrationCache;
        private string _guid = "xxxx";
        private string _token = "token";
        private ILineNotifySubscription _lineNotifySubscription;
        private string _link = "link";

        [SetUp]
        public void Setup()
        {
            _message = Substitute.For<IMessage>();
            _lineNotifySubscription = Substitute.For<ILineNotifySubscription>();
            _registrationCache = new RegistrationCache();
            _registrationHandler = new StubRegistrationHandler(_message, _registrationCache, _lineNotifySubscription);
            _lineNotifySubscription.GenerateLink(Arg.Any<string>()).Returns(_link);
        }
        
        [Test]
        public void Generate_Url()
        {
            GivenGuid(_guid);

            GivenEvent("我要註冊");

            ShouldReply(_link);
        }
        
        [Test]
        public void Already_Registered()
        {
            GivenEvent("我要註冊");

            GivenIsRegistered();
            
            ShouldReply("你已經註冊了");
        }


        [Test]
        public void Store_Subscription()
        {
            GiveCache(_userId);

            _registrationHandler.Subscribe(_token, _guid);
            
            _lineNotifySubscription.Received(1).SaveToken(_userId, _token);
        }
        
        [Test]
        public void Not_Store_Subscription_When_Subscription_Exist()
        {
            GivenIsRegistered();
            
            GiveCache(_userId);

            _registrationHandler.Subscribe(_token, _guid);
            
            _lineNotifySubscription.DidNotReceive().SaveToken(_userId, _token);
        }
        
        private void GivenIsRegistered()
        {
            _lineNotifySubscription.IsRegistered(Arg.Any<string>()).Returns(true);
        }
        
        private void GiveCache(string userId)
        {
            _registrationCache.Add(_guid, userId);
        }

        private void ShouldReply(string link)
        {
            _registrationHandler.Act(_event);
            _message.Received(1).Reply(_replyToken, link);
        }

        private void GivenGuid(string guid)
        {
            _registrationHandler.Guid = guid;
        }

        private void GivenEvent(string message)
        {
            _event = new Event()
            {
                source = new Source()
                {
                    userId = _userId
                },
                message = new Message()
                {
                    text = message
                },
                replyToken = _replyToken
            };
        }
    }

    internal class StubRegistrationHandler : RegistrationHandler 
    {
        public string Guid { get; set; }
        
        public StubRegistrationHandler(IMessage message,
            RegistrationCache registrationCache, ILineNotifySubscription lineNotifySubscription) : base(message, registrationCache, lineNotifySubscription)
        {
        }

        protected override string GenerateGuid()
        {
            return Guid;
        }
    }
}