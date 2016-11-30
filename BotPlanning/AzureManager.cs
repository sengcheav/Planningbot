using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using BotPlanning.DataModels;

namespace BotPlanning
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Timeline> timelineTable;
        private IMobileServiceTable<Plan> planTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://exampledb11.azurewebsites.net/");
            this.timelineTable = this.client.GetTable<Timeline>();
            this.planTable = this.client.GetTable<Plan>(); 

        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task AddTimeline(Timeline timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }

        public async Task<List<Timeline>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();

        }

        public async Task<List<Plan>> GetPlanTable()
        {
            return await this.planTable.ToListAsync();

        }

        public async Task AddPlan(Plan plan) {
            await this.planTable.InsertAsync(plan);

        }

        public async Task updatePlan(Plan plan) {
            await this.planTable.UpdateAsync(plan);   
        }

        public async Task<List<Plan>> GetUserPlan(string username) {
            List<Plan> list = await this.planTable
                .Where(plan => plan.username == username)
              
                .ToListAsync();
            return list; 
        }


        public async Task<List<Plan>> GetUserPlanOnDate(string username, string date)
        {
            List<Plan> list = await this.planTable
                .Where(plan => plan.username == username && plan.date == date)

                .ToListAsync();
            return list;
        }

        public async Task<List<double>> GetAnger(double n)
        {

            List<double> list = await this.timelineTable
                .Where(timeline => timeline.Anger == n)
                .Select(timeline => timeline.Disgust)
                .ToListAsync();
            return list;
        }
    }
}