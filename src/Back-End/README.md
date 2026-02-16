# AdMarketplace - Telegram Ads Marketplace MVP

A Telegram Mini App connecting channel owners with advertisers through an escrow-based deal flow, powered by TON blockchain.

## 🎯 Project Overview

AdMarketplace is a sophisticated two-sided marketplace that revolutionizes how Telegram channel advertising works by connecting channel owners with advertisers through a secure, automated, and transparent platform powered by TON blockchain escrow.

### What Makes This Platform Unique

**For Channel Owners (Supply Side):**
- **Automated Channel Verification**: Leverage Telegram Bot API and WTelegramClient to automatically fetch and display real-time channel statistics (subscribers, premium members, average views, language distribution)
- **Flexible Pricing Models**: Set custom pricing for posts with multiple pricing types: per hour, per day, or per thousand views (CPM)
- **Dual Discovery Modes**: Either browse active advertiser campaigns and apply, or receive direct invitations from interested advertisers
- **Negotiation & Counter-Offers**: Full negotiation system where both parties can propose terms, counter-offer on pricing, format, and posting schedule
- **Guaranteed Payments**: Funds are held in TON escrow and automatically released after successful post verification

**For Advertisers (Demand Side):**
- **Campaign Briefs with Targeting**: Create detailed campaign briefs with budget, target demographics, ad format preferences, and timeline
- **Smart Channel Discovery**: Browse channels with advanced filters (subscriber count, engagement rates, pricing, language, category)
- **Flexible Acquisition**: Either invite specific channels to your campaign or review applications from interested channel owners
- **Creative Control**: Complete creative approval workflow with revision cycles before content goes live
- **Budget Protection**: Funds locked in escrow until posting verification completes, with automatic refund for failed deals
- **Performance Tracking**: Monitor when posts go live and verify they meet agreed-upon terms

### The Complete Deal Lifecycle

1. **Discovery & Matching**: 
   - Advertisers create campaigns with budgets, targeting, and requirements
   - Channel owners can apply to campaigns OR advertisers can invite specific channels
   - Built-in filtering for both sides to find the perfect match

2. **Negotiation Phase**:
   - Initial proposal includes ad format, pricing model, posting schedule
   - Counter-offer system allows back-and-forth on terms
   - Applications can be accepted, rejected, or countered by either party

3. **Deal Creation & Escrow**:
   - Once terms are agreed, a Deal entity is created
   - Advertiser deposits funds to the platform's escrow wallet with a unique memo/reference
   - Deal status moves to `EscrowFunded`, signaling channel owner to draft content

4. **Creative Approval**:
   - Channel owner drafts the post/story/content and submits via Telegram bot
   - Advertiser reviews draft and either approves or requests revisions
   - Revision cycle continues until advertiser approves
   - Final content is locked and scheduled for posting

5. **Automated Posting**:
   - Background worker service (`AutoPostingWorker`) monitors scheduled posts
   - At the agreed time, content is auto-posted to the channel via Telegram Bot API
   - Post metadata (message ID, timestamp, content hash) is recorded

6. **Verification & Payment Release**:
   - `PostVerificationWorker` continuously monitors the posted content
   - Verifies post hasn't been deleted or edited
   - Ensures minimum visibility period (configurable, e.g., 24-48 hours)
   - Upon successful verification, funds are automatically released from escrow to channel owner
   - If verification fails (post deleted/edited), refund is triggered

7. **Timeout & Dispute Handling**:
   - `DealAutoCancelWorker` monitors deal inactivity
   - Automatic cancellation and refund if no progress within timeout period
   - Dispute status available for manual admin resolution

### Technical Architecture Highlights

**Backend Services:**
- **ChannelService**: Manages channel listings, stats updates, ownership verification
- **CampaignService**: Handles campaign creation, targeting, lifecycle management
- **CampaignApplicationService**: Channel owners applying to campaigns with proposals
- **ChannelApplicationService**: Advertisers inviting channels to campaigns
- **DealService**: Core escrow deal management and state transitions
- **TonPaymentService**: TON blockchain integration for wallet management and transactions
- **PostingService**: Telegram Bot API integration for automated posting
- **NotificationService**: Real-time Telegram notifications for deal milestones
- **AnalyticsUpdateService**: Automatic channel stats refresh from Telegram

**Background Workers:**
- **AutoPostingWorker**: Polls for scheduled posts and publishes them at the right time
- **PostVerificationWorker**: Continuous verification that posts remain live and unmodified
- **DealAutoCancelWorker**: Enforces timeout policies and triggers refunds

