using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System.Net;

namespace HWSession2_2
{
    [TestClass]
    public class RestClientTests
    {

        private static RestClient? restClient;
        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";
        private static readonly string PetEndPoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}/{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndPoint}/{data.Name}"));
                var restResponse = await restClient.GetAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task TestPostMethod()
        {
            //declare a constant variable for the PhotoUrl property of our sample pet data
            const string PHOTOURL = "https://www.rd.com/wp-content/uploads/2021/04/GettyImages-988013222-scaled-e1618857975729.jpg?w=1670";

            //define our pet data by setting properties to correct sample values
            //this is our pet data that we will use in Post method which we will be using in testing our Put method as well
            PetModel newPetData = new PetModel()
            {
                Id = 1,
                Category = new Category()
                {
                    Id = 0,
                    Name = "cat"
                },
                Name = "TestCat",
                PhotoUrls = PHOTOURL.Split(),
                Status = "available",
            };

            var temp = GetURI(PetEndPoint);
            var postRestRequest = new RestRequest(GetURI(PetEndPoint)).AddJsonBody(newPetData);
            var postRestResponse = await restClient.PostAsync(postRestRequest);

            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200.");

            cleanUpList.Add(newPetData);
        }
    }
}