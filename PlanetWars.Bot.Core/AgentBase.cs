using PlanetWars.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CSharpAgentCore
{
    public class AgentBase
    {
        private bool _isRunning = false;
        private readonly HttpClient _client = null;

        private List<MoveRequest> _pendingMoveRequests = new List<MoveRequest>();

        protected long TimeToNextTurn { get; set; }
        protected int CurrentTurn { get; set; }
        protected int GameId { get; set; }

        // string guid that acts as an authorization token, definitely not crypto secure
        public string AuthToken { get; set; }
        public string Name { get; set; }
        public int LastTurn { get; private set; }
        public int MyId { get; private set; }

        public AgentBase(string name, string endpoint)
        {
            Name = name;
            // connect to api and handle gzip compressed messasges
            _client = new HttpClient() { BaseAddress = new Uri(endpoint) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SendFleet(int sourcePlanetId, int destinationPlanetId, int numShips)
        {
            var moveRequest = new MoveRequest()
            {
                AuthToken = AuthToken,
                GameId = GameId,
                SourcePlanetId = sourcePlanetId,
                DestinationPlanetId = destinationPlanetId,
                NumberOfShips = numShips
            };
            _pendingMoveRequests.Add(moveRequest);
        }

        protected async Task<LogonResult> Logon()
        {
            var response = _client.PostAsync("api/logon", new StringContent( JsonConvert.SerializeObject(new LogonRequest()
            {
                AgentName = Name
            }), Encoding.UTF8, "application/json"));

            string json = await response.Result.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<LogonResult>(json);

            if (!result.Success)
            {
                Console.WriteLine($"Error talking to server {result.Message}");
                throw new Exception("Could not talk to sever");
            }
            AuthToken = result.AuthToken;
            GameId = result.GameId;
            MyId = result.Id;
            TimeToNextTurn = (long)result.GameStart.Subtract(DateTime.UtcNow).TotalMilliseconds;
            Console.WriteLine($"Your game Id is {result.GameId} auth {result.AuthToken} and starts in {TimeToNextTurn}ms");
            return result;
        }

        protected async Task<StatusResult> UpdateGameState()
        {
            var response = _client.PostAsync("api/status", new StringContent(JsonConvert.SerializeObject(new StatusRequest()
            {
                GameId = GameId
            }), Encoding.UTF8, "application/json"));

            string json = await response.Result.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<StatusResult>(json);

            if (!result.Success)
            {
                Console.WriteLine($"Error talking to server {result.Message}");
                throw new Exception("Could not talk to sever");
            }
            TimeToNextTurn = (long)result.NextTurnStart.Subtract(DateTime.UtcNow).TotalMilliseconds;
            CurrentTurn = result.CurrentTurn;
            Console.WriteLine($"Next turn in {TimeToNextTurn}ms");
            return result;
        }

        protected async Task<List<MoveResult>> SendUpdate(List<MoveRequest> moveCommands)
        {
            var response = _client.PostAsync("api/move", new StringContent(JsonConvert.SerializeObject(moveCommands), Encoding.UTF8, "application/json"));

            string json = await response.Result.Content.ReadAsStringAsync();

            var results = JsonConvert.DeserializeObject<List<MoveResult>>(json);

            foreach (var result in results)
            {
                Console.WriteLine(result.Message);
            }            
            return results;
        }

        public async Task Start()
        {
            await Logon();
            if (!_isRunning)
            {
                _isRunning = true;
                while (_isRunning)
                {

                    var gs = await UpdateGameState();
                    if (gs.IsGameOver)
                    {
                        _isRunning = false;
                        Console.WriteLine("Game Over!");
                        Console.WriteLine(gs.Status);
                        _client.Dispose();
                        break;
                    }

                    Update(gs);
                    var ur = await SendUpdate(this._pendingMoveRequests);
                    this._pendingMoveRequests.Clear();
                    if (TimeToNextTurn > 0)
                    {
                        await Task.Delay((int)(TimeToNextTurn));
                    }
                }
            }
        }

        public virtual void Update(StatusResult gs)
        {
            // override me
        }
    }
}
