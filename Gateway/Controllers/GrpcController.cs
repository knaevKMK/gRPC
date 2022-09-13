namespace Gateway.Controllers
{
    using Grpc.Core;
    using Grpc.Net.Client;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyApp.Namespace;

    [ApiController]
    [Route("[controller]/[action]")]
    public class GrpcController : ControllerBase
    {
        private const string Address = "https://localhost:5001";
        private readonly Identity.IdentityClient identityClient;
        private readonly ILogger<GrpcController> logger;
        public GrpcController(ILogger<GrpcController> logger)
        {
            var grpcChannel = GrpcChannel.ForAddress(Address);
            this.identityClient = new Identity.IdentityClient(grpcChannel);
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            logger.LogInformation($"Register as {registerRequest.Username}...");

            var response = await identityClient.RegisterAsync(registerRequest);
            if (response.Success)
            {
                logger.LogInformation("Successfully Register.");
                return Ok(response);
            }
            logger.LogError("Un-Successfully Register.");
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            logger.LogInformation($"Authenticating as {loginRequest.Username}...");

            var response = await identityClient.LoginAsync(loginRequest);
            if (response.Success)
            {
                logger.LogInformation("Successfully authenticated.");
                return Ok(response);
            }
            logger.LogError("Un-Successfully authenticated.");
            return BadRequest();
        }

        [HttpGet]
        //     [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            string userId = "";
            logger.LogInformation($"Get profile data for user: {userId}...");
            string token = "";
            logger.LogInformation($"Get token: {token}...");
            Metadata? headers = null;

            if (token != null)
            {
                headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token}");
            }

            var response = await identityClient.GetProfileAsync(null, headers);
            if (response.Success)
            {
                logger.LogInformation("Successfully received data profile.");
                return Ok(response);
            }
            logger.LogError("Un-Successfully received data profile.");
            return BadRequest();
        }
    }
}
