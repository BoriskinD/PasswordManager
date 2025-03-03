using Newtonsoft.Json;
using System.Text;

namespace Client.Model
{
    public class HttpWrapper
    {
        private static HttpWrapper? instance;
        private HttpClient httpClient;
        private string apiUrl, mediaType;

        private HttpWrapper()
        {
            mediaType = "application/json";
            apiUrl = "https://localhost:7130/api/Application/";
            httpClient = new()
            {
                BaseAddress = new Uri(apiUrl)
            };
        }

        public static HttpWrapper GetInstance()
        {
            if (instance == null)
                instance = new HttpWrapper();

            return instance;
        }

        public async Task<List<MyApp>> Get()
        {
            List<MyApp>? responseApps = null;
            using HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                responseApps = JsonConvert.DeserializeObject<List<MyApp>>(responseBody);
            }
            return responseApps;
        }

        //Добавление
        public async Task<HttpResponseMessage> Post(MyApp newApp)
        {
            string jsonString = JsonConvert.SerializeObject(newApp);
            StringContent jsonContent = new(jsonString, Encoding.UTF8, mediaType);
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, jsonContent);

            return response;
        }

        //Изменение
        public async Task<HttpResponseMessage> Put(MyApp changedApp)
        {
            string jsonString = JsonConvert.SerializeObject(changedApp);
            StringContent jsonContent = new(jsonString, Encoding.UTF8, mediaType);
            HttpResponseMessage response = await httpClient.PutAsync($"{apiUrl}", jsonContent);

            return response;
        }

        public async Task<bool> Delete(int id)
        {
            using HttpResponseMessage responce = await httpClient.DeleteAsync($"{apiUrl}{id}");
            return responce.IsSuccessStatusCode;
        }

        public async Task<HttpResponseMessage> RegisterUser(User user)
        {
            string jsonString = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, mediaType);

            HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}Register", content);
            return response;
        }

        public async Task<HttpResponseMessage> Login(User user)
        {
            string jsonString = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, mediaType);

            HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}Login", content);
            return response;
        }
    }
}
