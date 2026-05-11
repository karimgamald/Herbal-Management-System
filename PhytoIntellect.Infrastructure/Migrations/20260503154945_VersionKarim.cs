using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VersionKarim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diseases",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiseaseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiseaseType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSupportedByAi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diseases", x => x.DiseaseId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    EmailConfirmationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmationTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Herbalists",
                columns: table => new
                {
                    HerbalistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailableFrom = table.Column<TimeSpan>(type: "time", nullable: true),
                    AvailableTo = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herbalists", x => x.HerbalistId);
                    table.ForeignKey(
                        name: "FK_Herbalists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Herbs",
                columns: table => new
                {
                    HerbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HerbName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Benefits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Warnings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AddedByHerbalistId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herbs", x => x.HerbId);
                    table.ForeignKey(
                        name: "FK_Herbs_Herbalists_AddedByHerbalistId",
                        column: x => x.AddedByHerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HerbalistId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AverageRating = table.Column<float>(type: "real", nullable: false),
                    TotalRatings = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.RecipeId);
                    table.ForeignKey(
                        name: "FK_Recipes_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AiRecipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    WeightKg = table.Column<double>(type: "float", nullable: false),
                    HeightCm = table.Column<double>(type: "float", nullable: false),
                    Bmi = table.Column<double>(type: "float", nullable: false),
                    SeverityScore = table.Column<int>(type: "int", nullable: false),
                    SystolicBp = table.Column<int>(type: "int", nullable: false),
                    DiastolicBp = table.Column<int>(type: "int", nullable: false),
                    TemperatureCelsius = table.Column<double>(type: "float", nullable: false),
                    HeartRateBpm = table.Column<int>(type: "int", nullable: false),
                    SymptomDurationDays = table.Column<int>(type: "int", nullable: false),
                    HasDiabetes = table.Column<bool>(type: "bit", nullable: false),
                    HasHypertension = table.Column<bool>(type: "bit", nullable: false),
                    HasAllergies = table.Column<bool>(type: "bit", nullable: false),
                    IsPregnant = table.Column<bool>(type: "bit", nullable: false),
                    IsSmoker = table.Column<bool>(type: "bit", nullable: false),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedRecipeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ConfidenceScore = table.Column<double>(type: "float", nullable: false),
                    PreparationInstructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CautionWarning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Rating = table.Column<float>(type: "real", nullable: true),
                    HerbalistAverageRating = table.Column<float>(type: "real", nullable: false),
                    HerbalistTotalRatings = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiRecipes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalHistories",
                columns: table => new
                {
                    MedicalHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Diabetes = table.Column<bool>(type: "bit", nullable: false),
                    Hypertension = table.Column<bool>(type: "bit", nullable: false),
                    Asthma = table.Column<bool>(type: "bit", nullable: false),
                    Smoker = table.Column<bool>(type: "bit", nullable: false),
                    Pregnancy = table.Column<bool>(type: "bit", nullable: false),
                    HeartDisease = table.Column<bool>(type: "bit", nullable: false),
                    KidneyDisease = table.Column<bool>(type: "bit", nullable: false),
                    LiverDisease = table.Column<bool>(type: "bit", nullable: false),
                    OtherNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistories", x => x.MedicalHistoryId);
                    table.ForeignKey(
                        name: "FK_MedicalHistories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ItemsTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalPaymentID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HerbalistHerbs",
                columns: table => new
                {
                    HerbalistId = table.Column<int>(type: "int", nullable: false),
                    HerbId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerbalistHerbs", x => new { x.HerbalistId, x.HerbId });
                    table.ForeignKey(
                        name: "FK_HerbalistHerbs_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HerbalistHerbs_Herbs_HerbId",
                        column: x => x.HerbId,
                        principalTable: "Herbs",
                        principalColumn: "HerbId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeDiseases",
                columns: table => new
                {
                    RecipeDiseaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    DiseaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeDiseases", x => x.RecipeDiseaseId);
                    table.ForeignKey(
                        name: "FK_RecipeDiseases_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Diseases",
                        principalColumn: "DiseaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeDiseases_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeHerbs",
                columns: table => new
                {
                    RecipeHerbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    HerbId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeHerbs", x => x.RecipeHerbId);
                    table.ForeignKey(
                        name: "FK_RecipeHerbs_Herbs_HerbId",
                        column: x => x.HerbId,
                        principalTable: "Herbs",
                        principalColumn: "HerbId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeHerbs_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<float>(type: "real", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: true),
                    AiRecipeId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackId);
                    table.CheckConstraint("CK_Feedback_RatingValue", "[RatingValue] >= 1 AND [RatingValue] <= 5");
                    table.CheckConstraint("CK_Feedback_Target", "([RecipeId] IS NOT NULL AND [AiRecipeId] IS NULL) OR ([RecipeId] IS NULL AND [AiRecipeId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Feedbacks_AiRecipes_AiRecipeId",
                        column: x => x.AiRecipeId,
                        principalTable: "AiRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HerbalistAiRecipes",
                columns: table => new
                {
                    HerbalistId = table.Column<int>(type: "int", nullable: false),
                    AiRecipeId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerbalistAiRecipes", x => new { x.HerbalistId, x.AiRecipeId });
                    table.ForeignKey(
                        name: "FK_HerbalistAiRecipes_AiRecipes_AiRecipeId",
                        column: x => x.AiRecipeId,
                        principalTable: "AiRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HerbalistAiRecipes_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewRecipes",
                columns: table => new
                {
                    ReviewRecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<float>(type: "real", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AiRecipeId = table.Column<int>(type: "int", nullable: false),
                    HerbalistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewRecipes", x => x.ReviewRecipeId);
                    table.CheckConstraint("CK_ReviewRecipe_RatingValue", "[RatingValue] >= 1 AND [RatingValue] <= 5");
                    table.ForeignKey(
                        name: "FK_ReviewRecipes_AiRecipes_AiRecipeId",
                        column: x => x.AiRecipeId,
                        principalTable: "AiRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewRecipes_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubOrders",
                columns: table => new
                {
                    SubOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    HerbalistId = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalDeliveryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubOrders", x => x.SubOrderId);
                    table.ForeignKey(
                        name: "FK_SubOrders_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderAiRecipes",
                columns: table => new
                {
                    OrderAiRecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubOrderId = table.Column<int>(type: "int", nullable: false),
                    AiRecipeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAiRecipes", x => x.OrderAiRecipeId);
                    table.ForeignKey(
                        name: "FK_OrderAiRecipes_AiRecipes_AiRecipeId",
                        column: x => x.AiRecipeId,
                        principalTable: "AiRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAiRecipes_SubOrders_SubOrderId",
                        column: x => x.SubOrderId,
                        principalTable: "SubOrders",
                        principalColumn: "SubOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderHerbs",
                columns: table => new
                {
                    OrderHerbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubOrderId = table.Column<int>(type: "int", nullable: false),
                    HerbId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHerbs", x => x.OrderHerbId);
                    table.ForeignKey(
                        name: "FK_OrderHerbs_Herbs_HerbId",
                        column: x => x.HerbId,
                        principalTable: "Herbs",
                        principalColumn: "HerbId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderHerbs_SubOrders_SubOrderId",
                        column: x => x.SubOrderId,
                        principalTable: "SubOrders",
                        principalColumn: "SubOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderRecipes",
                columns: table => new
                {
                    OrderRecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubOrderId = table.Column<int>(type: "int", nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRecipes", x => x.OrderRecipeId);
                    table.ForeignKey(
                        name: "FK_OrderRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderRecipes_SubOrders_SubOrderId",
                        column: x => x.SubOrderId,
                        principalTable: "SubOrders",
                        principalColumn: "SubOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Diseases",
                columns: new[] { "DiseaseId", "Description", "DiseaseName", "DiseaseType", "IsSupportedByAi", "Symptoms" },
                values: new object[,]
                {
                    { 1, "Habitual sleeplessness or inability to sleep.", "Insomnia", "Neurological", false, "Difficulty falling asleep, waking up often." },
                    { 2, "A common disorder that affects the large intestine.", "Irritable Bowel Syndrome (IBS)", "Gastrointestinal", false, "Cramping, abdominal pain, bloating, gas." },
                    { 3, "A viral infection of your nose and throat.", "Common Cold", "Respiratory", false, "Runny nose, sore throat, cough, congestion." },
                    { 4, "A feeling of worry, nervousness, or unease.", "Anxiety", "Psychological", false, "Restlessness, rapid breathing, increased heart rate." },
                    { 5, "Discomfort in your upper abdomen.", "Indigestion", "Gastrointestinal", false, "Bloating, nausea, belching." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiRecipes_PatientId",
                table: "AiRecipes",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AiRecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "AiRecipeId", "PatientId" },
                unique: true,
                filter: "[AiRecipeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_PatientId",
                table: "Feedbacks",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "RecipeId", "PatientId" },
                unique: true,
                filter: "[RecipeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HerbalistAiRecipes_AiRecipeId",
                table: "HerbalistAiRecipes",
                column: "AiRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_HerbalistHerbs_HerbId",
                table: "HerbalistHerbs",
                column: "HerbId");

            migrationBuilder.CreateIndex(
                name: "IX_Herbalists_UserId",
                table: "Herbalists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Herbs_AddedByHerbalistId",
                table: "Herbs",
                column: "AddedByHerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistories_PatientId",
                table: "MedicalHistories",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAiRecipes_AiRecipeId",
                table: "OrderAiRecipes",
                column: "AiRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAiRecipes_SubOrderId",
                table: "OrderAiRecipes",
                column: "SubOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHerbs_HerbId",
                table: "OrderHerbs",
                column: "HerbId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHerbs_SubOrderId",
                table: "OrderHerbs",
                column: "SubOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRecipes_RecipeId",
                table: "OrderRecipes",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRecipes_SubOrderId",
                table: "OrderRecipes",
                column: "SubOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PatientId",
                table: "Orders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDiseases_DiseaseId",
                table: "RecipeDiseases",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDiseases_RecipeId",
                table: "RecipeDiseases",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHerbs_HerbId",
                table: "RecipeHerbs",
                column: "HerbId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHerbs_RecipeId",
                table: "RecipeHerbs",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_HerbalistId",
                table: "Recipes",
                column: "HerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipes_AiRecipeId_HerbalistId",
                table: "ReviewRecipes",
                columns: new[] { "AiRecipeId", "HerbalistId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipes_HerbalistId",
                table: "ReviewRecipes",
                column: "HerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_SubOrders_HerbalistId",
                table: "SubOrders",
                column: "HerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_SubOrders_OrderId",
                table: "SubOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId_TargetId_Type",
                table: "UserFavorites",
                columns: new[] { "UserId", "TargetId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "HerbalistAiRecipes");

            migrationBuilder.DropTable(
                name: "HerbalistHerbs");

            migrationBuilder.DropTable(
                name: "MedicalHistories");

            migrationBuilder.DropTable(
                name: "OrderAiRecipes");

            migrationBuilder.DropTable(
                name: "OrderHerbs");

            migrationBuilder.DropTable(
                name: "OrderRecipes");

            migrationBuilder.DropTable(
                name: "RecipeDiseases");

            migrationBuilder.DropTable(
                name: "RecipeHerbs");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ReviewRecipes");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "SubOrders");

            migrationBuilder.DropTable(
                name: "Diseases");

            migrationBuilder.DropTable(
                name: "Herbs");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "AiRecipes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Herbalists");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
