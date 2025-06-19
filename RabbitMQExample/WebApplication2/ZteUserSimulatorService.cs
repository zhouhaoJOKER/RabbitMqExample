
namespace WebApplication2
{
    public class ZteUserSimulatorService
    {
        private readonly Simulator _simulator;
        private readonly ILogger<ZteUserSimulatorService> _logger;
        private readonly UserSessionContext _userSessionContext;

        public ZteUserSimulatorService(Simulator simulator,
            ILogger<ZteUserSimulatorService> logger,
            UserSessionContext userSessionContext)
        {
            _simulator = simulator;
            _logger = logger;
            _userSessionContext = userSessionContext;
        }

        /// <summary>
        /// 用户登录中兴后台
        /// </summary>
        /// <returns></returns>
        public void LoginZte(string loginToken)
        {
            _logger.LogInformation($"_simulator:{_simulator.GetHashCode()}");
            _simulator.InitToken(loginToken);

            _userSessionContext.AddUserSimulator(loginToken, _simulator);
        }
    }
}
