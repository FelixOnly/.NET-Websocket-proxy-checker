using ProxyParser.Bridge.Loggers;
using ProxyParser.Bridge.ProxyChecker;
using ProxyParser.Bridge.ProxyChecker.Abstraction;
using ProxyParser.Interfaces;
using Spectre.Console;
using Terminal.Gui;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProxyParser.Bridge.Reader.Abstraction;


internal class Program
{
    /// <summary>
    /// Parameter with information on bans
    /// </summary>
    public static string BanRawFile = "";

    //App's settings
    public static SettingsFile Settings = new SettingsFile();
    public static SearchFilter filter = new SearchFilter();

    //IPs records
    private static List<AdressRecord> records = new List<AdressRecord>();
    private static List<AdressElement> adressPool = new List<AdressElement>();

    //Async work task pool
    private static int taskPool = 0;
    private static int ProceedCount = 0;

    //Log
    public static string logActive = string.Empty;

   


    private static void Main(string[] args)
    {
        //Settings apply 
        BanRawFile = File.ReadAllText(Settings.BANsFilePath);
        filter = JsonConvert.DeserializeObject<SearchFilter>(File.ReadAllText("filter.json"));
        Settings = JsonConvert.DeserializeObject<SettingsFile>(File.ReadAllText("settings.json"));
        adressPool = new ContentReader(new LocalReader()).GetAdresses(Settings.IPsFilePath).ToList();

        //Start main process
        Task mainProcess = Process();
        mainProcess.Start();

        //Start UI
        Application.Run<ControlWindow>();
    }

    public static async Task<string> GetUserIP()
    {
        var httpClientHandler = new HttpClientHandler
        {
            UseCookies = false,
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using (HttpClient client = new HttpClient(httpClientHandler, true))
        {
            HttpResponseMessage response = await client.GetAsync("https://api.ipify.org/");

            return await response.Content.ReadAsStringAsync();
        }
    }

    /// <summary>
    /// Procedure for working on proxy verification 
    /// </summary>
    private static async Task Process()
    {

        //Get user's IP
        ProxyHolder holder = new ProxyHolder(new WebsocketChecker());

        if (Settings.MyIp == null)
        {
            Settings.MyIp = await GetUserIP();
        }

        //Start task pool
        for (int i = 0; i < adressPool.Count - 1;)
        {
            Thread.Sleep(10);

            if (taskPool <= Settings.ThreadCount)
            {
                Task CheckTask = Task.Run(async () =>
                {
                    var responce = await holder.CheckProxy(adressPool[i], filter);

                    //Verification 
                    if (responce.Code == ResponceCode.OK)
                    {
                        AdressRecord record = (AdressRecord)responce.Data;

                        if (!records.Exists(x => x.Adress.GetRaw == record.Adress.GetRaw))
                        {

                            records.Add(record);
                            new InterfaceLogger().Success($"{record.Adress.Address}");

                            ControlWindow.IPlistView.SetSource(records.Select(x => x.Adress.GetRaw).ToList());

                        }
                    }

                    ProceedCount++;
                    taskPool--;
                });

                i++;
                taskPool++;
            }
        }
    }
    
    /// <summary>
    /// UI class
    /// </summary>
    private class ControlWindow : Window
    {

        public Label loglabel = new Label()
        {
            X = 1,
            TextAlignment = TextAlignment.Left,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = false
        };

        public Label SettingFirstLabel = new Label()
        {
            X = 1,
            Y = 0,
        };

        public Label SettingSecondLabel = new Label()
        {
            X = 16,
            Y = 0,
        };


        public static ListView IPlistView = new ListView(records.Select(x => x.Adress.GetRaw).ToList())
        {
            X = 0,
            Y = 0,
            Width = 55,
            Height = Dim.Fill(),
            AllowsMarking = false,
            CanFocus = true,
        };


        public ControlWindow()
        {


            Title = "Proxy parser (Ctrl+Q to quit)";

            var Logs = new FrameView("logs")
            {
                X = 60,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(80),
                CanFocus = false,
            };

            Logs.Add(loglabel);


            var SettingsFrame = new FrameView("settings")
            {
                X = 60,
                Y = Pos.Percent(80),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = false
            };

            SettingsFrame.Add(SettingFirstLabel);
            SettingsFrame.Add(SettingSecondLabel);


            var Ready = new FrameView("ready")
            {
                X = 0,
                Y = 0,
                Width = 60,
                Height = Dim.Percent(80),
                CanFocus = false
            };

            Ready.Add(IPlistView);



            var ProxyInfo = new FrameView("info")
            {
                X = 0,
                Y = Pos.Percent(80),
                Width = 60,
                Height = Dim.Fill(),
                CanFocus = false
            };

            Label ipadress = new Label("");

            ProxyInfo.Add(ipadress);


            IPlistView.SelectedItemChanged += (ListViewItemEventArgs obj) =>
            {
                ipadress.Text =
                    $"address: {obj.Value}\n" +
                    $"protocols: {records[obj.Item].Protocol}\n" +
                    $"levels: {records[obj.Item].Anonymous}\n" +
                    $"countries: {records[obj.Item].Country}\n" +
                    $"speed: {records[obj.Item].Speed}\n" +
                    $"ban: {records[obj.Item].HasBanned}\n" +
                    $"websites: {records[obj.Item].Websites.Count}";

            };

            Add(Logs, SettingsFrame, Ready, ProxyInfo);
            Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), UpdateTick);
        }

        private bool UpdateTick(MainLoop loop)
        {

            loglabel.Text = new InterfaceLogger().GetHistory();


            SettingFirstLabel.Text = $"Threads: {Settings.ThreadCount}\nActive: {taskPool}\n\nAmount: {adressPool.Count}\nProceed: {ProceedCount}";

            SettingSecondLabel.Text =
               $"Websites: {filter.Websites.Length}\n" +
               $"Protocols: {GetListAsString(filter.Protocols.ToList())}\n" +
               $"Levels: {GetListAsString(filter.Levels.ToList())}\n" +
               $"Countries: {GetListAsString(filter.Countries.ToList())}\n" +
               $"Your IP: {Settings.MyIp}";

            return true;
        }




        private string GetListAsString<T>(List<T> list)
        {
            string answer = string.Empty;

            list.ForEach(item => { answer += $"{item} "; });
            return answer;
        }
    }

}


//сделать настройки как файл отдельно в json формате 
//Проверка и очистка похожих прокси