# Ad Marketplace - Front-End

A Telegram Mini App for an ads marketplace connecting channel owners and advertisers through an escrow-style deal flow.

## Overview

This front-end is optimized for performance and designed to look and feel native to Telegram, providing a seamless user experience within the Telegram ecosystem.

## Features

- **Native Telegram Look & Feel** - UI/UX optimized to match Telegram's native design patterns
- **Dual Marketplace** - Support for both channel owner listings and advertiser campaigns
- **Real-time Channel Stats** - Display verified Telegram channel analytics
- **Deal Management** - Complete workflow from brief submission to auto-posting
- **TON Integration** - Escrow payments using TON blockchain

## Tech Stack

- React 19 + TypeScript
- Vite for blazing fast development
- TMA.js SDK for Telegram Mini App integration
- Zustand for state management
- React Router for navigation
- SCSS for styling

## Environment Variables

Create a `.env` file in the root directory with the following variables:

```env
VITE_BACKEND_BASE_URL=your_backend_api_url
VITE_BOT_USERNAME=your_telegram_bot_username
```

## Getting Started

1. Install dependencies:
```bash
npm install
```

2. Configure environment variables (see above)

3. Run development server:
```bash
npm run dev
```

4. Build for production:
```bash
npm run build
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run start` - Start development server with network access
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Project Structure

```
src/
├── components/     # Reusable UI components
├── pages/          # Page components
├── stores/         # Zustand state management
├── utils/          # Utility functions and API client
└── assets/         # Static assets
```
