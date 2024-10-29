using ProxyParser.Bridge.Loggers;
using ProxyParser.Bridge.ProxyChecker.Abstraction;
using ProxyParser.Interfaces;
using System.Net;
using System.Text.Json;

namespace ProxyParser.Bridge.ProxyChecker
{
    public class WebsocketChecker : IProxyChecker
    {
        private AdressElement _address;

        string[] protocols = new string[]
        {
            "http",
            "https",
            "socks4",
            "socks5",
        };

        public async Task<RequestResponce> IsProxy(AdressElement adress, SearchFilter filter)
        {
            AdressRecord record = new AdressRecord(adress, 0f);

            _address = adress;

            for (int i = 0; i < filter.Protocols.Length; i++)
            {
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy($"{protocols[(int)filter.Protocols[i]]}://{adress.Address}:{adress.Port}"),
                    UseCookies = false,
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                using (HttpClient client = new HttpClient(httpClientHandler, true))
                {
                    //TimeResponce check
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    //Work test
                    var rootRequest = await GetInfo(client);

                    Root root = (Root)rootRequest.Data;

                    if (rootRequest.Code != ResponceCode.OK)
                        continue;

                    watch.Stop();
                    record.Speed = watch.ElapsedMilliseconds;

                    record.Country = root.cc;

                    // Anonymous Test
                    record.Anonymous = await GetAnonimity(client);

                    if(record.Anonymous == AnonymityLevel.Transparent)
                        return new RequestResponce(null, ResponceCode.Conflict);

                    if (record.Anonymous == AnonymityLevel.None)
                        return new RequestResponce(null, ResponceCode.Conflict);


                    //Ban check
                    record.HasBanned = await GetBanCheck(root.ip);

                    foreach (string site in filter.Websites)
                    {
                        var siteCheck = await Get(client, site);

                        if (siteCheck.Code == ResponceCode.OK)
                            record.Websites.Add(site);
                    }

                }

                record.Protocol = filter.Protocols[i];

                return new RequestResponce(record, ResponceCode.OK);
            }

            return new RequestResponce(null, ResponceCode.Dead);
        }

        private async Task<RequestResponce> Get(HttpClient client, string url)
        {
            string data = string.Empty;

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                new InterfaceLogger().Error($"{_address.Address} got {ex.Message}");

                if (ex.HttpRequestError == HttpRequestError.ConnectionError)
                {
                    if (ex.Message.StartsWith("No connection could be made because the target machine actively refused it."))
                        return new RequestResponce(data, ResponceCode.NotAcceptable);

                    if (ex.Message.StartsWith("A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond."))
                        return new RequestResponce(data, ResponceCode.RequestTimeout);
                }

                if(ex.Message.Contains("failed with status code \'400\'"))
                    return new RequestResponce(data, ResponceCode.BadRequest);

                

                if (ex.Message.Contains("failed with status code \'403\'"))
                    return new RequestResponce(data, ResponceCode.RequiredCredentials);

                if (ex.Message.Contains("failed with status code \'404\'"))
                    return new RequestResponce(data, ResponceCode.Notfound);

                if (ex.Message.Contains("failed with status code \'405\'"))
                    return new RequestResponce(data, ResponceCode.BadRequest);

                if (ex.Message.Contains("failed with status code \'407\'"))
                    return new RequestResponce(data, ResponceCode.RequiredCredentials);

                if (ex.Message.Contains("failed with status code \'500\'"))
                    return new RequestResponce(data, ResponceCode.Unknow);

                if (ex.Message.Contains("failed with status code \'503\'"))
                    return new RequestResponce(data, ResponceCode.NotAcceptable);

                if (ex.Message.Contains("An error occurred while sending the request."))
                    return new RequestResponce(data, ResponceCode.BadRequest);

                if (ex.Message.Contains("An error occurred while establishing a connection to the proxy tunnel."))
                {
                    if (ex.InnerException.Message.Contains("Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host"))
                        return new RequestResponce(data, ResponceCode.NotAcceptable);

                    if (ex.InnerException.Message.Contains("The response ended prematurely."))
                        return new RequestResponce(data, ResponceCode.Conflict);

                    if (ex.InnerException.Message.Contains("SOCKS server failed to connect to the destination."))
                        return new RequestResponce(data, ResponceCode.BadGateway);

                    if (ex.InnerException.Message.Contains("Unable to read data from the transport connection: A connection attempt failed because the connected party did not properly respond after a period of time"))
                        return new RequestResponce(data, ResponceCode.RequestTimeout);

                    if (ex.InnerException.Message.Contains("Unexpected SOCKS protocol version."))
                        return new RequestResponce(data, ResponceCode.NotAcceptable);

                }

                if (ex.Message.Contains("The SSL connection could not be established, see inner exception."))
                {
                    if (ex.InnerException.Message.Contains("Cannot determine the frame size or a corrupted frame was received."))
                        return new RequestResponce(data, ResponceCode.Conflict);

                    if (ex.InnerException.Message.Contains("Received an unexpected EOF or 0 bytes from the transport stream."))
                        return new RequestResponce(data, ResponceCode.DataError);

                    if (ex.InnerException.Message.Contains("Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host"))
                        return new RequestResponce(data, ResponceCode.NotAcceptable);

                    if (ex.InnerException.Message.Contains("Authentication failed, see inner exception."))
                        return new RequestResponce(data, ResponceCode.RequiredCredentials);

                }

                return new RequestResponce(data, ResponceCode.Notfound);

            }
            catch(Exception ex) 
            {
                if (ex.Message.StartsWith("The request was canceled due to the configured HttpClient.Timeout of 100 seconds elapsing."))
                    return new RequestResponce(data, ResponceCode.RequestTimeout);

                //Console.WriteLine(ex.Message);
                return new RequestResponce(data, ResponceCode.Notfound);

            }


