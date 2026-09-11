using System.Text.Json;
using AlisPo.Api.Data;
using AlisPo.Api.Models;
using Dapper;

namespace AlisPo.Api.Seeders;

public sealed class ProductSeeder
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ProductSeeder(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task SeedAsync()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Imports",
            "products.json");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "products.json not found.",
                path);
        }

        var json = await File.ReadAllTextAsync(path);

        var products = JsonSerializer.Deserialize<List<ProductImport>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ProductImport>();

        if (products.Count == 0)
        {
            throw new InvalidOperationException(
                "products.json contains no products.");
        }

        var duplicateCodes = products
            .GroupBy(x => x.Id?.Trim() ?? "")
            .Where(g => string.IsNullOrWhiteSpace(g.Key) || g.Count() > 1)
            .Select(g => string.IsNullOrWhiteSpace(g.Key)
                ? "<empty ProductCode>"
                : g.Key)
            .ToList();

        if (duplicateCodes.Count > 0)
        {
            throw new InvalidOperationException(
                "Duplicate or empty ProductCode in products.json: "
                + string.Join(", ", duplicateCodes));
        }

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        // Only synchronize DisplayOrder.
        // Existing product data, suppliers, units, categories and images are untouched.
        var values = @"
(@Code0, @DisplayOrder0),
(@Code1, @DisplayOrder1),
(@Code2, @DisplayOrder2),
(@Code3, @DisplayOrder3),
(@Code4, @DisplayOrder4),
(@Code5, @DisplayOrder5),
(@Code6, @DisplayOrder6),
(@Code7, @DisplayOrder7),
(@Code8, @DisplayOrder8),
(@Code9, @DisplayOrder9),
(@Code10, @DisplayOrder10),
(@Code11, @DisplayOrder11),
(@Code12, @DisplayOrder12),
(@Code13, @DisplayOrder13),
(@Code14, @DisplayOrder14),
(@Code15, @DisplayOrder15),
(@Code16, @DisplayOrder16),
(@Code17, @DisplayOrder17),
(@Code18, @DisplayOrder18),
(@Code19, @DisplayOrder19),
(@Code20, @DisplayOrder20),
(@Code21, @DisplayOrder21),
(@Code22, @DisplayOrder22),
(@Code23, @DisplayOrder23),
(@Code24, @DisplayOrder24),
(@Code25, @DisplayOrder25),
(@Code26, @DisplayOrder26),
(@Code27, @DisplayOrder27),
(@Code28, @DisplayOrder28),
(@Code29, @DisplayOrder29),
(@Code30, @DisplayOrder30),
(@Code31, @DisplayOrder31),
(@Code32, @DisplayOrder32),
(@Code33, @DisplayOrder33),
(@Code34, @DisplayOrder34),
(@Code35, @DisplayOrder35),
(@Code36, @DisplayOrder36),
(@Code37, @DisplayOrder37),
(@Code38, @DisplayOrder38),
(@Code39, @DisplayOrder39),
(@Code40, @DisplayOrder40),
(@Code41, @DisplayOrder41),
(@Code42, @DisplayOrder42),
(@Code43, @DisplayOrder43),
(@Code44, @DisplayOrder44),
(@Code45, @DisplayOrder45),
(@Code46, @DisplayOrder46),
(@Code47, @DisplayOrder47),
(@Code48, @DisplayOrder48),
(@Code49, @DisplayOrder49),
(@Code50, @DisplayOrder50),
(@Code51, @DisplayOrder51),
(@Code52, @DisplayOrder52),
(@Code53, @DisplayOrder53),
(@Code54, @DisplayOrder54),
(@Code55, @DisplayOrder55),
(@Code56, @DisplayOrder56),
(@Code57, @DisplayOrder57),
(@Code58, @DisplayOrder58),
(@Code59, @DisplayOrder59),
(@Code60, @DisplayOrder60),
(@Code61, @DisplayOrder61),
(@Code62, @DisplayOrder62),
(@Code63, @DisplayOrder63),
(@Code64, @DisplayOrder64),
(@Code65, @DisplayOrder65),
(@Code66, @DisplayOrder66),
(@Code67, @DisplayOrder67),
(@Code68, @DisplayOrder68),
(@Code69, @DisplayOrder69),
(@Code70, @DisplayOrder70),
(@Code71, @DisplayOrder71),
(@Code72, @DisplayOrder72),
(@Code73, @DisplayOrder73),
(@Code74, @DisplayOrder74),
(@Code75, @DisplayOrder75),
(@Code76, @DisplayOrder76),
(@Code77, @DisplayOrder77),
(@Code78, @DisplayOrder78),
(@Code79, @DisplayOrder79),
(@Code80, @DisplayOrder80),
(@Code81, @DisplayOrder81),
(@Code82, @DisplayOrder82),
(@Code83, @DisplayOrder83),
(@Code84, @DisplayOrder84),
(@Code85, @DisplayOrder85),
(@Code86, @DisplayOrder86),
(@Code87, @DisplayOrder87),
(@Code88, @DisplayOrder88),
(@Code89, @DisplayOrder89),
(@Code90, @DisplayOrder90),
(@Code91, @DisplayOrder91),
(@Code92, @DisplayOrder92),
(@Code93, @DisplayOrder93),
(@Code94, @DisplayOrder94),
(@Code95, @DisplayOrder95),
(@Code96, @DisplayOrder96),
(@Code97, @DisplayOrder97),
(@Code98, @DisplayOrder98),
(@Code99, @DisplayOrder99),
(@Code100, @DisplayOrder100),
(@Code101, @DisplayOrder101),
(@Code102, @DisplayOrder102),
(@Code103, @DisplayOrder103),
(@Code104, @DisplayOrder104),
(@Code105, @DisplayOrder105),
(@Code106, @DisplayOrder106),
(@Code107, @DisplayOrder107),
(@Code108, @DisplayOrder108),
(@Code109, @DisplayOrder109),
(@Code110, @DisplayOrder110),
(@Code111, @DisplayOrder111),
(@Code112, @DisplayOrder112),
(@Code113, @DisplayOrder113),
(@Code114, @DisplayOrder114),
(@Code115, @DisplayOrder115),
(@Code116, @DisplayOrder116),
(@Code117, @DisplayOrder117),
(@Code118, @DisplayOrder118),
(@Code119, @DisplayOrder119),
(@Code120, @DisplayOrder120),
(@Code121, @DisplayOrder121),
(@Code122, @DisplayOrder122),
(@Code123, @DisplayOrder123),
(@Code124, @DisplayOrder124),
(@Code125, @DisplayOrder125),
(@Code126, @DisplayOrder126),
(@Code127, @DisplayOrder127),
(@Code128, @DisplayOrder128),
(@Code129, @DisplayOrder129),
(@Code130, @DisplayOrder130),
(@Code131, @DisplayOrder131),
(@Code132, @DisplayOrder132),
(@Code133, @DisplayOrder133),
(@Code134, @DisplayOrder134),
(@Code135, @DisplayOrder135),
(@Code136, @DisplayOrder136),
(@Code137, @DisplayOrder137),
(@Code138, @DisplayOrder138),
(@Code139, @DisplayOrder139),
(@Code140, @DisplayOrder140),
(@Code141, @DisplayOrder141),
(@Code142, @DisplayOrder142),
(@Code143, @DisplayOrder143),
(@Code144, @DisplayOrder144),
(@Code145, @DisplayOrder145),
(@Code146, @DisplayOrder146),
(@Code147, @DisplayOrder147),
(@Code148, @DisplayOrder148),
(@Code149, @DisplayOrder149),
(@Code150, @DisplayOrder150),
(@Code151, @DisplayOrder151),
(@Code152, @DisplayOrder152),
(@Code153, @DisplayOrder153),
(@Code154, @DisplayOrder154),
(@Code155, @DisplayOrder155),
(@Code156, @DisplayOrder156),
(@Code157, @DisplayOrder157),
(@Code158, @DisplayOrder158),
(@Code159, @DisplayOrder159),
(@Code160, @DisplayOrder160),
(@Code161, @DisplayOrder161),
(@Code162, @DisplayOrder162),
(@Code163, @DisplayOrder163),
(@Code164, @DisplayOrder164),
(@Code165, @DisplayOrder165),
(@Code166, @DisplayOrder166),
(@Code167, @DisplayOrder167),
(@Code168, @DisplayOrder168),
(@Code169, @DisplayOrder169),
(@Code170, @DisplayOrder170),
(@Code171, @DisplayOrder171),
(@Code172, @DisplayOrder172),
(@Code173, @DisplayOrder173),
(@Code174, @DisplayOrder174),
(@Code175, @DisplayOrder175),
(@Code176, @DisplayOrder176),
(@Code177, @DisplayOrder177),
(@Code178, @DisplayOrder178),
(@Code179, @DisplayOrder179),
(@Code180, @DisplayOrder180),
(@Code181, @DisplayOrder181),
(@Code182, @DisplayOrder182),
(@Code183, @DisplayOrder183),
(@Code184, @DisplayOrder184),
(@Code185, @DisplayOrder185),
(@Code186, @DisplayOrder186),
(@Code187, @DisplayOrder187),
(@Code188, @DisplayOrder188),
(@Code189, @DisplayOrder189),
(@Code190, @DisplayOrder190),
(@Code191, @DisplayOrder191),
(@Code192, @DisplayOrder192),
(@Code193, @DisplayOrder193),
(@Code194, @DisplayOrder194),
(@Code195, @DisplayOrder195),
(@Code196, @DisplayOrder196),
(@Code197, @DisplayOrder197),
(@Code198, @DisplayOrder198),
(@Code199, @DisplayOrder199),
(@Code200, @DisplayOrder200),
(@Code201, @DisplayOrder201),
(@Code202, @DisplayOrder202),
(@Code203, @DisplayOrder203),
(@Code204, @DisplayOrder204),
(@Code205, @DisplayOrder205),
(@Code206, @DisplayOrder206),
(@Code207, @DisplayOrder207),
(@Code208, @DisplayOrder208),
(@Code209, @DisplayOrder209),
(@Code210, @DisplayOrder210),
(@Code211, @DisplayOrder211),
(@Code212, @DisplayOrder212),
(@Code213, @DisplayOrder213),
(@Code214, @DisplayOrder214),
(@Code215, @DisplayOrder215),
(@Code216, @DisplayOrder216),
(@Code217, @DisplayOrder217),
(@Code218, @DisplayOrder218),
(@Code219, @DisplayOrder219),
(@Code220, @DisplayOrder220),
(@Code221, @DisplayOrder221),
(@Code222, @DisplayOrder222),
(@Code223, @DisplayOrder223),
(@Code224, @DisplayOrder224),
(@Code225, @DisplayOrder225),
(@Code226, @DisplayOrder226),
(@Code227, @DisplayOrder227),
(@Code228, @DisplayOrder228),
(@Code229, @DisplayOrder229),
(@Code230, @DisplayOrder230),
(@Code231, @DisplayOrder231),
(@Code232, @DisplayOrder232),
(@Code233, @DisplayOrder233),
(@Code234, @DisplayOrder234),
(@Code235, @DisplayOrder235),
(@Code236, @DisplayOrder236),
(@Code237, @DisplayOrder237),
(@Code238, @DisplayOrder238),
(@Code239, @DisplayOrder239),
(@Code240, @DisplayOrder240),
(@Code241, @DisplayOrder241),
(@Code242, @DisplayOrder242),
(@Code243, @DisplayOrder243),
(@Code244, @DisplayOrder244),
(@Code245, @DisplayOrder245),
(@Code246, @DisplayOrder246),
(@Code247, @DisplayOrder247),
(@Code248, @DisplayOrder248),
(@Code249, @DisplayOrder249),
(@Code250, @DisplayOrder250),
(@Code251, @DisplayOrder251),
(@Code252, @DisplayOrder252),
(@Code253, @DisplayOrder253),
(@Code254, @DisplayOrder254),
(@Code255, @DisplayOrder255),
(@Code256, @DisplayOrder256),
(@Code257, @DisplayOrder257),
(@Code258, @DisplayOrder258),
(@Code259, @DisplayOrder259),
(@Code260, @DisplayOrder260),
(@Code261, @DisplayOrder261),
(@Code262, @DisplayOrder262),
(@Code263, @DisplayOrder263),
(@Code264, @DisplayOrder264),
(@Code265, @DisplayOrder265),
(@Code266, @DisplayOrder266),
(@Code267, @DisplayOrder267),
(@Code268, @DisplayOrder268),
(@Code269, @DisplayOrder269),
(@Code270, @DisplayOrder270),
(@Code271, @DisplayOrder271),
(@Code272, @DisplayOrder272),
(@Code273, @DisplayOrder273),
(@Code274, @DisplayOrder274),
(@Code275, @DisplayOrder275),
(@Code276, @DisplayOrder276),
(@Code277, @DisplayOrder277),
(@Code278, @DisplayOrder278),
(@Code279, @DisplayOrder279),
(@Code280, @DisplayOrder280),
(@Code281, @DisplayOrder281),
(@Code282, @DisplayOrder282),
(@Code283, @DisplayOrder283),
(@Code284, @DisplayOrder284),
(@Code285, @DisplayOrder285),
(@Code286, @DisplayOrder286),
(@Code287, @DisplayOrder287),
(@Code288, @DisplayOrder288),
(@Code289, @DisplayOrder289),
(@Code290, @DisplayOrder290),
(@Code291, @DisplayOrder291),
(@Code292, @DisplayOrder292),
(@Code293, @DisplayOrder293),
(@Code294, @DisplayOrder294),
(@Code295, @DisplayOrder295),
(@Code296, @DisplayOrder296),
(@Code297, @DisplayOrder297),
(@Code298, @DisplayOrder298),
(@Code299, @DisplayOrder299),
(@Code300, @DisplayOrder300),
(@Code301, @DisplayOrder301),
(@Code302, @DisplayOrder302),
(@Code303, @DisplayOrder303),
(@Code304, @DisplayOrder304),
(@Code305, @DisplayOrder305),
(@Code306, @DisplayOrder306),
(@Code307, @DisplayOrder307),
(@Code308, @DisplayOrder308),
(@Code309, @DisplayOrder309),
(@Code310, @DisplayOrder310),
(@Code311, @DisplayOrder311),
(@Code312, @DisplayOrder312),
(@Code313, @DisplayOrder313),
(@Code314, @DisplayOrder314),
(@Code315, @DisplayOrder315),
(@Code316, @DisplayOrder316),
(@Code317, @DisplayOrder317),
(@Code318, @DisplayOrder318),
(@Code319, @DisplayOrder319),
(@Code320, @DisplayOrder320),
(@Code321, @DisplayOrder321),
(@Code322, @DisplayOrder322),
(@Code323, @DisplayOrder323),
(@Code324, @DisplayOrder324),
(@Code325, @DisplayOrder325),
(@Code326, @DisplayOrder326),
(@Code327, @DisplayOrder327),
(@Code328, @DisplayOrder328),
(@Code329, @DisplayOrder329),
(@Code330, @DisplayOrder330),
(@Code331, @DisplayOrder331),
(@Code332, @DisplayOrder332),
(@Code333, @DisplayOrder333),
(@Code334, @DisplayOrder334),
(@Code335, @DisplayOrder335),
(@Code336, @DisplayOrder336),
(@Code337, @DisplayOrder337),
(@Code338, @DisplayOrder338),
(@Code339, @DisplayOrder339),
(@Code340, @DisplayOrder340),
(@Code341, @DisplayOrder341),
(@Code342, @DisplayOrder342),
(@Code343, @DisplayOrder343),
(@Code344, @DisplayOrder344),
(@Code345, @DisplayOrder345),
(@Code346, @DisplayOrder346),
(@Code347, @DisplayOrder347),
(@Code348, @DisplayOrder348),
(@Code349, @DisplayOrder349),
(@Code350, @DisplayOrder350),
(@Code351, @DisplayOrder351),
(@Code352, @DisplayOrder352),
(@Code353, @DisplayOrder353),
(@Code354, @DisplayOrder354),
(@Code355, @DisplayOrder355),
(@Code356, @DisplayOrder356),
(@Code357, @DisplayOrder357),
(@Code358, @DisplayOrder358),
(@Code359, @DisplayOrder359),
(@Code360, @DisplayOrder360),
(@Code361, @DisplayOrder361),
(@Code362, @DisplayOrder362),
(@Code363, @DisplayOrder363),
(@Code364, @DisplayOrder364),
(@Code365, @DisplayOrder365),
(@Code366, @DisplayOrder366),
(@Code367, @DisplayOrder367),
(@Code368, @DisplayOrder368),
(@Code369, @DisplayOrder369),
(@Code370, @DisplayOrder370),
(@Code371, @DisplayOrder371),
(@Code372, @DisplayOrder372),
(@Code373, @DisplayOrder373),
(@Code374, @DisplayOrder374)
";

        var sql = $@"
