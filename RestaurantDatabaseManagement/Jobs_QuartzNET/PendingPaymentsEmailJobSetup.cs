using Microsoft.Extensions.Options;
using Quartz;

namespace RestaurantDatabaseManagement.Jobs
{
    // Class to set up the PendingPaymentsEmailJob with Quartz.NET
    public class PendingPaymentsEmailJobSetup : IConfigureOptions<QuartzOptions>
    {
        // Configure the Quartz job and trigger
        public void Configure(QuartzOptions options)
        {
            // Define a unique job key
            var jobKey = new JobKey(nameof(PendingPaymentsEmailJob));
            var triggerKey = new TriggerKey($"{nameof(PendingPaymentsEmailJob)}-trigger");

            // Define the job
            options.AddJob<PendingPaymentsEmailJob>(jobBuilder =>
                jobBuilder.WithIdentity(jobKey));

            // Define the trigger
            options.AddTrigger(triggerBuilder =>
                triggerBuilder
                    .ForJob(jobKey)
                    .WithIdentity(triggerKey)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInMinutes(1)
                            .RepeatForever()
                    )
            );
        }
    }
}