**Security & Trust:**
- **Admin Rights Re-verification**: Before any financial operation, system verifies user is still a channel admin via Telegram API
- **Centralized Escrow Wallet**: Single business wallet holds all escrow funds with deal-level tracking via memos/transaction references
- **Content Hashing**: Posted content is hashed to detect unauthorized edits
- **Immutable Audit Trail**: All status transitions, negotiations, and actions are timestamped and logged

### Why TON Blockchain?

- **Native Telegram Integration**: Seamless experience for Telegram users with built-in wallet support
- **Low Transaction Fees**: Cost-effective for micro-transactions typical in ad deals
- **Fast Finality**: Quick confirmation times for better user experience
- **Wallet Accessibility**: Easy integration with Telegram Wallet and TON Space

This isn't just a marketplace—it's a complete trust infrastructure that ensures fair dealing between advertisers and influencers, with automation handling the tedious parts and blockchain securing the financial aspects.

## 🏗️ Architecture

### Backend Stack
- **Framework**: ASP.NET Core 10.0 with FastEndpoints
- **Database**: PostgreSQL with Entity Framework Core
- **Blockchain**: TON (The Open Network) for escrow payments via TonSdk
- **Telegram Integration**: Telegram.Bot + WTelegramClient for stats + Webhook handlers
- **Background Jobs**: Worker services for auto-posting and verification
- **Logging**: Serilog with file and console sinks

### Project Structure

```
AdMarketplace/
├── AdMarketplace.WebApi/        # REST API, endpoints, and Mini App backend
├── AdMarketplace.Bot/           # Telegram bot for messaging & webhook handling
├── AdMarketplace.Domain/        # Domain models, contracts, DTOs, schemas
├── AdMarketplace.Database/      # EF Core context, migrations, configurations
└── AdMarketplace.Infra/         # Services, queues, external integrations
```

### Domain Model

**Core Entities:**
- `User`: Telegram users (can be both advertisers and channel owners)
- `Channel`: Channel listings with verified Telegram stats
- `ChannelPricing`: Pricing configuration with flexible pricing types (per hour, per day, per thousand views)
- `Campaign`: Advertiser requests/briefs with targeting criteria
- `CampaignApplication`: Channel owners applying to campaigns
- `CampaignInvitation`: Advertisers inviting specific channels
- `Deal`: Escrow-managed transactions between advertiser and channel
- `UserChannelConnection`: Links users to channels with admin verification
- `UserTransaction`: TON wallet payment tracking
- `Agent`: System bot accounts for automated channel operations (posting, analytics)

**Key Enums:**
- `ChannelStatusType`: Draft, Active, Suspended, Archived
- `DealStatusType`: Created, AwaitingPayment, Funded, InProgress, ContentSubmitted, ContentApproved, Published, Completed, Cancelled, Disputed, Refunded
- `CampaignStatusType`: Draft, Active, Paused, Completed, Cancelled
- `ApplicationStatusType`: Pending, Accepted, Rejected, Withdrawn

## 🚀 Features

### 1. Dual Marketplace Model

**Channel Owner Flow (Supply Side):**
1. Connect Telegram account
2. List channel with bot added as admin
3. Configure pricing for different ad formats
4. Receive verified channel stats automatically
5. Browse advertiser campaigns OR receive campaign invitations
6. Apply to campaigns or accept invitations
7. Negotiate deal terms
8. Draft creative content for approval
9. Auto-post upon approval
10. Receive payment after verification

**Advertiser Flow (Demand Side):**
1. Connect Telegram account
2. Create campaign brief with requirements
3. Browse channels OR receive applications from channels
4. Invite specific channels or accept applications
5. Negotiate deal terms
6. Deposit funds to TON escrow wallet
7. Review and approve creative content
8. Verify posting completion
9. Funds released automatically or refund if issues

### 2. Verified Channel Stats (via Telegram Bot API)

Automatically fetched and displayed metrics:
- ✅ Subscriber count (members)
- ✅ Premium subscriber count
- ✅ Average views per post
- ✅ Language distribution (viewer demographics)
- ✅ Channel metadata (title, username, description)
- ✅ Real-time stats refresh on-demand

### 3. Flexible Ad Formats & Pricing

**Currently Supported Ad Format:**
- **Post**: Standard channel post (MVP scope, more formats planned)

