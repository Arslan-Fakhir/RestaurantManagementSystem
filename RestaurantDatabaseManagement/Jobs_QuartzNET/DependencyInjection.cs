using Quartz;

namespace RestaurantDatabaseManagement.Jobs
{
    public static class DependencyInjection
    {
        // Extension method to add Quartz.NET services
        public static void AddInfrastructure(this IServiceCollection services)
        {
            // Add Quartz services
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory(); // Use DI for job creation

                // Configure persistent store
                options.UsePersistentStore(storeOptions =>
                {
                    storeOptions.UseProperties = true;
                    storeOptions.UseMySql("server=localhost;port=3306;database=quartz_practice;user=root;password=root;Allow User Variables=True");
                    storeOptions.UseSystemTextJsonSerializer();
                    storeOptions.PerformSchemaValidation = true;
                });
            });

            // Add Quartz hosted service
            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            // Register the job and its setup
            services.ConfigureOptions<PendingPaymentsEmailJobSetup>();
        }
    }
}