DECLARE @ProductOrder TABLE
(
    ProductCode NVARCHAR(50) NOT NULL PRIMARY KEY,
    DisplayOrder INT NOT NULL
);

INSERT INTO @ProductOrder (ProductCode, DisplayOrder)
VALUES
{{VALUES}};

UPDATE p
SET
    p.DisplayOrder = o.DisplayOrder
FROM Products p
INNER JOIN @ProductOrder o
    ON o.ProductCode = p.ProductCode;
";

        // Build parameters dynamically to keep the source readable.
        var parameterValues = new Dictionary<string, object?>();
        for (var i = 0; i < products.Count; i++)
        {
            parameterValues[$"Code{i}"] = products[i].Id.Trim();
            parameterValues[$"DisplayOrder{i}"] = i + 1;
        }

        sql = sql.Replace("{VALUES}", values);

        var updated = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                parameterValues,
                commandTimeout: 60));

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine(" Product DisplayOrder Sync Completed");
        Console.WriteLine("========================================");
        Console.WriteLine($"Products in JSON : {products.Count}");
        Console.WriteLine($"Rows updated     : {updated}");
        Console.WriteLine("Deleted          : 0");
        Console.WriteLine("Product data     : untouched");
        Console.WriteLine("Order            : follows product_data.dart");
        Console.WriteLine("========================================");
        Console.WriteLine();
    }
}