**Flexible Pricing Types:**
Channel owners can set pricing using different models:
- **Per Hour**: Price calculated based on how long the post stays pinned/visible
- **Per Day**: Daily rate for post visibility
- **Per Thousand Views (CPM)**: Performance-based pricing tied to actual post views

Each channel pricing entry includes:
- Ad format type
- Pricing type (per hour / per day / per thousand views)
- Price amount (in TON)

This flexibility allows channel owners to choose the pricing model that best fits their audience engagement patterns and advertiser preferences.

### 4. TON-Based Escrow System

**Security Features:**
- Centralized business wallet holds escrow funds
- Deal tracking via transaction hashes and memos
- Funds locked until posting verification completes
- Automated release after successful verification
- Refund support for cancelled/disputed deals

**Payment Lifecycle:**
```
Advertiser deposits → Funds held in escrow → Content posted 
→ Verification period → Auto-release to channel owner
```

**Timeout & Auto-Cancel:**
- Configurable inactivity timeouts per deal stage
- Automatic cancellation if no progress
- Refund processing for timed-out deals

### 5. Creative Approval Workflow

**Multi-stage approval process:**

1. **Deal Negotiation**: Terms, pricing, posting date agreed
2. **Brief Submission**: Advertiser provides campaign requirements
3. **Content Draft**: Channel owner creates draft post/story
4. **Review Cycle**: 
   - Advertiser reviews draft
   - Requests changes OR approves
   - Channel owner revises (loop until approved)
5. **Approval Lock**: Content frozen for posting
6. **Scheduled Posting**: Auto-post at agreed date/time
7. **Verification**: Post stays live and unmodified

**Status tracking:**
- Draft → Pending Review → Approved → Published → Verified

### 6. Auto-Posting & Verification

**Posting Capabilities:**
- Scheduled posting via Telegram Bot API
- Support for text, media, formatting
- Post to channels where bot is admin
- Configurable posting time (timezone-aware)

**Verification System:**
- Confirms post exists after publishing
- Checks post hasn't been deleted
- Validates post hasn't been edited
- Ensures minimum visibility period (configurable)
- Triggers fund release upon successful verification

**Worker Service:**
- Background service monitors pending posts
- Checks verification status periodically
- Handles failures and retries
- Updates deal status automatically

### 7. Practical Filters & Search

**Channel Discovery Filters:**
- Subscriber count range (min/max)
- Average views range
- Pricing range (per format)
- Language/demographics
- Channel status (active only)

**Campaign Discovery Filters:**
- Budget range
- Target audience size
- Campaign category/niche
- Application deadline
- Campaign status

## 📦 Installation & Setup

### Prerequisites