            return new RequestResponce(data, ResponceCode.OK);
        }

        private async Task<AnonymityLevel> GetAnonimity(HttpClient client)
        {
            string judje = string.Empty;


            // GET JUDJE
            string[] judjeCount = new string[]
            {
                "http://httpheader.net/",
                "http://azenv.net/",
                "https://pool.proxyspace.pro/judge.php",
                "http://proxyjudge.us/",
                "http://www.proxy-listen.de/azenv.php",
                "http://ip.spys.ru/",
                "http://proxyjudge.us/azenv.php",
                "https://mojeip.net.pl/asdfa/azenv.php",
                "http://www.wfuchs.de/azenv.php",
                "http://takamers.s35.xrea.com/p_check.cgi",
                "https://aranguren.org/azenv.php",
                "http://www.cknuckles.com/cgi/env.cgi",
                "http://www.proxyfire.net/fastenv"
            };

            // REQUEST JUDJE
            for (int i = 0; i < judjeCount.Length; i++)
            {
                var request = await Get(client, judjeCount[i]);

                judje = (string)request.Data;

                if (judje != string.Empty)
                    break;
            }


            // CHECK JUDJE
            if (judje == string.Empty)
                return AnonymityLevel.None;

            if(judje.Contains(Program.Settings.MyIp))
                return AnonymityLevel.Transparent;

            if( judje.Contains("HTTP_VIA") ||
                judje.Contains("HTTP_FORWARDED") ||
                judje.Contains("HTTP_X_FORWARDED_FOR") ||
                judje.Contains("HTTP_X_PROXY_ID") )
                return AnonymityLevel.Anonymin;

            return AnonymityLevel.Elite;

        }

        private async Task<RequestResponce> GetInfo(HttpClient client)
        {
            Root infoOject = new Root();

            var ipInfo = await Get(client, "https://api.myip.com/");

            if(ipInfo.Code != ResponceCode.OK)
                return new RequestResponce(null, ipInfo.Code);

            try
            {
                infoOject = JsonSerializer.Deserialize<Root>((string)ipInfo.Data);
            }
            catch 
            {
                return new RequestResponce(null, ResponceCode.DataError);
            }

            return new RequestResponce(infoOject, ResponceCode.OK);
        }

        private async Task<bool> GetBanCheck(string ip)
        {
            if (Program.BanRawFile.Contains(ip))
                return true;

            return false;

        }

        #region resource

        public class Root
        {
            public string ip { get; set; }
            public string country { get; set; }
            public string cc { get; set; }
        }

        #endregion
    }
}
