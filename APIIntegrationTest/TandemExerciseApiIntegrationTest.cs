using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TandemExercise.Business.Entities;
using TandemExercise.Business.Entities.DTO;
using Xunit;

namespace TandemExercise.IntegrationTest
{
    public class TandemExerciseApiIntegrationTest
    {
        [Fact]
        public async Task Create_User()
        {
            using (var client = new TestClientProvider().Client)
            {
                var user = new User()
                {
                    firstName = "John",
                    middleName = "Allen",
                    lastName = "Smith",
                    phoneNumber = "123-456-7890",
                    emailAddress = "johnasmith@tandemdiabetes.com"
                };

                var response = await client.PostAsync("user"
                        , new StringContent(
                        JsonConvert.SerializeObject(user),
                    Encoding.UTF8,
                    "application/json"));

                response.EnsureSuccessStatusCode();

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                string resp = await response.Content.ReadAsStringAsync();
                var resultUser = JsonConvert.DeserializeObject<User>(resp);
                Assert.NotNull(resultUser.id);
                Assert.Equal(resultUser.emailAddress, user.emailAddress);
            }
        }

        [Fact]
        public async Task Update_User_Email()
        {
            using (var client = new TestClientProvider().Client)
            {
                var getResponse = await client.GetAsync("/user/nelson.packer@tandemdiabetes.com");

                getResponse.EnsureSuccessStatusCode();
                Assert.NotNull(getResponse);
                var responseString = await getResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserDTO>(responseString);

                var response = await client.PutAsync($"/user/{user.userId}"
                        , new StringContent(
                        JsonConvert.SerializeObject(new User()
                        {
                            id = user.userId,
                            firstName = "Mike",
                            middleName = "Ronald",
                            lastName = "Hansen",
                            phoneNumber = "123-456-7890",
                            emailAddress = "mike.hansen@tandemdiabetes.com"
                        }),
                    Encoding.UTF8,
                    "application/json"));

                response.EnsureSuccessStatusCode();

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                string resp = await response.Content.ReadAsStringAsync();
                var resultUser = JsonConvert.DeserializeObject<User>(resp);
                Assert.Equal("Mike", resultUser.firstName);
                Assert.Equal("Ronald", resultUser.middleName);
                Assert.Equal("mike.hansen@tandemdiabetes.com", resultUser.emailAddress);
            }
        }

        [Fact]
        public async Task Update_User_Same_Email()
        {
            using (var client = new TestClientProvider().Client)
            {
                var getResponse = await client.GetAsync("/user/bob.smith@tandemdiabetes.com");

                getResponse.EnsureSuccessStatusCode();
                Assert.NotNull(getResponse);
                var responseString = await getResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserDTO>(responseString);

                var response = await client.PutAsync($"/user/{user.userId}"
                        , new StringContent(
                        JsonConvert.SerializeObject(new User()
                        {
                            id = user.userId,
                            firstName = "Bob",
                            middleName = "G",
                            lastName = "Smith",
                            phoneNumber = "123-456-7890",
                            emailAddress = "bob.smith@tandemdiabetes.com"
                        }),
                    Encoding.UTF8,
                    "application/json"));

                response.EnsureSuccessStatusCode();

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                string resp = await response.Content.ReadAsStringAsync();
                var resultUser = JsonConvert.DeserializeObject<User>(resp);
                Assert.Equal("Bob", resultUser.firstName);
                Assert.Equal("G", resultUser.middleName);
                Assert.Equal("bob.smith@tandemdiabetes.com", resultUser.emailAddress);
            }
        }

        [Fact]
        public async Task Create_User_Blank_Email()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.PostAsync("user", 
                    new StringContent(
                        JsonConvert.SerializeObject(new User()
                        {
                            firstName = "Bruce",
                            middleName = "James",
                            lastName = "White",
                            phoneNumber = "123-456-7890",
                            emailAddress = ""
                        }),
                    Encoding.UTF8,
                    "application/json"));

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task Create_User_Invalid_Email()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.PostAsync("user",
                    new StringContent(
                        JsonConvert.SerializeObject(new User()
                        {
                            firstName = "Bruce",
                            middleName = "James",
                            lastName = "White",
                            phoneNumber = "123-456-7890",
                            emailAddress = "bruce.james"
                        }),
                    Encoding.UTF8,
                    "application/json"));

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task Create_Duplicate_Email()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.PostAsync("user",
                    new StringContent(
                        JsonConvert.SerializeObject(new User()
                        {
                            firstName = "Julie",
                            middleName = "Marie",
                            lastName = "Jeffries",
                            phoneNumber = "123-456-7890",
                            emailAddress = "julie.jeffries@tandemdiabetes.com"
                        }),
                    Encoding.UTF8,
                    "application/json"));

                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            }
        }

        [Fact]
        public async Task Get_One()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync("/user/julie.jeffries@tandemdiabetes.com");

                response.EnsureSuccessStatusCode();
                Assert.NotNull(response);
                var responseString = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserDTO>(responseString);
                Assert.True(user != null);
                Assert.True(user.name == "Julie Ann Jeffries");
            }
        }

        [Fact]
        public async Task Get_Invalid()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync("/user/no.match@tandemdiabetes.com");

                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task Get_All()
        {
            using (var client = new TestClientProvider().Client)
            {
                var response = await client.GetAsync("/user");

                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var responseString = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDTO>>(responseString);

                Assert.True(users.Any());
            }
        }
    }
}