- **Docker & Docker Compose**
- **Telegram Bot Token** (from [@BotFather](https://t.me/BotFather))
- **Telegram API Credentials** (from [my.telegram.org](https://my.telegram.org))
- **TON Wallet** for escrow operations
- **TonCenter API Key** (from [@tonapibot](https://t.me/tonapibot))

### Docker Setup

1. **Clone the repository**
```bash
git clone <repository-url>
cd Ad-Marketplace-Contest/src/Back-End
```

2. **Create environment file**

Create a `.env` file in the project root:

```env
# Telegram Bot Settings
TELEGRAM_TOKEN=your-bot-token-from-botfather
WEBHOOK_URL=https://your-domain.com/bot
BOT_USERNAME=your_bot_username
APP_SHORTNAME=market
BOT_SECRET_TOKEN=your-random-secret-token

# Telegram API (for WTelegramClient - channel stats)
TELEGRAM_API_ID=your-api-id
TELEGRAM_API_HASH=your-api-hash

# TON Settings (optional - can be set in appsettings)
TON_API_KEY=your-toncenter-api-key
TON_BUSINESS_WALLET=your-ton-wallet-address
TON_MNEMONIC=your wallet mnemonic phrase
```

3. **Run with Docker Compose**

```bash
docker-compose up --build
```

This will start:
- **PostgreSQL 16**: Database on port 5432
- **AdMarketplace API**: Web application on port 80


### Docker Compose Configuration

The `compose.yaml` includes:

```yaml
services:
  admarketplace:
    image: admarketplace
    ports:
      - "80:8080"
    environment:
      - TelegramBot__Token=${TELEGRAM_TOKEN}
      - TelegramBot__BotApiServer=https://api.telegram.org
      - TelegramBot__WebhookUrl=${WEBHOOK_URL}
      - TelegramBot__BotUsername=${BOT_USERNAME}
      - TelegramBot__AppShortName=${APP_SHORTNAME}
      - TelegramBot__SecretToken=${BOT_SECRET_TOKEN}
      - TelegramApi__Credentials__0__ApiId=${TELEGRAM_API_ID}
      - TelegramApi__Credentials__0__ApiHash=${TELEGRAM_API_HASH}
      - ConnectionStrings__Main=Host=postgres;Port=5432;Database=AdMarketplace;Username=postgres;Password=postgres;
    depends_on:
      postgres:
        condition: service_healthy
    env_file:
      - .env

  postgres:
    image: postgres:16-alpine
    ports:
      - "127.0.0.1:5432:5432"
    environment:
      - POSTGRES_DB=AdMarketplace
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      timeout: 5s
      retries: 5

volumes:
  postgres_data:
```

### Configuration Reference

**TelegramBot Settings:**

| Key | Description |
| --- | --- |
| `Token` | Bot token from @BotFather |
| `BotApiServer` | Telegram Bot API server URL |
| `WebhookUrl` | Public URL for webhook callbacks |
| `BotUsername` | Bot username without @ |
| `AppShortName` | Mini App short name |
| `SecretToken` | Secret for webhook validation |

**TelegramApi Settings (for WTelegramClient):**

| Key | Description |
| --- | --- |
| `ApiId` | API ID from my.telegram.org |
| `ApiHash` | API Hash from my.telegram.org |

**TonSettings:**

| Key | Description |
| --- | --- |
| `ApiKey` | TonCenter API key |
| `IsTestnet` | `true` for testnet, `false` for mainnet |
| `BusinessWallet` | Escrow wallet address |
| `Mnemonic` | Wallet mnemonic phrase (24 words) |

**ConnectionStrings:**

| Key | Description |
| --- | --- |
| `Main` | PostgreSQL connection string |

## 🧪 Testing

### Test Data

Use the seeded categories and create test users via the Telegram bot.

**Test Flow:**
1. Start bot as user A → list a test channel
2. Start bot as user B → create a campaign
3. User A applies to campaign
4. Complete the deal flow end-to-end

## 📱 Deployment

### Production Deployment (Docker)

1. **Build Images**
```bash
docker-compose -f compose.yaml build
```

2. **Run Services**
```bash
docker-compose -f compose.yaml up -d
```

3. **Health Checks**
```bash
curl https://your-domain.com/health
```

### Environment Variables

For production, configure your `.env` file or set these environment variables:

**Required:**
- `TELEGRAM_TOKEN` - Bot token from @BotFather
- `WEBHOOK_URL` - Public webhook URL (https required)
- `BOT_USERNAME` - Bot username
- `BOT_SECRET_TOKEN` - Webhook secret token
- `TELEGRAM_API_ID` - API ID from my.telegram.org
- `TELEGRAM_API_HASH` - API Hash from my.telegram.org

**Optional (can override in compose.yaml):**
- `ConnectionStrings__Main` - PostgreSQL connection string
- `TonSettings__BusinessWallet` - Escrow wallet address
- `TonSettings__Mnemonic` - Wallet mnemonic
- `TonSettings__ApiKey` - TonCenter API key
- `TonSettings__IsTestnet` - `true` for testnet, `false` for mainnet
- `ASPNETCORE_ENVIRONMENT` - `Production` for production

### SSL/TLS Requirements

Telegram webhooks require HTTPS. Use:
- Reverse proxy (Nginx, Caddy)
- Cloud provider SSL (AWS ALB, Cloudflare)
- Let's Encrypt certificates

## 🔑 Key Design Decisions

### 1. **Dual Entry Point Convergence**

Both channel listings and advertiser campaigns lead to the same `Deal` entity. This unified model simplifies:
- Escrow management
- Payment processing
- State machine transitions
- Audit trails

Whether a deal originates from "channel applies to campaign" or "advertiser invites channel," the workflow is identical post-negotiation.

### 2. **Bot-First Messaging Philosophy**

Instead of building in-app chat, we leverage Telegram's native bot messaging:
- **Pros**: Native UX, push notifications, existing user habits, simpler backend
- **Cons**: Limited rich UI (but Mini App covers this)

Result: Better user experience with less code complexity.

### 3. **Centralized Escrow Wallet**

A single business wallet holds all escrow funds:
- **Simplicity**: No complex wallet generation or key management per deal
- **Traceability**: Deals tracked via transaction hashes and memos
- **Operational**: Easier to manage refunds and payouts from one wallet
- **Future Ready**: Can migrate to per-deal wallets or smart contracts later

### 4. **Admin Rights Re-verification**

Before every financial operation, we verify the user is still an admin by fetching the current admin list from Telegram and checking if the user has the required permissions.

This prevents:
- Unauthorized transactions after admin removal
- Fraudulent deals by ex-admins
- Stale permission exploits

### 5. **Status-Driven State Machines**

All major entities use explicit status enums with controlled transitions:
- Prevents invalid state transitions
- Enables clear business logic
- Simplifies debugging and logging
- Audit-friendly

Example: A `Deal` can only move from `Funded` → `InProgress`, not directly to `Completed`.

### 6. **Worker Services for Background Tasks**

Separate background workers handle:
- Auto-posting scheduled content
- Verification checks (post exists, not deleted, not edited)
- Timeout enforcement
- Payment processing

This keeps the API responsive and separates concerns.

### 7. **EF Core Configuration Segregation**

Each entity has its own `EfConfiguration` class:
- Cleaner `DbContext`
- Testable configurations
- Easy to modify individual entities
- Follows best practices

## 🔮 Future Enhancements

### Short-term (Next 3-6 months)
- [ ] **PR Manager Support**: Add team members to manage channels with role-based permissions
- [ ] **Dispute Resolution UI**: Admin dashboard for handling disputes
- [ ] **Analytics Dashboard**: Channel performance, campaign ROI metrics
- [ ] **Multi-language Support**: Localization for international markets
- [ ] **Rating System**: Channel and advertiser reputation scores
- [ ] **Advanced Filters**: AI-powered channel recommendations

### Medium-term (6-12 months)
- [ ] **Multi-currency Support**: USD, EUR, other crypto (TON, USDT)
- [ ] **Bulk Operations**: Manage multiple deals simultaneously
- [ ] **Campaign Templates**: Pre-built campaign types
- 
### Long-term (12+ months)
- [ ] **Revenue Sharing**: Multi-level commissions for agents


## ⚠️ Known Limitations

### Current MVP Constraints

1. **Manual Dispute Resolution**
   - Disputes require admin intervention
   - No automated arbitration yet
   - Basic logging only

2. **Stats Refresh Frequency**
   - Channel stats update on-demand
   - No real-time synchronization
   - May have slight delays

3. **Single Blockchain**
   - TON only
   - No multi-chain support
   - No fiat payment gateway

4. **Basic Search**
   - Simple filters, not full-text search
   - No fuzzy matching
   - Limited sorting options
   - May be slow with 10k+ channels

5. **Webhook Reliability**
   - Depends on Telegram webhook delivery
   - No retry mechanism for failed webhooks (yet)
   - Consider polling fallback for production

6. **Content Verification**
   - Checks post existence, not content quality
   - No plagiarism detection
   - No brand safety checks

7. **Scalability**
   - Not optimized for 100k+ concurrent users
   - Database queries need optimization
   - Consider Redis caching for production

8. **Authentication**
   - Telegram-only login
   - No email/password fallback
   - No 2FA yet

## 🔒 Security Considerations

### Implemented
- ✅ Admin rights verification before critical operations
- ✅ Centralized escrow wallet with deal-level tracking
- ✅ Webhook secret validation
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ Input validation and sanitization

## 🤖 AI Assistance Disclosure

**Approximate AI contribution: 35-40%**

### AI-Assisted Components:
- Entity class boilerplate and property definitions
- DTO/contract classes
- EF Core configuration classes
- Database migration scaffolding
- Basic CRUD operation templates
- XML documentation comments
- README documentation structure

### Human-Designed Components:
- Overall system architecture
- Business logic and state machines
- Deal flow and escrow system
- Creative approval workflow
- Telegram bot integration logic
- Worker service implementations
- Security model and admin verification
- API endpoint design
- Error handling strategies
- Performance optimization decisions

All critical business logic, security implementations, and architectural decisions were human-designed and reviewed.

## 📖 API Documentation

### Authentication

All API requests require the JWT token, which is being generated using init data of TMAs.

### Key Endpoints

Full API documentation available at `/swagger` when running in development mode.

## 🤝 Contributing

This is a hackathon submission. However, if you'd like to contribute:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Write tests for new features
5. Submit a pull request

Please open an issue first to discuss major changes.

## 📄 License

MIT License

Copyright (c)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---

**Built with ❤️ for the Telegram Mini App Contest**
