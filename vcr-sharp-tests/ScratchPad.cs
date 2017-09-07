using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace VcrSharp.Tests
{
    public class ScratchPad : IDisposable
    {
        [Fact]
        public async Task UseTheLocalFixtureToRetrieveTheResponse()
        {
            using (var httpClient = HttpClientFactory.WithCassette("example-test"))
            {
                Environment.SetEnvironmentVariable("VCR_MODE", "playback");

                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.iana.org/domains/reserved"));
                var body = await response.Content.ReadAsStringAsync();
                body.ShouldContain("Example domains");
            }
        }

        [Fact]
        public async Task ErrorsWhenTheRequestIsNotInTheCache()
        {
            using (var httpClient = HttpClientFactory.WithCassette("no-cache-defined"))
            {
                Environment.SetEnvironmentVariable("VCR_MODE", "playback");

                await Assert.ThrowsAsync<PlaybackException>(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.iana.org/domains/reserved")));
            }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Environment.SetEnvironmentVariable("VCR_MODE", "");
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
