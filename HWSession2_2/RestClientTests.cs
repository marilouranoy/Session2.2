/*
 * API Test Automation Training - Batch 4
 * Marilou A. Ranoy
 * 
 * Homework 2.2
 * RestAPI Test using RestSharp and MSTest
 * Base URL: https://petstore.swagger.io/#/
 * Endpoint: /pet
 * Create a test using POST request to add a new pet to the store
 * Request Payload should contain pet name, category, photo URLS, tags, and status
 * Add an Assertion for HTTP Status Code
 * Add an Assertion if pet name, category, photo URLS, tags, and status are correctly reflected
 * Create a cleanup method using DELETE request 
 * 
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace HWSession2_2
{
    //Test class for our test methods
    [TestClass]
    public class RestClientTests
    {
        //variable declarations for our test methods
        private static RestClient restClient;
        private static readonly string BaseURL = "https://petstore.swagger.io/v2";
        private static readonly string PetEndPoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}/{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        //test initialize method
        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        //test cleanup method
        //In this method, we delete test data that we added via Post method
        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndPoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        //our main method for testing Post method of the API endpoint https://petstore.swagger.io/#/pet/ using RestSharp
        [TestMethod]
        public async Task TestPostMethod()
        {
            //declare a constant variable for the PhotoUrl property of our sample pet data
            const string PHOTOURL = "https://www.rd.com/wp-content/uploads/2021/04/GettyImages-988013222-scaled-e1618857975729.jpg?w=1670";

            //define our pet data by setting properties to correct sample values
            //this is our pet data that we will use in testing our Post method
            PetModel newPetData = new PetModel()
            {
                Id = 8,
                Category = new Category()
                {
                    Id = 0,
                    Name = "cat"
                },
                Name = "TestCat",
                PhotoUrls = PHOTOURL.Split(),
                Status = "available",
                Tags = new[]
                {
                    new Category()
                    {
                        Id = 0,
                        Name = "fur animals"
                    },
                    new Category()
                    {
                        Id = 1,
                        Name = "cats"
                    },
                }
            };

            //add the test data to our cleanup list before calling HTTP request methods
            cleanUpList.Add(newPetData);

            //define our post request which includes the serialized pet data object, encoding, and the content type
            var postRestRequest = new RestRequest(GetURI(PetEndPoint)).AddJsonBody(newPetData);
            //execute our Post request
            var postRestResponse = await restClient.PostAsync(postRestRequest);

            //check if our Post request was successful
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200.");

            //execute Get request to check if we get the same pet data as what we defined above
            var restRequest = new RestRequest(GetURI($"{PetEndPoint}/{newPetData.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetModel>(restRequest);

            //Add an Assertion if status is 200 and if pet name, category, photo URLS, tags, and status are correctly reflected
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200.");
            Assert.AreEqual(newPetData.Name, restResponse.Data.Name, "Name did not match.");

            //in order to compare values, we need to serialize objects and convert them into JSON strings
            Assert.AreEqual(JsonConvert.SerializeObject(newPetData.Category), JsonConvert.SerializeObject(restResponse.Data.Category), "Category did not match.");
            Assert.AreEqual(JsonConvert.SerializeObject(newPetData.PhotoUrls), JsonConvert.SerializeObject(restResponse.Data.PhotoUrls), "Photo URL did not match.");
            Assert.AreEqual(JsonConvert.SerializeObject(newPetData.Tags), JsonConvert.SerializeObject(restResponse.Data.Tags), "Tag did not match.");
            Assert.AreEqual(newPetData.Status, restResponse.Data.Status, "Status did not match.");
        }
    }
}