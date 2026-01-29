using AdMarketplace.Database.Models;

namespace AdMarketplace.Database;

public static class CategorySeedData
{
    public static List<Category> GetCategories() =>
    [
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000001"), "News & Media", "News channels, media outlets, journalism", "📰", 1),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000002"), "Technology", "Tech news, gadgets, software, programming", "💻", 2),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000003"), "Entertainment", "Movies, TV shows, celebrities, pop culture", "🎬", 3),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000004"), "Gaming", "Video games, esports, game reviews", "🎮", 4),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000005"), "Music", "Music news, artists, albums, playlists", "🎵", 5),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000006"), "Sports", "Sports news, live scores, teams, athletes", "⚽", 6),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000007"), "Education", "Learning, courses, tutorials, academic content", "📚", 7),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000008"), "Business & Finance", "Business news, investing, cryptocurrency, stocks", "💼", 8),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000009"), "Crypto & Blockchain", "Cryptocurrency, NFTs, DeFi, Web3", "₿", 9),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000000a"), "Travel", "Travel tips, destinations, tourism", "✈️", 10),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000000b"), "Food & Cooking", "Recipes, restaurants, food reviews", "🍕", 11),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000000c"), "Fashion & Beauty", "Fashion trends, beauty tips, lifestyle", "👗", 12),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000000d"), "Health & Fitness", "Health tips, workout routines, wellness", "💪", 13),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000000e"), "Art & Design", "Art, photography, graphic design, creativity", "🎨", 14),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000000f"), "Science", "Science news, research, discoveries", "🔬", 15),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000010"), "Politics", "Political news, government, elections", "🏛️", 16),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000011"), "Humor & Memes", "Funny content, memes, jokes", "😂", 17),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000012"), "Animals & Pets", "Pet care, animal content, wildlife", "🐾", 18),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000013"), "Cars & Vehicles", "Automotive news, car reviews, motorcycles", "🚗", 19),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000014"), "Real Estate", "Property listings, real estate news, home improvement", "🏠", 20),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000015"), "Jobs & Careers", "Job listings, career advice, recruitment", "💼", 21),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000016"), "Shopping & Deals", "Discounts, promotions, product reviews", "🛒", 22),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000017"), "Lifestyle", "General lifestyle, self-improvement, motivation", "🌟", 23),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000018"), "Religion & Spirituality", "Religious content, spirituality, meditation", "🙏", 24),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-000000000019"), "Dating & Relationships", "Dating tips, relationship advice", "❤️", 25),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000001a"), "Parenting & Family", "Parenting tips, family content, children", "👨‍👩‍👧", 26),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000001b"), "Books & Literature", "Book reviews, reading recommendations, authors", "📖", 27),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000001c"), "Languages", "Language learning, translation, linguistics", "🗣️", 28),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000001d"), "Adult 18+", "Adult content (18+ only)", "🔞", 29),
        Category.CreateWithId(new Guid("00000000-0000-0000-0000-00000000001e"), "Other", "Miscellaneous content", "📌", 30)
    ];
}

