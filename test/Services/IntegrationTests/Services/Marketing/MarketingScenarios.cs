﻿namespace IntegrationTests.Services.Marketing
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using System;
    using Newtonsoft.Json;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Dto;

    public class MarketingScenarios
        : MarketingScenarioBase
    {
        [Fact]
        public async Task Get_get_all_campaigns_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.Campaigns);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_get_campaign_by_id_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.CampaignBy(1));

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_get_campaign_by_id_and_response_not_found_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.CampaignBy(9999999));

                Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task Post_add_new_campaign_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var fakeCampaignDto = GetFakeCampaignDto();
                var content = new StringContent(JsonConvert.SerializeObject(fakeCampaignDto), Encoding.UTF8, "application/json");
                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewCampaign, content);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Delete_delete_campaign_and_response_not_content_status_code()
        {
            using (var server = CreateServer())
            {
                var fakeCampaignDto = GetFakeCampaignDto();
                var content = new StringContent(JsonConvert.SerializeObject(fakeCampaignDto), Encoding.UTF8, "application/json");

                //add campaign
                var campaignResponse = await server.CreateClient()
                    .PostAsync(Post.AddNewCampaign, content);

                if (int.TryParse(campaignResponse.Headers.Location.Segments[4], out int id))
                {
                    var response = await server.CreateClient()
                    .DeleteAsync(Delete.CampaignBy(id));

                    Assert.True(response.StatusCode == HttpStatusCode.NoContent);
                }

                campaignResponse.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Put_update_campaign_and_response_not_content_status_code()
        {
            using (var server = CreateServer())
            {
                var fakeCampaignDto = GetFakeCampaignDto();
                var content = new StringContent(JsonConvert.SerializeObject(fakeCampaignDto), Encoding.UTF8, "application/json");

                //add campaign
                var campaignResponse = await server.CreateClient()
                    .PostAsync(Post.AddNewCampaign, content);

                if (int.TryParse(campaignResponse.Headers.Location.Segments[4], out int id))
                {
                    fakeCampaignDto.Description = "FakeCampaignUpdatedDescription";
                    content = new StringContent(JsonConvert.SerializeObject(fakeCampaignDto), Encoding.UTF8, "application/json");
                    var response = await server.CreateClient()
                    .PutAsync(Put.CampaignBy(id), content);

                    Assert.True(response.StatusCode == HttpStatusCode.Created);
                }

                campaignResponse.EnsureSuccessStatusCode();
            }
        }

        private static CampaignDTO GetFakeCampaignDto()
        {
            return new CampaignDTO()
            {
                Description = "FakeCampaignDescription",
                From = DateTime.Now,
                To = DateTime.Now.AddDays(7),
                Url = "http://CampaignUrl.test/fdaf91ad0cef5419719f50198",
                Rules = new List<RuleDTO>
                {
                    new RuleDTO
                    {
                        LocationId = 1,
                        Description = "testDescription",
                        RuleTypeId = 3,
                    }
                }
            }; 
        }
    }
}