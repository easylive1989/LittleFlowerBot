using System.Collections.Generic;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Renderer
{
    [TestFixture]
    public class BufferedReplyRendererTests
    {
        private IMessage _message = null!;
        private BufferedReplyRenderer _renderer = null!;

        [SetUp]
        public void Setup()
        {
            _message = Substitute.For<IMessage>();
            var configuration = Substitute.For<IConfiguration>();
            var logger = Substitute.For<ILogger<BufferedReplyRenderer>>();

            _renderer = new BufferedReplyRenderer(_message, configuration, logger);
            _renderer.ReplyToken = "test-reply-token";
        }

        [Test]
        public void RenderPrivate_ShouldBufferAndFlushViaPushMessages()
        {
            _renderer.RenderPrivate("user123", "hello");
            _renderer.Flush();

            _message.Received(1).PushMessages("user123", Arg.Any<List<ReplyMessageItem>>());
        }

        [Test]
        public void RenderPrivate_MultipleMessages_ShouldMergeAndFlush()
        {
            _renderer.RenderPrivate("user123", "line1");
            _renderer.RenderPrivate("user123", "line2");
            _renderer.Flush();

            _message.Received(1).PushMessages("user123",
                Arg.Is<List<ReplyMessageItem>>(msgs => msgs.Count == 1));
        }

        [Test]
        public void RenderPrivate_DifferentUsers_ShouldFlushSeparately()
        {
            _renderer.RenderPrivate("userA", "hello A");
            _renderer.RenderPrivate("userB", "hello B");
            _renderer.Flush();

            _message.Received(1).PushMessages("userA", Arg.Any<List<ReplyMessageItem>>());
            _message.Received(1).PushMessages("userB", Arg.Any<List<ReplyMessageItem>>());
        }

        [Test]
        public void RenderPrivate_NoMessages_ShouldNotCallPush()
        {
            _renderer.Flush();

            _message.DidNotReceive().PushMessages(Arg.Any<string>(), Arg.Any<List<ReplyMessageItem>>());
        }

        [Test]
        public void RenderPrivate_FlushClearsBuffer()
        {
            _renderer.RenderPrivate("user123", "hello");
            _renderer.Flush();
            _renderer.Flush();

            _message.Received(1).PushMessages("user123", Arg.Any<List<ReplyMessageItem>>());
        }
    }
}
