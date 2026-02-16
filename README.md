# Ad Marketplace - Telegram Mini App

A Telegram Mini App for an ads marketplace that connects channel owners and advertisers through an escrow-style deal flow powered by TON blockchain.

## 🎯 Overview

AdMarketplace is a two-sided marketplace platform that enables:
- **Channel Owners** to monetize their Telegram channels by listing them with custom pricing
- **Advertisers** to discover channels and run targeted campaigns with guaranteed delivery
- **Secure Transactions** via TON blockchain escrow ensuring trust between parties
- **Automated Workflow** from campaign creation to post verification and payment release

## 📁 Project Structure

This repository contains both the front-end and back-end components:

### [Front-End](./src/Front-End/README.md)
Telegram Mini App built with React and optimized for a native Telegram look and feel.

**Tech Stack:** React 19, TypeScript, Vite, TMA.js SDK, Zustand

👉 [View Front-End Documentation](./src/Front-End/README.md)

### [Back-End](./src/Back-End/README.md)
RESTful API and background workers handling the complete deal lifecycle, channel verification, and TON payments.

**Tech Stack:** .NET 10, ASP.NET Core, FastEndpoints, Entity Framework Core 10, PostgreSQL

👉 [View Back-End Documentation](./src/Back-End/README.md)

## ✨ Key Features

- **Dual Marketplace Model** - Support for both channel listings and advertiser campaigns
- **Verified Channel Stats** - Real-time Telegram channel analytics (subscribers, views, premium stats)
- **Flexible Pricing** - Multiple ad formats with customizable pricing models
- **TON Escrow** - Secure payment flow with automatic release upon delivery verification
- **Creative Approval** - Complete workflow from brief submission to content approval
- **Auto-Posting** - Automated content publishing and verification
- **Deal Lifecycle Management** - Auto-cancellation, timeouts, and dispute handling

## 🚀 Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/MahdiyarGHD/Ad-Marketplace-Contest.git
   cd Ad-Marketplace-Contest/src
   ```

2. **Set up the Back-End**
   ```bash
   cd Back-End
   # Follow instructions in Back-End/README.md
   ```

3. **Set up the Front-End**
   ```bash
   cd Front-End
   # Follow instructions in Front-End/README.md
   ```

## 📚 Documentation

For detailed setup instructions, configuration, and API documentation, please refer to the individual README files:

- **[Front-End README](./Front-End/README.md)** - UI setup, environment variables, and development guide
- **[Back-End README](./Back-End/README.md)** - API documentation, database setup, and architecture details

## 🔗 Repository

**GitHub:** [https://github.com/MahdiyarGHD/Ad-Marketplace-Contest](https://github.com/MahdiyarGHD/Ad-Marketplace-Contest)

## 📝 License

This project is open source and available for contribution.

