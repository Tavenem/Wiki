using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A client for receiving wiki discussion messages.
    /// </summary>
    public class WikiTalkClient : IWikiTalkClient
    {
        private static readonly TimeSpan _ConnectionTimeout = TimeSpan.FromSeconds(30);

        private readonly HubConnection _hubConnection;

        private CancellationTokenSource? _cts;
        private bool _disposed;
        private string? _topicId;

        /// <summary>
        /// Whether the connection is active.
        /// </summary>
        public bool IsConnected => !_disposed && _hubConnection.State == HubConnectionState.Connected;

        /// <summary>
        /// Receive a new message.
        /// </summary>
        public event EventHandler<IMessageResponse>? OnRecevied;

        /// <summary>
        /// Initializes a new instance of <see cref="WikiTalkClient"/>.
        /// </summary>
        /// <param name="url">The URL of the hub.</param>
        public WikiTalkClient(string url)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.On<IMessageResponse>(nameof(Receive), Receive);
            _hubConnection.Reconnected += Reconnected;
        }

        /// <summary>
        /// <para>
        /// Stops and disposes the current connection.
        /// </para>
        /// <para>
        /// Does nothing if no connection is active.
        /// </para>
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            _disposed = true;

            _hubConnection.Reconnected -= Reconnected;

            if (_hubConnection.State == HubConnectionState.Connected && !string.IsNullOrEmpty(_topicId))
            {
                _cts?.Dispose();
                _cts = new CancellationTokenSource((int)_ConnectionTimeout.TotalMilliseconds);
                try
                {
                    await _hubConnection.InvokeAsync(nameof(IWikiTalkHub.LeaveTopic), _topicId, _cts.Token).ConfigureAwait(false);
                }
                catch { }
            }

            _cts?.Dispose();
            await _hubConnection.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// <para>
        /// Receive a new message.
        /// </para>
        /// <para>
        /// Note: this method should only be invoked internally by an <see cref="IWikiTalkHub"/>.
        /// </para>
        /// </summary>
        /// <param name="message">
        /// An <see cref="IMessageResponse"/> with information about the message received.
        /// </param>
        public void Receive(IMessageResponse message) => OnRecevied?.Invoke(this, message);

        /// <summary>
        /// Send a reply.
        /// </summary>
        /// <param name="reply">
        /// <para>
        /// The message that has been sent.
        /// </para>
        /// <para>
        /// Note: messages with empty content are neither saved to the data source, nor forwarded to
        /// clients. Messages with missing topic IDs are also ignored.
        /// </para>
        /// </param>
        public async Task SendAsync(IReplyRequest reply)
        {
            if (_disposed)
            {
                throw new Exception("Client has been disposed.");
            }
            await _hubConnection.SendAsync(nameof(IWikiTalkHub.Send), reply).ConfigureAwait(false);
        }

        /// <summary>
        /// <para>
        /// Starts a connection to the given topic. Re-tries once per second if necessary.
        /// </para>
        /// <para>
        /// Times out after 30 seconds.
        /// </para>
        /// </summary>
        /// <param name="topicId">The ID of the topic to join.</param>
        /// <returns>
        /// <see langword="true"/> if the connection was successfully established; otherwise <see
        /// langword="false"/>
        /// </returns>
        public async Task<bool> StartAsync(string topicId)
        {
            if (_disposed)
            {
                throw new Exception("Client has been disposed.");
            }

            // Retry until server is ready, or timeout.
            _cts?.Dispose();
            _cts = new CancellationTokenSource((int)_ConnectionTimeout.TotalMilliseconds);
            var joinedGroup = false;
            if (_hubConnection.State == HubConnectionState.Connected && !string.IsNullOrEmpty(_topicId))
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        await _hubConnection.InvokeAsync(nameof(IWikiTalkHub.LeaveTopic), _topicId, _cts.Token).ConfigureAwait(false);
                        break;
                    }
                    catch
                    {
                        await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
                    }
                }
            }
            while (!_cts.IsCancellationRequested && _hubConnection.State != HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StartAsync(_cts.Token).ConfigureAwait(false);
                    break;
                }
                catch
                {
                    await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
                }
            }
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync(nameof(IWikiTalkHub.JoinTopic), topicId, _cts.Token).ConfigureAwait(false);
                    joinedGroup = true;
                    _topicId = topicId;
                }
                catch
                {
                    await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
                }
            }
            return joinedGroup;
        }

        private async Task Reconnected(string arg)
        {
            if (_hubConnection.State == HubConnectionState.Connected && !string.IsNullOrEmpty(_topicId))
            {
                _cts?.Dispose();
                _cts = new CancellationTokenSource((int)_ConnectionTimeout.TotalMilliseconds);
                await _hubConnection.InvokeAsync(nameof(IWikiTalkHub.LeaveTopic), _topicId, _cts.Token).ConfigureAwait(false);
            }
        }
    }
}
