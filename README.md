# Shopping Mall POS

A production-grade supermarket / shopping mart management system for Indian retail. Single Windows EXE runs in Server Mode (multi-terminal LAN POS with PostgreSQL) or Client Mode (WPF POS on checkout counters). Cloud dashboard for chain-wide analytics.

## Quick Start

### ZIP Distribution (Recommended)

The ZIP at `installer/ShoppingMall-POS-v1.0.0.zip` is a portable, no-install distribution (~158 MB).

**Server PC** (store server with PostgreSQL):

1. Install PostgreSQL 15+ and create a database named `shopping_mall`
2. Extract the ZIP to `C:\ShoppingMall\`
3. Edit `Server\appsettings.json` — set `Host` and `Password` for your PostgreSQL
4. Run `Run-Server.bat` (starts the ASP.NET API on port 5000)

**Client terminal** (checkout counter):

1. Extract the same ZIP to `C:\ShoppingMallPOS\`
2. Edit `appsettings.json`:
   - Set `ServerUrl` to `http://SERVER_PC_IP:5000`
   - Set `TerminalName` to a unique name (e.g. `POS-2`)
3. Run `Run-POS.bat`

The server auto-creates all database tables and seeds initial data on first startup.

### Building from Source

```powershell
# Prerequisites: .NET SDK 8.0, PostgreSQL 15+
.\installer\build-installer.ps1
```

Output: `installer/ShoppingMall-POS-v1.0.0.zip`

---

## Installation Guide

### Server Setup

| Requirement | Detail |
|-------------|--------|
| OS | Windows 10/11, Windows Server 2019+ |
| Database | PostgreSQL 15+ (no 10 GB limit like SQL Server Express) |
| RAM | 4 GB minimum |
| Network | Static LAN IP for client terminals to connect |

**Steps:**

1. Extract the ZIP to a permanent location (e.g. `C:\ShoppingMall\`)
2. Open `Server\appsettings.json` and set:

```json
{
  "ConnectionStrings:DefaultConnection": "Host=localhost;Port=5432;Database=shopping_mall;Username=postgres;Password=your_password",
  "Urls": "http://0.0.0.0:5000"
}
```

3. Run `Run-Server.bat` or install as a Windows Service:

```powershell
sc create "ShoppingMallServer" binPath="C:\ShoppingMall\Server\ShoppingMall.Server.exe" start=auto
sc start ShoppingMallServer
```

4. Verify: browse to `http://localhost:5000/api/health` — should return `Healthy`.

### Client Terminal Setup

1. Extract the ZIP to any folder on the checkout counter PC
2. Open `appsettings.json` and set:

```json
{
  "ServerUrl": "http://192.168.1.100:5000",
  "TerminalName": "POS-1",
  "AutoConnect": true
}
```

3. Run `Run-POS.bat`

### Cloud Dashboard

The Blazor WebAssembly dashboard is at `CloudDashboard\` in the distribution. Deploy to any static web host (Azure Static Web Apps, IIS, etc.) or serve it directly from the server PC.

## Test Plan

All **106 unit tests** pass. Written with **xUnit** + **Moq** + **FluentAssertions**. Run with:

```powershell
dotnet test tests\ShoppingMall.Tests
```

### Test Coverage

| Test File | Tests | What It Covers |
|-----------|-------|----------------|
| `GstCalculatorTests.cs` | 7 | CGST/SGST/IGST calculation, intra/inter-state detection, zero/negative amounts |
| `BarcodeServiceTests.cs` | 7 | EAN-13 normalization, PLU/SKU/weight barcode lookup, not-found cases, empty/null input |
| `PromotionEngineTests.cs` | 14 | BuyXGetY, FreeItem, Bundle, TieredDiscount, exclusive flags, coupon validation, usage recording |
| `AuthServiceTests.cs` | 7 | PIN hash/verify (PBKDF2), AuthenticateAsync with valid/invalid credentials, LogoutAsync |
| `PosServiceTests.cs` | 7 | CreateTransaction, AddLineItem, ProcessPayment, stock deduction, invalid-product edge case |
| `InventoryServiceTests.cs` | 8 | AddStock, DeductStock idempotency, out-of-stock, low-stock detection, product not-found |
| `LoyaltyServiceTests.cs` | 8 | Earn points, Redeem with conversion (100 pts = ₹1), tier multiplier, insufficient balance |
| `InterStoreTransferServiceTests.cs` | 16 | Full Draft→Shipped→Received state machine, partial shipment, cancellation, edge cases, not-found |
| `ExcelExportServiceTests.cs` | 5 | Sales/GST/Product reports as valid `.xlsx` (ZIP header check, ClosedXML) |
| `PdfExportServiceTests.cs` | 6 | Sales/GST/Product reports as valid PDF (`%PDF` header, QuestPDF) |
| `CloudBackupServiceTests.cs` | 8 | AES-256-CBC encrypt/decrypt roundtrip, large data, wrong key, Unicode, JSON payload serialization |
| `CartServiceTests.cs` | 18 | Add/Remove/UpdateItem, ApplyLineDiscount, ApplyPercentDiscount, GrandTotal with GST, Serialize/Clear |

## Solution Structure

```
ShoppingMall.sln
├── src/
│   ├── ShoppingMall.Core/         # Domain models, enums, interfaces
│   ├── ShoppingMall.Data/         # EF Core DbContext, migrations, repositories
│   ├── ShoppingMall.Business/     # Services: POS, Inventory, Promotions, Loyalty, etc.
│   ├── ShoppingMall.Server/       # ASP.NET API host (minimal API endpoints)
│   └── ShoppingMall.Client/       # WPF desktop client (POS UI, admin views)
├── cloud/
│   └── ShoppingMall.CloudDashboard/  # Blazor WebAssembly dashboard
├── tests/
│   └── ShoppingMall.Tests/        # 106 xUnit tests
├── installer/
│   ├── build-installer.ps1       # Build script → ZIP distribution
│   ├── Product.wxs               # WiX MSI template (WiX v4)
│   ├── Product.template.wxs      # Template for auto-harvesting
│   ├── Generate-FilesFragment.ps1 # Harvest script for WiX build
│   └── msi.wixproj               # WiX SDK project (dotnet build)
└── sql/                          # Raw SQL scripts for reference
```

## Key Technical Decisions

| Decision | Rationale |
|----------|-----------|
| **PostgreSQL over SQL Server** | No 10 GB limit on Express edition |
| **Single EXE dual mode** | Same binary, different config (Server vs Client) |
| **SQLite for offline cache** | Client writes locally when LAN is down; replays with idempotency keys |
| **Append-only stock ledger** | `StockLedger` table is INSERT-only; current balance = SUM of all entries |
| **Session auth over JWT** | Session ID in `X-Session-Id` header, simpler for LAN, no token signing |
| **QuestPDF over iTextSharp** | MIT license, fluent C# API, in-memory generation |
| **ClosedXML for Excel** | MIT license, pure .NET, native `.xlsx` |
| **Razorpay for UPI** | Mature SDK with static/dynamic QR, HMAC-SHA256 webhook verification |
| **Win32 RawPrinterHelper** | P/Invoke `winspool.drv` — correct way to send raw ESC/POS to thermal printers |

## Prerequisites for Development

- .NET SDK 8.0.423 (`C:\Program Files\dotnet\dotnet.exe`)
- Visual Studio 2022 (or `dotnet` CLI)
- PostgreSQL 15+

## License

Proprietary — ShoppingMall
