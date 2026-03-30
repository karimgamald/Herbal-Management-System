using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedExternalIdAndAddRecipePriceAndDiseaseAiSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Herbs",
                keyColumn: "HerbId",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "ExternalDeliveryID",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TrackingNumber",
                table: "SubOrders",
                newName: "ExternalDeliveryID");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Recipes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsSupportedByAi",
                table: "Diseases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 1,
                column: "IsSupportedByAi",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 2,
                column: "IsSupportedByAi",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 3,
                column: "IsSupportedByAi",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 4,
                column: "IsSupportedByAi",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 5,
                column: "IsSupportedByAi",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IsSupportedByAi",
                table: "Diseases");

            migrationBuilder.RenameColumn(
                name: "ExternalDeliveryID",
                table: "SubOrders",
                newName: "TrackingNumber");

            migrationBuilder.AddColumn<string>(
                name: "ExternalDeliveryID",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.InsertData(
                table: "Herbs",
                columns: new[] { "HerbId", "AddedByHerbalistId", "Benefits", "Description", "Dosage", "HerbName", "ImageURL", "IsApproved", "ScientificName", "Warnings" },
                values: new object[,]
                {
                    { 1, null, "Relieves nausea, reduces inflammation, and aids digestion.", "A widely used root known for its spicy flavor and medicinal properties.", "1-3 grams daily", "Ginger", null, true, "Zingiber officinale", "High doses may cause heartburn or interact with blood thinners." },
                    { 2, null, "Promotes sleep, reduces anxiety, and soothes stomach aches.", "A daisy-like plant commonly used to make herb infusions.", "1-2 cups of tea daily", "Chamomile", null, true, "Matricaria chamomilla", "May cause allergic reactions in people sensitive to ragweed." },
                    { 3, null, "Powerful anti-inflammatory and antioxidant effects.", "A bright yellow spice widely used in Indian cuisine and Ayurvedic medicine.", "500-2000 mg daily (with black pepper)", "Turmeric", null, true, "Curcuma longa", "Can cause stomach upset in large amounts." },
                    { 4, null, "Relieves irritable bowel syndrome (IBS), eases headaches, and clears congestion.", "A hybrid mint cross between watermint and spearmint.", "1-2 cups of tea or 0.2ml essential oil capsule", "Peppermint", null, true, "Mentha piperita", "May worsen acid reflux (GERD)." },
                    { 5, null, "Boosts immune system, reduces blood pressure, and improves cholesterol levels.", "A pungent bulb used extensively in cooking and traditional medicine.", "1-2 cloves raw daily", "Garlic", null, true, "Allium sativum", "Bad breath, heartburn, and may increase bleeding risk." },
                    { 6, null, "Reduces stress and cortisol levels, boosts brain function.", "An ancient medicinal herb classified as an adaptogen.", "300-500 mg root extract daily", "Ashwagandha", null, true, "Withania somnifera", "Not recommended for pregnant women or those with autoimmune diseases." },
                    { 7, null, "Prevents and treats the common cold, boosts immunity.", "A flowering plant in the daisy family, popular for fighting flu.", "300-500 mg daily during illness", "Echinacea", null, true, "Echinacea purpurea", "May cause mild stomach upset or allergic reactions." },
                    { 8, null, "Reduces anxiety, promotes restful sleep, and heals minor burns (topical).", "A fragrant purple flower known for its calming scent.", "1 cup of tea or aromatherapy", "Lavender", null, true, "Lavandula angustifolia", "Not recommended for young boys (hormonal effects) if used topically in large amounts." },
                    { 9, null, "Increases energy, lowers blood sugar, and improves cognitive function.", "A slow-growing plant with fleshy roots, popular in Chinese medicine.", "200-400 mg daily", "Ginseng", null, true, "Panax ginseng", "Can cause insomnia or interact with diabetes medications." },
                    { 10, null, "Improves memory and focus, promotes hair growth (topical).", "A fragrant evergreen herb native to the Mediterranean.", "1-2 cups of tea or used as a spice", "Rosemary", null, true, "Salvia rosmarinus", "Extremely high doses can trigger seizures." }
                });
        }
    }
}
