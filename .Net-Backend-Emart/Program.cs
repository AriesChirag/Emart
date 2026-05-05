using Microsoft.EntityFrameworkCore;
using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using Emart_DotNet.Services;
using Emart_DotNet.Configuration;
using Emart_DotNet.Utilities.Helpers;
using Emart_DotNet.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace Emart_DotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ==================== SERILOG CONFIGURATION ====================
            // Serilog = structured logging framework
            // Logs to both console (development) and file (production)
            // Better than Console.WriteLine() - structured, filterable, rotatable logs

            Log.Logger = new LoggerConfiguration()
                // Set minimum log level to Information
                // Levels: Verbose < Debug < Information < Warning < Error < Fatal
                .MinimumLevel.Information()

                // Suppress noisy logs from Microsoft frameworks
                // These frameworks log a LOT - we only want Warning+ from them
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                // Suppress ASP.NET Core infrastructure logs
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                // Suppress Entity Framework logs (very verbose)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                // Suppress System logs
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)

                // Write logs to console (real-time viewing in terminal)
                .WriteTo.Console()

                // Write logs to file
                // "emart-.txt" = creates emart-20250101.txt, emart-20250102.txt, etc.
                .WriteTo.File("Logs/emart-.txt",
                    rollingInterval: RollingInterval.Day,  // Create new file each day
                                                           // Format: "2025-01-09 14:30:45 [INF] Message {NewLine} Exception"
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                // ==================== WEB APPLICATION BUILDER ====================
                // WebApplication.CreateBuilder = setup configuration, services, middleware
                // This is the starting point for ASP.NET Core application
                var builder = WebApplication.CreateBuilder(args);

                // Use Serilog as logging provider instead of built-in logger
                builder.Host.UseSerilog();

                // Set server URL = localhost:8080
                // Client will connect to http://localhost:8080
                builder.WebHost.UseUrls("http://localhost:8080");

                // ==================== CONTROLLERS & FILTERS ====================
                // AddControllers = register all API controllers from project
                builder.Services.AddControllers(options =>
                {
                    // Add custom logging filter to ALL controllers
                    // This filter runs BEFORE and AFTER each API request
                    // Logs: request details, response time, errors, etc.
                    options.Filters.Add<Emart_DotNet.Utilities.Filters.LoggingActionFilter>();
                });

                // ==================== CONFIGURATION SETUP ====================
                // Configuration = read from appsettings.json file
                // JwtSettings section contains: Secret, Issuer, Audience, ExpirationMinutes
                builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

                // ==================== DEPENDENCY INJECTION - HELPERS & SERVICES ====================
                // AddScoped = create NEW instance for EACH HTTP request (disposed after request)
                // This is good for: DbContext, services, repositories (request-scoped data)

                // JwtHelper = generates JWT tokens after login/register
                builder.Services.AddScoped<JwtHelper>();

                // PasswordHelper = hashes passwords (stores hash in DB, not plain password)
                // Hashing = one-way conversion (password → hash, can't reverse)
                // For login: hash incoming password, compare with stored hash
                builder.Services.AddScoped<PasswordHelper>();

                // IAuthService = interface, AuthService = implementation
                // Service handles: login, register, OAuth, token refresh
                builder.Services.AddScoped<IAuthService, AuthService>();

                // ==================== JWT AUTHENTICATION SETUP ====================
                // JWT = JSON Web Token - stateless authentication
                // Token contains: user info, expiration, signature
                // No session stored on server (scalable)

                // Get JWT settings from appsettings.json
                var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
                var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

                // Secret key = used to sign/verify tokens
                // Must be at least 32 characters for security
                // If missing from config, use default (insecure - should never happen in production)
                var secretKey = jwtSettings?.Secret ?? "YourSuperSecretKeyForJwtSigning_MustBeAtLeast32CharsLong";

                // Configure authentication schemes
                builder.Services.AddAuthentication(options =>
                {
                    // Default authentication = JWT Bearer (validate token in Authorization header)
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                    // Default challenge = JWT Bearer (if not authenticated, send 401 challenge)
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                    // Default sign-in = Cookie (for OAuth flows, sign in via cookie first)
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                // Add Cookie authentication (needed for OAuth Google login flow)
                .AddCookie()

                // Add JWT Bearer token authentication
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Validate token issuer (who created the token)
                        ValidateIssuer = true,

                        // Validate token audience (who the token is for)
                        ValidateAudience = true,

                        // Validate expiration time (token must not be expired)
                        ValidateLifetime = true,

                        // Validate signature (token wasn't tampered with)
                        ValidateIssuerSigningKey = true,

                        // The issuer value that tokens must have
                        ValidIssuer = jwtSettings?.Issuer,

                        // The audience value that tokens must have
                        ValidAudience = jwtSettings?.Audience,

                        // Key used to verify signature
                        // SymmetricSecurityKey = same key for signing and verifying
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

                        // ClockSkew = allow small time differences (network latency)
                        // TimeSpan.Zero = no tolerance (strict)
                        ClockSkew = TimeSpan.Zero
                    };
                })

                // Add Google OAuth authentication
                .AddGoogle(googleOptions =>
                {
                    // Google Client ID from appsettings.json (get from Google Console)
                    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];

                    // Google Client Secret from appsettings.json (keep this SECRET!)
                    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

                    // When user logs in with Google, sign in via Cookie first
                    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    // Google redirects back here after authentication
                    // Must match redirect URI in Google Console exactly
                    googleOptions.CallbackPath = "/signin-google";
                });

                // ==================== SWAGGER/OPENAPI DOCUMENTATION ====================
                // Swagger = auto-generated API documentation
                // Developers can see all endpoints, try them, understand parameters

                // Enable endpoints for Swagger
                builder.Services.AddEndpointsApiExplorer();

                // Configure Swagger generation
                builder.Services.AddSwaggerGen(options =>
                {
                    // API documentation metadata
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "E-Mart API",    // API name in Swagger UI
                        Version = "v1"           // Version number
                    });

                    // ===== JWT in Swagger =====
                    // Allow Swagger UI to accept JWT tokens for testing

                    // Define "Bearer" security scheme
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",                    // Header name
                        Type = SecuritySchemeType.Http,            // HTTP authentication
                        Scheme = "Bearer",                         // Bearer token
                        BearerFormat = "JWT",                      // Token format
                        In = ParameterLocation.Header,             // Token location (header)
                        Description = "Enter 'Bearer' followed by your JWT token"
                    });

                    // Apply security requirement to all endpoints in Swagger
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });

                // ==================== DATABASE CONFIGURATION ====================
                // DbContext = Entity Framework's gateway to database
                // Maps C# objects to database tables and queries

                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(
                        // Connection string from appsettings.json
                        // Format: "Server=localhost;Database=emart;User=root;Password=..."
                        builder.Configuration.GetConnectionString("DefaultConnection"),

                        // MySQL version = 8.0.40
                        // Must match actual database version
                        Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"),

                        // MySQL-specific options
                        mySqlOptions => mySqlOptions
                            // Retry mechanism if connection fails
                            // If connection fails, retry up to 5 times
                            .EnableRetryOnFailure(
                                maxRetryCount: 5,                                   // 5 attempts
                                maxRetryDelay: TimeSpan.FromSeconds(10),           // Wait max 10 seconds between retries
                                errorNumbersToAdd: null)                           // Retry on all MySQL errors

                            // Query splitting behavior
                            // Avoids SQL cartesian explosion when joining multiple collections
                            // SplitQuery = execute multiple queries instead of one complex JOIN
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    ));

                // ==================== CORS CONFIGURATION ====================
                // CORS = Cross-Origin Resource Sharing
                // Allows frontend (React) on different URL/port to call backend API

                builder.Services.AddCors(options =>
                {
                    // Define CORS policy named "AllowReact"
                    options.AddPolicy("AllowReact",
                        policy => policy
                            // Allow requests from these origins (frontend URLs)
                            .WithOrigins("http://localhost:5173", "http://localhost:3000")
                            // Allow any HTTP header (Content-Type, Authorization, etc.)
                            .AllowAnyHeader()
                            // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                            .AllowAnyMethod());
                });

                // ==================== REPOSITORY REGISTRATION ====================
                // Repository Pattern = abstraction for data access
                // Interface (ICartRepository) ← Implementation (CartRepository)
                // Benefits: loose coupling, easy testing, centralized DB logic

                // AddScoped = create one instance per HTTP request

                // ===== Cart Repositories =====
                builder.Services.AddScoped<ICartRepository, CartRepository>();
                builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();

                // ===== Product Repositories =====
                builder.Services.AddScoped<IProductRepository, ProductRepository>();

                // ===== Order Repositories =====
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

                // ===== Address Repositories =====
                builder.Services.AddScoped<IAddressRepository, AddressRepository>();

                // ===== Store Repositories =====
                builder.Services.AddScoped<IStoreRepository, StoreRepository>();

                // ===== Customer Repositories =====
                builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

                // ===== Payment Repositories =====
                builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

                // ==================== SERVICE REGISTRATION ====================
                // Services = business logic layer
                // Takes data from repositories, applies business rules, returns results

                // ===== Core Services =====
                builder.Services.AddScoped<ICartService, CartService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<IPaymentService, PaymentService>();
                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<IStoreService, StoreService>();
                builder.Services.AddScoped<ICheckoutService, CheckoutService>();

                // ===== Invoice Services =====
                builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
                builder.Services.AddScoped<IInvoiceService, InvoiceService>();

                // ===== Category Services =====
                builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
                builder.Services.AddScoped<ICategoryService, CategoryService>();

                // ===== Sub-Category Services =====
                builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
                builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();

                // ===== Address Services =====
                builder.Services.AddScoped<IAddressService, AddressService>();

                // ===== Loyalty Points Services =====
                builder.Services.AddScoped<IEPointsService, EPointsService>();

                // ===== Card Services =====
                builder.Services.AddScoped<IEmartCardRepository, EmartCardRepository>();
                builder.Services.AddScoped<IEmartCardService, EmartCardService>();

                // ===== User Services =====
                // IUserService = handles user login, register, profile
                builder.Services.AddScoped<IUserService, UserService>();

                // ===== Email Services =====
                // Sends emails (password reset, order confirmation, etc.)
                builder.Services.AddScoped<IEmailService, EmailService>();

                // ===== Admin Services =====
                builder.Services.AddScoped<IAdminRepository, AdminRepository>();
                builder.Services.AddScoped<IAdminService, AdminService>();
                builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

                // ==================== HTTP CLIENT & UTILITIES ====================
                // HttpClient = for making HTTP calls to external APIs
                // Example: payment gateway API, SMS service, analytics service
                builder.Services.AddHttpClient();

                // Health Checks = endpoint to check if app is running
                // Used by load balancers, monitoring tools to verify health
                builder.Services.AddHealthChecks();

                // ==================== BUILD APPLICATION ====================
                // All configuration is complete, now build the app
                var app = builder.Build();

                // ==================== MIDDLEWARE PIPELINE ====================
                // Middleware = processes every HTTP request/response
                // Order matters! Execute top to bottom

                // Development-only middleware
                if (app.Environment.IsDevelopment())
                {
                    // Enable Swagger UI at /swagger
                    // Developers can view/test all endpoints
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    // In Production: disable Swagger (security - don't expose API structure)
                }

                // ===== EXCEPTION HANDLING =====
                // Global exception handler middleware (MUST be early in pipeline)
                // Catches unhandled exceptions from all requests
                // Returns consistent JSON error response instead of HTML error page
                app.UseGlobalExceptionHandler();

                // ===== STATIC FILES =====
                // Serve static files (images, CSS, JS) from wwwroot folder
                // Example: /wwwroot/images/product.jpg → accessible as /images/product.jpg
                app.UseStaticFiles();

                // ===== HTTPS REDIRECT =====
                // In production: redirect HTTP → HTTPS (security)
                app.UseHttpsRedirection();

                // ===== CORS =====
                // Use CORS policy defined earlier
                // Allows frontend (React) to call backend API
                app.UseCors(policy => policy
                    .AllowAnyOrigin()        // Allow any origin (frontend URL)
                    .AllowAnyMethod()        // Allow any HTTP method
                    .AllowAnyHeader());      // Allow any header

                // ===== AUTHENTICATION & AUTHORIZATION =====
                // Order: Authentication BEFORE Authorization (must know who you are before checking permissions)

                // Authentication = verify identity (who are you?)
                // Validates JWT tokens, cookies, OAuth credentials
                app.UseAuthentication();

                // Authorization = verify permissions (what can you do?)
                // Checks [Authorize], [AllowAnonymous] attributes on controllers
                app.UseAuthorization();

                // ===== ROUTE MAPPING =====
                // Map all controller routes
                // Example: POST /api/users/login → UserController.Login()
                app.MapControllers();

                // ===== HEALTH CHECKS & ACTUATOR =====
                // These are monitoring endpoints (similar to Spring Boot Actuator)

                // Health check endpoint
                // Returns 200 if app is running, can check DB connection too
                app.MapHealthChecks("/actuator/health");

                // Application info endpoint
                // Returns: app name, version
                app.MapGet("/actuator/info", () => new { app = new { name = "Emart .NET Backend", version = "1.0.0" } });

                // Metrics endpoint (for monitoring)
                // Not fully implemented but endpoint exists for future use
                app.MapGet("/actuator/metrics", () => new { message = "Metrics not fully implemented but endpoint exists" });

                // ==================== START APPLICATION ====================
                Log.Information("Emart .NET Backend started successfully");
                // Run the application on configured URL (localhost:8080)
                // Listens for HTTP requests and processes them through middleware pipeline
                app.Run();
            }
            catch (Exception ex)
            {
                // If app crashes during startup, log the fatal error
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                // Always flush logs when shutting down
                // Ensures all logs are written to file before closing
                Log.CloseAndFlush();
            }
        }
    }
}

